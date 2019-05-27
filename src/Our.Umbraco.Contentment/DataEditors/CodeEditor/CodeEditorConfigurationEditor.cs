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
        public CodeEditorConfigurationEditor()
            : base()
        {
            // TODO: Review. Look to add a note outline that other modes can be downloaded and dropped into the folder. [LK]

            var aceEditorPath = IOHelper.MapPath("~/Umbraco/lib/ace-builds/src-min-noconflict/");
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
                            "mode",
                            "Programming Language Mode",
                            "Select the programming language mode. By default, 'JavaScript' mode will be used.",
                            IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath),
                            new Dictionary<string, object>
                            {
                            { "allowEmpty", Constants.Values.False },
                            { Constants.Conventions.ConfigurationEditors.Items, modes },
                            { Constants.Conventions.ConfigurationEditors.DefaultValue, "javascript" }
                            });
                    }

                    if (themes.Count > 0)
                    {
                        Fields.Add(
                       "theme",
                       "Theme",
                       "Set the theme for the code editor. By default, 'Chrome' will be used.",
                       IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath),
                       new Dictionary<string, object>
                       {
                            { "allowEmpty", Constants.Values.False },
                            { Constants.Conventions.ConfigurationEditors.Items, themes },
                            { Constants.Conventions.ConfigurationEditors.DefaultValue, "chrome" }
                       });
                    }
                }
            }

            Fields.Add("fontSize", "Font Size", "Set the font size. The value must be a valid CSS font-size. The default value is '14px'.", "textstring");
            Fields.Add("useWrapMode", "Word Wrapping", "Select to enable word wrapping.", "boolean");

            // TODO: Hidden the advanced options (for now), need to review. [LK]
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

            if (config.ContainsKey("fontSize") == false)
            {
                config["fontSize"] = "14px";
            }

            return config;
        }
    }
}
