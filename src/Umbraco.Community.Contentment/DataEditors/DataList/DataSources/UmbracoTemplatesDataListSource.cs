/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoTemplatesDataListSource : DataListToDataPickerSourceBridge, IDataListSource, IDataSourceValueConverter
    {
        private readonly IFileService _fileService;
        private readonly IIOHelper _ioHelper;

        public UmbracoTemplatesDataListSource(
            IFileService fileService,
            IIOHelper ioHelper)
        {
            _fileService = fileService;
            _ioHelper = ioHelper;
        }

        public override string Name => "Umbraco Templates";

        public override string Description => "Populate the data source using defined view templates.";

        public override string Icon => UmbConstants.Icons.Template;

        public override string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<ContentmentConfigurationField> Fields => new[]
        {
            new ContentmentConfigurationField
            {
                Key = "valueType",
                Name = "Value type",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(RadioButtonListDataListEditor.DataEditorViewPath),
                PropertyEditorUiAlias = RadioButtonListDataListEditor.DataEditorUiAlias,
                Description = "Select the type of reference to store as the value for the template.",
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem
                            {
                                Name = "Alias",
                                Value = "alias",
                                Description = "The alias of the file's name, e.g. `homePage`",
                            },
                            new DataListItem
                            {
                                Name = "File path",
                                Value = "path",
                                Description = "The path of the file, e.g. `/Views/homePage.cshtml`",
                            },
                            new DataListItem
                            {
                                Name = "UDI",
                                Value = "udi",
                                Description = "The Umbraco reference ID, e.g. `umb://template/&lt;guid&gt;`",
                            },
                        }
                    },
                    { ShowDescriptionsConfigurationField.ShowDescriptions, Constants.Values.True },
                }
            },
        };

        public override Dictionary<string, object>? DefaultValues => new()
        {
            { "valueType", "udi" },
        };

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var valueType = config.GetValueAs("valueType", defaultValue: "udi");

            string? getValue(IFile file)
            {
                switch (valueType)
                {
                    case "path":
                        return file.VirtualPath;

                    case "udi":
                        return Udi.Create(UmbConstants.UdiEntityType.Template, file.Key).ToString();

                    case "alias":
                    default:
                        return file.Alias;
                }
            }

            return _fileService
                .GetTemplates()
                .OrderBy(x => x.Name)
                .Select(x => new DataListItem
                {
                    Name = x.Name ?? x.Alias,
                    Value = getValue(x) ?? x.Alias,
                    Icon = Icon,
                    Description = x.VirtualPath ?? x.Alias,
                });
        }

        public Type? GetValueType(Dictionary<string, object>? config)
        {
            var valueType = config?.GetValueAs("valueType", defaultValue: "udi");
            return valueType.InvariantEquals("udi") == true
                ? typeof(Udi)
                : typeof(string);
        }

        public object? ConvertValue(Type type, string value)
        {
            if (type == typeof(Udi))
            {
                return UdiParser.TryParse(value, out var udi) == true ? udi : default;
            }

            return value;
        }
    }
}
