using Leibit.Entities.Common;

namespace Leibit.Tests.Comparer
{
    internal class AreaComparer : LeibitComparer<Area>
    {
        private static AreaComparer m_Instance;

        private AreaComparer()
        {
        }

        internal static AreaComparer Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new AreaComparer();

                return m_Instance;
            }
        }

        internal override void Compare(Area expected, Area actual)
        {
            CompareScalar(expected.Id, actual.Id, "AreaId");
            CompareScalar(expected.Name, actual.Name, "AreaName");

            CompareList(expected.ESTWs, actual.ESTWs, ESTWComparer.Instance, "ESTWs");
            CompareDictionary(expected.Trains, actual.Trains, TrainComparer.Instance, "Trains");
        }

        internal override Identifier GetIdentifier(Area value)
        {
            return new Identifier(value.Id);
        }
    }
}
