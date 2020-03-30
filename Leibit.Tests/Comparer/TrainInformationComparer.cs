using Leibit.Entities.LiveData;

namespace Leibit.Tests.Comparer
{
    internal class TrainInformationComparer : LeibitComparer<TrainInformation>
    {
        private static TrainInformationComparer m_Instance;

        private TrainInformationComparer()
        {
        }

        internal static TrainInformationComparer Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new TrainInformationComparer();

                return m_Instance;
            }
        }

        internal override void Compare(TrainInformation expected, TrainInformation actual)
        {
            TrainComparer.Instance.Compare(expected.Train, actual.Train);
            BlockComparer.Instance.Compare(expected.Block, actual.Block);

            CompareScalar(expected.Delay, actual.Delay, "Delay");
            CompareScalar(expected.Direction, actual.Direction, "Direction");

            CompareList(expected.Schedules, actual.Schedules, LiveScheduleComparer.Instance, "LiveSchedules");
        }

        internal override Identifier GetIdentifier(TrainInformation value)
        {
            return TrainComparer.Instance.GetIdentifier(value.Train);
        }
    }
}
