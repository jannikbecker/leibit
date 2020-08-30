using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using System.Linq;

namespace Leibit.Tests.ExpectedData
{
    internal static class ExpectedValuesOfLiveDataBLLTest
    {

        internal static TrainInformation TestPunctualTrain(ESTW estw)
        {
            var Train = estw.Area.Trains[2007];

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Right;
            Result.Delay = 6;
            Result.Block = estw.Blocks["32G12"].First();

            var ProbeSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 7);
            ProbeSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 8);
            ProbeSchedule.LiveTrack = ProbeSchedule.Schedule.Track;
            ProbeSchedule.ExpectedArrival = ProbeSchedule.LiveArrival;
            ProbeSchedule.ExpectedDeparture = ProbeSchedule.LiveDeparture;
            Result.AddSchedule(ProbeSchedule);

            var TestdorfSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 12);
            TestdorfSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 14);
            TestdorfSchedule.LiveTrack = TestdorfSchedule.Schedule.Track;
            TestdorfSchedule.ExpectedArrival = TestdorfSchedule.LiveArrival;
            TestdorfSchedule.ExpectedDeparture = TestdorfSchedule.LiveDeparture;
            Result.AddSchedule(TestdorfSchedule);

            return Result;
        }

        internal static TrainInformation TestTrainDelayArrival(ESTW estw)
        {
            var Train = estw.Area.Trains[2007];

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Right;
            Result.Delay = 5;
            Result.Block = estw.Blocks["32G12"].First();

            var ProbeSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 0);
            ProbeSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 2);
            ProbeSchedule.ExpectedArrival = ProbeSchedule.LiveArrival;
            ProbeSchedule.ExpectedDeparture = ProbeSchedule.LiveDeparture;
            ProbeSchedule.LiveTrack = ProbeSchedule.Schedule.Track;
            Result.AddSchedule(ProbeSchedule);

            var TestdorfSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 11);
            TestdorfSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 13);
            TestdorfSchedule.LiveTrack = TestdorfSchedule.Schedule.Track;
            TestdorfSchedule.ExpectedArrival = TestdorfSchedule.LiveArrival;
            TestdorfSchedule.ExpectedDeparture = TestdorfSchedule.LiveDeparture;
            TestdorfSchedule.AddDelay(4, eDelayType.Arrival);
            Result.AddSchedule(TestdorfSchedule);

            return Result;
        }

        internal static TrainInformation TestTrainDelayDeparture(ESTW estw)
        {
            var Train = estw.Area.Trains[2007];

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Right;
            Result.Delay = 4;
            Result.Block = estw.Blocks["32G12"].First();

            var ProbeSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 0);
            ProbeSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 2);
            ProbeSchedule.LiveTrack = ProbeSchedule.Schedule.Track;
            ProbeSchedule.ExpectedArrival = ProbeSchedule.LiveArrival;
            ProbeSchedule.ExpectedDeparture = ProbeSchedule.LiveDeparture;
            Result.AddSchedule(ProbeSchedule);

            var TestdorfSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 07);
            TestdorfSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 12);
            TestdorfSchedule.LiveTrack = TestdorfSchedule.Schedule.Track;
            TestdorfSchedule.ExpectedArrival = TestdorfSchedule.LiveArrival;
            TestdorfSchedule.ExpectedDeparture = TestdorfSchedule.LiveDeparture;
            TestdorfSchedule.AddDelay(3, eDelayType.Departure);
            Result.AddSchedule(TestdorfSchedule);

            return Result;
        }

        internal static TrainInformation TestTrain2MinutesDelayDeparture(ESTW estw)
        {
            var Train = estw.Area.Trains[2007];

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Right;
            Result.Delay = 3;
            Result.Block = estw.Blocks["32G12"].First();

            var ProbeSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 0);
            ProbeSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 2);
            ProbeSchedule.LiveTrack = ProbeSchedule.Schedule.Track;
            ProbeSchedule.ExpectedArrival = ProbeSchedule.LiveArrival;
            ProbeSchedule.ExpectedDeparture = ProbeSchedule.LiveDeparture;
            Result.AddSchedule(ProbeSchedule);

            var TestdorfSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 07);
            TestdorfSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 11);
            TestdorfSchedule.LiveTrack = TestdorfSchedule.Schedule.Track;
            TestdorfSchedule.ExpectedArrival = TestdorfSchedule.LiveArrival;
            TestdorfSchedule.ExpectedDeparture = TestdorfSchedule.LiveDeparture;
            Result.AddSchedule(TestdorfSchedule);

            return Result;
        }

        internal static TrainInformation TestTrain2MinutesDelayArrival(ESTW estw)
        {
            var Train = estw.Area.Trains[2007];

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Right;
            Result.Delay = 5;
            Result.Block = estw.Blocks["32G12"].First();

            var ProbeSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 0);
            ProbeSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 2);
            ProbeSchedule.LiveTrack = ProbeSchedule.Schedule.Track;
            ProbeSchedule.ExpectedArrival = ProbeSchedule.LiveArrival;
            ProbeSchedule.ExpectedDeparture = ProbeSchedule.LiveDeparture;
            Result.AddSchedule(ProbeSchedule);

            var TestdorfSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 09);
            TestdorfSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 13);
            TestdorfSchedule.LiveTrack = TestdorfSchedule.Schedule.Track;
            TestdorfSchedule.ExpectedArrival = TestdorfSchedule.LiveArrival;
            TestdorfSchedule.ExpectedDeparture = TestdorfSchedule.LiveDeparture;
            Result.AddSchedule(TestdorfSchedule);

            return Result;
        }

        internal static TrainInformation TestPrematureTrain(ESTW estw)
        {
            var Train = estw.Area.Trains[2007];

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Right;
            Result.Delay = -10;
            Result.Block = estw.Blocks["32G12"].First();

            var ProbeSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 12, 32);
            ProbeSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 12, 33);
            ProbeSchedule.LiveTrack = ProbeSchedule.Schedule.Track;
            ProbeSchedule.ExpectedArrival = ProbeSchedule.LiveArrival;
            ProbeSchedule.ExpectedDeparture = ProbeSchedule.LiveDeparture;
            Result.AddSchedule(ProbeSchedule);

            var TestdorfSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 12, 46);
            TestdorfSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 12, 58);
            TestdorfSchedule.LiveTrack = TestdorfSchedule.Schedule.Track;
            TestdorfSchedule.ExpectedArrival = TestdorfSchedule.LiveArrival;
            TestdorfSchedule.ExpectedDeparture = TestdorfSchedule.LiveDeparture;
            Result.AddSchedule(TestdorfSchedule);

            return Result;
        }

        internal static TrainInformation TestChangedTrack(ESTW estw)
        {
            var Train = estw.Area.Trains[2007];

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Right;
            Result.Delay = 6;
            Result.Block = estw.Blocks["32G22"].First();

            var ProbeSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 7);
            ProbeSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 8);
            ProbeSchedule.LiveTrack = ProbeSchedule.Schedule.Track;
            ProbeSchedule.ExpectedArrival = ProbeSchedule.LiveArrival;
            ProbeSchedule.ExpectedDeparture = ProbeSchedule.LiveDeparture;
            Result.AddSchedule(ProbeSchedule);

            var TestdorfSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 12);
            TestdorfSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 14);
            TestdorfSchedule.LiveTrack = TestdorfSchedule.Schedule.Station.Tracks.FirstOrDefault(t => t.Name == "2");
            TestdorfSchedule.ExpectedArrival = TestdorfSchedule.LiveArrival;
            TestdorfSchedule.ExpectedDeparture = TestdorfSchedule.LiveDeparture;
            Result.AddSchedule(TestdorfSchedule);

            return Result;
        }

        internal static TrainInformation TestSpecialTrain(ESTW estw)
        {
            var Train = new Train(98765);

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Left;
            Result.Delay = 4;
            Result.Block = estw.Blocks["31G1"].First();

            var TestdorfSchedule = new LiveSchedule(Result, new Schedule(Train, estw.Stations.FirstOrDefault(s => s.ShortSymbol == "TTST")));
            TestdorfSchedule.Schedule.Arrival = new LeibitTime(18, 26);
            TestdorfSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 18, 29);
            TestdorfSchedule.Schedule.Departure = new LeibitTime(18, 26);
            TestdorfSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 18, 30);
            TestdorfSchedule.LiveTrack = TestdorfSchedule.Schedule.Station.Tracks.FirstOrDefault(t => t.Name == "3");
            TestdorfSchedule.ExpectedArrival = TestdorfSchedule.LiveArrival;
            TestdorfSchedule.ExpectedDeparture = TestdorfSchedule.LiveDeparture;
            Result.AddSchedule(TestdorfSchedule);

            var ProbeSchedule = new LiveSchedule(Result, new Schedule(Train, estw.Stations.FirstOrDefault(s => s.ShortSymbol == "TPRB")));
            ProbeSchedule.Schedule.Arrival = new LeibitTime(18, 32);
            ProbeSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 18, 35);
            ProbeSchedule.Schedule.Departure = new LeibitTime(18, 32);
            ProbeSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 18, 36);
            ProbeSchedule.LiveTrack = ProbeSchedule.Schedule.Station.Tracks.FirstOrDefault(t => t.Name == "1");
            ProbeSchedule.ExpectedArrival = ProbeSchedule.LiveArrival;
            ProbeSchedule.ExpectedDeparture = ProbeSchedule.LiveDeparture;
            Result.AddSchedule(ProbeSchedule);

            Train.AddSchedule(TestdorfSchedule.Schedule);
            Train.AddSchedule(ProbeSchedule.Schedule);
            return Result;
        }

        internal static TrainInformation TestSpecialTrainDelay(ESTW estw)
        {
            var Train = new Train(98765);

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Left;
            Result.Delay = 11;
            Result.Block = estw.Blocks["31G1"].First();

            var TestdorfSchedule = new LiveSchedule(Result, new Schedule(Train, estw.Stations.FirstOrDefault(s => s.ShortSymbol == "TTST")));
            TestdorfSchedule.Schedule.Arrival = new LeibitTime(18, 26);
            TestdorfSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 18, 29);
            TestdorfSchedule.Schedule.Departure = new LeibitTime(18, 28);
            TestdorfSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 18, 35);
            TestdorfSchedule.LiveTrack = TestdorfSchedule.Schedule.Station.Tracks.FirstOrDefault(t => t.Name == "3");
            TestdorfSchedule.ExpectedArrival = TestdorfSchedule.LiveArrival;
            TestdorfSchedule.ExpectedDeparture = TestdorfSchedule.LiveDeparture;
            TestdorfSchedule.AddDelay(4, eDelayType.Departure);
            Result.AddSchedule(TestdorfSchedule);

            var ProbeSchedule = new LiveSchedule(Result, new Schedule(Train, estw.Stations.FirstOrDefault(s => s.ShortSymbol == "TPRB")));
            ProbeSchedule.Schedule.Arrival = new LeibitTime(18, 33);
            ProbeSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 18, 43);
            ProbeSchedule.Schedule.Departure = new LeibitTime(18, 33);
            ProbeSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 18, 44);
            ProbeSchedule.LiveTrack = ProbeSchedule.Schedule.Station.Tracks.FirstOrDefault(t => t.Name == "1");
            ProbeSchedule.ExpectedArrival = ProbeSchedule.LiveArrival;
            ProbeSchedule.ExpectedDeparture = ProbeSchedule.LiveDeparture;
            ProbeSchedule.AddDelay(3, eDelayType.Arrival);
            Result.AddSchedule(ProbeSchedule);

            Train.AddSchedule(TestdorfSchedule.Schedule);
            Train.AddSchedule(ProbeSchedule.Schedule);
            return Result;
        }

        internal static TrainInformation TestSequentialStations(ESTW estw)
        {
            var Train = estw.Area.Trains[2007];

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Right;
            Result.Delay = 3;
            Result.Block = estw.Blocks["32G12"].First();

            var ProbeSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 7);
            ProbeSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 9);
            ProbeSchedule.LiveTrack = ProbeSchedule.Schedule.Track;
            ProbeSchedule.ExpectedArrival = ProbeSchedule.LiveArrival;
            ProbeSchedule.ExpectedDeparture = ProbeSchedule.LiveDeparture;
            Result.AddSchedule(ProbeSchedule);

            var TestdorfSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 9);
            TestdorfSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 11);
            TestdorfSchedule.LiveTrack = TestdorfSchedule.Schedule.Track;
            TestdorfSchedule.ExpectedArrival = TestdorfSchedule.LiveArrival;
            TestdorfSchedule.ExpectedDeparture = TestdorfSchedule.LiveDeparture;
            Result.AddSchedule(TestdorfSchedule);

            return Result;
        }

        internal static TrainInformation TestRefreshLiveSchedules(ESTW estw)
        {
            var Train = estw.Area.Trains[2007];

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Right;
            Result.Delay = 6;
            Result.Block = estw.Blocks["32G12"].First();

            var ProbeSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 7);
            ProbeSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 8);
            ProbeSchedule.LiveTrack = ProbeSchedule.Schedule.Track;
            ProbeSchedule.ExpectedArrival = ProbeSchedule.LiveArrival;
            ProbeSchedule.ExpectedDeparture = ProbeSchedule.LiveDeparture;
            Result.AddSchedule(ProbeSchedule);

            var TestdorfSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 12);
            TestdorfSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 14);
            TestdorfSchedule.LiveTrack = TestdorfSchedule.Schedule.Track;
            TestdorfSchedule.ExpectedArrival = TestdorfSchedule.LiveArrival;
            TestdorfSchedule.ExpectedDeparture = TestdorfSchedule.LiveDeparture;
            Result.AddSchedule(TestdorfSchedule);

            var RechtsheimSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TREH"));
            Result.AddSchedule(TestdorfSchedule);

            return Result;
        }

        internal static TrainInformation TestLoadSharedDelay_Existing(ESTW estw)
        {
            var Train = estw.Area.Trains[2007];

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Right;
            Result.Delay = 4;
            Result.Block = estw.Blocks["32G12"].First();

            var ProbeSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 0);
            ProbeSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 2);
            ProbeSchedule.LiveTrack = ProbeSchedule.Schedule.Track;
            ProbeSchedule.ExpectedArrival = ProbeSchedule.LiveArrival;
            ProbeSchedule.ExpectedDeparture = ProbeSchedule.LiveDeparture;
            Result.AddSchedule(ProbeSchedule);

            var TestdorfSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 07);
            TestdorfSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 12);
            TestdorfSchedule.LiveTrack = TestdorfSchedule.Schedule.Track;
            TestdorfSchedule.ExpectedArrival = TestdorfSchedule.LiveArrival;
            TestdorfSchedule.ExpectedDeparture = TestdorfSchedule.LiveDeparture;
            Result.AddSchedule(TestdorfSchedule);

            var Delay = TestdorfSchedule.AddDelay(3, eDelayType.Departure);
            Delay.Reason = "Keine Ahnung";
            Delay.CausedBy = 4711;

            return Result;
        }

        internal static TrainInformation TestLoadSharedDelay_NonExisting(ESTW estw)
        {
            var Train = estw.Area.Trains[2007];

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Right;
            Result.Delay = 6;
            Result.Block = estw.Blocks["32G12"].First();

            var ProbeSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 7);
            ProbeSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 8);
            ProbeSchedule.LiveTrack = ProbeSchedule.Schedule.Track;
            ProbeSchedule.ExpectedArrival = ProbeSchedule.LiveArrival;
            ProbeSchedule.ExpectedDeparture = ProbeSchedule.LiveDeparture;
            Result.AddSchedule(ProbeSchedule);

            var TestdorfSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 12);
            TestdorfSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 14);
            TestdorfSchedule.LiveTrack = TestdorfSchedule.Schedule.Track;
            TestdorfSchedule.ExpectedArrival = TestdorfSchedule.LiveArrival;
            TestdorfSchedule.ExpectedDeparture = TestdorfSchedule.LiveDeparture;
            Result.AddSchedule(TestdorfSchedule);

            var Delay = TestdorfSchedule.AddDelay(4, eDelayType.Departure);
            Delay.Reason = "Keine Ahnung";
            Delay.CausedBy = 4711;

            return Result;
        }

        internal static TrainInformation TestExpectedTimesDelayed(ESTW estw)
        {
            var Train = estw.Area.Trains[12345];

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Left;
            Result.Delay = 8;
            Result.Block = estw.Blocks["33BP"].First();

            var TestdorfSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.ExpectedArrival = new LeibitTime(13, 14);
            TestdorfSchedule.ExpectedDeparture = new LeibitTime(13, 16);
            Result.AddSchedule(TestdorfSchedule);

            var ProbeSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.ExpectedArrival = new LeibitTime(13, 21);
            Result.AddSchedule(ProbeSchedule);

            return Result;
        }

        internal static TrainInformation TestExpectedTimesPremature(ESTW estw)
        {
            var Train = estw.Area.Trains[12345];

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Left;
            Result.Delay = -5;
            Result.Block = estw.Blocks["33BP"].First();

            var TestdorfSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.ExpectedArrival = new LeibitTime(13, 01);
            TestdorfSchedule.ExpectedDeparture = new LeibitTime(13, 10);
            Result.AddSchedule(TestdorfSchedule);

            var ProbeSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.ExpectedArrival = new LeibitTime(13, 15);
            Result.AddSchedule(ProbeSchedule);

            return Result;
        }

        internal static TrainInformation TestExpectedTimesDelayedPassing(ESTW estw)
        {
            var Train = estw.Area.Trains[86312];

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Left;
            Result.Delay = 5;
            Result.Block = estw.Blocks["33BP"].First();

            var TestdorfSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.ExpectedArrival = new LeibitTime(13, 13);
            TestdorfSchedule.ExpectedDeparture = new LeibitTime(13, 13);
            Result.AddSchedule(TestdorfSchedule);

            var ProbeSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.ExpectedArrival = new LeibitTime(13, 17);
            ProbeSchedule.ExpectedDeparture = new LeibitTime(13, 17);
            Result.AddSchedule(ProbeSchedule);

            return Result;
        }

        internal static TrainInformation TestExpectedTimesPrematurePassing(ESTW estw)
        {
            var Train = estw.Area.Trains[86312];

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Left;
            Result.Delay = -6;
            Result.Block = estw.Blocks["33BP"].First();

            var TestdorfSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.ExpectedArrival = new LeibitTime(13, 2);
            TestdorfSchedule.ExpectedDeparture = new LeibitTime(13, 2);
            Result.AddSchedule(TestdorfSchedule);

            var ProbeSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.ExpectedArrival = new LeibitTime(13, 6);
            ProbeSchedule.ExpectedDeparture = new LeibitTime(13, 6);
            Result.AddSchedule(ProbeSchedule);

            return Result;
        }

        internal static TrainInformation TestMisdirectedTrain(ESTW estw)
        {
            var Train = estw.Area.Trains[4711];

            var Result = new TrainInformation(Train);
            Result.Direction = eBlockDirection.Right;
            Result.Delay = 0;
            Result.Block = estw.Blocks["32G11"].First();

            var ProbeSchedule = new LiveSchedule(Result, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 14, 4);
            ProbeSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 14, 5);
            ProbeSchedule.LiveTrack = ProbeSchedule.Schedule.Track;
            ProbeSchedule.ExpectedArrival = ProbeSchedule.LiveArrival;
            ProbeSchedule.ExpectedDeparture = ProbeSchedule.LiveDeparture;
            Result.AddSchedule(ProbeSchedule);

            var TestdorfSchedule = new LiveSchedule(Result, new Schedule(Train, estw.Stations.FirstOrDefault(s => s.ShortSymbol == "TTST")));
            TestdorfSchedule.Schedule.Arrival = new LeibitTime(14, 11);
            TestdorfSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 14, 11);
            TestdorfSchedule.Schedule.Departure = new LeibitTime(14, 12);
            TestdorfSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 14, 12);
            TestdorfSchedule.LiveTrack = TestdorfSchedule.Schedule.Station.Tracks.FirstOrDefault(t => t.Name == "1A");
            TestdorfSchedule.ExpectedArrival = TestdorfSchedule.LiveArrival;
            TestdorfSchedule.ExpectedDeparture = TestdorfSchedule.LiveDeparture;
            Result.AddSchedule(TestdorfSchedule);

            Train.AddSchedule(TestdorfSchedule.Schedule);
            return Result;
        }

    }
}
