﻿/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class PhysicalFileSystemDataSource : IDataListSource
    {
        public string Name => "File System";

        public string Description => "Select file paths from the file system as the data source.";

        public string Icon => "icon-folder-close";

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new ConfigurationField
            {
                Key = "path",
                Name = "Folder Path",
                Description = "Enter the relative path of the folder. e.g. <code>~/css</code>",
                View = "textstring",
            },
            new ConfigurationField
            {
                Key = "filter",
                Name = "Filename filter",
                Description = "Enter a wildcard filter for the filenames. e.g. <code>*.css</code>",
                View = "textstring",
            }
        };

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "path", "~/" },
            { "filter", "*.*" },
        };

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var path = config.GetValueAs("path", string.Empty);
            var filter = config.GetValueAs("filter", string.Empty);

            var virtualRoot = string.IsNullOrWhiteSpace(path) == false
                ? path.EnsureEndsWith("/")
                : "~/";

            var fileFilter = string.IsNullOrWhiteSpace(filter) == false
                ? filter
                : "*.*";

            var fs = new PhysicalFileSystem(virtualRoot);
            var files = fs.GetFiles(".", fileFilter);

            return files.Select(x => new DataListItem
            {
                Name = x,
                Value = path + x,
                Description = path + x,
            });
        }
    }
}
