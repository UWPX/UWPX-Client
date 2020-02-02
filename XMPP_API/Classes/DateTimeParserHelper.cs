using System;
using System.Text.RegularExpressions;
using Logging;

namespace XMPP_API.Classes
{
    /// <summary>
    /// A helper class to parse the by XEP-0082 defined date and time strings
    /// https://xmpp.org/extensions/xep-0082.html
    /// </summary>
    public class DateTimeParserHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private readonly Regex DATE_REGEX;
        private readonly Regex TIME_REGEX;
        private readonly Regex DATE_TIME_REGEX;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/09/2017 Created [Fabian Sauter]
        /// </history>
        public DateTimeParserHelper()
        {
            // Regex: https://regex101.com/
            DATE_REGEX = new Regex(@"\d{4}(-?\d{2}){2}");
            TIME_REGEX = new Regex(@"(\d{2}:){2}\d{2}(.\d{3})?\D*");
            DATE_TIME_REGEX = new Regex(@"\d{4}(-?\d{2}){2}T(\d{2}:){2}\d{2}(.\d{3})?\D*");
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public DateTime parse(string dateString)
        {
            if (dateString != null)
            {
                if (DATE_TIME_REGEX.IsMatch(dateString))
                {
                    return parseDateTimeString(dateString);
                }
                else if (DATE_REGEX.IsMatch(dateString))
                {
                    return parseDateString(dateString);
                }
                else if (TIME_REGEX.IsMatch(dateString))
                {
                    return parseTimeString(dateString, DateTime.MinValue);
                }
            }
            return DateTime.MinValue;
        }

        public string toString(DateTime dateTime)
        {
            TimeSpan uTCoffset = TimeZoneInfo.Local.GetUtcOffset(dateTime.ToUniversalTime());

            dateTime = dateTime.ToUniversalTime();
            string result = dateTime.ToString("yyyy-MM-dd") + 'T' + dateTime.ToString(@"HH\:mm\:ss");
            if (uTCoffset.TotalHours == 0)
            {
                result += 'Z';
            }
            else
            {
                if (uTCoffset.TotalHours >= 0)
                {
                    result += '+';
                }
                else
                {
                    result += '-';
                }
                result += uTCoffset.ToString(@"hh\:mm");
            }
            return result;
        }

        #endregion

        #region --Misc Methods (Private)--
        private DateTime parseDateTimeString(string dateString)
        {
            DateTime dateTime = DateTime.MinValue;
            try
            {
                DateTime date = parseDateString(dateString.Substring(0, dateString.IndexOf('T')));
                dateTime = parseTimeString(dateString.Substring(dateString.IndexOf('T') + 1), date);
            }
            catch (Exception e)
            {
                Logger.Error("Error during parsing dateString in parseDateTimeString() - DateTimeParserHelper " + dateString, e);
            }
            return dateTime;
        }

        private DateTime parseDateString(string dateString)
        {
            DateTime dateTime = DateTime.MinValue;
            try
            {
                int year = -1;
                int month = -1;
                int day = -1;
                if (dateString.Length > 8)
                {
                    int.TryParse(dateString.Substring(0, 4), out year);
                    int.TryParse(dateString.Substring(5, 2), out month);
                    int.TryParse(dateString.Substring(8, 2), out day);
                }
                else
                {
                    int.TryParse(dateString.Substring(0, 4), out year);
                    int.TryParse(dateString.Substring(4, 2), out month);
                    int.TryParse(dateString.Substring(6, 2), out day);
                }
                dateTime = new DateTime(year, month, day);
            }
            catch (Exception e)
            {
                Logger.Error("Error during parsing dateString in parseDateString() - DateTimeParserHelper", e);
            }
            return dateTime;
        }

        private DateTime parseTimeString(string dateString, DateTime date)
        {
            try
            {
                int hour = -1;
                int minute = -1;
                int second = -1;
                int millisecond = 0;
                int uTCOffset = 0;

                int.TryParse(dateString.Substring(0, 2), out hour);
                int.TryParse(dateString.Substring(3, 2), out minute);
                int.TryParse(dateString.Substring(6, 2), out second);
                bool hasMilli = false;
                if (dateString.Contains("."))
                {
                    hasMilli = true;
                    int.TryParse(dateString.Substring(9, 3), out millisecond);
                }
                if (dateString.Contains("+") || dateString.Contains("-"))
                {
                    if (hasMilli)
                    {
                        int.TryParse(dateString.Substring(12, 3), out uTCOffset);
                    }
                    else
                    {
                        int.TryParse(dateString.Substring(8, 3), out uTCOffset);
                    }
                    uTCOffset *= -1;
                    uTCOffset += (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours;
                }
                else if (dateString.EndsWith("Z"))
                {
                    uTCOffset = (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours;
                }


                date = new DateTime(date.Year, date.Month, date.Day, hour, minute, second, millisecond);
                date = date.AddHours(uTCOffset);
            }
            catch (Exception e)
            {
                Logger.Error("Error during parsing dateString in parseTimeString() - DateTimeParserHelper", e);
            }
            return date;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
