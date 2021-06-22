/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Umbraco.Cms.Core.WebAssets
{
    internal class ContentmentCssFile : CssFile
    {
        public ContentmentCssFile()
            : base(Community.Contentment.Constants.WebAssets.CssPath)
        { }
    }
}
