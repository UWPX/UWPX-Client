using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_XMPP_Client.DataTemplates;

namespace Thread_Save_Components.Classes.Collections
{
    public class ObservableOrderedDictionary : ICollection<ChatTemplate>
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public int Count => LIST.Count;
        public bool IsReadOnly => false;

        private readonly Dictionary<string, LinkedListNode<ChatTemplate>> DICTIONARY;
        private readonly LinkedList<ChatTemplate> LIST;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 20/07/2018 Created [Fabian Sauter]
        /// </history>
        public ObservableOrderedDictionary()
        {
            this.DICTIONARY = new Dictionary<string, LinkedListNode<ChatTemplate>>();
            this.LIST = new LinkedList<ChatTemplate>();
        }





        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public IEnumerator<ChatTemplate> GetEnumerator()
        {
            return LIST.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return LIST.GetEnumerator();
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void Add(ChatTemplate item)
        {
            if (!DICTIONARY.ContainsKey(item.chat.id))
            {
                LinkedListNode<ChatTemplate> node = addSortedToList(item);
                DICTIONARY.Add(item.chat.id, node);
            }
        }

        public void Clear()
        {
            DICTIONARY.Clear();
            LIST.Clear();
        }

        public bool Contains(ChatTemplate item)
        {
            return DICTIONARY.ContainsKey(item.chat.id);
        }

        public void CopyTo(ChatTemplate[] array, int arrayIndex)
        {
            LIST.CopyTo(array, arrayIndex);
        }

        public bool Remove(ChatTemplate item)
        {
            if (DICTIONARY.ContainsKey(item.chat.id))
            {
                LIST.Remove(DICTIONARY[item.chat.id]);
                DICTIONARY.Remove(item.chat.id);
                return true;
            }
            return false;
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--
        protected LinkedListNode<ChatTemplate> addSortedToList(ChatTemplate item)
        {
            LinkedListNode<ChatTemplate> cur = LIST.First;
            while (cur != null)
            {
                if (cur.Value.chat.lastActive.CompareTo(item.chat.lastActive) >= 0)
                {
                    return LIST.AddBefore(cur, item);
                }
                cur = cur.Next;
            }
            return LIST.AddLast(item);
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
