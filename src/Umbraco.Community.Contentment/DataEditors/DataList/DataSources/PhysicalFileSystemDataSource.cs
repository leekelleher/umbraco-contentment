/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;
using IHostingEnvironment = Umbraco.Cms.Core.Hosting.IHostingEnvironment;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class PhysicalFileSystemDataSource : DataListToDataPickerSourceBridge, IContentmentDataSource, IContentmentListTemplateItem
    {
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IIOHelper _ioHelper;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<PhysicalFileSystem> _logger;

        public PhysicalFileSystemDataSource(
            IIOHelper ioHelper,
            IHostingEnvironment hostingEnvironment,
            IWebHostEnvironment webHostEnvironment,
            ILogger<PhysicalFileSystem> logger,
            IShortStringHelper shortStringHelper)
        {
            _shortStringHelper = shortStringHelper;
            _ioHelper = ioHelper;
            _hostingEnvironment = hostingEnvironment;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public override string Name => "File System";

        public string? NameTemplate => default;

        public override string Description => "Select paths from the physical file system as the data source.";

        public string? DescriptionTemplate => "{= path }; {= filter }";

        public override string Icon => "icon-fa-folder-tree";

        public override string Group => Constants.Conventions.DataSourceGroups.Data;

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<ContentmentConfigurationField> Fields => new[]
        {
            new ContentmentConfigurationField
            {
                Key = "path",
                Name = "Folder path",
                Description = "Enter the relative path of the folder. e.g. <code>~/css</code><br>Please note, this is relative to the web root folder, e.g. wwwroot.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new ContentmentConfigurationField
            {
                Key = "filter",
                Name = "Filename filter",
                Description = "Enter a wildcard filter for the filenames. e.g. <code>*.css</code>",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new ContentmentConfigurationField
            {
                Key = "friendlyName",
                Name = "Use friendly filenames?",
                Description = "Enabling this option will remove the file extension and spaces-out any uppercase letters, hyphens and underscores from the item name.",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.Toggle",
            }
        };

        public override Dictionary<string, object> DefaultValues => new()
        {
            { "path", "~/css" },
            { "filter", "*.css" },
        };

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var path = config.GetValueAs("path", string.Empty);
            var filter = config.GetValueAs("filter", string.Empty);
            var friendlyName = config.GetValueAs("friendlyName", false);

            var virtualRoot = string.IsNullOrWhiteSpace(path) == false
                ? path.EnsureEndsWith("/")
                : "~/";

            var fileFilter = string.IsNullOrWhiteSpace(filter) == false
                ? filter
                : "*.*";

            var fs = new PhysicalFileSystem(_ioHelper, _hostingEnvironment, _logger, _webHostEnvironment.MapPathWebRoot(virtualRoot), _hostingEnvironment.ToAbsolute(virtualRoot));

            var files = fs.GetFiles(".", fileFilter);

            return files.Select(x => new DataListItem
            {
                Name = friendlyName == true ? x.SplitPascalCasing(_shortStringHelper).ToFriendlyName() : x,
                Value = virtualRoot + x,
                Description = virtualRoot + x,
                Icon = UmbConstants.Icons.DefaultIcon,
            });
        }
    }
}
