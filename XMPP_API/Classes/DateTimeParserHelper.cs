using Logging;
using System;
using System.Text.RegularExpressions;

namespace XMPP_API.Classes
{
    /// <summary>
    /// A helper class to parse the by XEP-0082 defined date and time strings
    /// https://xmpp.org/extensions/xep-0082.html#sect-idm139847125559776
    /// </summary>
    class DateTimeParserHelper
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
            this.DATE_REGEX = new Regex(@"\d{4}(-?\d{2}){2}");
            this.TIME_REGEX = new Regex(@"(\d{2}:){2}\d{2}(.\d{3})?\D*");
            this.DATE_TIME_REGEX = new Regex(@"\d{4}(-?\d{2}){2}T(\d{2}:){2}\d{2}(.\d{3})?\D*");
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public DateTime parse(string dateString)
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
                return parseTimeString(dateString);
            }
            return DateTime.MinValue;
        }

        public string toString(DateTime dateTime)
        {
            TimeSpan uTCoffset = TimeZoneInfo.Local.GetUtcOffset(dateTime.ToUniversalTime());

            dateTime = dateTime.ToUniversalTime();
            string result = dateTime.ToString("yyyy-MM-dd") + 'T' + dateTime.ToString(@"HH\:mm\:ss");
            if(uTCoffset.TotalHours == 0)
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
                DateTime time = parseTimeString(dateString.Substring(dateString.IndexOf('T') + 1));
                dateTime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
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

        private DateTime parseTimeString(string dateString)
        {
            DateTime dateTime = DateTime.MinValue;
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
                if (dateString.Contains("+") || dateString.Contains("+"))
                {
                    if (hasMilli)
                    {
                        int.TryParse(dateString.Substring(12, 3), out uTCOffset);
                    }
                    else
                    {
                        int.TryParse(dateString.Substring(8, 3), out uTCOffset);
                    }
                }
                else if (dateString.EndsWith("Z"))
                {
                    uTCOffset = (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours;
                }


                dateTime = DateTime.Now;
                dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, minute, second, millisecond);
                dateTime.AddHours(uTCOffset);
            }
            catch (Exception e)
            {
                Logger.Error("Error during parsing dateString in parseTimeString() - DateTimeParserHelper", e);
            }
            return dateTime;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
