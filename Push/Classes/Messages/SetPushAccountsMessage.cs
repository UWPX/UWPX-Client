using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Shared.Classes;

namespace Push.Classes.Messages
{
    public class SetPushAccountsMessage: AbstractMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public const string ACTION_CONST = "set_push_accounts";

        public List<string> accounts;
        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public SetPushAccountsMessage(List<string> accounts) : base(ACTION_CONST)
        {
            this.accounts = accounts;
        }

        public SetPushAccountsMessage(JObject json) : base(json) { }

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
            json["accounts"] = new JArray(from a in accounts select new JObject(new JProperty("account_id", a)));
            return json;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void FromJson(JObject json)
        {
            base.FromJson(json);
            accounts = new List<string>();
            foreach (JObject item in json.Value<JArray>("accounts"))
            {
                accounts.Append(item.Value<string>("account_id"));
            }
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
