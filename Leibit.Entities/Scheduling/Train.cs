using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Leibit.Entities.Scheduling
{
    [DebuggerDisplay("{Number}")]
    public class Train
    {

        private int m_Number;
        private string m_Type;
        private string m_Start;
        private string m_Destination;
        private List<Schedule> m_Schedules;

        public readonly object LockSchedules = new object();

        public Train(int number)
        {
            m_Number = number;
            m_Schedules = new List<Schedule>();
        }

        public Train(int number, string type, string start, string destination)
            : this(number)
        {
            m_Type = type;
            m_Start = start;
            m_Destination = destination;
        }

        public int Number
        {
            get
            {
                return m_Number;
            }
        }

        public string Type
        {
            get
            {
                return m_Type;
            }
        }

        public string Start
        {
            get
            {
                return m_Start;
            }
        }

        public string Destination
        {
            get
            {
                return m_Destination;
            }
        }

        public string Composition { get; set; }

        public ReadOnlyCollection<Schedule> Schedules
        {
            get
            {
                lock (LockSchedules)
                {
                    return m_Schedules.AsReadOnly();
                }
            }
        }

        public void AddSchedule(Schedule schedule)
        {
            lock (LockSchedules)
            {
                m_Schedules.Add(schedule);
            }
        }

        public void RemoveSchedule(Schedule schedule)
        {
            lock (LockSchedules)
            {
                m_Schedules.Remove(schedule);
            }
        }

    }
}
