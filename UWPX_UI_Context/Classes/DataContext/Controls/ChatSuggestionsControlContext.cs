using System.Threading.Tasks;
using Manager.Classes;
using UWPX_UI_Context.Classes.DataTemplates.Controls;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public sealed class ChatSuggestionsControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly ChatSuggestionsControlDataTemplate MODEL = new ChatSuggestionsControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public Task UpdateViewAsync(Client client)
        {
            return MODEL.UpdateViewAsync(client);
        }

        public void UpdateView(string filterText)
        {
            MODEL.UpdateView(filterText);
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
