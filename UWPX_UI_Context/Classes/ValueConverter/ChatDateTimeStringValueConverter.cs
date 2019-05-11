using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace UWPX_UI_Context.Classes.ValueConverter
{
    public sealed class ChatDateTimeStringValueConverter: IValueConverter
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime date && date.CompareTo(DateTime.MinValue) != 0)
            {
                if (date.CompareTo(DateTime.MaxValue) == 0)
                {
                    return "In the future";
                }

                // Today:
                if (date.Date.CompareTo(DateTime.Now.Date) == 0)
                {
                    return date.ToString("HH:mm");
                }

                // Yesterday:
                if (date.Date.CompareTo(DateTime.Now.Date.AddDays(-1)) == 0)
                {
                    return "Yesterday" + date.ToString(" HH:mm");
                }

                // Day of the last week:
                for (int i = -2; i > -7; i--)
                {
                    if (date.Date.CompareTo(DateTime.Now.Date.AddDays(i)) == 0)
                    {
                        return CultureInfo.CurrentUICulture.DateTimeFormat.GetAbbreviatedDayName(date.DayOfWeek) + date.ToString(" HH:mm");
                    }
                }

                // Same year:
                if (date.Year == DateTime.Now.Year)
                {
                    return date.ToString("dd. MMMM");
                }

                // Fallback:
                return date.ToString("dd.MM.yyyy");
            }
            return "A long time ago";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
