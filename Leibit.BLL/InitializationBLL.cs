﻿using Leibit.Core.Common;
using Leibit.Core.Properties;
using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.Scheduling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

[assembly: InternalsVisibleTo("Leibit.Tests")]
namespace Leibit.BLL
{
    public class InitializationBLL : BLLBase
    {

        #region - Needs -
        private XmlDocument m_AreasXml;
        private SettingsBLL m_SettingsBll;
        private object m_LockXml = new object();
        #endregion

        #region - Ctor -
        public InitializationBLL()
            : base()
        {
        }
        #endregion

        #region - Properties -

        #region [AreasXml]
        private XmlDocument AreasXml
        {
            get
            {
                lock (m_LockXml)
                {
                    if (m_AreasXml == null)
                    {
                        m_AreasXml = new XmlDocument();
                        m_AreasXml.LoadXml(Resources.Areas);
                    }

                    return m_AreasXml;
                }
            }
        }
        #endregion

        #region [SettingsBLL]
        private SettingsBLL SettingsBLL
        {
            get
            {
                if (m_SettingsBll == null)
                    m_SettingsBll = new SettingsBLL();

                return m_SettingsBll;
            }
        }
        #endregion

        #region [CustomEstwXmlStream]
        internal Func<ESTW, Stream> CustomEstwXmlStream { private get; set; }
        #endregion

        #endregion

        #region - Public methods -

        #region [GetAreaInformation]
        public OperationResult<List<Area>> GetAreaInformation()
        {
            try
            {
                var Result = new OperationResult<List<Area>>();
                Result.Result = new List<Area>();

                foreach (XmlNode AreaNode in AreasXml.DocumentElement.SelectNodes("area"))
                {
                    var Area = __GetArea(AreaNode);
                    if (Area == null)
                        continue;

                    foreach (XmlNode EstwNode in AreaNode.SelectNodes("estw"))
                    {
                        __GetEstw(EstwNode, Area);
                    }

                    Result.Result.Add(Area);
                }

                Result.Succeeded = true;
                return Result;
            }
            catch (Exception ex)
            {
                return new OperationResult<List<Area>> { Message = ex.Message };
            }
        }
        #endregion

        #region [LoadESTW]
        public OperationResult<bool> LoadESTW(ESTW Estw)
        {
            try
            {
                var Result = new OperationResult<bool>();
                Estw.Stations.Clear();
                Estw.Blocks.Clear();
                Estw.IsLoaded = false;

                var PathResult = SettingsBLL.GetPath(Estw.Id);
                ValidateResult(PathResult);

                //if (PathResult.Result.IsNullOrWhiteSpace())
                //{
                //    Result.Succeeded = true;
                //    return Result;
                //}

                using (var xmlStream = __GetEstwXmlStream(Estw))
                {
                    if (xmlStream == null)
                    {
                        Result.Message = "ESTW-Projektierung nicht gefunden";
                        Result.Succeeded = false;
                        return Result;
                    }

                    var xml = new XmlDocument();
                    xml.Load(xmlStream);
                    var EstwNode = xml.DocumentElement;

                    if (EstwNode == null)
                    {
                        Result.Message = "Ungültiges ESTW";
                        Result.Succeeded = false;
                        return Result;
                    }

                    var Stations = EstwNode.SelectNodes("station");
                    Result.Result = true;

                    foreach (XmlNode StationNode in Stations)
                    {
                        Station Station = null;

                        try
                        {
                            Station = __GetStation(StationNode, Estw);
                            if (Station == null)
                                continue;

                            var Tracks = StationNode.SelectNodes("track");

                            foreach (XmlNode TrackNode in Tracks)
                            {
                                var Track = __GetTrack(TrackNode, Station);
                                if (Track == null)
                                    continue;

                                foreach (XmlNode ChildTrackNode in TrackNode.SelectNodes("track"))
                                {
                                    __GetTrack(ChildTrackNode, Station, Track);
                                }
                            }

                            foreach (XmlNode TrackNode in Tracks)
                            {
                                var TrackName = TrackNode.Attributes["name"];
                                if (TrackName == null)
                                    continue;

                                var Track = Station.Tracks.FirstOrDefault(t => t.Name == TrackName.InnerText);
                                if (Track == null)
                                    continue;

                                __GetAlternatives(TrackNode, Track);

                                foreach (XmlNode ChildTrackNode in TrackNode.SelectNodes("track"))
                                {
                                    var ChildTrackName = ChildTrackNode.Attributes["name"];
                                    if (ChildTrackName == null)
                                        continue;

                                    var ChildTrack = Station.Tracks.FirstOrDefault(t => t.Name == ChildTrackName.InnerText);
                                    if (ChildTrack == null)
                                        continue;

                                    __GetAlternatives(ChildTrackNode, ChildTrack);
                                }
                            }

                            if (PathResult.Result.IsNotNullOrWhiteSpace())
                            {
                                __GetSchedule(Station, PathResult.Result);
                                __ResolveDuplicates(Station.Schedules);
                                __GetLocalOrders(Station, PathResult.Result);
                            }

                            Result.Succeeded = true;
                        }
                        catch (Exception ex)
                        {
                            if (Station != null)
                                Estw.Stations.Remove(Station);

                            Result.Message = ex.Message;
                            Result.Result = false;
                        }
                    }
                }

                if (PathResult.Result.IsNotNullOrWhiteSpace())
                    __LoadTrainCompositions(Estw, PathResult.Result);

                Estw.SchedulesLoaded = PathResult.Result.IsNotNullOrWhiteSpace();
                Estw.IsLoaded = true;
                return Result;
            }
            catch (Exception ex)
            {
                return new OperationResult<bool> { Message = ex.Message };
            }
        }
        #endregion

        #endregion

        #region - Private helpers -

        #region [__GetArea]
        private Area __GetArea(XmlNode node)
        {
            if (node == null)
                return null;

            var AreaId = node.Attributes["id"];
            var AreaName = node.Attributes["name"];

            if (AreaId == null || AreaName == null)
                return null;

            return new Area(AreaId.InnerText, AreaName.InnerText);
        }
        #endregion

        #region [__GetEstw]
        private ESTW __GetEstw(XmlNode node, Area area)
        {
            if (node == null)
                return null;

            var EstwId = node.Attributes["id"];
            var EstwName = node.Attributes["name"];
            var EstwDataFile = node.Attributes["dataFile"];
            var InfrastructureManagerAttr = node.Attributes["infrastructureManager"];

            if (EstwId == null || EstwName == null || EstwDataFile == null || InfrastructureManagerAttr == null || !Enum.TryParse(InfrastructureManagerAttr.InnerText, true, out eInfrastructureManager InfrastructureManager))
                return null;

            return new ESTW(EstwId.InnerText, EstwName.InnerText, EstwDataFile.InnerText, InfrastructureManager, area);
        }
        #endregion

        #region [__GetStation]
        private Station __GetStation(XmlNode node, ESTW estw)
        {
            if (node == null)
                return null;

            var StationName = node.Attributes["name"];
            var StationShort = node.Attributes["short"];
            var StationNumber = node.Attributes["refNr"];
            var ScheduleFile = node.Attributes["scheduleFile"];
            var LocalOrderFile = node.Attributes["localOrderFile"];

            if (StationName == null || StationShort == null || StationNumber == null)
                return null;

            short number;

            if (!Int16.TryParse(StationNumber.InnerText, out number))
                return null;

            string Schedule = ScheduleFile == null ? null : ScheduleFile.InnerText;
            string LocalOrders = LocalOrderFile == null ? null : LocalOrderFile.InnerText;

            var station = new Station(StationName.InnerText, StationShort.InnerText, number, Schedule, LocalOrders, estw);

            foreach (XmlNode scheduleFileNode in node.SelectNodes("scheduleFile"))
            {
                var fileName = scheduleFileNode.Attributes["fileName"];

                if (fileName == null || fileName.InnerText.IsNullOrWhiteSpace())
                    continue;

                var scheduleFile = new ScheduleFile(fileName.InnerText);
                scheduleFile.Tracks = scheduleFileNode.SelectNodes("track").Cast<XmlNode>().Select(x => x.InnerText).ToList();
                station.ScheduleFiles.Add(scheduleFile);
            }

            return station;
        }
        #endregion

        #region [__GetTrack]
        private Track __GetTrack(XmlNode node, Station station, Track parent = null)
        {
            if (node == null)
                return null;

            var TrackName = node.Attributes["name"];
            var IsPlatformAttr = node.Attributes["isPlatform"];
            var CalculateDelayAttr = node.Attributes["calculateDelay"];
            var DisplayNameAttr = node.Attributes["displayName"];
            var DisplaySubNameAttr = node.Attributes["displaySubName"];

            string Name = TrackName == null ? null : TrackName.InnerText;

            bool IsPlatform = true;
            if (IsPlatformAttr != null && !bool.TryParse(IsPlatformAttr.InnerText, out IsPlatform))
                return null;

            bool CalculateDelay = true;
            if (CalculateDelayAttr != null && !bool.TryParse(CalculateDelayAttr.InnerText, out CalculateDelay))
                return null;

            var Track = new Track(Name, IsPlatform, CalculateDelay, station, parent, DisplayNameAttr?.InnerText, DisplaySubNameAttr?.InnerText);
            __GetBlocks(node, Track);

            return Track;
        }
        #endregion

        #region [__GetBlocks]
        private void __GetBlocks(XmlNode trackNode, Track track)
        {
            if (trackNode == null)
                return;

            foreach (XmlNode BlockNode in trackNode.SelectNodes("block"))
            {
                var BlockName = BlockNode.Attributes["name"];
                var DirectionAttr = BlockNode.Attributes["direction"];

                if (BlockName == null)
                    continue;

                eBlockDirection Direction = eBlockDirection.Both;
                if (DirectionAttr != null && !Enum.TryParse(DirectionAttr.InnerText, true, out Direction))
                    continue;

                new Block(BlockName.InnerText, Direction, track);
            }
        }
        #endregion

        #region [__GetAlternatives]
        private void __GetAlternatives(XmlNode trackNode, Track track)
        {
            if (trackNode == null)
                return;

            foreach (XmlNode AlternativeNode in trackNode.SelectNodes("alternative"))
            {
                var TrackAttr = AlternativeNode.Attributes["track"];

                if (TrackAttr == null)
                    continue;

                var Alternative = track.Station.Tracks.FirstOrDefault(t => t.Name == TrackAttr.InnerText && t != track);

                if (Alternative != null)
                    track.Alternatives.Add(Alternative);
            }
        }
        #endregion

        #region [__GetSchedule]
        private void __GetSchedule(Station station, string path)
        {
            if (station == null)
                return;

            if (station.ScheduleFile.IsNotNullOrWhiteSpace())
                __LoadScheduleFromFile(station, Path.Combine(path, Constants.SCHEDULE_FOLDER, station.ScheduleFile), new List<string>());

            foreach (var scheduleFile in station.ScheduleFiles)
                __LoadScheduleFromFile(station, Path.Combine(path, Constants.SCHEDULE_FOLDER, scheduleFile.FileName), scheduleFile.Tracks);
        }
        #endregion

        #region [__LoadScheduleFromFile]
        private void __LoadScheduleFromFile(Station station, string scheduleFile, List<string> tracks)
        {
            if (!File.Exists(scheduleFile))
                return;

            // Encoding e.g. for German Umlaute
            using (var reader = new StreamReader(scheduleFile, Encoding.GetEncoding("iso-8859-1")))
            {
                reader.ReadLine();

                var header = reader.ReadLine();

                if (!__GetBounds(header, "VON", out int startFrom, out int startLength)
                    || !__GetBounds(header, "VT", out int daysFrom, out int daysLength)
                    || !__GetBounds(header, "BEMERKUNGEN", out int remarkFrom, out int remarkLength)
                    || !__GetBounds(header, "NACH", out int destinationFrom, out _))
                    return;

                while (!reader.EndOfStream)
                {
                    string ScheduleLine = reader.ReadLine();

                    if (ScheduleLine.StartsWith("240000 999999"))
                        break;

                    if (ScheduleLine.Length < 91)
                        continue;

                    string sHour = ScheduleLine.Substring(0, 2);
                    string sMinute = ScheduleLine.Substring(2, 2);
                    string sStop = ScheduleLine.Substring(4, 2);
                    string sHandling = ScheduleLine.Substring(6, 1);
                    string sTrain = ScheduleLine.Substring(7, 6);
                    string sDirection = ScheduleLine.Substring(13, 1);
                    string Type = ScheduleLine.Substring(14, 3).Trim();
                    string sTrack = ScheduleLine.Substring(18, 5).Trim();
                    string Start = ScheduleLine.Substring(startFrom, startLength).Trim();
                    string sDays = ScheduleLine.Substring(daysFrom, daysLength).Trim().ToLower();
                    string Remark = ScheduleLine.Substring(remarkFrom, remarkLength).Trim();
                    string Destination = ScheduleLine.Substring(destinationFrom).Trim();

                    if (sTrain.Length > 5)
                        sTrain = sTrain.Substring(sTrain.Length - 5);

                    int TrainNr;
                    if (!Int32.TryParse(sTrain, out TrainNr))
                        continue;

                    if (tracks.Any() && !tracks.Contains(sTrack))
                        continue;

                    Train Train = station.ESTW.Area.Trains.AddOrUpdate(TrainNr, new Train(TrainNr, Type, Start, Destination), (trainNo, existingTrain) =>
                    {
                        existingTrain.Type = Type;
                        existingTrain.Start = Start;
                        existingTrain.Destination = Destination;
                        return existingTrain;
                    });

                    int Hour;
                    if (!Int32.TryParse(sHour, out Hour))
                        continue;

                    int Minute;
                    if (!Int32.TryParse(sMinute, out Minute))
                        continue;

                    int Stop;
                    if (!Int32.TryParse(sStop, out Stop))
                        continue;

                    eScheduleDirection Direction;

                    switch (sDirection)
                    {
                        case "<":
                            Direction = eScheduleDirection.RightToLeft;
                            break;
                        case "L":
                            Direction = eScheduleDirection.LeftToLeft;
                            break;
                        case "R":
                            Direction = eScheduleDirection.RightToRight;
                            break;
                        default:
                            Direction = eScheduleDirection.LeftToRight;
                            break;
                    }

                    eHandling Handling = eHandling.Unknown;

                    if (Stop == 60)
                        Handling = eHandling.Start;
                    else if (sHandling == " ")
                        Handling = eHandling.StopPassengerTrain;
                    else if (sHandling == "X")
                        Handling = eHandling.StopFreightTrain;
                    else if (sHandling == "A" || sHandling == "Ä")
                        Handling = eHandling.Destination;
                    else if (sHandling == "L")
                        Handling = eHandling.StaffChange;
                    else if (sHandling == "D" || sHandling == "d")
                        Handling = eHandling.Transit;
                    else if (sHandling == "(")
                    {
                        Handling = eHandling.Transit;
                        Direction = eScheduleDirection.RightToLeft;
                    }
                    else if (sHandling == ")")
                    {
                        Handling = eHandling.Transit;
                        Direction = eScheduleDirection.LeftToRight;
                    }

                    var Days = LeibitTime.ParseDays(sDays);

                    LeibitTime Departure = new LeibitTime(Hour, Minute);
                    LeibitTime Arrival;

                    if (Handling == eHandling.Transit || Handling == eHandling.Start)
                        Arrival = null;
                    else if (Handling == eHandling.StaffChange && Stop == 0)
                        Arrival = Departure.AddMinutes(Constants.STAFF_CHANGE_STOPTIME * -1);
                    else if (Handling == eHandling.Destination)
                    {
                        Arrival = Departure;
                        Departure = null;
                    }
                    else
                        Arrival = Departure.AddMinutes(Stop * -1);

                    Track Track = station.Tracks.FirstOrDefault(t => t.Name == sTrack);

                    if (Track == null)
                        Track = new Track(sTrack, true, false, station, null);

                    new Schedule(Train, Arrival, Departure, Track, Days, Direction, Handling, Remark);
                }
            }
        }
        #endregion

        #region [__ResolveDuplicates]
        private void __ResolveDuplicates(List<Schedule> schedules)
        {
            var StartSchedules = schedules.Where(s => s.Handling == eHandling.Start).ToList();

            foreach (var Schedule in StartSchedules)
            {
                var Duplicate = schedules.FirstOrDefault(s => s.Train == Schedule.Train
                                                           && s.Track == Schedule.Track
                                                           && s.Handling != eHandling.Start
                                                           && __AreSchedulesClose(s, Schedule)
                                                           && s != Schedule);

                if (Duplicate != null)
                {
                    new Schedule(Schedule.Train, Duplicate.Arrival, Schedule.Departure, Schedule.Track, Schedule.Days, Schedule.Direction, eHandling.StopPassengerTrain, Schedule.Remark, Schedule.LocalOrders);
                    schedules.Remove(Schedule);
                    schedules.Remove(Duplicate);
                    Schedule.Train.RemoveSchedule(Duplicate);
                    Schedule.Train.RemoveSchedule(Schedule);
                }
            }
        }
        #endregion

        #region [__GetLocalOrders]
        private void __GetLocalOrders(Station station, string path)
        {
            if (station == null || station.LocalOrderFile.IsNullOrWhiteSpace())
                return;

            string LocalOrderFile = Path.Combine(path, Constants.LOCAL_ORDERS_FOLDER, station.LocalOrderFile);

            if (!File.Exists(LocalOrderFile))
                return;

            // Encoding.Default e.g. for German Umlaute
            using (var reader = new StreamReader(LocalOrderFile, Encoding.GetEncoding("iso-8859-1")))
            {
                int CurrentTrainNumber = 0;
                var Content = new StringBuilder();

                while (!reader.EndOfStream)
                {
                    var Line = reader.ReadLine();

                    if (Line.StartsWith("*"))
                    {
                        var match = Regex.Match(Line, @"^\*( )+[a-z]+( )+([0-9]+)", RegexOptions.IgnoreCase);

                        if (match != null && match.Success)
                        {
                            if (int.TryParse(match.Groups[3].Value, out int TrainNumber))
                            {
                                __SetLocalOrders(station, CurrentTrainNumber, Content.ToString());
                                CurrentTrainNumber = TrainNumber;
                                Content = new StringBuilder();
                            }
                        }
                    }
                    else
                    {
                        var LineParts = Regex.Split(Line, @"\s");

                        if (LineParts[0].IsNotNullOrEmpty())
                        {
                            // Line does not start with whitespace
                            __SetLocalOrders(station, CurrentTrainNumber, Content.ToString());
                            CurrentTrainNumber = 0;
                            Content = new StringBuilder();
                        }

                        if (int.TryParse(LineParts[0], out int TrainNumber))
                            CurrentTrainNumber = TrainNumber;
                    }

                    Content.AppendLine(Line);
                }

                __SetLocalOrders(station, CurrentTrainNumber, Content.ToString());
            }
        }
        #endregion

        #region [__SetLocalOrders]
        private void __SetLocalOrders(Station station, int trainNumber, string content)
        {
            if (trainNumber == 0)
                return;

            var Schedules = station.Schedules.Where(s => s.Train.Number == trainNumber);
            Schedules.ForEach(s => s.LocalOrders = content.TrimEnd());
        }
        #endregion

        #region [__LoadTrainCompositions]
        private void __LoadTrainCompositions(ESTW estw, string path)
        {
            var compositionFilesPath = Path.Combine(path, Constants.TRAIN_COMPOSITION_FOLDER);

            if (!Directory.Exists(compositionFilesPath))
                return;

            foreach (var file in Directory.EnumerateFiles(compositionFilesPath, "*.zug"))
            {
                var fileInfo = new FileInfo(file);
                var sTrainNumber = fileInfo.Name.Replace(fileInfo.Extension, string.Empty).Replace("_", string.Empty);

                if (!int.TryParse(sTrainNumber, out int trainNumber))
                    continue;

                if (!estw.Area.Trains.ContainsKey(trainNumber))
                    continue;

                using (var streamReader = new StreamReader(file, Encoding.GetEncoding("iso-8859-1")))
                {
                    var content = streamReader.ReadToEnd();

                    if (content != null)
                        content = content.Trim();

                    if (content.IsNotNullOrEmpty())
                        estw.Area.Trains[trainNumber].Composition = content;
                }
            }
            #endregion
        }

        #region [__AreSchedulesClose]
        private bool __AreSchedulesClose(Schedule schedule1, Schedule schedule2)
        {
            if (schedule1.Days.Count != schedule2.Days.Count)
                return false;

            var times1 = schedule1.Days.Select(day => new LeibitTime(day, schedule1.Time.Hour, schedule1.Time.Minute)).ToList();
            var times2 = schedule2.Days.Select(day => new LeibitTime(day, schedule2.Time.Hour, schedule2.Time.Minute)).ToList();

            foreach (var time1 in times1)
            {
                if (!times2.Any(time2 => time1 > time2.AddHours(-12)))
                    return false;
            }

            return true;
        }
        #endregion

        #region [__GetBounds]
        private bool __GetBounds(string headerLine, string header, out int start, out int length)
        {
            var match = Regex.Match(headerLine, $"{header}_*");

            if (match != null && match.Success)
            {
                start = match.Index;
                length = match.Length;
                return true;
            }
            else
            {
                start = 0;
                length = 0;
                return false;
            }
        }
        #endregion

        #region [__GetEstwXmlStream]
        private Stream __GetEstwXmlStream(ESTW estw)
        {
            if (CustomEstwXmlStream == null)
            {
                var assembly = typeof(Resources).Assembly;
                return assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Data.{estw.Area.Id}.{estw.Id}.xml");
            }
            else
                return CustomEstwXmlStream(estw);
        }
        #endregion

        #endregion

    }
}
