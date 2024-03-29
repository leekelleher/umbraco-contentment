﻿/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
#if NET472
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class UmbracoFilesDataListSource : IDataListSource
    {
        private static readonly Dictionary<string, string> _icons = new Dictionary<string, string>
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

        public string Name => "Umbraco Files";

        public string Description => "Use files defined in Umbraco, such as scripts or stylesheets.";

        public string Icon => "icon-notepad-alt";

        public string Group => Constants.Conventions.DataSourceGroups.Umbraco;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
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
            new ConfigurationField
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

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "fileType", UmbConstants.UdiEntityType.Stylesheet },
            { "valueType", "alias" },
        };

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var fileType = config.GetValueAs("fileType", defaultValue: UmbConstants.UdiEntityType.Stylesheet);
            var valueType = config.GetValueAs("valueType", defaultValue: "alias");

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

            string getValue(IFile file)
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
                    Name = x.Name,
                    Value = getValue(x),
                    Icon = _icons.ContainsKey(fileType) == true ? _icons[fileType] : Icon,
                    Description = x.VirtualPath,
                });
        }
    }
}
