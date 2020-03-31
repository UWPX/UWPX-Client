using Newtonsoft.Json.Linq;

namespace Push.Classes.Messages
{
    public abstract class AbstractResponseMessage: AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string ACTION_CONST = "response";

        public uint status;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public AbstractResponseMessage(JObject json) : base(json) { }

        public AbstractResponseMessage(uint status) : base(ACTION_CONST)
        {
            this.status = status;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void FromJson(JObject json)
        {
            base.FromJson(json);
            status = json.Value<uint>("status");
        }

        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json["status"] = status;
            return json;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
