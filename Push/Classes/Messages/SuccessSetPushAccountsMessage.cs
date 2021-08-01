using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Push.Classes.Messages
{
    public struct PushAccount
    {
        public string accountId;
        public string node;
        public string secret;
        public bool success;

        public JObject ToJson()
        {
            return new JObject()
            {
                ["account_id"] = accountId,
                ["node"] = node,
                ["secret"] = secret,
                ["success"] = success,
            };
        }

        public static PushAccount FromJson(JObject json)
        {
            return new PushAccount
            {
                accountId = json.Value<string>("account_id"),
                node = json.Value<string>("node"),
                secret = json.Value<string>("secret"),
                success = json.Value<bool>("success")
            };
        }
    }

    public class SuccessSetPushAccountsMessage: SuccessResultMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public string pushServerBareJid;
        public List<PushAccount> accounts;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SuccessSetPushAccountsMessage(List<PushAccount> accounts) : base()
        {
            this.accounts = accounts;
        }

        public SuccessSetPushAccountsMessage(JObject json) : base(json) { }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json["push_bare_jid"] = pushServerBareJid;
            json["account"] = new JArray(from a in accounts select a.ToJson());
            return json;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void FromJson(JObject json)
        {
            base.FromJson(json);
            pushServerBareJid = json.Value<string>("push_bare_jid");
            accounts = new List<PushAccount>(from a in json.Value<JArray>("accounts") select PushAccount.FromJson(a.ToObject<JObject>()));
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
