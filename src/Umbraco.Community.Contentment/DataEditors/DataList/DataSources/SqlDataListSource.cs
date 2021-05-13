/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
//using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Configuration;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class SqlDataListSource : IDataListSource
    {
        private readonly string _codeEditorMode;
        private readonly IEnumerable<DataListItem> _connectionStrings;
        private readonly IIOHelper _ioHelper;
        private readonly IConfiguration _configuration;

        public SqlDataListSource(
            IHostingEnvironment hostingEnvironment,
            IIOHelper ioHelper,
            IConfiguration configuration)
        {
            // NOTE: Umbraco doesn't ship with SqlServer mode, so we check if its been added manually, otherwise defautls to Razor.
            _codeEditorMode = File.Exists(hostingEnvironment.MapPathWebRoot("~/umbraco/lib/ace-builds/src-min-noconflict/mode-sqlserver.js"))
                ? "sqlserver"
                : "razor";

            _connectionStrings = configuration
                .GetSection("ConnectionStrings")
                .GetChildren()
                .Select(x => new DataListItem
                {
                    Name = x.Key,
                    Value = x.Key
                });

            _ioHelper = ioHelper;
            _configuration = configuration;
        }

        public string Name => "SQL Data";

        public string Description => "Use a SQL Server database as the data source.";

        public string Icon => "icon-server-alt";

        public string Group => default;

        public OverlaySize OverlaySize => OverlaySize.Medium;

        public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
        {
            new NotesConfigurationField(_ioHelper, @"<details class=""well well-small"">
<summary><strong><em>Important:</em> A note about your SQL query.</strong></summary>
<p>Your SQL query should be designed to return a minimum of 2 columns, (and a maximum of 5 columns). These columns will be used to populate the List Editor items.</p>
<p>The columns will be mapped in the following order:</p>
<ol>
<li><strong>Name</strong> <em>(e.g. item's label)</em></li>
<li><strong>Value</strong></li>
<li>Description <em>(optional)</em></li>
<li>Icon <em>(optional)</em></li>
<li>Disabled <em>(optional)</em></li>
</ol>
<p>If you need assistance with SQL syntax, please refer to this resource: <a href=""https://www.w3schools.com/sql/"" target=""_blank""><strong>w3schools.com/sql</strong></a>.</p>
</details>", true),
            new ConfigurationField
            {
                Key = "query",
                Name = "SQL query",
                Description = "Enter your SQL query.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(CodeEditorDataEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { CodeEditorConfigurationEditor.Mode, _codeEditorMode },
                    { CodeEditorConfigurationEditor.MinLines, 20 },
                    { CodeEditorConfigurationEditor.MaxLines, 40 },
                }
            },
            new ConfigurationField
            {
                Key = "connectionString",
                Name = "Connection string",
                Description = "Select the connection string.",
                View = _ioHelper.ResolveRelativeOrVirtualUrl(DropdownListDataListEditor.DataEditorViewPath),
                Config = new Dictionary<string, object>
                {
                    { DropdownListDataListEditor.AllowEmpty, Constants.Values.False },
                    { Constants.Conventions.ConfigurationFieldAliases.Items, _connectionStrings },
                }
            },
        };

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "query", $"-- This is an example query that will select all the content nodes that are at level 1.\r\nSELECT\r\n\t[text],\r\n\t[uniqueId]\r\nFROM\r\n\t[umbracoNode]\r\nWHERE\r\n\t[nodeObjectType] = '{UmbConstants.ObjectTypes.Strings.Document}'\r\n\tAND\r\n\t[level] = 1\r\nORDER BY\r\n\t[sortOrder] ASC\r\n;" },
            { "connectionString", UmbConstants.System.UmbracoConnectionName }
        };

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var items = new List<DataListItem>();

            var query = config.GetValueAs("query", string.Empty);
            var connectionStringName = config.GetValueAs("connectionString", string.Empty);

            if (string.IsNullOrWhiteSpace(query) == true || string.IsNullOrWhiteSpace(connectionStringName) == true)
            {
                return items;
            }

            var connectionString = _configuration.GetConnectionString(connectionStringName);
            if (string.IsNullOrWhiteSpace(connectionString) == true)
            {
                return items;
            }

            var configConnectionString = new ConfigConnectionString(connectionStringName, connectionString);
            if (configConnectionString.IsConnectionStringConfigured() == false)
            {
                return items;
            }

            // TODO: [v9] [LK:2021-05-07] Review SQLCE
            // NOTE: SQLCE uses a different connection/command. I'm trying to keep this as generic as possible, without resorting to using NPoco. [LK]
            // I've tried digging around Umbraco's `IUmbracoDatabase` layer, but I couldn't get my head around it.
            // At the end of the day, if the user has SQLCE configured, it'd be nice for them to query it.
            // But I don't want to add an assembly dependency (for SQLCE) to Contentment itself. I'd like to leverage Umbraco's code.

            //if (settings.ProviderName.InvariantEquals(UmbConstants.DatabaseProviders.SqlCe) == true)
            //{
            //    items.AddRange(GetSqlItems<SqlCeConnection, SqlCeCommand>(query, settings.ConnectionString));
            //}
            //else
            //{
            items.AddRange(GetSqlItems<SqlConnection, SqlCommand>(query, connectionString));
            //}

            return items;
        }

        private IEnumerable<DataListItem> GetSqlItems<TConnection, TCommand>(string query, string connectionString)
            where TConnection : DbConnection, new()
            where TCommand : DbCommand, new()
        {
            using (var connection = new TConnection() { ConnectionString = connectionString })
            using (var command = new TCommand() { Connection = connection, CommandText = query })
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read() == true)
                    {
                        if (reader.FieldCount > 0)
                        {
                            var item = new DataListItem
                            {
                                Name = reader[0].TryConvertTo<string>().Result
                            };

                            item.Value = reader.FieldCount > 1
                                ? reader[1].TryConvertTo<string>().Result
                                : item.Name;

                            if (reader.FieldCount > 2)
                                item.Description = reader[2].TryConvertTo<string>().Result;

                            if (reader.FieldCount > 3)
                                item.Icon = reader[3].TryConvertTo<string>().Result;

                            if (reader.FieldCount > 4)
                                item.Disabled = reader[4].ToString().TryConvertTo<bool>().Result;

                            yield return item;
                        }
                    }
                }
            }
        }
    }
}
