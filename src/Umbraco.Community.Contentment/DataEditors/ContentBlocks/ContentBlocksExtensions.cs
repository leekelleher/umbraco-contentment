/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
#if NET472
using Umbraco.Core;
using Umbraco.Core.Models.Blocks;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.Blocks;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public static class ContentBlocksExtensions
    {
        public static BlockEditorData ToBlockListEditorData(this IEnumerable<ContentBlock> blocks)
        {
            return blocks.ToBlockEditorData(UmbConstants.PropertyEditors.Aliases.BlockList);
        }

        public static BlockEditorData ToBlockEditorData(this IEnumerable<ContentBlock> blocks, string propertyEditorAlias)
        {
            var blockValue = blocks.ToBlockValue();

            var references = Enumerable
                .Range(0, blockValue.ContentData.Count)
                .Select(idx => new ContentAndSettingsReference(
                    blockValue.ContentData.ElementAtOrDefault(idx)?.Udi,
                    blockValue.SettingsData.ElementAtOrDefault(idx)?.Udi
                )
            );

            return new BlockEditorData(propertyEditorAlias, references, blockValue);
        }

        public static BlockValue ToBlockValue(this IEnumerable<ContentBlock> blocks)
        {
            var layout = new List<BlockListLayoutItem>();
            var contentData = new List<BlockItemData>();

            foreach (var block in blocks)
            {
                var data = new BlockItemData
                {
                    ContentTypeKey = block.ElementType,
                    Udi = Udi.Create(UmbConstants.UdiEntityType.Element, block.Key),
                };

                if (block.Value != null)
                {
                    data.RawPropertyValues = block.Value;
                }

                contentData.Add(data);

                layout.Add(new BlockListLayoutItem { ContentUdi = data.Udi });
            }

            return new BlockValue
            {
                ContentData = contentData,
                Layout = new Dictionary<string, JToken>()
                {
                    { UmbConstants.PropertyEditors.Aliases.BlockList, JArray.FromObject(layout) }
                },
            };
        }

        public static IEnumerable<ContentBlock> ToContentBlocks(
            this BlockEditorData blockEditorData,
            string propertyEditorAlias = UmbConstants.PropertyEditors.Aliases.BlockList)
        {
            return blockEditorData.BlockValue.ToContentBlocks(propertyEditorAlias);
        }

        public static IEnumerable<ContentBlock> ToContentBlocks(
            this BlockValue blockValue,
            string propertyEditorAlias = UmbConstants.PropertyEditors.Aliases.BlockList)
        {
            var blocks = new List<ContentBlock>();

            if (blockValue.Layout.TryGetValue(propertyEditorAlias, out var token) == true)
            {
                var layout = token.ToObject<IEnumerable<BlockListLayoutItem>>();
                var lookup = blockValue.ContentData.ToDictionary(x => x.Udi);

                foreach (var item in layout)
                {
                    if (lookup.TryGetValue(item.ContentUdi, out var data) == true)
                    {
                        blocks.Add(new ContentBlock
                        {
                            ElementType = data.ContentTypeKey,
                            Key = data.Key,
                            Udi = data.Udi,
                            Value = data.RawPropertyValues,
                        });
                    }
                }
            }

            return blocks;
        }
    }
}
