/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using ClientDependency.Core;
using Newtonsoft.Json;
using Umbraco.Community.Contentment.Configuration;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Web;

namespace Umbraco.Community.Contentment.Telemetry
{
    internal sealed class ContentmentTelemetryComponent : IComponent
    {
        internal static bool Disabled { get; set; }

        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public ContentmentTelemetryComponent(IUmbracoContextAccessor umbracoContextAccessor)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public void Initialize()
        {
            DataTypeService.Saved += DataTypeService_Saved;
        }

        public void Terminate()
        {
            DataTypeService.Saved -= DataTypeService_Saved;
        }

        private void DataTypeService_Saved(IDataTypeService sender, SaveEventArgs<IDataType> e)
        {
            if (Disabled == true)
            {
                return;
            }

            foreach (var entity in e.SavedEntities)
            {
                if (entity.EditorAlias.InvariantStartsWith(Constants.Internals.DataEditorAliasPrefix) == true)
                {
                    try
                    {
                        // TODO: [LK] After v8.10.0 bump, switch this to use `IUmbracoSettingsSection.BackOffice.Id`.
                        var umbracoId = _umbracoContextAccessor.UmbracoContext?.HttpContext?.Server != null
                            ? new Guid(_umbracoContextAccessor.UmbracoContext.HttpContext.Server.MachineName.GenerateMd5())
                            : Guid.Empty;

                        // No identifiable details, just a quick call home.
                        var data = new
                        {
                            dataType = entity.Key,
                            editorAlias = entity.EditorAlias.Substring(Constants.Internals.DataEditorAliasPrefix.Length),
                            umbracoId = umbracoId,
                            umbracoVersion = UmbracoVersion.SemanticVersion.ToString(),
                            contentmentVersion = ContentmentVersion.SemanticVersion.ToString(),
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
