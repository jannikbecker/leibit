
namespace Leibit.BLL
{
    internal static class Constants
    {
        internal const int STAFF_CHANGE_STOPTIME = 4;
        internal const int PERS_TRAIN_STOPTIME = 2;

        internal const string SHARED_DELAY_FOLDER = "Kommunikation";
        internal const string SHARED_DELAY_PREFIX = "leibit_delay_";
        internal const string SHARED_DELAY_FILE_TEMPLATE = SHARED_DELAY_PREFIX + "{0}_{1}.dat";
        internal const string SCHEDULE_FOLDER = "Bahnhof Fahrplan";
        internal const string LOCAL_ORDERS_FOLDER = "Bahnhof Anweisungen";
        internal const string ESTWONLINE_SETTINGS_FILE = "ESTWonline.ini";
        internal const string ESTWONLINE_DIRECTORY_KEY = "Verz";
        internal const string SERIALIZED_FILE_ENDING = "leibit";
    }
}
