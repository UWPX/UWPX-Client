using Newtonsoft.Json.Linq;

namespace Push.Classes.Messages
{
    public abstract class AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public uint version;
        public string action;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AbstractMessage(string action)
        {
            version = 1;
            this.action = action;
        }

        public AbstractMessage(JObject json)
        {
            FromJson(json);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public virtual JObject ToJson()
        {
            return new JObject
            {
                ["version"] = 1,
                ["action"] = action
            };
        }

        /// <summary>
        /// Returns the JSON representation of this message as a string.
        /// </summary>
        public override string ToString()
        {
            return ToJson().ToString();
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected virtual void FromJson(JObject json)
        {
            version = json.Value<uint>("version");
            action = json.Value<string>("action");
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
