/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class TemplatedListDataListEditor : IDataListEditor
    {
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "templated-list.html";

        private readonly IIOHelper _ioHelper;

        public TemplatedListDataListEditor(IIOHelper ioHelper)
        {
            _ioHelper = ioHelper;
        }

        public string Name => "Templated List";

        public string Description => "Select items from a list rendered with custom markup.";

        public string Icon => "icon-fa fa-code";

        public string Group => default;

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new NotesConfigurationField(_ioHelper, @"<details class=""well well-small"">
<summary><strong>Do you need help with your custom template?</strong></summary>
<p>Your custom template will be used to display an individual item from your configured data source.</p>
<p>The data for the item will be in the following format:</p>
<pre><code>{
  ""name"": ""..."",
  ""value"": ""..."",
  ""description"": ""..."", // optional
  ""icon"": ""..."", // optional
  ""disabled"": ""true|false"", // optional
  ""selected"": ""true|false"",
}</code></pre>
<p>If you are familiar with AngularJS template syntax, you can display the values using an expression: e.g. <code ng-non-bindable>{{ item.name }}</code>.</p>
<p>If you need assistance with AngularJS expression syntax, please refer to this resource: <a href=""https://docs.angularjs.org/guide/expression"" target=""_blank""><strong>docs.angularjs.org</strong></a>.</p>
<hr>
<p>If you would like a starting point for your custom template, here is an example.</p>
<umb-code-snippet language=""'AngularJS template'"">&lt;i class=""icon"" ng-class=""item.icon""&gt;&lt;/i&gt;
&lt;span ng-bind=""item.name""&gt;&lt;/span&gt;</umb-code-snippet>
</details>", true),
            new ConfigurationField
            {
                Key = "template",
                Name = "Template",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(CodeEditorDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { CodeEditorConfigurationEditor.Mode, "razor" },
                    { "minLines", 12 },
                    { "maxLines", 30 },
                }
            },
            new AllowClearConfigurationField(),
            new ConfigurationField
            {
                Key = "enableMultiple",
                Name = "Multiple selection?",
                Description = "Select to enable picking multiple items.",
                View = "boolean",
            },
            new HtmlAttributesConfigurationField(_ioHelper),
        };

        public Dictionary<string, object> DefaultConfig => default;

        public Dictionary<string, object> DefaultValues => default;

        public bool HasMultipleValues(Dictionary<string, object> config)
        {
            return config.TryGetValueAs("enableMultiple", out bool enableMultiple) == true && enableMultiple == true;
        }

        public OverlaySize OverlaySize => OverlaySize.Medium;

        public string View => _ioHelper.ResolveRelativeOrVirtualUrl(DataEditorViewPath);
    }
}
