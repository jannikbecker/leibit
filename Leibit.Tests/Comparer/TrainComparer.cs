using Leibit.Entities.Scheduling;

namespace Leibit.Tests.Comparer
{
    internal class TrainComparer : LeibitComparer<Train>
    {
        private static TrainComparer m_Instance;

        private TrainComparer()
        {
        }

        internal static TrainComparer Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new TrainComparer();

                return m_Instance;
            }
        }

        internal override void Compare(Train expected, Train actual)
        {
            CompareScalar(expected.Number, actual.Number, "TrainNumber");
            CompareScalar(expected.Type, actual.Type, "Type");
            CompareScalar(expected.Start, actual.Start, "Start");
            CompareScalar(expected.Destination, actual.Destination, "Destination");
            CompareScalar(expected.Composition, actual.Composition, "Composition");

            CompareList(expected.Schedules, actual.Schedules, ScheduleComparer.Instance, "Schedules");
        }

        internal override Identifier GetIdentifier(Train value)
        {
            return new Identifier(value.Number);
        }
    }
}
