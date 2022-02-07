using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Leibit.Entities.Scheduling
{
    [DebuggerDisplay("{Number}")]
    public class Train
    {

        private int m_Number;
        private List<Schedule> m_Schedules;

        public readonly object LockSchedules = new object();

        public Train(int number)
        {
            m_Number = number;
            m_Schedules = new List<Schedule>();
            PreviousServices = new List<TrainRelation>();
            FollowUpServices = new List<TrainRelation>();
        }

        public Train(int number, string type, string start, string destination)
            : this(number)
        {
            Type = type;
            Start = start;
            Destination = destination;
        }

        public int Number
        {
            get
            {
                return m_Number;
            }
        }

        public string Type { get; set; }

        public string Start { get; set; }

        public string Destination { get; set; }

        public string Composition { get; set; }

        public List<TrainRelation> PreviousServices { get; }

        public List<TrainRelation> FollowUpServices { get; }

        public string Line { get; set; }

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
