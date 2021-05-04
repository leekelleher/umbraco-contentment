/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.IO;
using Umbraco.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Hosting;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class CodeEditorConfigurationEditor : ConfigurationEditor
    {
        internal const string FontSize = "fontSize";
        internal const string Mode = "mode";
        internal const string Theme = "theme";
        internal const string UseWrapMode = "useWrapMode";
        internal const string MinLines = "minLines";
        internal const string MaxLines = "maxLines";

        public CodeEditorConfigurationEditor()
            : base()
        {
            var targetPath = "~/umbraco/lib/ace-builds/src-min-noconflict/";
            var aceEditorPath = IOHelper.MapPath(targetPath);

            if (Directory.Exists(aceEditorPath) == true)
            {
                var aceEditorFiles = Directory.GetFiles(aceEditorPath, "*.js");
                if (aceEditorFiles != null && aceEditorFiles.Length > 0)
                {
                    var modes = new List<DataListItem>();
                    var themes = new List<DataListItem>();

                    foreach (var file in aceEditorFiles)
                    {
                        var filename = Path.GetFileNameWithoutExtension(file);
                        if (filename.StartsWith("mode-") == true)
                        {
                            var mode = filename.Replace("mode-", string.Empty).ToLower();
                            modes.Add(new DataListItem { Name = mode.ToFirstUpperInvariant(), Value = mode });
                        }

                        if (filename.StartsWith("theme-") == true)
                        {
                            var theme = filename.Replace("theme-", string.Empty).ToLower();
                            themes.Add(new DataListItem { Name = theme.ToFirstUpperInvariant(), Value = theme });
                        }
                    }

                    if (modes.Count > 0)
                    {
                        DefaultConfiguration.Add(Mode, "razor");
                        Fields.Add(new ConfigurationField
                        {
                            Key = Mode,
                            Name = "Language mode",
                            Description = "Select the programming language mode. The default mode is 'Razor'.",
                            View = IOHelper.ResolveUrl(DropdownListDataListEditor.DataEditorViewPath),
                            Config = new Dictionary<string, object>
                            {
                                { DropdownListDataListEditor.AllowEmpty, Constants.Values.False },
                                { Constants.Conventions.ConfigurationFieldAliases.Items, modes },
                            }
                        });
                    }

                    if (themes.Count > 0)
                    {
                        DefaultConfiguration.Add(Theme, "chrome");
                        Fields.Add(new ConfigurationField
                        {
                            Key = Theme,
                            Name = nameof(Theme),
                            Description = "Set the theme for the code editor. The default theme is 'Chrome'.",
                            View = IOHelper.ResolveUrl(DropdownListDataListEditor.DataEditorViewPath),
                            Config = new Dictionary<string, object>
                            {
                                { DropdownListDataListEditor.AllowEmpty, Constants.Values.False },
                                { Constants.Conventions.ConfigurationFieldAliases.Items, themes },
                            }
                        });
                    }

                    if (modes.Count > 0 || themes.Count > 0)
                    {
                        Fields.Add(new NotesConfigurationField($@"<details class=""well well-small"">
<summary>Would you like to add more <strong>language modes</strong> and <strong>themes</strong>?</summary>
<p>This property editor makes use of <a href=""https://ace.c9.io/"" target=""_blank""><strong>AWS Cloud 9's Ace editor</strong></a> library that is distributed with Umbraco. By default, Umbraco ships a streamlined set of programming language modes and themes.</p>
<p>If you would like to add more modes and themes, you can do this by <a href=""https://github.com/ajaxorg/ace-builds/releases"" target=""_blank""><strong>downloading the latest pre-packaged version of the Ace editor</strong></a> and copy any of the <code>mode-*</code> or <code>theme-*</code> files from the <code>src-min-noconflict</code> folder over to the <code>{targetPath}</code> folder in this Umbraco installation.</p>
<p>When you reload this screen, the new programming language modes and themes will appear in the dropdown options above.</p>
</details>", true));
                    }
                }
            }

            DefaultConfiguration.Add(FontSize, "small");
            Fields.Add(new ConfigurationField
            {
                Key = FontSize,
                Name = "Font size",
                Description = @"Set the font size. The value must be a valid CSS <a href=""https://developer.mozilla.org/en-US/docs/Web/CSS/font-size"" target=""_blank""  rel=""noopener""><strong>font-size</strong></a> value. The default size is 'small'.",
                View = IOHelper.ResolveUrl(TextInputDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { Constants.Conventions.ConfigurationFieldAliases.Items, new[] {
                        new DataListItem { Name = "Extra extra small", Value = "xx-small" },
                        new DataListItem { Name = "Extra small", Value = "x-small" },
                        new DataListItem { Name = "Small", Value = "small" },
                        new DataListItem { Name = "Medium", Value = "medium" },
                        new DataListItem { Name = "Large", Value = "large" },
                        new DataListItem { Name = "Extra large", Value = "x-large" },
                        new DataListItem { Name = "Extra extra large", Value = "xx-large" },
                        new DataListItem { Name = "Extra extra extra large", Value = "xxx-large" },
                        new DataListItem { Name = "Use pixels?", Value = "14px" },
                        new DataListItem { Name = "Use percentage?", Value = "80%" },
                        new DataListItem { Name = "Use ems?", Value = "0.8em" },
                        new DataListItem { Name = "Use rems?", Value = "1.2rem" },
                    } },
                }
            });

            Fields.Add(new ConfigurationField
            {
                Key = UseWrapMode,
                Name = "Word wrapping",
                Description = "Select to enable word wrapping.",
                View = "boolean"
            });

            // NOTE: [LK:2019-06-07] Hidden the advanced options (for now), need to review.
            //Fields.Add("showGutter", "Show gutter?", "Select to show the left-hand side gutter in the code editor.", "boolean");
            //Fields.Add("firstLineNumber", "First Line Number", "[A friendly description]", "number");
            //Fields.Add("showInvisibles", "showInvisibles", "[A friendly description]", "boolean");// showInvisibles: 0,
            //Fields.Add("showIndentGuides", "showIndentGuides", "[A friendly description]", "boolean");// showIndentGuides: 0,
            //Fields.Add("useSoftTabs", "useSoftTabs", "[A friendly description]", "boolean");// useSoftTabs: 1,
            //Fields.Add("showPrintMargin", "showPrintMargin", "[A friendly description]", "boolean");// showPrintMargin: 0,
            //Fields.Add("disableSearch", "disableSearch", "[A friendly description]", "boolean");// disableSearch: 0,
            //Fields.Add("enableSnippets", "enableSnippets", "[A friendly description]", "boolean");// enableSnippets: 0,
            //Fields.Add("enableBasicAutocompletion", "enableBasicAutocompletion", "[A friendly description]", "boolean");// enableBasicAutocompletion: 0,
            //Fields.Add("enableLiveAutocompletion", "enableLiveAutocompletion", "[A friendly description]", "boolean");// enableLiveAutocompletion: 0,
            //Fields.Add("readonly", "readonly", "[A friendly description]", "boolean");// readonly: 0,

            DefaultConfiguration.Add(MinLines, 12);
            Fields.Add(new ConfigurationField
            {
                Key = MinLines,
                Name = "Minimum lines",
                Description = "Set the minimum number of lines that the editor will be. The default is 12 lines.",
                View = IOHelper.ResolveUrl(NumberInputDataEditor.DataEditorViewPath)
            });

            DefaultConfiguration.Add(MaxLines, 30);
            Fields.Add(new ConfigurationField
            {
                Key = MaxLines,
                Name = "Maximum lines",
                Description = "Set the maximum number of lines that the editor can be. If left empty, the editor will not auto-scale.",
                View = IOHelper.ResolveUrl(NumberInputDataEditor.DataEditorViewPath)
            });
        }
    }
}
