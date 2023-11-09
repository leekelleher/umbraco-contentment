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
    public sealed class PhysicalFileSystemDataSource : IDataListSource, IContentmentListTemplateItem
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

        public string Name => "File System";

        public string NameTemplate => default;

        public string Description => "Select paths from the physical file system as the data source.";

        public string DescriptionTemplate => "{{ path }}; {{ filter }}";

        public string Icon => "icon-folder-close";

        public string Group => Constants.Conventions.DataSourceGroups.Data;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
            {
                Key = "path",
                Name = "Folder path",
                Description = "Enter the relative path of the folder. e.g. <code>~/css</code><br>Please note, this is relative to the web root folder, e.g. wwwroot.",
                View = "textstring",
            },
            new ConfigurationField
            {
                Key = "filter",
                Name = "Filename filter",
                Description = "Enter a wildcard filter for the filenames. e.g. <code>*.css</code>",
                View = "textstring",
            },
            new ConfigurationField
            {
                Key = "friendlyName",
                Name = "Use friendly filenames?",
                Description = "Enabling this option will remove the file extension and spaces-out any uppercase letters, hyphens and underscores from the item name.",
                View = "boolean",
            }
        };

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "path", "~/css" },
            { "filter", "*.css" },
        };

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
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
