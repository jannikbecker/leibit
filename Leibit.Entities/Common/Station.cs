using Leibit.Entities.Scheduling;
using System.Collections.Generic;
using System.Diagnostics;

namespace Leibit.Entities.Common
{
    [DebuggerDisplay("{Name}")]
    public class Station
    {

        private string m_Name;
        private string m_ShortSymbol;
        private short m_RefNr;
        private string m_ScheduleFile;
        private string m_LocalOrderFile;
        private ESTW m_Estw;
        private List<Track> m_Tracks;
        private List<Schedule> m_Schedules;
        private object m_LockSchedules;

        public Station(string name, string shortSymbol, short refNr, string scheduleFile, string localOrderFile, ESTW estw)
        {
            m_Name = name;
            m_ShortSymbol = shortSymbol;
            m_RefNr = refNr;
            m_ScheduleFile = scheduleFile;
            m_LocalOrderFile = localOrderFile;
            m_Estw = estw;

            m_Tracks = new List<Track>();
            m_Schedules = new List<Schedule>();
            m_LockSchedules = new object();
            ScheduleFiles = new List<ScheduleFile>();

            if (estw != null)
                estw.Stations.Add(this);
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        public string ShortSymbol
        {
            get
            {
                return m_ShortSymbol;
            }
        }

        public short RefNumber
        {
            get
            {
                return m_RefNr;
            }
        }

        public ESTW ESTW
        {
            get
            {
                return m_Estw;
            }
        }

        public List<Track> Tracks
        {
            get
            {
                return m_Tracks;
            }
        }

        public List<Schedule> Schedules
        {
            get
            {
                return m_Schedules;
            }
        }

        public string ScheduleFile
        {
            get
            {
                return m_ScheduleFile;
            }
        }

        public string LocalOrderFile
        {
            get
            {
                return m_LocalOrderFile;
            }
        }

        public List<ScheduleFile> ScheduleFiles { get; }

        internal void AddSchedule(Schedule schedule)
        {
            lock (m_LockSchedules)
            {
                Schedules.Add(schedule);
            }
        }

    }
}
