/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
#if NET472
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;
#else
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal class TemplatedDataPickerDisplayMode : IDataPickerDisplayMode
    {
        private readonly IIOHelper _ioHelper;

        public TemplatedDataPickerDisplayMode(IIOHelper ioHelper)
        {
            _ioHelper = ioHelper;
        }

        public string Name => "Templated";

        public string Description => "Items will be rendered with custom markup.";

        public string Icon => TemplatedLabelDataEditor.DataEditorIcon;

        public string Group => default;

        public string View => Constants.Internals.EditorsPathRoot + "data-picker.html";

        public Dictionary<string, object> DefaultValues => default;

        public Dictionary<string, object> DefaultConfig => new Dictionary<string, object>
        {
            { "displayMode", "templated" },
        };

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new NotesConfigurationField(_ioHelper, @"<details class=""well well-small"">
<summary><strong>Do you need help with your custom template?</strong></summary>
<p>Your custom template will be used to display an individual block.</p>
<p>The data for the item will the raw/string value.</p>
<p>If you are familiar with AngularJS template syntax, you can display the values using an expression: e.g. <code ng-non-bindable>{{ item.name }}</code>.</p>
<p>If you need assistance with AngularJS expression syntax, please refer to this resource: <a href=""https://docs.angularjs.org/guide/expression"" target=""_blank""><strong>docs.angularjs.org</strong></a>.</p>
<hr>
<p>If you would like a starting point for your custom template, here is an example.</p>
<umb-code-snippet language=""'AngularJS template'"" >&lt;div class=""contentment umb-readonlyvalue editor-note flex items-center alert-form umb-listview""&gt;
    &lt;umb-icon icon=""{{model.config.code}}""&gt;&lt;/umb-icon&gt;
    &lt;div&gt;
        &lt;h5 class=""mt1 mb1"" ng-bind=""vm.populate(item, $index, 'name')"">&lt;/h5&gt;
        &lt;span ng-bind=""item""&gt;&lt;/span&gt;
    &lt;/div&gt;
&lt;/div&gt;</umb-code-snippet>
</details>", true) { Config = { { "code", "{{vm.populate(item, $index, 'icon')}}" } } },
            new ConfigurationField
            {
                Key = "itemTemplate",
                Name = "Item template",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(CodeEditorDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { CodeEditorConfigurationEditor.Mode, "razor" },
                    { "minLines", 12 },
                    { "maxLines", 30 },
                }
            },
        };

        public OverlaySize OverlaySize => OverlaySize.Large;
    }
}
