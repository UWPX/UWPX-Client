using Shared.Classes.Network;
using Storage.Classes.Contexts;

namespace Storage.Classes.Models.Chat
{
    public class ChatMessageImageReceivedModel: AbstractDownloadableObject
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--


        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ChatMessageImageReceivedModel(string sourceUrl) : base(sourceUrl) { }

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
