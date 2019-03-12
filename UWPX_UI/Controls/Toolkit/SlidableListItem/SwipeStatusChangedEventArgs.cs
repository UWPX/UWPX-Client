// Backport of the SlidableListItem from the Windows Community Toolkit
// Source: https://github.com/windows-toolkit/WindowsCommunityToolkit/blob/v4.0.0/Microsoft.Toolkit.Uwp.UI.Controls/SlidableListItem/SwipeStatusChangedEventArgs.cs
// Original license:
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace UWPX_UI.Controls.Toolkit.SlidableListItem
{
    /// <summary>
    /// Event args for a SwipeStatus changing event
    /// </summary>
    public class SwipeStatusChangedEventArgs
    {
        /// <summary>
        /// Gets the old value.
        /// </summary>
        public SwipeStatus OldValue { get; internal set; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        public SwipeStatus NewValue { get; internal set; }
    }
}
