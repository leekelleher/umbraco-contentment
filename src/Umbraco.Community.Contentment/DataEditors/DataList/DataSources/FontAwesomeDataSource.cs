/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;

namespace Umbraco.Community.Contentment.DataEditors
{
    [HideFromTypeFinder]
    internal class FontAwesomeDataSource : IDataListSource
    {
        public string Name => "Font Awesome";

        public string Description => "Select icons from Font Awesome.";

        public string Icon => "fa fa-font-awesome";

        public IEnumerable<DataListItem> GetItems()
        {
            var items = new HashSet<string>();

            var path = IOHelper.MapPath("~/umbraco/lib/font-awesome/css/font-awesome.min.css");
            if (File.Exists(path))
            {
                var contents = File.ReadAllText(path);

                var regex = new Regex("\\.fa-([^:]*?):before", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var matches = regex.Matches(contents);

                foreach (Match match in matches)
                {
                    var text = match.Groups[1].Value.Trim();
                    if (text.Length > 2 && items.Contains(text) == false)
                    {
                        items.Add(text);
                    }
                }
            }

            return items
                .OrderBy(x => x)
                .Select(x => new DataListItem
                {
                    Name = ToAwesomeName(x),
                    Value = x,
                    Icon = $"fa fa-{x}"
                });
        }

        private string ToAwesomeName(string input)
        {
            string GetName(string name)
            {
                if (name.InvariantEquals("o"))
                    return "(outline)";

                if (name.InvariantEquals("adn"))
                    return "App.net";

                if (name.InvariantEquals("buysellads"))
                    return "BuySellAds";

                if (name.InvariantEquals("cc"))
                    return "CC";

                if (name.InvariantEquals("css3"))
                    return "CSS3";

                if (name.InvariantEquals("html5"))
                    return "HTML5";

                if (name.InvariantEquals("lastfm"))
                    return "Last.fm";

                if (name.InvariantEquals("vcard"))
                    return "VCard";

                if (name.InvariantEquals("wifi"))
                    return "WiFi";

                if (name.InvariantEquals("youtube"))
                    return "YouTube";

                return name.ToFirstUpperInvariant();
            };

            if (string.IsNullOrWhiteSpace(input))
                return input;

            return string.Join(" ", input.ToDelimitedList("-").Select(GetName));
        }
    }
}
