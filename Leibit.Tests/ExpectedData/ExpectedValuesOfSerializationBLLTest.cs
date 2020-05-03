using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Serialization;
using System;
using System.Linq;

namespace Leibit.Tests.ExpectedData
{
    internal static class ExpectedValuesOfSerializationBLLTest
    {

        internal static SerializationContainer Serialize_Deserialize()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.Single(e => e.Id == "TTST");
            Estw.Time = new LeibitTime(eDaysOfService.Thursday, 13, 40);
            Estw.LastUpdatedOn = DateTime.Now.AddSeconds(-10);

            var Train = Area.Trains[2007];

            var LiveTrain = new TrainInformation(Train);
            LiveTrain.Block = Estw.Blocks["32G12"].First();

            var ProbeSchedule = new LiveSchedule(LiveTrain, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 7);
            ProbeSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 8);
            ProbeSchedule.LiveTrack = ProbeSchedule.Schedule.Track;
            LiveTrain.AddSchedule(ProbeSchedule);

            var TestdorfSchedule = new LiveSchedule(LiveTrain, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 12);
            TestdorfSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 14);
            TestdorfSchedule.LiveTrack = TestdorfSchedule.Schedule.Track;
            LiveTrain.AddSchedule(TestdorfSchedule);

            Area.LiveTrains.TryAdd(Train.Number, LiveTrain);


            Train = Area.Trains[12345];

            LiveTrain = new TrainInformation(Train);
            LiveTrain.Block = Estw.Blocks["31G3"].First();

            ProbeSchedule = new LiveSchedule(LiveTrain, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 14);
            ProbeSchedule.LiveTrack = ProbeSchedule.Schedule.Track;
            LiveTrain.AddSchedule(ProbeSchedule);

            TestdorfSchedule = new LiveSchedule(LiveTrain, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 7);
            TestdorfSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 10);
            TestdorfSchedule.LiveTrack = TestdorfSchedule.Schedule.Track;
            LiveTrain.AddSchedule(TestdorfSchedule);

            Area.LiveTrains.TryAdd(Train.Number, LiveTrain);


            Train = Area.Trains[12346];

            LiveTrain = new TrainInformation(Train);
            LiveTrain.Block = Estw.Blocks["32G22"].First();

            ProbeSchedule = new LiveSchedule(LiveTrain, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TPRB"));
            ProbeSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 15);
            ProbeSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 25);
            ProbeSchedule.LiveTrack = ProbeSchedule.Schedule.Track;

            var ProbeDelay = ProbeSchedule.AddDelay(5, eDelayType.Departure);
            ProbeDelay.Reason = "Meep";

            LiveTrain.AddSchedule(ProbeSchedule);

            TestdorfSchedule = new LiveSchedule(LiveTrain, Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
            TestdorfSchedule.LiveArrival = new LeibitTime(eDaysOfService.Thursday, 13, 29);
            TestdorfSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Thursday, 13, 30);
            TestdorfSchedule.LiveTrack = TestdorfSchedule.Schedule.Station.Tracks.Single(t => t.Name == "2");
            LiveTrain.AddSchedule(TestdorfSchedule);

            Area.LiveTrains.TryAdd(Train.Number, LiveTrain);

            var Result = new SerializationContainer();
            Result.Area = Area;
            return Result;
        }

    }
}
