using Newtonsoft.Json.Linq;

namespace Push.Classes.Messages
{
    public class PushMessage: AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string ACTION_CONST = "push";

        public string accountId;
        public int messageCount;
        public int pendingSubscriptionCount;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public PushMessage(JObject json) : base(json) { }

        public PushMessage(string accountId, int messageCount, int pendingSubscriptionCount) : base(ACTION_CONST)
        {
            this.accountId = accountId;
            this.messageCount = messageCount;
            this.pendingSubscriptionCount = pendingSubscriptionCount;
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
            json["account_id"] = accountId;
            json["message_count"] = messageCount;
            json["pending_subscription_count"] = pendingSubscriptionCount;
            return json;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void FromJson(JObject json)
        {
            base.FromJson(json);
            accountId = json.Value<string>("account_id");
            messageCount = json.Value<int>("message_count");
            pendingSubscriptionCount = json.Value<int>("pending_subscription_count");
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
