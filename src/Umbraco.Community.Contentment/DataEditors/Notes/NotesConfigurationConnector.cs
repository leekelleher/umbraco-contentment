/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Deploy;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class NotesConfigurationConnector : IDataTypeConfigurationConnector
    {
        private readonly IConfigurationEditorJsonSerializer _configurationEditorJsonSerializer;
        private readonly ILocalLinkParser _localLinkParser;
        private readonly IImageSourceParser _imageSourceParser;

        public IEnumerable<string> PropertyEditorAliases => new[] { NotesDataEditor.DataEditorName };

        public NotesConfigurationConnector(
            IConfigurationEditorJsonSerializer configurationEditorJsonSerializer,
            ILocalLinkParser localLinkParser,
            IImageSourceParser imageSourceParser)
        {
            _configurationEditorJsonSerializer = configurationEditorJsonSerializer;
            _localLinkParser = localLinkParser;
            _imageSourceParser = imageSourceParser;
        }

        public async Task<IDictionary<string, object>> FromArtifactAsync(IDataType dataType, string? configuration, IContextCache contextCache, CancellationToken cancellationToken = default)
        {
            var dataTypeConfigurationEditor = dataType.Editor?.GetConfigurationEditor();

            var db = dataTypeConfigurationEditor?.FromDatabase(configuration, _configurationEditorJsonSerializer);

            if (db is Dictionary<string, object> config &&
                config.TryGetValueAs(NotesConfigurationField.Notes, out string? notes) == true &&
                string.IsNullOrWhiteSpace(notes) == false)
            {
                notes = await _localLinkParser.FromArtifactAsync(notes, contextCache, cancellationToken);
                notes = await _imageSourceParser.FromArtifactAsync(notes, contextCache, cancellationToken);

                config[NotesConfigurationField.Notes] = notes ?? string.Empty;

                return config;
            }

            return db ?? new Dictionary<string, object>();
        }

        public async Task<string?> ToArtifactAsync(IDataType dataType, ICollection<ArtifactDependency> dependencies, IContextCache contextCache, CancellationToken cancellationToken = default)
        {
            if (dataType.ConfigurationObject is Dictionary<string, object> config &&
                config.TryGetValueAs(NotesConfigurationField.Notes, out string? notes) == true &&
                string.IsNullOrWhiteSpace(notes) == false)
            {
                var udis = new List<Udi>();

                notes = await _localLinkParser.ToArtifactAsync(notes, udis, contextCache, cancellationToken);
                notes = await _imageSourceParser.ToArtifactAsync(notes, udis, contextCache, cancellationToken);

                foreach (var udi in udis)
                {
                    //var mode = udi.EntityType == UmbConstants.UdiEntityType.Macro
                    //    ? ArtifactDependencyMode.Match
                    //    : ArtifactDependencyMode.Exist;
                    var mode = ArtifactDependencyMode.Exist;

                    dependencies.Add(new ArtifactDependency(udi, false, mode));
                }

                config[NotesConfigurationField.Notes] = notes ?? string.Empty;
            }

            var dataTypeConfigurationEditor = dataType.Editor?.GetConfigurationEditor();
            return dataTypeConfigurationEditor?.ToDatabase(dataType.ConfigurationData, _configurationEditorJsonSerializer);
        }
    }
}
