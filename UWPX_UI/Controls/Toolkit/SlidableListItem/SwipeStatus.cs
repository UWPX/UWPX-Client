// Backport of the SlidableListItem from the Windows Community Toolkit
// Source: https://github.com/windows-toolkit/WindowsCommunityToolkit/blob/v4.0.0/Microsoft.Toolkit.Uwp.UI.Controls/SlidableListItem/SwipeStatus.cs
// Original license:
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace UWPX_UI.Controls.Toolkit.SlidableListItem
{
    /// <summary>
    /// Types of swipe status.
    /// </summary>
    public enum SwipeStatus
    {
        /// <summary>
        /// Swiping is not occurring.
        /// </summary>
        Idle,

        /// <summary>
        /// Swiping is going to start.
        /// </summary>
        Starting,

        /// <summary>
        /// Swiping to the left, but the command is disabled.
        /// </summary>
        DisabledSwipingToLeft,

        /// <summary>
        /// Swiping to the left below the threshold.
        /// </summary>
        SwipingToLeftThreshold,

        /// <summary>
        /// Swiping to the left above the threshold.
        /// </summary>
        SwipingPassedLeftThreshold,

        /// <summary>
        /// Swiping to the right, but the command is disabled.
        /// </summary>
        DisabledSwipingToRight,

        /// <summary>
        /// Swiping to the right below the threshold.
        /// </summary>
        SwipingToRightThreshold,

        /// <summary>
        /// Swiping to the right above the threshold.
        /// </summary>
        SwipingPassedRightThreshold
    }
}
