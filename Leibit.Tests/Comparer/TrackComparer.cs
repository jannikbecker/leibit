using Leibit.Entities.Common;

namespace Leibit.Tests.Comparer
{
    internal class TrackComparer : LeibitComparer<Track>
    {
        private static TrackComparer m_Instance;

        private TrackComparer()
        {
        }

        internal static TrackComparer Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new TrackComparer();

                return m_Instance;
            }
        }

        internal override void Compare(Track expected, Track actual)
        {
            CompareScalar(expected.Name, actual.Name, "TrackName");
            CompareScalar(expected.IsPlatform, actual.IsPlatform, "IsPlatform");
            CompareScalar(expected.CalculateDelay, actual.CalculateDelay, "CalculateDelay");

            if (expected.Parent != null && actual.Parent != null)
                CompareScalar(GetIdentifier(expected.Parent), GetIdentifier(actual.Parent), "Parent");
            else
                CompareScalar(expected.Parent, actual.Parent, "Parent");

            CompareScalar(StationComparer.Instance.GetIdentifier(expected.Station), StationComparer.Instance.GetIdentifier(actual.Station), "Station");

            CompareList(expected.Blocks, actual.Blocks, BlockComparer.Instance, "Blocks");
            CompareList(expected.Alternatives, actual.Alternatives, this, "Alternatives");
        }

        internal override Identifier GetIdentifier(Track value)
        {
            return new Identifier((value.Name == null ? "" : value.Name), value.Station.ShortSymbol);
        }
    }
}
