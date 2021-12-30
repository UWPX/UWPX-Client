using System;
using System.Collections.Generic;
using Manager.Classes.Chat;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Classes;
using Storage.Classes.Models.Account;
using Storage.Classes.Models.Chat;

namespace Component_Tests.Classes.Collections
{
    [TestClass]
    public class Test_SaveObservableChatDictionaryList
    {
        private static Random rnd = new Random((int)DateTime.Now.Ticks);

        [TestCategory("Collections")]
        [TestMethod]
        public void Test_SaveObservableChatDictionaryList_Add()
        {
            // Add with different chat IDs:
            SaveObservableChatDictionaryList chats = new SaveObservableChatDictionaryList();
            for (int i = 0; i < 1000; i++)
            {
                chats.Add(GetNewChat(i));
                Assert.AreEqual(i + 1, chats.Count);
            }

            // Add the same chat ID:
            for (int i = 0; i < 1000; i++)
            {
                chats.Add(GetNewChat(42));
                Assert.AreEqual(1000, chats.Count);
            }
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Test_SaveObservableChatDictionaryList_AddRange()
        {
            // Add with different chat IDs:
            SaveObservableChatDictionaryList chats = new SaveObservableChatDictionaryList();
            List<ChatDataTemplate> chatsTmp = new List<ChatDataTemplate>();
            for (int i = 0; i < 1000; i++)
            {
                chatsTmp.Add(GetNewChat(i));
            }
            chats.AddRange(chatsTmp, true);
            Assert.AreEqual(1000, chats.Count);

            // Add the same chat ID:
            chatsTmp.Clear();
            for (int i = 0; i < 1000; i++)
            {
                chatsTmp.Add(GetNewChat(1001));
            }
            chats.AddRange(chatsTmp, true);
            Assert.AreEqual(1001, chats.Count);
        }

        [TestCategory("Collections")]
        [TestMethod]
        public void Test_SaveObservableChatDictionaryList_Index_Access()
        {
            // Add with different chat IDs:
            SaveObservableChatDictionaryList chats = new SaveObservableChatDictionaryList();
            for (int i = 0; i < 1000; i++)
            {
                chats.Add(GetNewChat(i));
                Assert.AreEqual(i + 1, chats.Count);
            }

            for (int i = 0; i < 1000; i++)
            {
                ChatDataTemplate chat = chats[i];
                Assert.IsNotNull(chat);
                Assert.AreEqual(i, chats[i].Index);
            }
        }

        private ChatDataTemplate GetNewChat(int chatId)
        {
            JidModel jidChat = new JidModel
            {
                domainPart = "chat.example.org",
                localPart = RandomString(),
                resourcePart = RandomString()
            };
            JidModel jidAccount = new JidModel
            {
                domainPart = "chat.example.org",
                localPart = RandomString(),
                resourcePart = RandomString()
            };
            ChatDataTemplate chat = new ChatDataTemplate(new ChatModel(jidAccount.BareJid(), new AccountModel(jidChat, "#FF00FF")), null);
            chat.Chat.id = chatId;
            return chat;
        }

        private string RandomString()
        {
            byte[] data = new byte[16];
            rnd.NextBytes(data);
            return SharedUtils.ToHexString(data);
        }
    }
}
