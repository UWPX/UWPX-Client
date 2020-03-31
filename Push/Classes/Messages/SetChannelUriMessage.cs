using Newtonsoft.Json.Linq;
using Shared.Classes;

namespace Push.Classes.Messages
{
    public class SetChannelUriMessage: AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string ACTION_CONST = "set_channel_uri";

        public string channelUri;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SetChannelUriMessage(string channelUri) : base(ACTION_CONST)
        {
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
            json["device_id"] = SharedUtils.GetUniqueDeviceId();
            json["channel_uri"] = channelUri;
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
