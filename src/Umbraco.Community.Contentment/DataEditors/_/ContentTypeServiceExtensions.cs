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
        // TODO: [LK:2019-09-13] My PR to Umbraco core got merged in. Remove this code when v8.2.0 is out. https://github.com/umbraco/Umbraco-CMS/pull/6262
        public static IEnumerable<IContentType> GetAllElementTypes(this IContentTypeService contentTypeService)
        {
            if (contentTypeService == null)
            {
                return Enumerable.Empty<IContentType>();
            }

            return contentTypeService.GetAll().Where(x => x.IsElement);
        }
    }
}
