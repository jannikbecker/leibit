
using Leibit.Core.Scheduling;
using Leibit.Entities.Common;
using System.Collections.Generic;

namespace Leibit.Entities.Scheduling
{
    public class Schedule
    {

        private Train m_Train;
        private LeibitTime m_Arrival;
        private LeibitTime m_Departure;
        private Track m_Track;
        private List<eDaysOfService> m_Days;
        private eScheduleDirection m_Direction;
        private eHandling m_Handling;
        private string m_Remark;
        private string m_LocalOrders;
        private Station m_Station;

        public Schedule(Train train, Station station)
        {
            m_Train = train;
            m_Station = station;
            m_Days = new List<eDaysOfService>();
            m_Direction = eScheduleDirection.Unknown;
            m_Handling = eHandling.Unknown;
        }

        public Schedule(Train train, LeibitTime arrival, LeibitTime departure, Track track, List<eDaysOfService> days, eScheduleDirection direction, eHandling handling, string remark)
        {
            m_Train = train;
            m_Arrival = arrival;
            m_Departure = departure;
            m_Track = track;
            m_Days = days;
            m_Direction = direction;
            m_Handling = handling;
            m_Remark = remark;

            if (m_Train != null)
                m_Train.AddSchedule(this);

            if (m_Track != null)
            {
                m_Track.Station.Schedules.Add(this);
                m_Station = m_Track.Station;
            }
        }

        public Schedule(Train train, LeibitTime arrival, LeibitTime departure, Track track, List<eDaysOfService> days, eScheduleDirection direction, eHandling handling, string remark, string localOrders)
            : this(train, arrival, departure, track, days, direction, handling, remark)
        {
            m_LocalOrders = localOrders;
        }

        public Train Train
        {
            get
            {
                return m_Train;
            }
        }

        public Station Station
        {
            get
            {
                return m_Station;
            }
        }

        public LeibitTime Arrival
        {
            get
            {
                return m_Arrival;
            }
            set
            {
                m_Arrival = value;
            }
        }

        public LeibitTime Departure
        {
            get
            {
                return m_Departure;
            }
            set
            {
                m_Departure = value;
            }
        }

        public Track Track
        {
            get
            {
                return m_Track;
            }
        }

        public List<eDaysOfService> Days
        {
            get
            {
                return m_Days;
            }
        }

        public eScheduleDirection Direction
        {
            get
            {
                return m_Direction;
            }
        }

        public eHandling Handling
        {
            get
            {
                return m_Handling;
            }
        }

        public string Remark
        {
            get
            {
                return m_Remark;
            }
        }

        public string LocalOrders
        {
            get
            {
                return m_LocalOrders;
            }
            set
            {
                m_LocalOrders = value;
            }
        }

        public LeibitTime Time
        {
            get
            {
                var time = (m_Departure == null) ? m_Arrival : m_Departure;

                if (time == null)
                    return null;

                return new LeibitTime(time.Hour, time.Minute);
            }
        }

    }
}
