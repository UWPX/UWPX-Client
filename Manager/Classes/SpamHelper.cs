using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Logging;
using Storage.Classes;
using Storage.Classes.Contexts;
using Storage.Classes.Models.Chat;

namespace Manager.Classes
{
    public class SpamHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static SpamHelper INSTANCE = new SpamHelper();

        public const string DEFAULT_SPAM_REGEX = @"\p{IsCyrillic}";
        private Regex spamRegex = null;
        private readonly SemaphoreSlim SPAM_SEMA = new SemaphoreSlim(1);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        private SpamHelper()
        {
            string regex = Settings.GetSettingString(SettingsConsts.SPAM_REGEX, DEFAULT_SPAM_REGEX);
            UpdateSpamRegex(regex);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public bool IsSpam(string msg)
        {
            using (MainDbContext ctx = new MainDbContext())
            {
                SpamMessageModel spamMsg = ctx.SpamMessages.Where(m => string.Equals(m.text, msg)).FirstOrDefault();
                if (spamMsg is null)
                {
                    SPAM_SEMA.Wait();
                    if (spamRegex is null || !spamRegex.IsMatch(msg))
                    {
                        SPAM_SEMA.Release();
                        return false;
                    }
                    SPAM_SEMA.Release();
                    return true;
                }
            }
            return false;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateSpamRegex(string regex)
        {
            SPAM_SEMA.Wait();
            try
            {
                spamRegex = new Regex(regex);
            }
            catch (Exception e)
            {
                Logger.Error("Failed to create spam regular expression for: " + regex, e);
                spamRegex = null;
            }
            SPAM_SEMA.Release();
        }

        public void AddSpamMessage(string msg)
        {
            Debug.Assert(!string.IsNullOrEmpty(msg));
            using (MainDbContext ctx = new MainDbContext())
            {
                SpamMessageModel spamMsg = ctx.SpamMessages.Where(m => m.text.Contains(msg)).FirstOrDefault();
                if (spamMsg is null)
                {
                    spamMsg = new SpamMessageModel()
                    {
                        text = msg
                    };
                }
                spamMsg.lastReceived = DateTime.Now;
                ++spamMsg.count;
                ctx.Add(spamMsg);
            }
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
