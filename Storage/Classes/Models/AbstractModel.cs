using System.Threading;
using Shared.Classes;
using Storage.Classes.Contexts;

namespace Storage.Classes.Models
{
    public abstract class AbstractModel: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        protected readonly SemaphoreSlim LOCK_SEMA = new SemaphoreSlim(1);

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Updates the current model in the <see cref="MainDbContext"/>.
        /// </summary>
        public void Update()
        {
            using (MainDbContext ctx = new MainDbContext())
            {
                ctx.Update(this);
            }
        }

        /// <summary>
        /// Adds the current model to the <see cref="MainDbContext"/>.
        /// </summary>
        public void Add()
        {
            using (MainDbContext ctx = new MainDbContext())
            {
                ctx.Add(this);
            }
        }

        public void NewLock()
        {

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
