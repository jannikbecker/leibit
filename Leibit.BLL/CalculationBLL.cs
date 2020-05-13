using Leibit.Core.Common;
using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leibit.BLL
{
    public class CalculationBLL : BLLBase
    {

        #region - Needs -
        private SettingsBLL m_SettingsBll;
        #endregion

        #region - Ctor -
        public CalculationBLL()
            : base()
        {
            m_SettingsBll = new SettingsBLL();
        }
        #endregion

        #region - Public methods -

        #region [GetSchedulesByTime]
        public OperationResult<List<Schedule>> GetSchedulesByTime(IList<Schedule> Schedules, LeibitTime time)
        {
            try
            {
                var Result = new OperationResult<List<Schedule>>();
                Result.Result = new List<Schedule>();

                LeibitTime FirstTime = null;

                foreach (var Schedule in Schedules.OrderBy(s => s.Time))
                {
                    foreach (eDaysOfService value in Enum.GetValues(typeof(eDaysOfService)))
                    {
                        if (Schedule.Days.Contains(value) || !Schedule.Days.Any())
                        {
                            var ScheduleTime = new LeibitTime(value, Schedule.Time.Hour, Schedule.Time.Minute);
                            var ReferenceTime = FirstTime == null ? time : FirstTime;

                            if (ScheduleTime > ReferenceTime.AddHours(-12) && ScheduleTime <= ReferenceTime.AddHours(12))
                            {
                                Result.Result.Add(Schedule);

                                if (FirstTime == null)
                                    FirstTime = ScheduleTime;
                            }
                        }
                    }
                }

                Result.Succeeded = true;
                return Result;
            }
            catch (Exception ex)
            {
                return new OperationResult<List<Schedule>> { Message = ex.Message };
            }
        }
        #endregion

        #region [CalculateDelay]
        public OperationResult<int?> CalculateDelay(TrainInformation Train, ESTW Estw)
        {
            try
            {
                var Result = new OperationResult<int?>();

                bool HasPreviousSchedule = false;
                int PreviousDelay = 0;

                var settingsResult = m_SettingsBll.GetSettings();
                ValidateResult(settingsResult);
                var delayJustificationEnabled = settingsResult.Result.DelayJustificationEnabled;
                var delayJustificationMinutes = settingsResult.Result.DelayJustificationMinutes;

                foreach (var Schedule in Train.Schedules)
                {
                    if (Schedule.LiveArrival == null)
                        continue;

                    var Arrival = Schedule.Schedule.Arrival == null ? Schedule.Schedule.Departure : Schedule.Schedule.Arrival;
                    var DelayArrival = (Schedule.LiveArrival - Arrival).TotalMinutes;

                    if (delayJustificationEnabled && HasPreviousSchedule && DelayArrival - PreviousDelay >= delayJustificationMinutes && !Schedule.Delays.Any(d => d.Type == eDelayType.Arrival))
                        Schedule.AddDelay(DelayArrival - PreviousDelay, eDelayType.Arrival);

                    PreviousDelay = DelayArrival < 0 ? 0 : DelayArrival;

                    if (Schedule.Schedule.Departure != null)
                    {
                        if (Schedule.LiveDeparture != null)
                        {
                            var DelayDeparture = (Schedule.LiveDeparture - Schedule.Schedule.Departure).TotalMinutes;

                            if (delayJustificationEnabled && DelayDeparture - PreviousDelay >= delayJustificationMinutes && !Schedule.Delays.Any(d => d.Type == eDelayType.Departure))
                                Schedule.AddDelay(DelayDeparture - PreviousDelay, eDelayType.Departure);

                            PreviousDelay = DelayDeparture < 0 ? 0 : DelayDeparture;
                            Result.Result = DelayDeparture;
                        }
                        else
                        {
                            var Delay = (Estw.Time - Schedule.Schedule.Departure).TotalMinutes;
                            Result.Result = Delay < DelayArrival ? DelayArrival : Delay;
                        }
                    }
                    else
                        Result.Result = DelayArrival;

                    HasPreviousSchedule = true;
                }

                Result.Succeeded = true;
                return Result;
            }
            catch (Exception ex)
            {
                return new OperationResult<int?> { Message = ex.Message };
            }
        }
        #endregion

        #region [CalculateExpectedTimes]
        public OperationResult<bool> CalculateExpectedTimes(TrainInformation Train, ESTW Estw)
        {
            try
            {
                var Result = new OperationResult<bool>();
                int Delay = Train.Delay;
                var Start = false;

                var FirstSchedule = Train.Schedules.FirstOrDefault(s => s.LiveArrival != null);

                if (FirstSchedule != null)
                {
                    var Arrival = FirstSchedule.Schedule.Arrival == null ? FirstSchedule.Schedule.Departure : FirstSchedule.Schedule.Arrival;
                    Delay = (FirstSchedule.LiveArrival - Arrival).TotalMinutes;
                }

                foreach (var Schedule in Train.Schedules)
                {
                    if (Schedule.LiveArrival != null)
                        Start = true;

                    if (Schedule.Schedule.Station.ESTW == Estw)
                        Start = true;

                    if (!Start)
                        continue;

                    var CalculateDelay = Schedule.Schedule.Track != null && Schedule.Schedule.Track.CalculateDelay;
                    var CurrentIndex = Train.Schedules.IndexOf(Schedule);

                    if (Train.Schedules.Where((schedule, index) => index > CurrentIndex && schedule.LiveArrival != null).Any())
                        CalculateDelay = false;

                    var Arrival = Schedule.Schedule.Arrival == null ? Schedule.Schedule.Departure : Schedule.Schedule.Arrival;

                    if (Schedule.LiveArrival != null)
                        Schedule.ExpectedArrival = Schedule.LiveArrival;
                    else
                    {
                        Schedule.ExpectedArrival = Arrival.AddMinutes(Delay);

                        if (Schedule.ExpectedArrival < Estw.Time && CalculateDelay)
                            Schedule.ExpectedArrival = Estw.Time;
                    }

                    Delay = (Schedule.ExpectedArrival - Arrival).TotalMinutes;

                    if (Schedule.LiveDeparture != null)
                    {
                        Schedule.ExpectedDeparture = Schedule.LiveDeparture;
                    }
                    else if (Schedule.Schedule.Departure != null)
                    {
                        int MinStoptime = 0;
                        bool DepartureBeforeScheduled = true;

                        if (Schedule.Schedule.Handling == eHandling.StopPassengerTrain)
                        {
                            MinStoptime = Constants.PERS_TRAIN_STOPTIME;
                            DepartureBeforeScheduled = false;
                        }

                        if (Schedule.Schedule.Handling == eHandling.StaffChange)
                            MinStoptime = Constants.STAFF_CHANGE_STOPTIME;

                        if (Schedule.Schedule.Handling == eHandling.StopFreightTrain || Schedule.Schedule.Handling == eHandling.Start)
                            DepartureBeforeScheduled = false;

                        int StopMinutes = Schedule.Schedule.Departure.TotalMinutes - Arrival.TotalMinutes;

                        if (StopMinutes > MinStoptime)
                            StopMinutes = MinStoptime;

                        var Departure = Schedule.ExpectedArrival.AddMinutes(StopMinutes);

                        if (Departure < Schedule.Schedule.Departure && !DepartureBeforeScheduled)
                            Schedule.ExpectedDeparture = Schedule.Schedule.Departure;
                        else
                            Schedule.ExpectedDeparture = Departure;

                        if (Schedule.ExpectedDeparture < Estw.Time && CalculateDelay)
                            Schedule.ExpectedDeparture = Estw.Time;
                    }

                    if (Schedule.ExpectedDeparture != null)
                        Delay = (Schedule.ExpectedDeparture - Schedule.Schedule.Time).TotalMinutes;
                }

                Result.Result = true;
                Result.Succeeded = true;
                return Result;
            }
            catch (Exception ex)
            {
                return new OperationResult<bool> { Message = ex.Message };
            }
        }
        #endregion

        #endregion

    }
}
