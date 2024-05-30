/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoFilesDataListSource : DataListToDataPickerSourceBridge, IDataListSource
    {
        private static readonly Dictionary<string, string> _icons = new()
        {
            { UmbConstants.UdiEntityType.Script, "icon-script" },
            { UmbConstants.UdiEntityType.Stylesheet, "icon-brackets" },
        };

        private readonly IFileService _fileService;
        private readonly IIOHelper _ioHelper;
        private readonly ILocalizedTextService _textService;

        public UmbracoFilesDataListSource(
            IFileService fileService,
            IIOHelper ioHelper,
            ILocalizedTextService textService)
        {
            _fileService = fileService;
            _ioHelper = ioHelper;
            _textService = textService;
        }

        public override string Name => "Umbraco Files";

        public override string Description => "Use files defined in Umbraco, such as scripts or stylesheets.";

        public override string Icon => "icon-notepad-alt";

        public override string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<ContentmentConfigurationField> Fields => new[]
        {
            new ContentmentConfigurationField
            {
                Key = "fileType",
                Name = "File type",
                Description = "Select the Umbraco file type to use.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(RadioButtonListDataListEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>()
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem
                            {
                                Name = _textService.Localize("treeHeaders", "scripts"),
                                Value = UmbConstants.UdiEntityType.Script
                            },
                            new DataListItem
                            {
                                Name = _textService.Localize("treeHeaders", "stylesheets"),
                                Value = UmbConstants.UdiEntityType.Stylesheet
                            }
                        }
                    },
                }
            },
            new ContentmentConfigurationField
            {
                Key = "valueType",
                Name = "Value type",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(RadioButtonListDataListEditor.DataEditorViewPath),
                Description = "Select the type of value to reference the file.",
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[]
                        {
                            new DataListItem
                            {
                                Name = _textService.Localize("content", "alias"),
                                Value = "alias",
                                Description = "The alias of the file's name, e.g. `styles`",
                            },
                            new DataListItem
                            {
                                Name = _textService.Localize("general", "path"),
                                Value = "path",
                                Description = "The path of the file, e.g. `/css/styles.css`",
                            }
                        }
                    },
                    { ShowDescriptionsConfigurationField.ShowDescriptions, Constants.Values.True },
                }
            },
        };

        public override Dictionary<string, object>? DefaultValues => new()
        {
            { "fileType", UmbConstants.UdiEntityType.Stylesheet },
            { "valueType", "alias" },
        };

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var fileType = config.GetValueAs("fileType", defaultValue: UmbConstants.UdiEntityType.Stylesheet) ?? UmbConstants.UdiEntityType.Stylesheet;
            var valueType = config.GetValueAs("valueType", defaultValue: "alias") ?? "alias";

            IEnumerable<IFile> getFiles()
            {
                switch (fileType)
                {
                    case UmbConstants.UdiEntityType.Script:
                        return _fileService.GetScripts();

                    case UmbConstants.UdiEntityType.Stylesheet:
                    default:
                        return _fileService.GetStylesheets();
                }
            };

            string? getValue(IFile file)
            {
                switch (valueType)
                {
                    case "path":
                        return file.VirtualPath;

                    case "alias":
                    default:
                        return file.Alias;
                }
            }

            return getFiles()
                .OrderBy(x => x.Name)
                .Select(x => new DataListItem
                {
                    Name = x.Name ?? x.Alias,
                    Value = getValue(x) ?? x.Alias,
                    Icon = _icons.ContainsKey(fileType) == true ? _icons[fileType] : Icon,
                    Description = x.VirtualPath ?? x.Alias,
                });
        }
    }
}
