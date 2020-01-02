// Based on the IncrementalLoadingCollection from the Windows Community Toolkit
// Source: https://github.com/windows-toolkit/WindowsCommunityToolkit/blob/v5.1.1/Microsoft.Toolkit.Uwp/IncrementalLoadingCollection/IncrementalLoadingCollection.cs
// Original license:
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**
 * Changes: Replaced ObservableCollection with CustomObservableCollection
 **/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Collections;
using Shared.Classes.Collections;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace UWPX_UI_Context.Classes.Collections.Toolkit
{
    /// <summary>
    /// This class represents an <see cref="CustomObservableCollection{IType}"/> whose items can be loaded incrementally.
    /// </summary>
    /// <typeparam name="IType">
    /// The type of collection items.
    /// </typeparam>
    /// <seealso cref="ISupportIncrementalLoading"/>
    public class IncrementalLoadingCollection<IType>: CustomObservableCollection<IType>, ISupportIncrementalLoading
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// Gets or sets an <see cref="Action"/> that is called when a retrieval operation begins.
        /// </summary>
        public Action OnStartLoading { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="Action"/> that is called when a retrieval operation ends.
        /// </summary>
        public Action OnEndLoading { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="Action"/> that is called if an error occurs during data retrieval. The actual <see cref="Exception"/> is passed as an argument.
        /// </summary>
        public Action<Exception> OnError { get; set; }

        /// <summary>
        /// Gets a value indicating how many items that must be retrieved for each incremental call.
        /// </summary>
        protected int ItemsPerPage { get; }

        /// <summary>
        /// Gets or sets a value indicating The zero-based index of the current items page.
        /// </summary>
        protected int CurrentPageIndex { get; set; }

        private bool _isLoading;
        private bool _hasMoreItems;
        private CancellationToken _cancellationToken;
        private bool _refreshOnLoad;

        /// <summary>
        /// Gets a value indicating whether new items are being loaded.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;

            private set
            {
                if (value != _isLoading)
                {
                    _isLoading = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsLoading)));

                    if (_isLoading)
                    {
                        OnStartLoading?.Invoke();
                    }
                    else
                    {
                        OnEndLoading?.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains more items to retrieve.
        /// </summary>
        public bool HasMoreItems
        {
            get => _cancellationToken.IsCancellationRequested ? false : _hasMoreItems;

            private set
            {
                if (value != _hasMoreItems)
                {
                    _hasMoreItems = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(HasMoreItems)));
                }
            }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Initializes a new instance of the <see cref="IncrementalLoadingCollection{TSource, IType}"/> class using the specified <see cref="IIncrementalSource{TSource}"/> implementation and, optionally, how many items to load for each data page.
        /// </summary>
        /// <param name="itemsPerPage">
        /// The number of items to retrieve for each call. Default is 20.
        /// </param>
        /// <param name="onStartLoading">
        /// An <see cref="Action"/> that is called when a retrieval operation begins.
        /// </param>
        /// <param name="onEndLoading">
        /// An <see cref="Action"/> that is called when a retrieval operation ends.
        /// </param>
        /// <param name="onError">
        /// An <see cref="Action"/> that is called if an error occurs during data retrieval.
        /// </param>
        /// <param name="invokeInUiThread">
        /// An <see cref="bool"/> that indicates whether the UI thread should be invoked once the collection changes.
        /// </param>
        /// <seealso cref="IIncrementalSource{TSource}"/>
        public IncrementalLoadingCollection(int itemsPerPage = 20, Action onStartLoading = null, Action onEndLoading = null, Action<Exception> onError = null, bool invokeInUiThread = true) : base(invokeInUiThread)
        {
            OnStartLoading = onStartLoading;
            OnEndLoading = onEndLoading;
            OnError = onError;

            ItemsPerPage = itemsPerPage;
            _hasMoreItems = true;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Initializes incremental loading from the view.
        /// </summary>
        /// <param name="count">
        /// The number of items to load.
        /// </param>
        /// <returns>
        /// An object of the <see cref="LoadMoreItemsAsync(uint)"/> that specifies how many items have been actually retrieved.
        /// </returns>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return LoadMoreItemsAsync(count, new CancellationToken(false)).AsAsyncOperation();
        }

        /// <summary>
        /// Clears the collection and triggers/forces a reload of the first page
        /// </summary>
        /// <returns>This method does not return a result</returns>
        public Task RefreshAsync()
        {
            if (IsLoading)
            {
                _refreshOnLoad = true;
            }
            else
            {
                int previousCount = Count;
                Clear();
                CurrentPageIndex = 0;
                HasMoreItems = true;

                if (previousCount == 0)
                {
                    // When the list was empty before clearing, the automatic reload isn't fired, so force a reload.
                    return LoadMoreItemsAsync(0).AsTask();
                }
            }

            return Task.CompletedTask;
        }

        #endregion

        #region --Misc Methods (Private)--
        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(uint count, CancellationToken cancellationToken)
        {
            uint resultCount = 0;
            _cancellationToken = cancellationToken;

            try
            {
                if (!_cancellationToken.IsCancellationRequested)
                {
                    IEnumerable<IType> data = null;
                    try
                    {
                        IsLoading = true;
                        data = await LoadDataAsync(_cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // The operation has been canceled using the Cancellation Token.
                    }
                    catch (Exception ex) when (OnError != null)
                    {
                        OnError.Invoke(ex);
                    }

                    if (data != null && data.Any() && !_cancellationToken.IsCancellationRequested)
                    {
                        resultCount = (uint)data.Count();

                        foreach (IType item in data)
                        {
                            Add(item);
                        }
                    }
                    else
                    {
                        HasMoreItems = false;
                    }
                }
            }
            finally
            {
                IsLoading = false;

                if (_refreshOnLoad)
                {
                    _refreshOnLoad = false;
                    await RefreshAsync();
                }
            }

            return new LoadMoreItemsResult { Count = resultCount };
        }

        #endregion

        #region --Misc Methods (Protected)--
        /// <summary>
        /// Actually performs the incremental loading.
        /// </summary>
        /// <param name="cancellationToken">
        /// Used to propagate notification that operation should be canceled.
        /// </param>
        /// <returns>
        /// Returns a collection of <typeparamref name="IType"/>.
        /// </returns>
        protected virtual async Task<IEnumerable<IType>> LoadDataAsync(CancellationToken cancellationToken)
        {
            IEnumerable<IType> result = await GetPagedItemsAsync(CurrentPageIndex++, ItemsPerPage, cancellationToken);
            return result;
        }

        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
