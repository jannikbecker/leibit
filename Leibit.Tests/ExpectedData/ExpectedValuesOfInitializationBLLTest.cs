using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leibit.Tests.ExpectedData
{
    internal static class ExpectedValuesOfInitializationBLLTest
    {
        private static readonly List<eDaysOfService> _ALL_DAYS = Enum.GetValues(typeof(eDaysOfService)).Cast<eDaysOfService>().ToList();

        internal static List<Area> GetAreaInformation()
        {
            var Result = new List<Area>();

            var Testland = new Area("myTestArea", "Testland");
            var Testdorf = new ESTW("TTST", "Testdorf", "leibit_TEST.dat", Testland);
            var Rechtsheim = new ESTW("TREH", "Rechtsheim", "leibit_RECHTSHEI.dat", Testland);
            Result.Add(Testland);

            var Paradies = new Area("another", "Paradies");
            var Paradiesbahnhof = new ESTW("PPP", "Paradiesbahnhof", "leibit_PARADIES.dat", Paradies);
            Result.Add(Paradies);

            return Result;
        }

        internal static Area LoadTestdorfESTW()
        {
            var Area = new Area("myTestArea", "Testland");
            var EstwTestdorf = new ESTW("TTST", "Testdorf", "leibit_TEST.dat", Area);
            var EstwRechtsheim = new ESTW("TREH", "Rechtsheim", "leibit_RECHTSHEI.dat", Area);

            var Linksdorf = new Station("Linksdorf", "TLDF", 30, null, null, EstwTestdorf);
            var LinksdorfTrack = new Track(null, false, false, Linksdorf, null);
            new Block("30BN", eBlockDirection.Right, LinksdorfTrack);
            new Block("31B4711", eBlockDirection.Right, LinksdorfTrack);

            var Probe = new Station("Probe", "TPRB", 31, "g31_____.abf", "BF31____.abf", EstwTestdorf);
            var ProbeTrack1 = new Track("1", true, true, Probe, null);
            new Block("31G1", eBlockDirection.Both, ProbeTrack1);
            var ProbeTrack2 = new Track("2", true, true, Probe, null);
            new Block("31G2", eBlockDirection.Both, ProbeTrack2);
            var ProbeTrack3 = new Track("3", true, true, Probe, null);
            new Block("31G3", eBlockDirection.Both, ProbeTrack3);

            var UeStMitte = new Station("ÜSt Mitte", "T UM", 32, null, null, EstwTestdorf);
            var UeStMitteTrack = new Track(null, false, false, UeStMitte, null);
            new Block("32B815", eBlockDirection.Right, UeStMitteTrack);
            new Block("32B816", eBlockDirection.Left, UeStMitteTrack);
            new Block("32B817", eBlockDirection.Right, UeStMitteTrack);
            new Block("31B814", eBlockDirection.Left, UeStMitteTrack);

            var Testdorf = new Station("Testdorf", "TTST", 32, "g32_____.abf", null, EstwTestdorf);
            var TestdorfTrack1 = new Track("1", true, true, Testdorf, null);
            var TestdorfTrack1A = new Track("1A", true, true, Testdorf, TestdorfTrack1);
            new Block("32G11", eBlockDirection.Both, TestdorfTrack1A);
            var TestdorfTrack1B = new Track("1B", true, true, Testdorf, TestdorfTrack1);
            new Block("32G12", eBlockDirection.Both, TestdorfTrack1B);
            var TestdorfTrack2 = new Track("2", true, true, Testdorf, null);
            new Block("32G21", eBlockDirection.Both, TestdorfTrack2);
            new Block("32G22", eBlockDirection.Both, TestdorfTrack2);
            var TestdorfTrack3 = new Track("3", true, true, Testdorf, null);
            new Block("32G31", eBlockDirection.Both, TestdorfTrack3);
            new Block("32G32", eBlockDirection.Both, TestdorfTrack3);
            var TestdorfTrack4 = new Track("4", true, true, Testdorf, null);
            new Block("32G41", eBlockDirection.Both, TestdorfTrack4);
            var TestdorfTrack4A = new Track("4A", true, true, Testdorf, null);
            new Block("32G42", eBlockDirection.Both, TestdorfTrack4A);

            TestdorfTrack1.Alternatives.Add(TestdorfTrack2);
            TestdorfTrack1.Alternatives.Add(TestdorfTrack3);
            TestdorfTrack1.Alternatives.Add(TestdorfTrack4);
            TestdorfTrack4A.Alternatives.Add(TestdorfTrack1A);
            TestdorfTrack4A.Alternatives.Add(TestdorfTrack2);
            TestdorfTrack4A.Alternatives.Add(TestdorfTrack3);

            var Rechtsheim = new Station("Rechtsheim", "TREH", 33, null, null, EstwTestdorf);
            var RechtsheimTrack = new Track(null, false, false, Rechtsheim, null);
            new Block("33BP", eBlockDirection.Left, RechtsheimTrack);
            new Block("32B4712", eBlockDirection.Left, RechtsheimTrack);

            var Train2007 = new Train(2007, "IC", "Linksdorf Hbf", "Rechtsheim");
            Area.Trains.TryAdd(2007, Train2007);
            new Schedule(Train2007, null, new LeibitTime(13, 1), ProbeTrack2, _ALL_DAYS, eScheduleDirection.LeftToRight, eHandling.Transit, String.Empty, null);
            new Schedule(Train2007, new LeibitTime(13, 6), new LeibitTime(13, 8), TestdorfTrack1, _ALL_DAYS, eScheduleDirection.LeftToRight, eHandling.StopPassengerTrain, String.Empty, null);

            var Train86312 = new Train(86312, "GC", "Irgendwo", "Linksdorf Gbf");
            Area.Trains.TryAdd(86312, Train86312);
            new Schedule(Train86312, null, new LeibitTime(13, 12), ProbeTrack1, new List<eDaysOfService> { eDaysOfService.Tuesday, eDaysOfService.Thursday, eDaysOfService.Friday }, eScheduleDirection.RightToLeft, eHandling.Transit, String.Empty, null);
            new Schedule(Train86312, null, new LeibitTime(13, 8), TestdorfTrack3, new List<eDaysOfService> { eDaysOfService.Tuesday, eDaysOfService.Thursday, eDaysOfService.Friday }, eScheduleDirection.RightToLeft, eHandling.Transit, "Ü 12345", null);

            var Train12345 = new Train(12345, "RB", "Rechtsheim", "Probe");
            Area.Trains.TryAdd(12345, Train12345);
            new Schedule(Train12345, new LeibitTime(13, 6), new LeibitTime(13, 10), TestdorfTrack4, new List<eDaysOfService> { eDaysOfService.Monday, eDaysOfService.Tuesday, eDaysOfService.Wednesday, eDaysOfService.Thursday, eDaysOfService.Friday }, eScheduleDirection.RightToLeft, eHandling.StopPassengerTrain, "@ 86312", null);

            var ProbeSchedule12345 = new Schedule(Train12345, new LeibitTime(13, 15), null, ProbeTrack3, new List<eDaysOfService> { eDaysOfService.Monday, eDaysOfService.Tuesday, eDaysOfService.Wednesday, eDaysOfService.Thursday, eDaysOfService.Friday }, eScheduleDirection.RightToLeft, eHandling.Destination, "> 12346", null);
            ProbeSchedule12345.LocalOrders = String.Format("12345 RE     tgl   --> Bahnsteigwende für RB 12346", Environment.NewLine);

            var Train12346 = new Train(12346, "RB", "Probe", "Rechtsheim");
            Area.Trains.TryAdd(12346, Train12346);
            new Schedule(Train12346, null, new LeibitTime(13, 20), ProbeTrack3, new List<eDaysOfService> { eDaysOfService.Monday, eDaysOfService.Tuesday, eDaysOfService.Wednesday, eDaysOfService.Thursday, eDaysOfService.Friday }, eScheduleDirection.LeftToRight, eHandling.Start, "< 12345", null);
            new Schedule(Train12346, new LeibitTime(13, 18), new LeibitTime(13, 20), ProbeTrack3, new List<eDaysOfService> { eDaysOfService.Sunday }, eScheduleDirection.LeftToRight, eHandling.StopPassengerTrain, string.Empty, null);
            new Schedule(Train12346, new LeibitTime(13, 25), new LeibitTime(13, 26), TestdorfTrack1B, new List<eDaysOfService> { eDaysOfService.Monday, eDaysOfService.Tuesday, eDaysOfService.Wednesday, eDaysOfService.Thursday, eDaysOfService.Friday, eDaysOfService.Sunday }, eScheduleDirection.LeftToRight, eHandling.StopPassengerTrain, String.Empty, null);

            var Train60652 = new Train(60652, "RC", "Testdorf", "Rechtsheim");
            Area.Trains.TryAdd(60652, Train60652);
            new Schedule(Train60652, null, new LeibitTime(13, 15), TestdorfTrack4A, new List<eDaysOfService> { eDaysOfService.Wednesday }, eScheduleDirection.RightToLeft, eHandling.Start, String.Empty, null);
            new Schedule(Train60652, null, new LeibitTime(13, 41), TestdorfTrack2, new List<eDaysOfService> { eDaysOfService.Wednesday }, eScheduleDirection.LeftToRight, eHandling.Transit, String.Empty, null);

            var ProbeSchedule60652 = new Schedule(Train60652, new LeibitTime(13, 21), new LeibitTime(13, 35), ProbeTrack2, new List<eDaysOfService> { eDaysOfService.Wednesday }, eScheduleDirection.RightToRight, eHandling.StopFreightTrain, "s. öAno", null);
            ProbeSchedule60652.LocalOrders = String.Format("60652 RC     Mo-Fr  --> 3 Wg (80 m) für RB 14300 Mo-Fr (Ü am Bstg),{0}                        Tfz von Schl Zug zum Bw (über Gl. 12){0}             Sa     --> 3 Wg (80 m) für RB 14302 Sa (bleiben stehen),{0}                        Tfz von Schl Zug zum Bw (über Gl. 12){0}             So     --> 3 Wg (80 m) für RB 14306 So (bleiben stehen),{0}                        Tfz von Schl Zug zum Bw (über Gl. 12)", Environment.NewLine);

            var Train66789 = new Train(66789, "IRC", "Linksdorf Gbf", "Irgendwo");
            Area.Trains.TryAdd(66789, Train66789);
            new Schedule(Train66789, null, new LeibitTime(13, 45), ProbeTrack2, new List<eDaysOfService> { eDaysOfService.Tuesday, eDaysOfService.Wednesday, eDaysOfService.Thursday, eDaysOfService.Saturday }, eScheduleDirection.LeftToRight, eHandling.Transit, String.Empty, null);
            new Schedule(Train66789, new LeibitTime(13, 50), new LeibitTime(15, 0), TestdorfTrack1, new List<eDaysOfService> { eDaysOfService.Tuesday, eDaysOfService.Wednesday, eDaysOfService.Thursday, eDaysOfService.Saturday }, eScheduleDirection.LeftToRight, eHandling.StopPassengerTrain, "Ankunft 13:50 Uhr", null);

            var Train65123 = new Train(65123, "KC", "Irgendwo", "Linksdorf Gbf");
            Area.Trains.TryAdd(65123, Train65123);
            new Schedule(Train65123, null, new LeibitTime(0, 12), TestdorfTrack4, new List<eDaysOfService> { eDaysOfService.Friday, eDaysOfService.Saturday }, eScheduleDirection.RightToLeft, eHandling.Start, String.Empty, null);
            new Schedule(Train65123, new LeibitTime(23, 41), new LeibitTime(0, 12), TestdorfTrack4, new List<eDaysOfService> { eDaysOfService.Monday, eDaysOfService.Tuesday, eDaysOfService.Wednesday, eDaysOfService.Thursday }, eScheduleDirection.RightToLeft, eHandling.StopPassengerTrain, "Ankunft 23:41 Uhr", null);
            new Schedule(Train65123, null, new LeibitTime(0, 12), ProbeTrack1, new List<eDaysOfService> { eDaysOfService.Monday, eDaysOfService.Tuesday, eDaysOfService.Wednesday, eDaysOfService.Thursday }, eScheduleDirection.RightToLeft, eHandling.Transit, String.Empty, null);
            new Schedule(Train65123, null, new LeibitTime(0, 12), ProbeTrack1, new List<eDaysOfService> { eDaysOfService.Friday, eDaysOfService.Saturday }, eScheduleDirection.RightToLeft, eHandling.Transit, String.Empty, null);

            EstwTestdorf.IsLoaded = true;
            return Area;
        }
    }
}
