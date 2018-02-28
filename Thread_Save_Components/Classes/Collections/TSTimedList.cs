using System;
using System.Collections.Generic;
using System.Threading;

namespace Thread_Save_Components.Classes.Collections
{
    public class TSTimedList<T>
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool itemAdded;
        private Timer timer;
        public int cleanupIntervallInMs;
        public int itemTimeoutInMs;
        private readonly List<TimedListEntry<T>> LIST;
        private static readonly object _locker = new object();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 03/01/2018 Created [Fabian Sauter]
        /// </history>
        public TSTimedList()
        {
            this.LIST = new List<TimedListEntry<T>>();
            this.itemAdded = false;
            this.cleanupIntervallInMs = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;
            this.itemTimeoutInMs = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;
            this.timer = null;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public TimedListEntry<T> getTimed(T item)
        {
            lock (_locker)
            {
                for (int i = 0; i < LIST.Count; i++)
                {
                    if (LIST[i].item.Equals(item))
                    {
                        TimedListEntry<T> entry = LIST[i];
                        LIST.RemoveAt(i);
                        return entry;
                    }
                }
            }
            return null;
        }

        public bool isEmpty()
        {
            return LIST.Count <= 0;
        }

        public List<T> getEntries()
        {
            List<T> list = new List<T>();
            foreach (TimedListEntry<T> item in LIST)
            {
                list.Add(item.item);
            }
            return list;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void addTimed(T item)
        {
            lock (_locker)
            {
                LIST.Add(new TimedListEntry<T>(item));
            }
            onItemAdded();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void onItemAdded()
        {
            if (timer == null)
            {
                startTimer();
            }
            else
            {
                itemAdded = true;
            }
        }

        private void startTimer()
        {
            timer = new Timer((obj) =>
            {
                cleanupList();
                if (itemAdded)
                {
                    startTimer();
                    itemAdded = false;
                }
                else
                {
                    timer = null;
                }
            }, null, cleanupIntervallInMs, Timeout.Infinite);
        }

        private void cleanupList()
        {
            int countRemoved = 0;
            lock (_locker)
            {
                for (int i = 0; i < LIST.Count; i++)
                {
                    if (DateTime.Now.Subtract(LIST[i].insertionTime).TotalMilliseconds >= itemTimeoutInMs && LIST[i].canGetRemoved())
                    {
                        LIST.RemoveAt(i);
                        countRemoved++;
                    }
                }
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
