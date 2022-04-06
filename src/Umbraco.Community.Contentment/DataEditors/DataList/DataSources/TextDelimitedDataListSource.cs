/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
#if NET472
using Umbraco.Core;
using Umbraco.Core.Hosting;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;
#else
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class TextDelimitedDataListSource : IDataListSource, IContentmentListTemplateItem
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IIOHelper _ioHelper;

#if NET472
        private readonly ILogger _logger;

        public TextDelimitedDataListSource(
            ILogger logger,
            IHostingEnvironment hostingEnvironment,
            IIOHelper ioHelper)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _ioHelper = ioHelper;
        }
#else
        private readonly ILogger<TextDelimitedDataListSource> _logger;

        public TextDelimitedDataListSource(
            ILogger<TextDelimitedDataListSource> logger,
            IHostingEnvironment hostingEnvironment,
            IIOHelper ioHelper)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _ioHelper = ioHelper;
        }
#endif

        public string Name => "Text Delimited Data";

        public string NameTemplate => default;

        public string Description => "Configure text-delimited data to populate the data source.";

        public string DescriptionTemplate => "{{ url }}";

        public string Icon => "icon-fa fa-file-text-o";

        public string Group => Constants.Conventions.DataSourceGroups.Data;

        public OverlaySize OverlaySize => OverlaySize.Small;

        public IEnumerable<ConfigurationField> Fields => new[]
        {
            new NotesConfigurationField(_ioHelper, @"<details class=""well well-small"">
<summary><strong>A note about using this data source.</strong></summary>
<p>The text contents will be retrieved and split into lines. Each line will be split into fields by the delimiting character.</p>
<p>The fields are then assigned by index position.</p>
</details>", true),
            new ConfigurationField
            {
                Key = "url",
                Name = "URL",
                Description = "Enter the URL of the text-delimited data source.<br>This can be either a remote URL, or local relative file path.",
                View = "textstring"
            },
            new ConfigurationField
            {
                Key = "delimiter",
                Name = "Delimiter",
                Description = "Enter the character to use as the delimiter.<br>The default delimiter is a comma, <code>,</code>.",
                View = "textstring"
            },
            new ConfigurationField
            {
                Key = "ignoreFirstLine",
                Name = "Ignore the first line?",
                Description = "Select to ignore the first line. Typically with text delimited data sources, the first line can be used for column headings.",
                View = "boolean"
            },
            new ConfigurationField
            {
                Key = "nameIndex",
                Name = "Name Index",
                Description = "Enter the index position of the name field from the delimited line.<br>The default index position is <code>0</code>.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(NumberInputDataEditor.DataEditorViewPath),
            },
            new ConfigurationField
            {
                Key = "valueIndex",
                Name = "Value Index",
                Description = "Enter the index position of the value (key) field from the delimited line.<br>The default index position is <code>1</code>.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(NumberInputDataEditor.DataEditorViewPath),
            },
            new ConfigurationField
            {
                Key = "iconIndex",
                Name = "Icon Index",
                Description = "<em>(optional)</em> Enter the index position of the icon field from the delimited line. To ignore this option, set the value to <code>-1</code>.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(NumberInputDataEditor.DataEditorViewPath),
            },
            new ConfigurationField
            {
                Key = "descriptionIndex",
                Name = "Description Index",
                Description = "<em>(optional)</em> Enter the index position of the description field from the delimited line. To ignore this option, set the value to <code>-1</code>.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(NumberInputDataEditor.DataEditorViewPath),
            }
        };

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "url", "https://leekelleher.com/umbraco/contentment/data.csv" },
            { "delimiter", "," },
            { "nameIndex", 0 },
            { "valueIndex", 1 },
            { "iconIndex", 2 },
            { "descriptionIndex", 3 },
        };

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var items = new List<DataListItem>();

            var url = config.GetValueAs("url", string.Empty);
            var delimiter = config.GetValueAs("delimiter", ",");
            var ignoreFirstLine = config.GetValueAs("ignoreFirstLine", false);
            var nameIndex = config.GetValueAs("nameIndex", 0);
            var valueIndex = config.GetValueAs("valueIndex", 0);
            var iconIndex = config.GetValueAs("iconIndex", -1);
            var descriptionIndex = config.GetValueAs("descriptionIndex", -1);

            if (string.IsNullOrWhiteSpace(url) == true)
            {
                return items;
            }

            var lines = GetTextLines(url);

            if (lines == null || lines.Length == 0)
            {
                return items;
            }

            var separator = new[] { delimiter };
            var trimChars = new[] { ' ', '"', '\'' };

            for (var i = 0; i < lines.Length; i++)
            {
                if (i == 0 && ignoreFirstLine == true)
                {
                    continue;
                }

                var fields = lines[i].Split(separator, StringSplitOptions.None);

                if (fields.Length == 0)
                {
                    continue;
                }

                var name = nameIndex >= 0 && fields.Length > nameIndex
                    ? fields[nameIndex].Trim(trimChars)
                    : null;

                var value = valueIndex >= 0 && fields.Length > valueIndex
                    ? fields[valueIndex].Trim(trimChars)
                    : null;

                if (name != null && value != null)
                {
                    var icon = iconIndex >= 0 && fields.Length > iconIndex
                        ? fields[iconIndex].Trim(trimChars)
                        : null;

                    var description = descriptionIndex >= 0 && fields.Length > descriptionIndex
                        ? fields[descriptionIndex].Trim(trimChars)
                        : null;

                    items.Add(new DataListItem
                    {
                        Icon = icon,
                        Name = name,
                        Description = description,
                        Value = value
                    });
                }
            }

            return items;
        }

        private string[] GetTextLines(string url)
        {
            if (url.InvariantStartsWith("http") == true)
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        return client.DownloadString(url).Split('\r', '\n');
                    }
                }
                catch (WebException ex)
                {
#if NET472
                    _logger.Error<TextDelimitedDataListSource>(ex, "Unable to fetch remote data.");
#else
                    _logger.LogError(ex, "Unable to fetch remote data.");
#endif
                }
            }
            else
            {
                // assume local file
                var path = _hostingEnvironment.MapPathWebRoot(url);
                if (File.Exists(path) == true)
                {
                    return File.ReadAllLines(path);
                }
                else
                {
#if NET472
                    _logger.Warn<TextDelimitedDataListSource>("Unable to find the local file path.");
#else
                    _logger.LogWarning("Unable to find the local file path.");
#endif
                }
            }

            return default;
        }
    }
}
