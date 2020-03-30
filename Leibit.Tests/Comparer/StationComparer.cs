using Leibit.Entities.Common;

namespace Leibit.Tests.Comparer
{
    internal class StationComparer : LeibitComparer<Station>
    {
        private static StationComparer m_Instance;

        private StationComparer()
        {
        }

        internal static StationComparer Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new StationComparer();

                return m_Instance;
            }
        }

        internal override void Compare(Station expected, Station actual)
        {
            CompareScalar(expected.Name, actual.Name, "StationName");
            CompareScalar(expected.ShortSymbol, actual.ShortSymbol, "ShortSymbol");
            CompareScalar(expected.RefNumber, actual.RefNumber, "RefNumber");
            CompareScalar(expected.ScheduleFile, actual.ScheduleFile, "ScheduleFile");
            CompareScalar(expected.LocalOrderFile, actual.LocalOrderFile, "LocalOrderFile");

            CompareScalar(ESTWComparer.Instance.GetIdentifier(expected.ESTW), ESTWComparer.Instance.GetIdentifier(actual.ESTW), "Estw");

            CompareList(expected.Tracks, actual.Tracks, TrackComparer.Instance, "Tracks");
            CompareList(expected.Schedules, actual.Schedules, ScheduleComparer.Instance, "Schedules");
        }

        internal override Identifier GetIdentifier(Station value)
        {
            return new Identifier(value.ShortSymbol);
        }
    }
}
