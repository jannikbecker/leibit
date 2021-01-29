using Leibit.Core.Scheduling;
using Leibit.Entities.Common;
using Leibit.Entities.Scheduling;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Leibit.Entities.LiveData
{
    [DebuggerDisplay("{Train.Number}")]
    public class TrainInformation
    {
        private Train m_Train;
        private List<LiveSchedule> m_LiveSchedules;
        private Block m_Block;
        private List<Block> m_BlockHistory;

        public readonly object LockSchedules = new object();

        public TrainInformation(Train train)
        {
            m_Train = train;
            m_LiveSchedules = new List<LiveSchedule>();
            m_BlockHistory = new List<Block>();
        }

        public Train Train
        {
            get
            {
                return m_Train;
            }
        }

        public int Delay
        {
            get;
            set;
        }

        public Block Block
        {
            get => m_Block;
            set
            {
                if (value != null && value != m_Block)
                    m_BlockHistory.Add(value);

                m_Block = value;
            }
        }

        public List<Block> BlockHistory => m_BlockHistory;

        public eBlockDirection Direction
        {
            get;
            set;
        }

        public LeibitTime LastModified
        {
            get;
            set;
        }

        public ReadOnlyCollection<LiveSchedule> Schedules
        {
            get
            {
                lock (LockSchedules)
                {
                    return m_LiveSchedules.ToList().AsReadOnly();
                }
            }
        }

        public void AddSchedule(LiveSchedule schedule)
        {
            lock (LockSchedules)
            {
                m_LiveSchedules.Add(schedule);
            }

            SortSchedules();
        }

        public void TruncateSchedules()
        {
            lock (LockSchedules)
            {
                m_LiveSchedules.Clear();
            }
        }

        internal void SortSchedules()
        {
            lock (LockSchedules)
            {
                //m_LiveSchedules = m_LiveSchedules.OrderBy(s => s.Schedule.Time == null).ThenBy(s => s.Schedule.Time).ToList();
                m_LiveSchedules.Sort();
            }
        }
    }
}
