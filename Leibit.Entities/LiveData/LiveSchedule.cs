using Leibit.Core.Scheduling;
using Leibit.Entities.Common;
using Leibit.Entities.Scheduling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Leibit.Entities.LiveData
{
    public class LiveSchedule : IComparable<LiveSchedule>
    {
        private TrainInformation m_Train;
        private LeibitTime m_LiveArrival;
        private LeibitTime m_LiveDeparture;
        private Schedule m_Schedule;
        private List<DelayInfo> m_Delays;
        private object m_LockDelays = new object();

        public LiveSchedule(TrainInformation Train, Station Station)
            : this(Train, new Schedule(Train.Train, Station))
        {
        }

        public LiveSchedule(TrainInformation Train, Schedule Schedule)
        {
            m_Train = Train;
            m_Schedule = Schedule;
            m_Delays = new List<DelayInfo>();
        }

        public TrainInformation Train
        {
            get
            {
                return m_Train;
            }
        }

        public Schedule Schedule
        {
            get
            {
                return m_Schedule;
            }
        }

        public LeibitTime LiveArrival
        {
            get
            {
                return m_LiveArrival;
            }
            set
            {
                var oldValue = m_LiveArrival;
                m_LiveArrival = value;

                if (value != null && m_Schedule.Arrival == null && m_Schedule.Track == null)
                    m_Schedule.Arrival = m_Schedule.Station.ESTW.Time.AddMinutes(m_Train.Delay * (-1));

                if (value != oldValue)
                    Train.SortSchedules();
            }
        }

        public LeibitTime LiveDeparture
        {
            get
            {
                return m_LiveDeparture;
            }
            set
            {
                m_LiveDeparture = value;

                if (value != null && m_Schedule.Departure == null && m_Schedule.Track == null)
                {
                    m_Schedule.Departure = m_Schedule.Station.ESTW.Time.AddMinutes(m_Train.Delay * (-1));
                    Train.SortSchedules();
                }
            }
        }

        public bool IsArrived { get; set; }

        public bool IsDeparted { get; set; }

        public Track LiveTrack
        {
            get;
            set;
        }

        public LeibitTime ExpectedArrival
        {
            get;
            set;
        }

        public LeibitTime ExpectedDeparture
        {
            get;
            set;
        }

        public int? ExpectedDelay
        {
            get;
            set;
        }

        public ReadOnlyCollection<DelayInfo> Delays
        {
            get
            {
                lock (m_LockDelays)
                {
                    return m_Delays.AsReadOnly();
                }
            }
        }

        public DelayInfo AddDelay(int minutes, eDelayType type)
        {
            lock (m_LockDelays)
            {
                var Result = new DelayInfo(this, minutes, type);
                m_Delays.Add(Result);
                return Result;
            }
        }

        public int CompareTo(LiveSchedule other)
        {
            if (this.LiveArrival != null && other.LiveArrival != null)
                return this.LiveArrival.CompareTo(other.LiveArrival);

            if (this.Schedule.Time == null)
                return 1;

            return this.Schedule.Time.CompareTo(other.Schedule.Time);
        }

    }
}
