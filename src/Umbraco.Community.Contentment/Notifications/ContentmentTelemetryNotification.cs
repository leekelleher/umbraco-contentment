/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Net.Mime;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Notifications
{
    internal sealed class ContentmentTelemetryNotification : INotificationAsyncHandler<DataTypeSavedNotification>
    {
        private readonly ContentmentSettings _contentmentSettings;
        private readonly GlobalSettings _globalSettings;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUmbracoVersion _umbracoVersion;

        public ContentmentTelemetryNotification(
            IOptions<ContentmentSettings> contentmentSettings,
            IOptions<GlobalSettings> globalSettings,
            IJsonSerializer jsonSerializer,
            IHttpClientFactory httpClientFactory,
            IUmbracoVersion umbracoVersion)
        {
            _contentmentSettings = contentmentSettings.Value;
            _globalSettings = globalSettings.Value;
            _jsonSerializer = jsonSerializer;
            _httpClientFactory = httpClientFactory;
            _umbracoVersion = umbracoVersion;
        }

        public async Task HandleAsync(DataTypeSavedNotification notification, CancellationToken cancellationToken)
        {
            if (_contentmentSettings.DisableTelemetry == true)
            {
                return;
            }

            var umbracoId = Guid.TryParse(_globalSettings.Id, out var telemetrySiteIdentifier) == true
                ? telemetrySiteIdentifier
                : Guid.Empty;

            if (umbracoId.Equals(Guid.Empty) == true)
            {
                return;
            }

            foreach (var entity in notification.SavedEntities)
            {
                if (entity.EditorAlias.InvariantStartsWith(Constants.Internals.DataEditorAliasPrefix) == true)
                {
                    try
                    {
                        var dataTypeConfig = new Dictionary<string, object>();

                        if (entity.ConfigurationObject is Dictionary<string, object> config)
                        {
                            void AddConfigurationEditorKey(string alias)
                            {
                                if (config.ContainsKey(alias) == true &&
                                    config.TryGetValueAs(alias, out JsonArray? array) == true &&
                                    array?.Count > 0 &&
                                    array[0] is JsonObject item &&
                                    item.TryGetValueAs("key", out string? key) == true &&
                                    string.IsNullOrWhiteSpace(key) == false)
                                {
                                    if (key.InvariantStartsWith(Constants.Internals.ProjectNamespace) == true && key.Length > 73)
                                    {
                                        // Strips off the namespace and assembly.
                                        // e.g. "Umbraco.Community.Contentment.DataEditors.[DataSourceName], Umbraco.Community.Contentment"
                                        key = key.Substring(42, key.Length - 73);
                                    }

                                    dataTypeConfig.Add(alias, key);
                                }
                            };

                            switch (entity.EditorAlias)
                            {
                                case DataListDataEditor.DataEditorAlias:
                                    AddConfigurationEditorKey("dataSource");
                                    AddConfigurationEditorKey("listEditor");
                                    break;

                                case DataPickerDataEditor.DataEditorAlias:
                                    AddConfigurationEditorKey("displayMode");
                                    AddConfigurationEditorKey("dataSource");
                                    break;

                                case ContentBlocksDataEditor.DataEditorAlias:
                                    AddConfigurationEditorKey("displayMode");
                                    break;

                                case TextInputDataEditor.DataEditorAlias:
                                    AddConfigurationEditorKey(Constants.Conventions.ConfigurationFieldAliases.Items);
                                    break;

                                default:
                                    break;
                            }
                        }

                        // No identifiable details, just a quick call home.
                        var data = new
                        {
                            dataType = entity.Key,
                            editorAlias = entity.EditorAlias.Substring(Constants.Internals.DataEditorAliasPrefix.Length),
                            umbracoId,
                            umbracoVersion = _umbracoVersion.SemanticVersion.ToSemanticStringWithoutBuild(),
                            contentmentVersion = ContentmentVersion.SemanticVersion?.ToSemanticStringWithoutBuild(),
                            dataTypeConfig,
                        };

                        var address = new Uri("https://leekelleher.com/umbraco/contentment/telemetry/");
                        var json = _jsonSerializer.Serialize(data);
                        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
                        var payload = new StringContent(base64, Encoding.UTF8, MediaTypeNames.Text.Plain);

                        using var client = _httpClientFactory.CreateClient();
                        using var post = await client.PostAsync(address, payload, CancellationToken.None);
                    }
                    catch { /* ¯\_(ツ)_/¯ */ }
                }
            }
        }
    }
}
