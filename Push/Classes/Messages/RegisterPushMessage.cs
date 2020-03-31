using Newtonsoft.Json.Linq;
using Shared.Classes;

namespace Push.Classes.Messages
{
    public class RegisterPushMessage: AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string ACTION_CONST = "register_push";

        public string bareJid;
        public string channelUri;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public RegisterPushMessage(string bareJid, string channelUri) : base(ACTION_CONST)
        {
            this.bareJid = bareJid;
            this.channelUri = channelUri;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json["account"] = new JArray()
            {
                ["bare_jid"] = bareJid,
                ["device_id"] = SharedUtils.GetUniqueDeviceId(),
                ["channel_uri"] = channelUri
            };
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
