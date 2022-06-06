/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
#if NET472
using Umbraco.Core;
using Umbraco.Core.Deploy;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
#else
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Deploy;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class RenderMacroConfigurationConnector : IDataTypeConfigurationConnector
    {
        public IEnumerable<string> PropertyEditorAliases => new[] { RenderMacroDataEditor.DataEditorName };

#if NET472 == false
        private readonly IConfigurationEditorJsonSerializer _configurationEditorJsonSerializer;

        public RenderMacroConfigurationConnector(IConfigurationEditorJsonSerializer configurationEditorJsonSerializer)
        {
            _configurationEditorJsonSerializer = configurationEditorJsonSerializer;
        }
#endif

        public object FromArtifact(IDataType dataType, string configuration)
        {
            var dataTypeConfigurationEditor = dataType.Editor.GetConfigurationEditor();

#if NET472
            return dataTypeConfigurationEditor.FromDatabase(configuration);
#else
            return dataTypeConfigurationEditor.FromDatabase(configuration, _configurationEditorJsonSerializer);
#endif
        }

        public string ToArtifact(IDataType dataType, ICollection<ArtifactDependency> dependencies)
        {
            if (dataType.Configuration is Dictionary<string, object> config &&
                config.TryGetValueAs(RenderMacroConfigurationEditor.Macro, out JArray array) == true &&
                array.Count > 0 &&
                array[0] is JObject obj &&
                obj.ContainsKey("udi") == true &&
                UdiParser.TryParse(obj.Value<string>("udi"), out Udi udi) == true)
            {
                dependencies.Add(new ArtifactDependency(udi, false, ArtifactDependencyMode.Match));
            }

#if NET472
            return ConfigurationEditor.ToDatabase(dataType.Configuration);
#else
            return ConfigurationEditor.ToDatabase(dataType.Configuration, _configurationEditorJsonSerializer);
#endif
        }
    }
}
