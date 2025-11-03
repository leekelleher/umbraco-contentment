/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Models.Blocks;

namespace Umbraco.Community.Contentment.DataEditors
{
    public static class ContentBlocksExtensions
    {
        public static BlockEditorData<BlockListValue, BlockListLayoutItem> ToBlockListEditorData(this IEnumerable<ContentBlock> blocks)
        {
            var blockValue = blocks.ToBlockListValue();

            var references = Enumerable
                .Range(0, blockValue.ContentData.Count)
                .Select(idx => new ContentAndSettingsReference(
                    blockValue.ContentData.ElementAt(idx).Key,
                    blockValue.SettingsData.ElementAtOrDefault(idx)?.Key
                )
            );

            return new BlockEditorData<BlockListValue, BlockListLayoutItem>(references, blockValue);
        }

        public static BlockListValue ToBlockListValue(this IEnumerable<ContentBlock> blocks)
        {
            var layout = new List<BlockListLayoutItem>();
            var contentData = new List<BlockItemData>();

            foreach (var block in blocks)
            {
                var data = new BlockItemData
                {
                    ContentTypeKey = block.ElementType,
                    Key = block.Key,
                };

                if (block.Value != null)
                {
                    data.Values = block.Value.Select(x => new BlockPropertyValue { Alias = x.Key, Value = x.Value }).ToList();
                }

                contentData.Add(data);

                layout.Add(new BlockListLayoutItem { ContentKey = data.Key });
            }

            return new BlockListValue(layout) { ContentData = contentData };
        }
    }
}
