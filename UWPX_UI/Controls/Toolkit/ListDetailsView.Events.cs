// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// Source: https://github.com/CommunityToolkit/WindowsCommunityToolkit/tree/v7.1.0/Microsoft.Toolkit.Uwp.UI.Controls.Layout/ListDetailsView

using System;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Toolkit
{
    /// <summary>
    /// Panel that allows for a List/Details pattern.
    /// </summary>
    /// <seealso cref="ItemsControl" />
    public partial class ListDetailsView
    {
        /// <summary>
        /// Occurs when the currently selected item changes.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Occurs when the view state changes.
        /// </summary>
        public event EventHandler<ListDetailsViewState> ViewStateChanged;

        private void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
    }
}
