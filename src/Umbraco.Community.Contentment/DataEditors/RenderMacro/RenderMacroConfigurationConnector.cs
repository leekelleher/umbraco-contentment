/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Deploy;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class RenderMacroConfigurationConnector : IDataTypeConfigurationConnector
    {
        public IEnumerable<string> PropertyEditorAliases => new[] { RenderMacroDataEditor.DataEditorName };

        private readonly IConfigurationEditorJsonSerializer _configurationEditorJsonSerializer;

        public RenderMacroConfigurationConnector(IConfigurationEditorJsonSerializer configurationEditorJsonSerializer)
        {
            _configurationEditorJsonSerializer = configurationEditorJsonSerializer;
        }

        public object? FromArtifact(IDataType dataType, string? configuration)
            => FromArtifact(dataType, configuration, PassThroughCache.Instance);

        public object? FromArtifact(IDataType dataType, string? configuration, IContextCache contextCache)
        {
            var dataTypeConfigurationEditor = dataType.Editor?.GetConfigurationEditor();

            return dataTypeConfigurationEditor?.FromDatabase(configuration, _configurationEditorJsonSerializer);
        }

        public string? ToArtifact(IDataType dataType, ICollection<ArtifactDependency> dependencies)
            => ToArtifact(dataType, dependencies, PassThroughCache.Instance);

        public string? ToArtifact(IDataType dataType, ICollection<ArtifactDependency> dependencies, IContextCache contextCache)
        {
            if (dataType.Configuration is Dictionary<string, object> config &&
                config.TryGetValueAs(RenderMacroConfigurationEditor.Macro, out JArray? array) == true &&
                array?.Count > 0 &&
                array[0] is JObject obj &&
                obj.ContainsKey("udi") == true &&
                obj.Value<string>("udi") is string s &&
                UdiParser.TryParse(s, out var udi) == true)
            {
                dependencies.Add(new ArtifactDependency(udi, false, ArtifactDependencyMode.Match));
            }

            return ConfigurationEditor.ToDatabase(dataType.Configuration, _configurationEditorJsonSerializer);
        }
    }
}
