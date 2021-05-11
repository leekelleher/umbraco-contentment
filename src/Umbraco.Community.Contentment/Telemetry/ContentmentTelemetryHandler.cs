/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Configuration;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Telemetry
{
    internal sealed class ContentmentTelemetryHandler : INotificationHandler<SavedNotification<IDataType>>
    {
        private readonly ContentmentSettings _contentmentSettings;
        private readonly GlobalSettings _globalSettings;
        private readonly IUmbracoVersion _umbracoVersion;

        public ContentmentTelemetryHandler(
            IOptions<ContentmentSettings> contentmentSettings,
            IOptions<GlobalSettings> globalSettings,
            IUmbracoVersion umbracoVersion)
        {
            _contentmentSettings = contentmentSettings.Value;
            _globalSettings = globalSettings.Value;
            _umbracoVersion = umbracoVersion;
        }

        public void Handle(SavedNotification<IDataType> notification)
        {
            if (_contentmentSettings.DisableTelemetry == true)
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

                        var umbracoId = Guid.TryParse(_globalSettings.Id, out var telemetrySiteIdentifier) == true
                            ? telemetrySiteIdentifier
                            : Guid.Empty;

                        // No identifiable details, just a quick call home.
                        var data = new
                        {
                            dataType = entity.Key,
                            editorAlias = entity.EditorAlias.Substring(Constants.Internals.DataEditorAliasPrefix.Length),
                            umbracoId = umbracoId,
                            umbracoVersion = _umbracoVersion.SemanticVersion.ToString(),
                            contentmentVersion = ContentmentVersion.SemanticVersion.ToString(),
                            dataTypeConfig = dataTypeConfig,
                        };

                        using (var client = new WebClient())
                        {
                            var address = new Uri("https://leekelleher.com/umbraco/contentment/telemetry/");
                            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                            var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

                            client.Headers.Add("Content-Type", MediaTypeNames.Text.Plain);
                            Task.Run(() => client.UploadStringAsync(address, payload));
                        }
                    }
                    catch { /* ¯\_(ツ)_/¯ */ }
                }
            }
        }
    }
}
