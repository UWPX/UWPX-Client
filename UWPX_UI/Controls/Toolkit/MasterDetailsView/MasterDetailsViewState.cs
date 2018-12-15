// Backport of the MasterDetailsView from the Windows Community Toolkit
// Source: https://github.com/windows-toolkit/WindowsCommunityToolkit/blob/master/Microsoft.Toolkit.Uwp.UI.Controls/MasterDetailsView/MasterDetailsViewState.cs
// Original license:
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace UWPX_UI.Controls.Toolkit.MasterDetailsView
{
    /// <summary>
    /// The <see cref="MasterDetailsView"/> state.
    /// </summary>
    public enum MasterDetailsViewState
    {
        /// <summary>
        /// Only the Master view is shown
        /// </summary>
        Master,

        /// <summary>
        /// Only the Details view is shown
        /// </summary>
        Details,

        /// <summary>
        /// Both the Master and Details views are shown
        /// </summary>
        Both
    }
}
