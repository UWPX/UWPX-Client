using System;
using System.Collections.Generic;
using System.Threading;

namespace Data_Manager2.Classes
{
    public class TimedList<T> : List<TimedListEntry<T>>
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool itemAdded;
        private Timer timer;
        public int cleanupIntervallInMs;
        public int itemTimeoutInMs;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 03/01/2018 Created [Fabian Sauter]
        /// </history>
        public TimedList()
        {
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
            for (int i = 0; i < Count; i++)
            {
                if (this[i].item.Equals(item))
                {
                    TimedListEntry<T> entry = this[i];
                    RemoveAt(i);
                    return entry;
                }
            }
            return null;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void addTimed(T item)
        {
            Add(new TimedListEntry<T>(item));
            onItemAdded();
        }

        #endregion

        #region --Misc Methods (Private)--
        private void onItemAdded()
        {
            if(timer == null)
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
            timer = new Timer((obj) => {
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
            for (int i = 0; i < Count; i++)
            {
                if (DateTime.Now.Subtract(this[i].insertionTime).TotalMilliseconds >= itemTimeoutInMs)
                {
                    RemoveAt(i);
                    countRemoved++;
                }
            }
            Logging.Logger.Debug("Removed " + countRemoved + " item(s) from the TimedList.");
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
