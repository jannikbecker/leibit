using Leibit.Entities.LiveData;

namespace Leibit.Tests.Comparer
{
    internal class DelayInfoComparer : LeibitComparer<DelayInfo>
    {
        private static DelayInfoComparer m_Instance;

        private DelayInfoComparer()
        {
        }

        internal static DelayInfoComparer Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new DelayInfoComparer();

                return m_Instance;
            }
        }

        internal override void Compare(DelayInfo expected, DelayInfo actual)
        {
            CompareScalar(expected.Type, actual.Type, "Type");
            CompareScalar(expected.Minutes, actual.Minutes, "Minutes");
            CompareScalar(expected.Reason, actual.Reason, "Reason");
            CompareScalar(expected.CausedBy, actual.CausedBy, "CausedBy");
        }

        internal override Identifier GetIdentifier(DelayInfo value)
        {
            return new Identifier(value.Minutes, value.Type);
        }
    }
}
