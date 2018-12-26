using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataTemplates;
using Windows.UI.Xaml;

namespace UWPX_UI_Context.Classes.DataContext
{
    public sealed class SpeechBubbleImageContentControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly SpeechBubbleImageContentControlDataTemplate MODEL = new SpeechBubbleImageContentControlDataTemplate();
        private SpeechBubbleContentContext SpeechBubbleViewModel = null;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task UpdateViewAsync(DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is SpeechBubbleContentContext newValue)
            {
                SpeechBubbleViewModel = newValue;
                await LoadImageAsync(SpeechBubbleViewModel);
            }
            else
            {
                SpeechBubbleViewModel = null;
            }
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task LoadImageAsync(SpeechBubbleContentContext speechBubbleContentViewModel)
        {
            if (SpeechBubbleViewModel is null || SpeechBubbleViewModel.ChatMessageModel.Message is null)
            {
                return;
            }
            ImageTable image = await ImageDBManager.INSTANCE.getImageForMessageAsync(SpeechBubbleViewModel.ChatMessageModel.Message);
            MODEL.UpdateView(image);
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
