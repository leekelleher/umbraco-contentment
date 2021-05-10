/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class DropdownListDataListEditor : IDataListEditor
    {
        internal const string AllowEmpty = "allowEmpty";
        internal const string DataEditorViewPath = Constants.Internals.EditorsPathRoot + "dropdown-list.html";

        private readonly IIOHelper _ioHelper;

        public DropdownListDataListEditor(IIOHelper ioHelper)
        {
            _ioHelper = ioHelper;
        }

        public string Name => "Dropdown List";

        public string Description => "Select a single value from a dropdown select list.";

        public string Icon => "icon-fa fa-caret-square-o-down";

        public string Group => default;

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new ConfigurationField
            {
                Key = AllowEmpty,
                Name = "Allow empty?",
                Description = "Enable to allow an empty option at the top of the dropdown list.<br>When disabled, the default value will be set to the first option.",
                View = "views/propertyeditors/boolean/boolean.html",
                Config = new Dictionary<string, object>
                {
                    { "default", Constants.Values.True }
                }
            },
            new HtmlAttributesConfigurationField(_ioHelper),
        };

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { AllowEmpty, Constants.Values.True }
        };

        public Dictionary<string, object> DefaultConfig => default;

        public bool HasMultipleValues(Dictionary<string, object> config) => false;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public string View => _ioHelper.ResolveRelativeOrVirtualUrl(DataEditorViewPath);
    }
}
