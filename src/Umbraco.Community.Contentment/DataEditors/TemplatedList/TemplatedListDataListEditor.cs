/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [Core.Composing.HideFromTypeFinder]
    public sealed class TemplatedListDataListEditor : IDataListEditor
    {
        public string Name => "Templated List";

        public string Description => "Something, Something, Something, Dark Side.";

        public string Icon => "icon-fa fa-code";

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new ConfigurationField
            {
                Key = "template",
                Name = "Template",
                View = IOHelper.ResolveUrl(CodeEditorDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { CodeEditorConfigurationEditor.Mode, "razor" },
                    { "minLines", 12 },
                    { "maxLines", 30 },
                }
            },
            new EnableMultipleConfigurationField(),
            new HtmlAttributesConfigurationField(),
        };

        public Dictionary<string, object> DefaultConfig => default;

        public Dictionary<string, object> DefaultValues => default;

        public bool HasMultipleValues(Dictionary<string, object> config) => false;

        public string View => Constants.Internals.EditorsPathRoot + "templated-list.html";
    }
}
