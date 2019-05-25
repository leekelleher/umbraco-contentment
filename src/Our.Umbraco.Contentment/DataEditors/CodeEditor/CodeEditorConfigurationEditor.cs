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
            // TODO: Get some nice friendly descriptions from here: https://ace.c9.io/#nav=howto [LK]

            Fields.Add("showGutter", "showGutter", "[A friendly description]", "boolean");// showGutter: 1,
            Fields.Add("useWrapMode", "useWrapMode", "[A friendly description]", "boolean");// useWrapMode: 1,
            Fields.Add("showInvisibles", "showInvisibles", "[A friendly description]", "boolean");// showInvisibles: 0,
            Fields.Add("showIndentGuides", "showIndentGuides", "[A friendly description]", "boolean");// showIndentGuides: 0,
            Fields.Add("useSoftTabs", "useSoftTabs", "[A friendly description]", "boolean");// useSoftTabs: 1,
            Fields.Add("showPrintMargin", "showPrintMargin", "[A friendly description]", "boolean");// showPrintMargin: 0,
            Fields.Add("disableSearch", "disableSearch", "[A friendly description]", "boolean");// disableSearch: 0,

            // TODO: Other modes can be downloaded and dropped into the folder. Let's make the dropdown dynamically populated. [LK]
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
                            var label = mode.Length == 3 ? mode.ToUpper() : mode.ToFirstUpper();
                            modes.Add(new { label = label, value = mode });
                        }

                        if (filename.StartsWith("theme-"))
                        {
                            var theme = filename.Replace("theme-", string.Empty).ToLower();
                            themes.Add(new { label = theme.ToFirstUpper(), value = theme });
                        }
                    }

                    Fields.Add(
                        "theme",
                        "theme",
                        "[A friendly description]",
                        IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath),
                        new Dictionary<string, object>
                        {
                            { "allowEmpty", Constants.Values.False },
                            { Constants.Conventions.ConfigurationEditors.Items, themes },
                            { Constants.Conventions.ConfigurationEditors.DefaultValue, "chrome" }
                        });

                    Fields.Add(
                        "mode",
                        "Programming Language Mode",
                        "Select the programming language mode. By default, javascript mode will be used.",
                        IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath),
                        new Dictionary<string, object>
                        {
                            { "allowEmpty", Constants.Values.False },
                            { Constants.Conventions.ConfigurationEditors.Items, modes },
                            { Constants.Conventions.ConfigurationEditors.DefaultValue, "javascript" }
                        });
                }
            }

            Fields.Add("firstLineNumber", "firstLineNumber", "[A friendly description]", "number");// firstLineNumber: 1,
            Fields.Add("fontSize", "fontSize", "[A friendly description]", "textstring");// fontSize: "14px",
            Fields.Add("enableSnippets", "enableSnippets", "[A friendly description]", "boolean");// enableSnippets: 0,
            Fields.Add("enableBasicAutocompletion", "enableBasicAutocompletion", "[A friendly description]", "boolean");// enableBasicAutocompletion: 0,
            Fields.Add("enableLiveAutocompletion", "enableLiveAutocompletion", "[A friendly description]", "boolean");// enableLiveAutocompletion: 0,
            Fields.AddHideLabel();
        }
    }
}
