using System.Text.Json;
using Microsoft.Toolkit.Uwp.Helpers;

namespace Storage.Classes
{
    public class JsonObjectSerializer: IObjectSerializer
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
        object IObjectSerializer.Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value);
        }

        public T Deserialize<T>(object value)
        {
            return JsonSerializer.Deserialize<T>(value as string);
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
