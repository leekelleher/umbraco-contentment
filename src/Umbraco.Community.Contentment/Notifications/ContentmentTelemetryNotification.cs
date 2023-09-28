/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Configuration;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Notifications
{
    internal sealed class ContentmentTelemetryNotification : INotificationAsyncHandler<DataTypeSavedNotification>
    {
        private readonly ContentmentSettings _contentmentSettings;
        private readonly GlobalSettings _globalSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUmbracoVersion _umbracoVersion;

        public ContentmentTelemetryNotification(
            IOptions<ContentmentSettings> contentmentSettings,
            IOptions<GlobalSettings> globalSettings,
            IHttpClientFactory httpClientFactory,
            IUmbracoVersion umbracoVersion)
        {
            _contentmentSettings = contentmentSettings.Value;
            _globalSettings = globalSettings.Value;
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

                        if (entity.Configuration is Dictionary<string, object> config)
                        {
                            void AddConfigurationEditorKey(string alias)
                            {
                                if (config.ContainsKey(alias) == true &&
                                    config.TryGetValueAs(alias, out JArray array) == true &&
                                    array.Count > 0 &&
                                    array[0] is JObject item &&
                                    item.ContainsKey("key") == true)
                                {
                                    var key = item.Value<string>("key");

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
                                    AddConfigurationEditorKey(DataListConfigurationEditor.DataSource);
                                    AddConfigurationEditorKey(DataListConfigurationEditor.ListEditor);
                                    break;

                                case DataPickerDataEditor.DataEditorAlias:
                                    AddConfigurationEditorKey(DataPickerConfigurationEditor.DisplayMode);
                                    AddConfigurationEditorKey(DataPickerConfigurationEditor.DataSource);
                                    break;

                                case ContentBlocksDataEditor.DataEditorAlias:
                                    AddConfigurationEditorKey(ContentBlocksConfigurationEditor.DisplayMode);
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
                            umbracoId = umbracoId,
                            umbracoVersion = _umbracoVersion.SemanticVersion.ToSemanticStringWithoutBuild(),
                            contentmentVersion = ContentmentVersion.SemanticVersion.ToString(),
                            dataTypeConfig = dataTypeConfig,
                        };

                        var address = new Uri("https://leekelleher.com/umbraco/contentment/telemetry/");
                        var json = JsonConvert.SerializeObject(data, Formatting.None);
                        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
                        var payload = new StringContent(base64, Encoding.UTF8, MediaTypeNames.Text.Plain);

                        using var client = _httpClientFactory.CreateClient();
                        using var post = await client.PostAsync(address, payload);
                    }
                    catch { /* ¯\_(ツ)_/¯ */ }
                }
            }
        }
    }
}
