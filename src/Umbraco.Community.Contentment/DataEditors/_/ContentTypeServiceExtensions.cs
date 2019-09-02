/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal static partial class ContentTypeServiceExtensions
    {
        // TODO: [LK:2019-08-05] This extension method should be in Umbraco core. Send a PR.
        public static IEnumerable<IContentType> GetAllElementTypes(this IContentTypeService contentTypeService)
        {
            return contentTypeService.GetAll().Where(x => x.IsElement);
        }
    }
}
