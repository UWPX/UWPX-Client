using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Shared.Classes;
using Windows.Storage;
using Windows.Storage.Search;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class FolderSizeControlDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _Text;
        public string Text
        {
            get => _Text;
            set => SetProperty(ref _Text, value);
        }

        private CancellationTokenSource calcSizeCancelToken = null;
        private Task calcSizeTask = null;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public Task UpdateViewAsync(string path)
        {
            return CalcFolderSizeAsync(path);
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task CalcFolderSizeAsync(string path)
        {
            if (calcSizeCancelToken is not null)
            {
                calcSizeCancelToken.Cancel();
            }
            calcSizeCancelToken = new CancellationTokenSource();

            if (calcSizeTask is not null)
            {
                await calcSizeTask;
            }

            Text = "Calculating size...";

            long size = 0;
            if (!string.IsNullOrWhiteSpace(path))
            {
                calcSizeTask = Task.Run(async () =>
                {
                    try
                    {
                        StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);
                        if (folder is not null)
                        {
                            StorageFileQueryResult result = folder.CreateFileQuery(CommonFileQuery.OrderByName);
                            System.Collections.Generic.IEnumerable<Task<ulong>> fileSizeTasks = (await result.GetFilesAsync()).Select(async file => (await file.GetBasicPropertiesAsync()).Size);
                            ulong[] fileSizes = await Task.WhenAll(fileSizeTasks);

                            // Sum up and convert to kilo byte:
                            size = fileSizes.Sum(l => (long)l) / 1024;
                        }
                    }
                    catch (FileNotFoundException e)
                    {
                        Logger.Error("Failed to calculate folder size for path: " + path, e);
                        Text = "Invalid path!";

                    }
                    catch (Exception e)
                    {
                        Logger.Error("Failed to calculate folder size for path: " + path, e);
                    }
                }, calcSizeCancelToken.Token);

                await calcSizeTask;
                if (calcSizeCancelToken is null || calcSizeCancelToken.IsCancellationRequested)
                {
                    return;
                }
            }

            StringBuilder sb = new StringBuilder("~ ");
            if (size >= 1024)
            {
                sb.Append(size / 1024);
                sb.Append(" MB");
            }
            else
            {
                sb.Append(size);
                sb.Append(" KB");
            }
            Text = sb.ToString();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
