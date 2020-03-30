using Leibit.Entities.Common;

namespace Leibit.Tests.Comparer
{
    internal class BlockComparer : LeibitComparer<Block>
    {
        private static BlockComparer m_Instance;

        private BlockComparer()
        {
        }

        internal static BlockComparer Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new BlockComparer();

                return m_Instance;
            }
        }

        internal override void Compare(Block expected, Block actual)
        {
            CompareScalar(expected.Name, actual.Name, "BlockName");
            CompareScalar(expected.Direction, actual.Direction, "Direction");

            CompareScalar(TrackComparer.Instance.GetIdentifier(expected.Track), TrackComparer.Instance.GetIdentifier(actual.Track), "Track");
        }

        internal override Identifier GetIdentifier(Block value)
        {
            return new Identifier(value.Name);
        }
    }
}
