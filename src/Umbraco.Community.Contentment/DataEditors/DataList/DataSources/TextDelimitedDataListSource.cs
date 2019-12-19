/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    [HideFromTypeFinder]
    internal sealed class TextDelimitedDataListSource : IDataListSource
    {
        private readonly ILogger _logger;

        public TextDelimitedDataListSource(ILogger logger)
        {
            _logger = logger;
        }

        public string Name => "Text Delimited Data";

        public string Description => "Configure text-delimited data to populate the data source.";

        public string Icon => "icon-ordered-list";

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "delimiter", "," },
            { "nameIndex", 0 },
            { "valueIndex", 1 },
            { "iconIndex", -1 },
            { "descriptionIndex", -1 },
        };

        [ConfigurationField(typeof(TextDelimitedNotesConfigurationField))]
        public string Notes { get; set; }

        [ConfigurationField("url", "URL", "textstring", Description = "Enter the URL of the text-delimited data source.<br>This can be either a remote URL, or local relative file path.")]
        public string Url { get; set; }

        [ConfigurationField("delimiter", "Delimiter", "textstring", Description = "Enter the character to use as the delimiter. The default delimiter is a comma, <code>,</code>.")]
        public string Delimiter { get; set; }

        [ConfigurationField("ignoreFirstLine", "Ignore the first line?", "boolean", Description = "Select to ignore the first line. Typically with text delimited data sources, the first line can be used for column headings.")]
        public bool IgnoreFirstLine { get; set; }

        [ConfigurationField("nameIndex", "Name Index", "number", Description = "Enter the index position of the name field from the delimited line. The default index position is '0'.")]
        public int NameIndex { get; set; }

        [ConfigurationField("valueIndex", "Value Index", "number", Description = "Enter the index position of the value (key) field from the delimited line. The default index position is '1'.")]
        public int ValueIndex { get; set; }

        [ConfigurationField("iconIndex", "Icon Index", "number", Description = "<em>(optional)</em> Enter the index position of the icon field from the delimited line. To ignore this option, set to '-1'.")]
        public int IconIndex { get; set; }

        [ConfigurationField("descriptionIndex", "Description Index", "number", Description = "<em>(optional)</em> Enter the index position of the description field from the delimited line. To ignore this option, set to '-1'.")]
        public int DescriptionIndex { get; set; }

        public IEnumerable<DataListItem> GetItems()
        {
            var items = new List<DataListItem>();

            var lines = GetTextLines();

            if (lines == null || lines.Length == 0)
                return items;

            var delimiter = string.IsNullOrEmpty(Delimiter) == false ? new[] { Delimiter } : new[] { "," };
            var nameIndex = NameIndex >= 0 ? NameIndex : 0;
            var valueIndex = ValueIndex >= 0 ? ValueIndex : 0;
            var iconIndex = IconIndex >= 0 ? IconIndex : -1;
            var descriptionIndex = DescriptionIndex >= 0 ? DescriptionIndex : -1;

            for (var i = 0; i < lines.Length; i++)
            {
                if (i == 0 && IgnoreFirstLine)
                {
                    continue;
                }

                var fields = lines[i].Split(delimiter, StringSplitOptions.None);

                if (fields.Length == 0)
                {
                    continue;
                }

                var trimChars = new[] { ' ', '"', '\'' };

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

        private string[] GetTextLines()
        {
            if (string.IsNullOrWhiteSpace(Url))
                return default;

            if (Url.InvariantStartsWith("http"))
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        return client.DownloadString(Url).Split('\r', '\n');
                    }
                }
                catch (WebException ex)
                {
                    _logger.Error<TextDelimitedDataListSource>(ex, "Unable to fetch remote data.");
                }
            }
            else
            {
                // assume local file
                var path = IOHelper.MapPath(Url);
                if (File.Exists(path))
                {
                    return File.ReadAllLines(path);
                }
                else
                {
                    _logger.Warn<TextDelimitedDataListSource>("Unable to find the local file path.");
                }
            }

            return default;
        }

        class TextDelimitedNotesConfigurationField : NotesConfigurationField
        {
            public TextDelimitedNotesConfigurationField()
                : base(@"<div class=""alert alert-info"">
<p><strong>A note about using this data source.</strong></p>
<p>The text contents will be retrieved and split into lines. Each line will be split into fields by the delimiting character.</p>
<p>The fields are then assigned by index position.</p>
</div>", true)
            { }
        }
    }
}
