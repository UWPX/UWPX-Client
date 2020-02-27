using System;
using System.Collections.Generic;
using System.Threading;

namespace Shared.Classes.Collections
{
    public class TSTimedList<T>
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool itemAdded;
        private Timer timer;
        private int cleanupIntervallInMs = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;
        public int itemTimeoutInMs = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;
        private readonly List<TimedListEntry<T>> LIST = new List<TimedListEntry<T>>();
        private static readonly object LOCKER = new object();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public TSTimedList() { }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public TimedListEntry<T> GetTimed(T item)
        {
            lock (LOCKER)
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

        public List<T> GetEntries()
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
        public void AddTimed(T item)
        {
            lock (LOCKER)
            {
                LIST.Add(new TimedListEntry<T>(item));
            }
            OnItemAdded();
        }

        public void Clear()
        {
            lock (LOCKER)
            {
                LIST.Clear();
                if (!(timer is null))
                {
                    timer.Dispose();
                    timer = null;
                }
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private void OnItemAdded()
        {
            if (timer is null)
            {
                StartTimer();
            }
            else
            {
                itemAdded = true;
            }
        }

        private void StartTimer()
        {
            timer = new Timer((obj) =>
            {
                CleanupList();
                if (itemAdded)
                {
                    StartTimer();
                    itemAdded = false;
                }
                else
                {
                    timer = null;
                }
            }, null, cleanupIntervallInMs, Timeout.Infinite);
        }

        private void CleanupList()
        {
            int countRemoved = 0;
            lock (LOCKER)
            {
                for (int i = 0; i < LIST.Count; i++)
                {
                    if (DateTime.Now.Subtract(LIST[i].insertionTime).TotalMilliseconds >= itemTimeoutInMs && LIST[i].CanGetRemoved())
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
