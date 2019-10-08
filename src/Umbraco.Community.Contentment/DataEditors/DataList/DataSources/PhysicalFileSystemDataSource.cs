/* Copyright © 2019 Lee Kelleher.
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
    internal sealed class PhysicalFileSystemDataSource : IDataListSource
    {
        public string Name => "File System";

        public string Description => "Select file paths from the file system as the data source.";

        public string Icon => "icon-folder-close";

        [ConfigurationField("path", "Folder Path", "textstring", Description = "Enter the relative path of the folder. e.g. `~/css`")]
        public string Path { get; set; }

        [ConfigurationField("filter", "Filename filter", "textstring", Description = "Enter a wildcard filter for the filenames. e.g. `*.css`")]
        public string Filter { get; set; }

        public IEnumerable<DataListItem> GetItems()
        {
            var path = string.IsNullOrWhiteSpace(Path) == false
                ? Path.EnsureEndsWith("/")
                : "~/";

            var filter = string.IsNullOrWhiteSpace(Filter) == false
                ? Filter
                : "*.*";

            var fs = new PhysicalFileSystem(path);
            var files = fs.GetFiles(".", filter);

            return files.Select(x => new DataListItem
            {
                Name = x,
                Value = path + x,
                Description = path + x,
            });
        }
    }
}
