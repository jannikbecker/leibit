using Leibit.Entities.LiveData;

namespace Leibit.Tests.Comparer
{
    internal class SharedDelayComparer : LeibitComparer<SharedDelay>
    {
        private static SharedDelayComparer m_Instance;

        private SharedDelayComparer()
        {
        }

        internal static SharedDelayComparer Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new SharedDelayComparer();

                return m_Instance;
            }
        }

        internal override void Compare(SharedDelay expected, SharedDelay actual)
        {
            CompareScalar(expected.TrainNumber, actual.TrainNumber, "TrainNumber");
            CompareScalar(expected.StationShortSymbol, actual.StationShortSymbol, "StationShortSymbol");
            CompareScalar(expected.ScheduleIndex, actual.ScheduleIndex, "ScheduleIndex");
            CompareScalar(expected.Type, actual.Type, "Type");
            CompareScalar(expected.Minutes, actual.Minutes, "Minutes");
            CompareScalar(expected.Reason, actual.Reason, "Reason");
            CompareScalar(expected.CausedBy, actual.CausedBy, "CausedBy");
        }

        internal override Identifier GetIdentifier(SharedDelay value)
        {
            return new Identifier(value.TrainNumber, value.StationShortSymbol, value.ScheduleIndex, value.Type);
        }
    }
}
