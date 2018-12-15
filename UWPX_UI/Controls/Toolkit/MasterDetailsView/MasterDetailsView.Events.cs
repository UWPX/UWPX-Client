// Backport of the MasterDetailsView from the Windows Community Toolkit
// Source: https://github.com/windows-toolkit/WindowsCommunityToolkit/blob/master/Microsoft.Toolkit.Uwp.UI.Controls/MasterDetailsView/MasterDetailsView.Events.cs
// Original license:
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using Windows.UI.Xaml.Controls;

namespace UWPX_UI.Controls.Toolkit.MasterDetailsView
{
    /// <summary>
    /// Panel that allows for a Master/Details pattern.
    /// </summary>
    /// <seealso cref="ItemsControl" />
    public partial class MasterDetailsView
    {
        /// <summary>
        /// Occurs when the currently selected item changes.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Occurs when the view state changes
        /// </summary>
        public event EventHandler<MasterDetailsViewState> ViewStateChanged;

        private void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
    }
}
