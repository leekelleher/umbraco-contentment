/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class TemplatedListDataListEditor : IContentmentListEditor
    {
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "templated-list.html";
        internal const string DataEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "TemplatedList";

        public string Name => "Templated List";

        public string Description => "Select items from a list rendered with custom markup.";

        public string Icon => TemplatedLabelDataEditor.DataEditorIcon;

        public string? Group => default;

        public IEnumerable<ContentmentConfigurationField> Fields => new ContentmentConfigurationField[]
        {
            // TODO: [LK] Add a note to explain why this has changed from AngularJS template to a web component.

//            new NotesConfigurationField(_ioHelper, @"<details class=""well well-small"">
//<summary><strong>Do you need help with your custom template?</strong></summary>
//<p>Your custom template will be used to display an individual item from your configured data source.</p>
//<p>The data for the item will be in the following format:</p>
//<pre><code>{
//  ""name"": ""..."",
//  ""value"": ""..."",
//  ""description"": ""..."", // optional
//  ""icon"": ""..."", // optional
//  ""disabled"": ""true|false"", // optional
//  ""selected"": ""true|false"",
//}</code></pre>
//<p>If you are familiar with AngularJS template syntax, you can display the values using an expression: e.g. <code ng-non-bindable>{{ item.name }}</code>.</p>
//<p>If you need assistance with AngularJS expression syntax, please refer to this resource: <a href=""https://docs.angularjs.org/guide/expression"" target=""_blank""><strong>docs.angularjs.org</strong></a>.</p>
//<hr>
//<p>If you would like a starting point for your custom template, here is an example.</p>
//<umb-code-block language=""'AngularJS template'"" copy>&lt;i class=""icon"" ng-class=""item.icon""&gt;&lt;/i&gt;
//&lt;span ng-bind=""item.name""&gt;&lt;/span&gt;</umb-code-block>
//</details>", true),
            new ContentmentConfigurationField
            {
                Key = "component",
                Name = "Component",
                PropertyEditorUiAlias = Constants.Internals.DataEditorUiAliasPrefix + "ManifestPicker",
                Config = new Dictionary<string, object>
                {
                    { "extensionType", "contentmentDataListItemUi" },
                    { "maxItems", 1 },
                }
            },
            new AllowClearConfigurationField(),
            new ContentmentConfigurationField
            {
                Key = "enableMultiple",
                Name = "Multiple selection?",
                Description = "Select to enable picking multiple items.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle",
            },
            new ()
            {
                Key = "orientation",
                Name = "Orientation",
                Description = "Select the orientation of the list. By default this is set to 'vertical' (column).",
                PropertyEditorUiAlias = RadioButtonListDataListEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem { Name = "Horizontal", Value = "horizontal" },
                            new DataListItem { Name = "Vertical", Value = "vertical" },
                        }
                    },
                    { "orientation", "horizontal" },
                }
            },
            new()
            {
                Key = "listStyles",
                Name = "List styles",
                Description = "<em>(optional)</em> Enter CSS rules for the list's container , e.g. <code>&lt;ul&gt;</code> element.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new ()
            {
                Key = "listItemStyles",
                Name = "List item styles",
                Description = "<em>(optional)</em> Enter CSS rules for each list item, e.g. <code>&lt;li&gt;</code> element.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
        };

        public Dictionary<string, object>? DefaultConfig => default;

        public Dictionary<string, object>? DefaultValues => new()
        {
            { "orientation", "vertical" },
        };

        public bool HasMultipleValues(Dictionary<string, object>? config)
        {
            return config?.TryGetValueAs("enableMultiple", out bool enableMultiple) == true && enableMultiple == true;
        }

        public OverlaySize OverlaySize => OverlaySize.Medium;

        [Obsolete("To be removed in Contentment 7.0. Migrate to use `PropertyEditorUiAlias`.")]
        public string View => DataEditorViewPath;

        public string PropertyEditorUiAlias => DataEditorUiAlias;
    }
}
