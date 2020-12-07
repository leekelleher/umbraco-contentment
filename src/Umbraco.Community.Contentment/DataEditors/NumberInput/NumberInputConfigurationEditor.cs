/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
using UmbConstants = Umbraco.Core.Constants;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class NumberInputConfigurationEditor : ConfigurationEditor
    {
        public NumberInputConfigurationEditor()
            : base()
        {
            Fields.Add(
                "sizeClass",
                "Numeric size",
                "How big will the number get?",
                IOHelper.ResolveUrl(RadioButtonListDataListEditor.DataEditorViewPath),
                new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem { Name = "Small", Value = "umb-property-editor-tiny", Description = "Ideal for numbers under 100; comfortably fits 3 digits." },
                            new DataListItem { Name = "Medium", Value = "umb-property-editor-small", Description = "Better when dealing with hundreds and thousands; comfortably fits 6 digits." },
                            new DataListItem { Name = "Large", Value = "umb-property-editor--limit-width", Description = "Did you know a 18 digit number is called a quintillion!" },
                            new DataListItem { Name = "Massive", Value = "umb-property-editor", Description = "Useful when aligning with full width text inputs. Fits 88 digits - that's over an octovigintillion folks!" },
                        }
                    },
                    { ShowDescriptionsConfigurationField.ShowDescriptions, Constants.Values.True },
                    { Constants.Conventions.ConfigurationFieldAliases.DefaultValue, "umb-property-editor-tiny" },
                });

            // NOTE: When using the key "umbracoDataValueType", Umbraco will auto-set the database type.
            // https://github.com/umbraco/Umbraco-CMS/blob/release-8.6.1/src/Umbraco.Core/Models/DataType.cs#L122-L126
            Fields.Add(
                UmbConstants.PropertyEditors.ConfigurationKeys.DataValueType,
                "Value type",
                "Select the value type of this number input.",
                IOHelper.ResolveUrl(RadioButtonListDataListEditor.DataEditorViewPath),
                new Dictionary<string, object>()
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem { Name = nameof(ValueTypes.Decimal), Value = ValueTypes.Decimal, Description = "Accepts any number with decimal places." },
                            new DataListItem { Name = nameof(ValueTypes.Integer), Value = ValueTypes.Integer, Description = "Accepts positive and negative whole numbers." },
                        }
                    },
                    { ShowDescriptionsConfigurationField.ShowDescriptions, Constants.Values.True },
                    { Constants.Conventions.ConfigurationFieldAliases.DefaultValue, ValueTypes.Integer },
                });

            // TODO: Add the "min/max" and "step" fields.

            Fields.Add(
                "placeholderText",
                "Placeholder text",
                "Add placeholder text for the number input.<br>This is to be used as instructional information, not as a default value.",
                "textstring"
            );
        }
    }
}
