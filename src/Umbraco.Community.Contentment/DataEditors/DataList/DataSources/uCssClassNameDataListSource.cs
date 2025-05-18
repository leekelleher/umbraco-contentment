/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
#pragma warning disable IDE1006 // Naming Styles
    public class uCssClassNameDataListSource : DataListToDataPickerSourceBridge, IContentmentDataSource
#pragma warning restore IDE1006 // Naming Styles
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public uCssClassNameDataListSource(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public override string Name => "uCssClassName";

        public override string Description => "A homage to @marcemarc's bingo-famous uCssClassNameDropdown package!";

        public override string Icon => "icon-fa-css3";

        public override string Group => Constants.Conventions.DataSourceGroups.Web;

        public override IEnumerable<ContentmentConfigurationField> Fields => new[]
        {
            new NotesConfigurationField(@"<details class=""well"">
<summary><strong>uCssClassName? <em>What sort of a name is that?</em></strong></summary>
<p>Welcome to a piece of Umbraco package history.</p>
<p>First released back in 2013 by <a href=""http://tooorangey.co.uk/"" target=""_blank"">Marc Goodson</a>, <a href=""https://our.umbraco.com/packages/backoffice-extensions/ucssclassnamedropdown/"" target=""_blank""><strong>uCssClassNameDropdown</strong></a> became one of the most popular packages for Umbraco v4.11.3.</p>
<p>This was Marc's vision...</p>
<blockquote cite=""http://tooorangey.co.uk/posts/ucssclassnamedropdown-property-editor-for-umbraco-7/"">
<p>&ldquo;Based on the principle that 'all data' should be stored in stylesheets, it allows you to populate a data list of Css Class Names pulled directly from the source stylesheet file.</p>
<p>Perfect for font-awesome icons or surfacing similar icon or background image choices to the editor.&rdquo;</p>
</blockquote>
<p>As a mark of respect to the loyal fans of the original package, I hereby offer this data source as tribute.</p>
<p style=""text-align:center;""><img src=""https://leekelleher.com/umbraco/contentment/assets/ucssclassname.png"" alt=""English Heritage Blue Plaque for uCssClassName"" loading=""lazy""></p>
</details>", true),
            new ContentmentConfigurationField
            {
                Key = "cssPath",
                Name = "PathToStylesheet",
                Description = "Put in the relative path to the stylesheet",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new ContentmentConfigurationField
            {
                Key = "cssRegex",
                Name = "Class Name Regex",
                Description = "put in the regex pattern that matches the class names",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new ContentmentConfigurationField
            {
                Key = "excludeList",
                Name = "Exclusions",
                Description = "comma delimited list of styles to exclude",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
            new ContentmentConfigurationField
            {
                Key = "iconPattern",
                Name = "Icon Pattern",
                Description = "Class name pattern to display icon, eg 'icon icon-{0}'",
                PropertyEditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            },
        };

        public override Dictionary<string, object>? DefaultValues => new()
        {
            // TODO: I need to find an alternative, since FontAwesome no longer ships with Umbraco. [LK]
            { "cssPath", "~/umbraco/lib/font-awesome/css/font-awesome.min.css" },
            { "cssRegex", "\\.fa-([^:]*?):before" },
            { "excludeList", "" },
            { "iconPattern", "icon-fa fa-{0}" },
        };

        public override OverlaySize OverlaySize => OverlaySize.Small;

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var cssPath = config.GetValueAs("cssPath", string.Empty) ?? string.Empty;
            var cssRegex = config.GetValueAs("cssRegex", string.Empty) ?? string.Empty;
            var excludeList = config.GetValueAs("excludeList", string.Empty)?.ToDelimitedList();
            var iconPattern = config.GetValueAs("iconPattern", string.Empty);

            var items = new HashSet<string>();
            var contents = GetCssFileContents(cssPath);

            if (string.IsNullOrWhiteSpace(contents) == false)
            {
                var regex = new Regex(cssRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var matches = regex.Matches(contents);

                foreach (Match match in matches)
                {
                    var text = match.Groups[1].Value.Trim();
                    if (text.Length > 2 && items.Contains(text) == false)
                    {
                        _ = items.Add(text);
                    }
                }
            }

            return items
                .Where(x => excludeList?.Count == 0 || excludeList?.Any(x.InvariantContains) == false)
                .OrderBy(x => x)
                .Select(x => new DataListItem
                {
                    Name = x,
                    Value = x,
                    Icon = string.IsNullOrWhiteSpace(iconPattern) == false ? string.Format(iconPattern, x) : null,
                });
        }

        private string? GetCssFileContents(string cssPath)
        {
            var file = _webHostEnvironment.WebRootFileProvider.GetFileInfo(cssPath.TrimStart("~/"));

            if (file.Exists == true)
            {
                using var stream = file.CreateReadStream();
                using var reader = new StreamReader(stream);

                return reader.ReadToEnd();
            }

            return default;
        }
    }
}
