/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET6_0
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#else
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class SqlDataListSource : IDataListSource
    {
        private readonly string _codeEditorMode;
        private readonly IEnumerable<DataListItem> _connectionStrings;
        private readonly IIOHelper _ioHelper;
        private readonly IConfiguration _configuration;
        private readonly IScopeProvider _scopeProvider;

        public SqlDataListSource(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration,
            IScopeProvider scopeProvider,
            IIOHelper ioHelper)
        {
            // NOTE: Umbraco doesn't ship with SqlServer mode, so we check if its been added manually, otherwise defautls to Razor.
            _codeEditorMode = webHostEnvironment.WebPathExists("~/umbraco/lib/ace-builds/src-min-noconflict/mode-sqlserver.js") == true
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

            _configuration = configuration;
            _scopeProvider = scopeProvider;
            _ioHelper = ioHelper;
        }

        public string Name => "SQL Data";

        public string Description => "Use a SQL Server database query as the data source.";

        public string Icon => "icon-server-alt";

        public string Group => Constants.Conventions.DataSourceGroups.Data;

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
            { "query", $"SELECT\r\n\t[text],\r\n\t[uniqueId]\r\nFROM\r\n\t[umbracoNode]\r\nWHERE\r\n\t[nodeObjectType] = '{UmbConstants.ObjectTypes.Strings.Document}'\r\n\tAND\r\n\t[level] = 1\r\nORDER BY\r\n\t[sortOrder] ASC\r\n\r\n-- This is an example query that will select all the content nodes that are at level 1.\r\n;" },
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

            if (connectionStringName == UmbConstants.System.UmbracoConnectionName)
            {
                using (var scope = _scopeProvider.CreateScope())
                {
                    //new NPoco.PocoExpando().Values.ElementAtOrDefault(0);
                    var results = scope.Database.Fetch<dynamic>(query);

                    foreach (NPoco.PocoExpando result in results)
                    {
                        var item = new DataListItem
                        {
                            Name = result.Values.ElementAtOrDefault(0).TryConvertTo<string>().ResultOr(string.Empty),
                        };

                        item.Value = result.Values.ElementAtOrDefault(1).TryConvertTo<string>().ResultOr(item.Name);

                        if (result.Values.Count > 2)
                            item.Description = result.Values.ElementAtOrDefault(2).TryConvertTo<string>().ResultOr(string.Empty);

                        if (result.Values.Count > 3)
                            item.Icon = result.Values.ElementAtOrDefault(3).TryConvertTo<string>().ResultOr(string.Empty);

                        if (result.Values.Count > 4)
                            item.Disabled = result.Values.ElementAtOrDefault(4).TryConvertTo<bool>().ResultOr(false);

                        items.Add(item);
                    }
                }
            }
            else
            {
#if NET6_0
                // TODO: [LK:2022-04-06] [v10] Add support for querying generic SQLite database.
                items.AddRange(GetSqlItems<SqlConnection, SqlCommand>(query, connectionString));
#elif NET7_0_OR_GREATER
                // TODO: [LK:2022-11-26] [v11] Figure out how to do a generic SQL query, most likely now NPoco.
#endif
            }

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
