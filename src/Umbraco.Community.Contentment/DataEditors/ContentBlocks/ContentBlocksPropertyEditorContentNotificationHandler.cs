/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ContentBlocksPropertyEditorContentNotificationHandler : ComplexPropertyEditorContentNotificationHandler
    {
        protected override string EditorAlias => ContentBlocksDataEditor.DataEditorAlias;

        protected override string FormatPropertyValue(string rawJson, bool onlyMissingKeys)
        {
            // onlyMissingKeys: saving=true; copying=false;
            // NOTE: Not interested if it's being saved, or the value is null, not JSON, or empty JSON.
            if (onlyMissingKeys == true ||
                string.IsNullOrWhiteSpace(rawJson) == true ||
                rawJson.DetectIsJson() == false ||
                rawJson.DetectIsEmptyJson() == true)
            {
                return rawJson;
            }

            var blocks = JsonConvert.DeserializeObject<IEnumerable<ContentBlock>>(rawJson);

            foreach (var block in blocks)
            {
                // NOTE: When copying a content node, we need to replace the `key` value/Guid of each block item,
                // otherwise this may lead to caching issues within the content-cache (NuCache).
                block.Key = Guid.NewGuid();

                // TODO: [LK:2022-09-01] Next up, interrogate the inner JSON to see if
                // there are nested ContentBlock values, and recursively update those keys.
                // Research how BlockList and NestedContent handle those scenarios.
                // https://github.com/umbraco/Umbraco-CMS/blob/v10/contrib/src/Umbraco.Infrastructure/PropertyEditors/BlockEditorPropertyHandler.cs
                // https://github.com/umbraco/Umbraco-CMS/blob/v10/contrib/src/Umbraco.Infrastructure/PropertyEditors/NestedContentPropertyHandler.cs
            }

            return JsonConvert.SerializeObject(blocks, Formatting.None);
        }
    }
}
