/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.IO;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class CodeEditorConfigurationEditor : ConfigurationEditor
    {
        public const string FontSize = "fontSize";
        public const string HideLabel = Constants.Conventions.ConfigurationEditors.HideLabel;
        public const string Mode = "mode";
        public const string Theme = "theme";
        public const string UseWrapMode = "useWrapMode";

        public CodeEditorConfigurationEditor()
            : base()
        {
            Fields.AddNotes(@"<div class=""alert alert-info"">
<p>This property editor makes use of <a href=""https://ace.c9.io/"" target=""_blank""><strong>AWS Cloud 9's Ace editor</strong></a> that is distributed with Umbraco. By default, Umbraco ships a minimal set of programming language modes and themes.</p>
<p>If you would like to add more modes and themes, you can do this by <a href=""https://github.com/ajaxorg/ace-builds/releases"" target=""_blank""><strong>downloading the latest pre-packaged version of the Ace editor</strong></a> and copy whichever <code>mode-*</code> or <code>theme-*</code> files from the <code>src-min-noconflict</code> folder over to the <code>~/umbraco/lib/ace-builds/src-min-noconflict</code> folder in this Umbraco installation.</p>
<p>When you reload this screen, the new programming language modes and themes will appear in the dropdown options below.</p>
</div>");

            var aceEditorPath = IOHelper.MapPath("~/umbraco/lib/ace-builds/src-min-noconflict/");
            if (Directory.Exists(aceEditorPath))
            {
                var aceEditorFiles = Directory.GetFiles(aceEditorPath, "*.js");
                if (aceEditorFiles != null && aceEditorFiles.Length > 0)
                {
                    var modes = new List<object>();
                    var themes = new List<object>();

                    foreach (var file in aceEditorFiles)
                    {
                        var filename = Path.GetFileNameWithoutExtension(file);
                        if (filename.StartsWith("mode-"))
                        {
                            var mode = filename.Replace("mode-", string.Empty).ToLower();
                            modes.Add(new { name = mode.ToFirstUpper(), value = mode });
                        }

                        if (filename.StartsWith("theme-"))
                        {
                            var theme = filename.Replace("theme-", string.Empty).ToLower();
                            themes.Add(new { name = theme.ToFirstUpper(), value = theme });
                        }
                    }

                    if (modes.Count > 0)
                    {
                        Fields.Add(
                            Mode,
                            "Programming language mode",
                            "Select the programming language mode. By default, 'Razor' mode will be used.",
                            IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath),
                            new Dictionary<string, object>
                            {
                                { DropdownListConfigurationEditor.AllowEmpty, Constants.Values.False },
                                { DropdownListConfigurationEditor.Items, modes },
                                { DropdownListConfigurationEditor.DefaultValue, "razor" }
                            });
                    }

                    if (themes.Count > 0)
                    {
                        Fields.Add(
                            Theme,
                            nameof(Theme),
                            "Set the theme for the code editor. By default, 'Chrome' will be used.",
                            IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath),
                            new Dictionary<string, object>
                            {
                                { DropdownListConfigurationEditor.AllowEmpty, Constants.Values.False },
                                { DropdownListConfigurationEditor.Items, themes },
                                { DropdownListConfigurationEditor.DefaultValue, "chrome" }
                            });
                    }
                }
            }

            Fields.Add(FontSize, "Font size", "Set the font size. The value must be a valid CSS font-size. The default value is '14px'.", "textstring");
            Fields.Add(UseWrapMode, "Word wrapping", "Select to enable word wrapping.", "boolean");

            // NOTE: [LK:2019-06-07] Hidden the advanced options (for now), need to review.
            //Fields.Add("showGutter", "Show gutter?", "Select to show the left-hand side gutter in the code editor.", "boolean"); // TODO: Tempted to reverse the logic here, then use ToValueEditor to negate it? [LK]
            //Fields.Add("firstLineNumber", "First Line Number", "[A friendly description]", "number");
            //Fields.Add("showInvisibles", "showInvisibles", "[A friendly description]", "boolean");// showInvisibles: 0,
            //Fields.Add("showIndentGuides", "showIndentGuides", "[A friendly description]", "boolean");// showIndentGuides: 0,
            //Fields.Add("useSoftTabs", "useSoftTabs", "[A friendly description]", "boolean");// useSoftTabs: 1,
            //Fields.Add("showPrintMargin", "showPrintMargin", "[A friendly description]", "boolean");// showPrintMargin: 0,
            //Fields.Add("disableSearch", "disableSearch", "[A friendly description]", "boolean");// disableSearch: 0,
            //Fields.Add("enableSnippets", "enableSnippets", "[A friendly description]", "boolean");// enableSnippets: 0,
            //Fields.Add("enableBasicAutocompletion", "enableBasicAutocompletion", "[A friendly description]", "boolean");// enableBasicAutocompletion: 0,
            //Fields.Add("enableLiveAutocompletion", "enableLiveAutocompletion", "[A friendly description]", "boolean");// enableLiveAutocompletion: 0,

            Fields.AddHideLabel();
        }

        public override IDictionary<string, object> ToConfigurationEditor(object configuration)
        {
            var config = base.ToConfigurationEditor(configuration);

            //if (config.ContainsKey("firstLineNumber") == false)
            //{
            //    config["firstLineNumber"] = 1;
            //}

            if (config.ContainsKey(FontSize) == false)
            {
                config[FontSize] = "14px";
            }

            return config;
        }
    }
}
