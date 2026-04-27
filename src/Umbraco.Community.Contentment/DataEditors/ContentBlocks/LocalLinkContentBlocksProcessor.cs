/* Copyright © 2025 Lee Kelleher, Umbraco Community Contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Infrastructure.Migrations.Upgrade.V_15_0_0.LocalLinks;

namespace Umbraco.Community.Contentment.DataEditors
{
    /// <summary>
    /// Handles the processing and migration of local links within Content Blocks
    /// as part of the Umbraco upgrade process to version 15.0.0.
    /// </summary>
    /// <remarks>
    /// Content Blocks stores its data as a JSON array of <see cref="ContentBlock"/> objects,
    /// where each block's <see cref="ContentBlock.Value"/> dictionary contains the property
    /// values for that block's element type. This processor iterates through all blocks and
    /// their property values, delegating to the appropriate nested processor (e.g. RTE, Block List)
    /// for each property value.
    /// </remarks>
#pragma warning disable CS0618 // Type or member is obsolete
    public class LocalLinkContentBlocksProcessor : ITypedLocalLinkProcessor
    {
        /// <summary>
        /// Gets the type of the property editor value, which is <see cref="List{ContentBlock}"/>.
        /// This matches the runtime type returned by <see cref="ContentBlocksDataValueEditor.ToEditor"/>
        /// when deserializing the Content Blocks JSON value.
        /// </summary>
        public Type PropertyEditorValueType => typeof(List<ContentBlock>);

        /// <summary>
        /// Gets the collection of property editor aliases that this processor supports.
        /// </summary>
        public IEnumerable<string> PropertyEditorAliases => [ContentBlocksDataEditor.DataEditorAlias];

        /// <summary>
        /// Gets a function that processes Content Blocks data containing local links.
        /// </summary>
        public Func<object?, Func<object?, bool>, Func<string, string>, bool> Process => ProcessContentBlocks;

        private static bool ProcessContentBlocks(
            object? value,
            Func<object?, bool> processNested,
            Func<string, string> processStringValue)
        {
            if (value is not IEnumerable<ContentBlock> blocks)
            {
                return false;
            }

            var hasChanged = false;

            foreach (var block in blocks)
            {
                foreach (var property in block.Value)
                {
                    if (processNested(property.Value))
                    {
                        hasChanged = true;
                    }
                }
            }

            return hasChanged;
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
