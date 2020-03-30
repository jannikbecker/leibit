using Leibit.Core.Scheduling;

namespace Leibit.Tests.Comparer
{
    internal class DayComparer : LeibitComparer<eDaysOfService>
    {
        private static DayComparer m_Instance;

        private DayComparer()
        {
        }

        internal static DayComparer Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new DayComparer();

                return m_Instance;
            }
        }

        internal override void Compare(eDaysOfService expected, eDaysOfService actual)
        {
            CompareScalar((int)expected, (int)actual, "Day");
        }

        internal override Identifier GetIdentifier(eDaysOfService value)
        {
            return new Identifier((int)value);
        }
    }
}
