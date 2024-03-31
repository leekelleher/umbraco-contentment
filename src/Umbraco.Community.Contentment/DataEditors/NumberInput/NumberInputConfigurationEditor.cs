/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class NumberInputConfigurationEditor : ConfigurationEditor
    {
        public NumberInputConfigurationEditor(IIOHelper ioHelper)
            : base()
        {
            DefaultConfiguration.Add("size", "s");

            Fields.Add(new ContentmentConfigurationField
            {
                Key = "size",
                Name = "Numeric size",
                Description = "How big will the number get?",
                View = ioHelper.ResolveRelativeOrVirtualUrl(RadioButtonListDataListEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem { Name = "Small", Value = "s", Description = "Ideal for numbers under 100, comfortably fits 3 digits." },
                            new DataListItem { Name = "Medium", Value = "m", Description = "Better when dealing with hundreds and thousands, comfortably fits 6 digits." },
                            new DataListItem { Name = "Large", Value = "l", Description = "Did you know a 18 digit number is called a quintillion!" },
                            new DataListItem { Name = "Extra Large", Value = "xl", Description = "Useful when aligning with full width text inputs. Fits 88 digits <em>- that's over an octovigintillion!</em>" },
                        }
                    },
                    { ShowDescriptionsConfigurationField.ShowDescriptions, Constants.Values.True },
                    { Constants.Conventions.ConfigurationFieldAliases.DefaultValue, "s" },
                }
            });

            // TODO: [LK:2020-12-11] Commented out the value-type feature for the time being. Adds additional complexity that I don't currently need.
            //// NOTE: When using the key "umbracoDataValueType", Umbraco will auto-set the database type.
            //// https://github.com/umbraco/Umbraco-CMS/blob/release-8.6.1/src/Umbraco.Core/Models/DataType.cs#L122-L126
            //Fields.Add(
            //    UmbConstants.PropertyEditors.ConfigurationKeys.DataValueType,
            //    "Value type",
            //    "Select the value type of this number input.",
            //    IOHelper.ResolveUrl(RadioButtonListDataListEditor.DataEditorViewPath),
            //    new Dictionary<string, object>()
            //    {
            //        { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
            //            {
            //                new DataListItem { Name = nameof(ValueTypes.Decimal), Value = ValueTypes.Decimal, Description = "Accepts any number with decimal places." },
            //                new DataListItem { Name = nameof(ValueTypes.Integer), Value = ValueTypes.Integer, Description = "Accepts positive and negative whole numbers." },
            //            }
            //        },
            //        { ShowDescriptionsConfigurationField.ShowDescriptions, Constants.Values.True },
            //        { Constants.Conventions.ConfigurationFieldAliases.DefaultValue, ValueTypes.Integer },
            //    });

            // TODO: [LK:2020-12-11] Add "min/max" and "step" fields.

            Fields.Add(new ContentmentConfigurationField
            {
                Key = "placeholderText",
                Name = "Placeholder text",
                Description = "Add placeholder text for the number input.<br>This is to be used as instructional information, not as a default value.",
                View = "textstring",
            });
        }
    }
}
