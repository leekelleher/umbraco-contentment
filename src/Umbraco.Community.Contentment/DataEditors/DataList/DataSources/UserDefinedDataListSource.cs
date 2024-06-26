/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UserDefinedDataListSource : DataListToDataPickerSourceBridge, IDataListSource, IContentmentListTemplateItem
    {
        private readonly IIOHelper _ioHelper;

        public UserDefinedDataListSource(IIOHelper ioHelper)
        {
            _ioHelper = ioHelper;
        }

        public override string Name => "User-defined List";

        public string? NameTemplate => default;

        public override string Description => "Manually configure the items for the data source.";

        public string? DescriptionTemplate => "{{ items.length }} {{ items.length === 1 ? 'item' : 'items' }} defined.";

        public override string Icon => UmbConstants.Icons.DataType;

        public override string Group => Constants.Conventions.DataSourceGroups.Data;

        public override IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
            {
                Key = "items",
                Name = "Options",
                Description = "Configure the option items for the data list.<br><br>Please try to avoid using duplicate values, as this may cause adverse issues with list editors.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(ListItemsDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>()
                {
                    { "confirmRemoval", Constants.Values.True },
                    { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.True },
                    { MaxItemsConfigurationField.MaxItems, 0 },
                    { NotesConfigurationField.Notes, @"<details class=""well well-small"">
<summary><strong><em>Advanced:</em> Paste in the raw JSON?</strong></summary>
<p>If you have copied the raw JSON from the Data List preview panel, <button class=""btn-reset"" ng-click=""vm.edit()""><strong>you can paste it in here</strong></button>.</p>
<p>The JSON format must be an array of the Data List item structure.<br />For example...</p>
<umb-code-snippet language=""'JSON'"">[
  {
    ""name"": ""Ready"",
    ""value"": ""value1"",
    ""icon"": ""icon-stop-alt color-red"",
    ""description"": ""One for the money.""
  }, {
    ""name"": ""Steady"",
    ""value"": ""value2"",
    ""icon"": ""icon-stop-alt color-orange"",
    ""description"": ""Two for the show.""
  }, {
    ""name"": ""Go!"",
    ""value"": ""value3"",
    ""icon"": ""icon-stop-alt color-green"",
    ""description"": ""Three to get ready. Now go, cat, go.""
  }
]</umb-code-snippet>
</details>" },
                },
            }
        };

        public override Dictionary<string, object>? DefaultValues => default;

        public override OverlaySize OverlaySize => OverlaySize.Medium;

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            return config.TryGetValueAs("items", out JArray? array) == true
                ? array?.ToObject<IEnumerable<DataListItem>>()?.DistinctBy(x => x.Value) ?? Enumerable.Empty<DataListItem>()
                : Enumerable.Empty<DataListItem>();
        }
    }
}
