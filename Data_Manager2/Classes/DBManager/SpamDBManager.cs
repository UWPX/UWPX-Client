using Data_Manager2.Classes.DBTables;
using Logging;
using Shared.Classes.SQLite;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Data_Manager2.Classes.DBManager
{
    public class SpamDBManager : AbstractDBManager
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly SpamDBManager INSTANCE = new SpamDBManager();

        public const string DEFAULT_SPAM_REGEX = @"\p{IsCyrillic}";
        private Regex spamRegex = null;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public bool isSpam(string text)
        {
            if(!(spamRegex is null))
            {
                return !(getSpam(text) is null) || spamRegex.IsMatch(text);
            }
            return !(getSpam(text) is null);
        }

        public SpamMessageTable getSpam(string text)
        {
            SQLiteCommand cmd = dB.CreateCommand("SELECT * FROM " + DBTableConsts.SPAM_MESSAGE_TABLE + " WHERE text LIKE @TEXT;");
            cmd.Bind("@TEXT", '%' + text + '%');
            List<SpamMessageTable> result = dB.ExecuteCommand<SpamMessageTable>(true, cmd);
            if (result.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void addSpamMessage(string text, DateTime dateTime)
        {
            SpamMessageTable spam = getSpam(text);
            if(spam is null)
            {
                spam = new SpamMessageTable
                {
                    lastReceived = dateTime,
                    text = text,
                    count = 1,
                };
            }
            else
            {
                spam.count++;
                spam.lastReceived = dateTime;
            }
            dB.InsertOrReplace(spam);
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected override void createTables()
        {
            dB.CreateTable<SpamMessageTable>();
        }

        protected override void dropTables()
        {
            dB.DropTable<SpamMessageTable>();
        }

        public override void initManager()
        {
            string regex = Settings.getSettingString(SettingsConsts.SPAM_REGEX, DEFAULT_SPAM_REGEX);
            updateSpamRegex(regex);
        }

        public void updateSpamRegex(string regex)
        {
            try
            {
                spamRegex = new Regex(regex);
            }
            catch (Exception e)
            {
                Logger.Error("Failed to create spam regular expression for: " + regex, e);
                spamRegex = null;
            }
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
