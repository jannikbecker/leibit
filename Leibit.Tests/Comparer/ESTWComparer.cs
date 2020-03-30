using Leibit.Entities.Common;

namespace Leibit.Tests.Comparer
{
    internal class ESTWComparer : LeibitComparer<ESTW>
    {
        private static ESTWComparer m_Instance;

        private ESTWComparer()
        {
        }

        internal static ESTWComparer Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new ESTWComparer();

                return m_Instance;
            }
        }

        internal override void Compare(ESTW expected, ESTW actual)
        {
            CompareScalar(expected.Id, actual.Id, "EstwId");
            CompareScalar(expected.Name, actual.Name, "EstwName");
            CompareScalar(expected.DataFile, actual.DataFile, "DataFile");
            CompareScalar(expected.IsLoaded, actual.IsLoaded, "IsLoaded");

            CompareScalar(AreaComparer.Instance.GetIdentifier(expected.Area), AreaComparer.Instance.GetIdentifier(actual.Area), "Area");

            CompareList(expected.Stations, actual.Stations, StationComparer.Instance, "Stations");
            CompareDictionary(expected.Blocks, actual.Blocks, BlockComparer.Instance, "Blocks");
        }

        internal override Identifier GetIdentifier(ESTW value)
        {
            return new Identifier(value.Id);
        }
    }
}
