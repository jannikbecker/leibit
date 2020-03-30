using Leibit.Entities.Scheduling;

namespace Leibit.Tests.Comparer
{
    internal class ScheduleComparer : LeibitComparer<Schedule>
    {
        private static ScheduleComparer m_Instance;

        private ScheduleComparer()
        {
        }

        internal static ScheduleComparer Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new ScheduleComparer();

                return m_Instance;
            }
        }

        internal override void Compare(Schedule expected, Schedule actual)
        {
            CompareScalar(expected.Arrival, actual.Arrival, "Arrival");
            CompareScalar(expected.Departure, actual.Departure, "Departure");
            CompareScalar(expected.Remark, actual.Remark, "Remark");
            CompareScalar(expected.LocalOrders, actual.LocalOrders, "LocalOrders");
            CompareList(expected.Days, actual.Days, DayComparer.Instance, "Days");
            CompareScalar(expected.Direction, actual.Direction, "Direction");

            CompareScalar(StationComparer.Instance.GetIdentifier(expected.Station), StationComparer.Instance.GetIdentifier(actual.Station), "Station");
            CompareScalar(TrainComparer.Instance.GetIdentifier(expected.Train), TrainComparer.Instance.GetIdentifier(actual.Train), "Train");

            if (expected.Track != null && actual.Track != null)
                TrackComparer.Instance.Compare(expected.Track, actual.Track);
            else
                CompareScalar(expected.Track, actual.Track, "Track");
        }

        internal override Identifier GetIdentifier(Schedule value)
        {
            int days = 0;
            value.Days.ForEach(d => days |= (int)d);

            return new Identifier(value.Train.Number, value.Station.ShortSymbol, value.Direction, days);
        }
    }
}
