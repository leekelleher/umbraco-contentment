/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class TemplatedLabelConfigurationEditor : ConfigurationEditor
    {
        public TemplatedLabelConfigurationEditor(IIOHelper ioHelper)
            : base()
        {
            var valueTypes = new[]
            {
                new DataListItem { Name = nameof(ValueTypes.Bigint), Value = ValueTypes.Bigint },
                new DataListItem { Name = nameof(ValueTypes.Date), Value = ValueTypes.Date },
                new DataListItem { Name = nameof(ValueTypes.DateTime), Value = ValueTypes.DateTime },
                new DataListItem { Name = nameof(ValueTypes.Decimal), Value = ValueTypes.Decimal },
                new DataListItem { Name = nameof(ValueTypes.Integer), Value = ValueTypes.Integer },
                new DataListItem { Name = nameof(ValueTypes.Json), Value = ValueTypes.Json },
                new DataListItem { Name = nameof(ValueTypes.String), Value = ValueTypes.String },
                new DataListItem { Name = nameof(ValueTypes.Text), Value = ValueTypes.Text },
                new DataListItem { Name = nameof(ValueTypes.Time), Value = ValueTypes.Time },
                new DataListItem { Name = nameof(ValueTypes.Xml), Value = ValueTypes.Xml },
            };

            DefaultConfiguration.Add(UmbConstants.PropertyEditors.ConfigurationKeys.DataValueType, ValueTypes.String);

            Fields.Add(new ConfigurationField
            {
                Key = UmbConstants.PropertyEditors.ConfigurationKeys.DataValueType,
                Name = "Value type",
                Description = "Select the value's type. This defines how the underlying value is stored in the database.",
                View = ioHelper.ResolveRelativeOrVirtualUrl(DropdownListDataListEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { DropdownListDataListEditor.AllowEmpty, Constants.Values.False },
                    { Constants.Conventions.ConfigurationFieldAliases.Items, valueTypes },
                }
            });

            Fields.Add(new NotesConfigurationField(ioHelper, @"<details class=""well well-small umb-property-editor--limit-width"">
<summary><strong>Do you need help with your custom template?</strong></summary>
<p>Your custom template will be used to display the label on the property from the underlying value.</p>
<p>If you are familiar with AngularJS template syntax, you can display the value using an expression: e.g. <code ng-non-bindable>{{ model.value }}</code>.</p>
<p>If you need assistance with AngularJS expression syntax, please refer to this resource: <a href=""https://docs.angularjs.org/guide/expression"" target=""_blank""><strong>docs.angularjs.org</strong></a>.</p>
<hr>
<p>If you would like a starting point for your custom template, here is an example.</p>
<umb-code-snippet language=""'AngularJS template'"">&lt;details class=""well well-small umb-property-editor--limit-width""&gt;
    &lt;summary&gt;View data&lt;/summary&gt;
    &lt;umb-code-snippet language=""'JSON'""&gt;{{model.config.code}}&lt;/umb-code-snippet&gt;
&lt;/details&gt;</umb-code-snippet>
</details>", false) { Key = "_notes", Name = "", Config = { { "code", "{{ model.value | json }}" } } });

            Fields.Add(new ConfigurationField
            {
                Key = "notes",
                Name = "Template",
                Description = "Enter the AngularJS template to be displayed for the label.",
                View = ioHelper.ResolveRelativeOrVirtualUrl(CodeEditorDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { CodeEditorConfigurationEditor.Mode, "razor" },
                    { "minLines", 20 },
                    { "maxLines", 30 },
                }
            });

            Fields.Add(new HideLabelConfigurationField());
            Fields.Add(new HidePropertyGroupConfigurationField());
            Fields.Add(new EnableDevModeConfigurationField());
        }
    }
}
