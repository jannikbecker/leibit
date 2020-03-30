using Leibit.Entities.LiveData;

namespace Leibit.Tests.Comparer
{
    internal class LiveScheduleComparer : LeibitComparer<LiveSchedule>
    {
        private static LiveScheduleComparer m_Instance;

        private LiveScheduleComparer()
        {
        }

        internal static LiveScheduleComparer Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new LiveScheduleComparer();

                return m_Instance;
            }
        }

        internal override void Compare(LiveSchedule expected, LiveSchedule actual)
        {
            ScheduleComparer.Instance.Compare(expected.Schedule, actual.Schedule);

            CompareScalar(expected.LiveArrival, actual.LiveArrival, "LiveArrival");
            CompareScalar(expected.LiveDeparture, actual.LiveDeparture, "LiveDeparture");
            CompareScalar(expected.ExpectedArrival, actual.ExpectedArrival, "ExpectedArrival");
            CompareScalar(expected.ExpectedDeparture, actual.ExpectedDeparture, "ExpectedDeparture");

            if (expected.LiveTrack != null && actual.LiveTrack != null)
                TrackComparer.Instance.Compare(expected.LiveTrack, actual.LiveTrack);
            else
                CompareScalar(expected.LiveTrack, actual.LiveTrack, "LiveTrack");

            CompareList(expected.Delays, actual.Delays, DelayInfoComparer.Instance, "Delays");
        }

        internal override Identifier GetIdentifier(LiveSchedule value)
        {
            return ScheduleComparer.Instance.GetIdentifier(value.Schedule);
        }
    }
}
