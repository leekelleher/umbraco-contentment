/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
#if NET472
using Umbraco.Core.PropertyEditors;
#else
using Umbraco.Cms.Core.PropertyEditors;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public sealed class ConfigurationEditorModel : IConfigurationEditorItem
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Group { get; set; }

        public IEnumerable<ConfigurationField> Fields { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> DefaultValues { get; set; }

        public OverlaySize OverlaySize { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Expressions { get; set; }

        [Obsolete("Please use Expressions instead. e.g. { \"name\", \"{{ AngularJS expression }}\" }", false)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string NameTemplate { get; set; }

        [Obsolete("Please use Expressions instead. e.g. { \"description\", \"{{ AngularJS expression }}\" }", false)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DescriptionTemplate { get; set; }
    }
}
