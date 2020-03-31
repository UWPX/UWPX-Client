using Newtonsoft.Json.Linq;

namespace Push.Classes.Messages
{
    public class ErrorResultMessage: AbstractResponseMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const uint STATUS_CONST = 0;

        public string error;
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ErrorResultMessage(JObject json) : base(json) { }

        public ErrorResultMessage(string error) : base(STATUS_CONST)
        {
            this.error = error;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        protected override void FromJson(JObject json)
        {
            base.FromJson(json);
            error = json.Value<string>("error");
        }

        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json["error"] = error;
            return json;
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
