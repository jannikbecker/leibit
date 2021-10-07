using System;
using System.Collections.Generic;
using System.Linq;

namespace Leibit.Core.Scheduling
{
    [Serializable]
    public class LeibitTime : IComparable<LeibitTime>, IComparable
    {

        #region - Needs -
        private eDaysOfService m_Day;
        private int m_Hour;
        private int m_Minute;
        #endregion

        #region - Ctor -
        public LeibitTime(eDaysOfService day, int hour, int minute)
        {
            m_Day = day;
            m_Hour = hour;
            m_Minute = minute;
        }

        public LeibitTime(int hour, int minute)
            : this(eDaysOfService.None, hour, minute)
        {
        }
        #endregion

        #region - Properties -

        #region [Day]
        public eDaysOfService Day
        {
            get
            {
                return m_Day;
            }
        }
        #endregion

        #region [Hour]
        public int Hour
        {
            get
            {
                return m_Hour;
            }
        }
        #endregion

        #region [Minute]
        public int Minute
        {
            get
            {
                return m_Minute;
            }
        }
        #endregion

        #region [TotalMinutes]
        public int TotalMinutes
        {
            get
            {
                int Result = Hour * 60 + Minute;

                if (Result > 720)
                    return Result - 1440;

                return Result;
            }
        }
        #endregion

        #endregion

        #region - Public methods -

        #region [AddDays]
        public LeibitTime AddDays(int days)
        {
            if (m_Day == eDaysOfService.None)
                return new LeibitTime(m_Hour, m_Minute);

            double day = (int)m_Day * Math.Pow(2, days);

            while (day >= 128)
                day /= 128;

            while (day < 1)
                day *= 128;

            return new LeibitTime((eDaysOfService)day, m_Hour, m_Minute);
        }
        #endregion

        #region [AddHours]
        public LeibitTime AddHours(int hours)
        {
            int hour = m_Hour + hours;
            int dayDiff = 0;

            while (hour > 23)
            {
                hour -= 24;
                dayDiff++;
            }

            while (hour < 0)
            {
                hour += 24;
                dayDiff--;
            }

            return new LeibitTime(m_Day, hour, m_Minute).AddDays(dayDiff);
        }
        #endregion

        #region [AddMinutes]
        public LeibitTime AddMinutes(int minutes)
        {
            int minute = m_Minute + minutes;
            int hourDiff = 0;

            while (minute > 59)
            {
                minute -= 60;
                hourDiff++;
            }

            while (minute < 0)
            {
                minute += 60;
                hourDiff--;
            }

            return new LeibitTime(m_Day, m_Hour, minute).AddHours(hourDiff);
        }
        #endregion

        #region [ParseDay]
        public static List<eDaysOfService> ParseDays(string days)
        {
            if (String.IsNullOrWhiteSpace(days))
                return Enum.GetValues(typeof(eDaysOfService)).Cast<eDaysOfService>().ToList();

            var Result = new List<eDaysOfService>();
            string[] parts;

            if (days.Contains("+"))
                parts = days.Split('+');
            else
                parts = days.Split(',');

            foreach (var part in parts)
            {
                if (part.Contains("-"))
                {
                    var Parts = part.Split('-');

                    eDaysOfService StartDay;
                    eDaysOfService EndDay;

                    if (!TryParseSingleDay(Parts[0], out StartDay) || !TryParseSingleDay(Parts[1], out EndDay))
                        continue;

                    for (int i = (int)StartDay; i != (int)EndDay;)
                    {
                        Result.Add((eDaysOfService)i);

                        i *= 2;

                        if (i > 64)
                            i = 1;
                    }

                    Result.Add(EndDay);
                }
                else
                {
                    eDaysOfService Day;

                    if (TryParseSingleDay(part, out Day))
                        Result.Add(Day);
                }
            }

            return Result;
        }

        public static eDaysOfService ParseSingleDay(string day)
        {
            eDaysOfService Result;

            if (TryParseSingleDay(day, out Result))
                return Result;
            else
                throw new ArgumentException(String.Format("Day {0} could not be parsed.", day));
        }

        public static bool TryParseSingleDay(string day, out eDaysOfService result)
        {
            switch (day.Trim().ToLower())
            {
                case "mo":
                case "1":
                    result = eDaysOfService.Monday;
                    return true;
                case "di":
                case "2":
                    result = eDaysOfService.Tuesday;
                    return true;
                case "mi":
                case "3":
                    result = eDaysOfService.Wednesday;
                    return true;
                case "do":
                case "4":
                    result = eDaysOfService.Thursday;
                    return true;
                case "fr":
                case "5":
                    result = eDaysOfService.Friday;
                    return true;
                case "sa":
                case "6":
                    result = eDaysOfService.Saturday;
                    return true;
                case "so":
                case "7":
                    result = eDaysOfService.Sunday;
                    return true;
                default:
                    result = eDaysOfService.None;
                    return false;
            }
        }
        #endregion

        #endregion

        #region - Operators -

        #region [==]
        public static bool operator ==(LeibitTime time1, LeibitTime time2)
        {
            if ((object)time1 == null && (object)time2 == null)
                return true;
            if ((object)time1 != null && (object)time2 != null)
                return time1.Equals(time2);

            return false;
        }
        #endregion

        #region [!=]
        public static bool operator !=(LeibitTime time1, LeibitTime time2)
        {
            if ((object)time1 == null && (object)time2 == null)
                return false;
            if ((object)time1 != null && (object)time2 != null)
                return !time1.Equals(time2);

            return true;
        }
        #endregion

        #region [<]
        public static bool operator <(LeibitTime time1, LeibitTime time2)
        {
            if (time1 == null && time2 == null)
                return false;
            if (time1 == null && time2 != null)
                return true;
            if (time1 != null && time2 == null)
                return false;

            return time1.CompareTo(time2) < 0;
        }
        #endregion

        #region [>]
        public static bool operator >(LeibitTime time1, LeibitTime time2)
        {
            if (time1 == null && time2 == null)
                return false;
            if (time1 == null && time2 != null)
                return false;
            if (time1 != null && time2 == null)
                return true;

            return time1.CompareTo(time2) > 0;
        }
        #endregion

        #region [<=]
        public static bool operator <=(LeibitTime time1, LeibitTime time2)
        {
            return time1 == time2 || time1 < time2;
        }
        #endregion

        #region [>=]
        public static bool operator >=(LeibitTime time1, LeibitTime time2)
        {
            return time1 == time2 || time1 > time2;
        }
        #endregion

        #region [-]
        public static LeibitTime operator -(LeibitTime time1, LeibitTime time2)
        {
            return time1.AddMinutes(time2.TotalMinutes * (-1));
        }
        #endregion

        #endregion

        #region - Overrides -

        #region [ToString]
        public override string ToString()
        {
            return String.Format("{0:00}:{1:00}", Hour, Minute);
        }
        #endregion

        #region [Equals]
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is LeibitTime))
                return false;

            var LeibitTime = obj as LeibitTime;
            return CompareTo(LeibitTime) == 0;
        }
        #endregion

        #region [GetHashCode]
        public override int GetHashCode()
        {
            return (int)m_Day ^ m_Hour ^ m_Minute;
        }
        #endregion

        #region [CompareTo]
        public int CompareTo(LeibitTime other)
        {
            if (other == null)
                return 1;

            if (this.Day == other.Day && this.Hour == other.Hour && this.Minute == other.Minute)
                return 0;

            if (this.Day != eDaysOfService.None && other.Day != eDaysOfService.None)
                for (int i = -3; i <= 3; i++)
                    if (other.AddDays(i).Day == this.Day && i != 0)
                        return i;

            int hourDiff = this.Hour - other.Hour;
            int minuteDiff = this.Minute - other.Minute;

            if (this.Day == eDaysOfService.None || other.Day == eDaysOfService.None)
            {
                if (hourDiff < -12)
                    hourDiff += 24;
                if (hourDiff > 12)
                    hourDiff -= 24;

                if (Math.Abs(hourDiff) == 12 && minuteDiff != 0)
                    return minuteDiff * (-1);
            }

            if (hourDiff != 0)
                return hourDiff;

            return minuteDiff;
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo(obj as LeibitTime);
        }
        #endregion

        #endregion

        #region - Private helpers -
        #endregion

    }
}
