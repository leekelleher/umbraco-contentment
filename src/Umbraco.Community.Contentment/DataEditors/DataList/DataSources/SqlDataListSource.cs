/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NPoco;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed partial class SqlDataListSource : DataListToDataPickerSourceBridge, IContentmentDataSource
    {
        private readonly string _codeEditorMode;
        private readonly IEnumerable<DataListItem> _connectionStrings;
        private readonly IConfiguration _configuration;
        private readonly IDbProviderFactoryCreator _dbProviderFactoryCreator;
        private readonly IScopeProvider _scopeProvider;

        public SqlDataListSource(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration,
            IDbProviderFactoryCreator dbProviderFactoryCreator,
            IScopeProvider scopeProvider)
        {
            // NOTE: Umbraco doesn't ship with SqlServer mode, so we check if its been added manually, otherwise defaults to Razor.
            _codeEditorMode = webHostEnvironment.WebPathExists("~/umbraco/lib/ace-builds/src-min-noconflict/mode-sqlserver.js") == true
                ? "sqlserver"
                : "razor";

            _connectionStrings = configuration
                .GetSection("ConnectionStrings")
                .GetChildren()
                .Where(x => x.Key.InvariantEndsWith("_ProviderName") == false)
                .Select(x => new DataListItem
                {
                    Name = x.Key,
                    Value = x.Key
                });

            _configuration = configuration;
            _dbProviderFactoryCreator = dbProviderFactoryCreator;
            _scopeProvider = scopeProvider;
        }

        public override string Name => "SQL Data";

        public override string Description => "Use a SQL Server database query as the data source.";

        public override string Icon => "icon-server-alt";

        public override string Group => Constants.Conventions.DataSourceGroups.Data;

        public override OverlaySize OverlaySize => OverlaySize.Medium;

        public override IEnumerable<ContentmentConfigurationField> Fields => new ContentmentConfigurationField[]
        {
            new NotesConfigurationField(@"<details class=""well well-small"">
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
            new ContentmentConfigurationField
            {
                Key = "query",
                Name = "SQL query",
                Description = "Enter your SQL query.",
                PropertyEditorUiAlias = CodeEditorDataEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { CodeEditorConfigurationEditor.Mode, _codeEditorMode },
                    { CodeEditorConfigurationEditor.MinLines, 20 },
                    { CodeEditorConfigurationEditor.MaxLines, 40 },
                }
            },
            new ContentmentConfigurationField
            {
                Key = "connectionString",
                Name = "Connection string",
                Description = "Select the connection string.",
                PropertyEditorUiAlias = DropdownListDataListEditor.DataEditorUiAlias,
                Config = new Dictionary<string, object>
                {
                    { DropdownListDataListEditor.AllowEmpty, false },
                    { Constants.Conventions.ConfigurationFieldAliases.Items, _connectionStrings },
                }
            },
        };

        public override Dictionary<string, object> DefaultValues => new()
        {
            { "query", $"SELECT\r\n\t[text],\r\n\t[uniqueId]\r\nFROM\r\n\t[umbracoNode]\r\nWHERE\r\n\t[nodeObjectType] = '{UmbConstants.ObjectTypes.Strings.Document}'\r\n\tAND\r\n\t[level] = 1\r\nORDER BY\r\n\t[sortOrder] ASC\r\n\r\n-- This is an example query that will select all the content nodes that are at level 1.\r\n;" },
            { "connectionString", UmbConstants.System.UmbracoConnectionName }
        };

        public override IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var items = new List<DataListItem>();

            var query = config.GetValueAs("query", string.Empty);
            var connectionStringName = config.GetValueAs("connectionString", string.Empty);

            if (string.IsNullOrWhiteSpace(query) == true || string.IsNullOrWhiteSpace(connectionStringName) == true)
            {
                return items;
            }

            items.AddRange(GetSqlItems(query, connectionStringName));

            return items;
        }

        private IEnumerable<DataListItem> GetSqlItems(string query, string connectionStringName)
        {
            using (var scope = _scopeProvider.CreateScope())
            {
                var database = GetDatabase(connectionStringName) ?? scope.Database;
                var results = database.Fetch<dynamic>(query);

                foreach (PocoExpando result in results)
                {
                    var item = new DataListItem
                    {
                        Name = result.Values.ElementAtOrDefault(0).TryConvertTo<string>().ResultOr(string.Empty),
                    };

                    item.Value = result.Values.ElementAtOrDefault(1).TryConvertTo<string>().ResultOr(item.Name);

                    if (result.Values.Count > 2)
                    {
                        item.Description = result.Values.ElementAtOrDefault(2).TryConvertTo<string>().ResultOr(string.Empty);
                    }

                    if (result.Values.Count > 3)
                    {
                        item.Icon = result.Values.ElementAtOrDefault(3).TryConvertTo<string>().ResultOr(string.Empty);
                    }

                    if (result.Values.Count > 4)
                    {
                        item.Disabled = result.Values.ElementAtOrDefault(4).TryConvertTo<bool>().ResultOr(false);
                    }

                    yield return item;
                }

                _ = scope.Complete();
            }
        }

#pragma warning disable CA1859 // Use concrete types when possible for improved performance
        private IDatabase? GetDatabase(string connectionStringName)
#pragma warning restore CA1859 // Use concrete types when possible for improved performance
        {
            if (connectionStringName != UmbConstants.System.UmbracoConnectionName)
            {
                var connectionString = _configuration.GetUmbracoConnectionString(connectionStringName, out var providerName);

                if (string.IsNullOrWhiteSpace(providerName) == true)
                {
                    providerName = Constants.Persistance.Providers.SqlServer;
                }

                var dbProviderFactory = _dbProviderFactoryCreator.CreateFactory(providerName);

                if (string.IsNullOrWhiteSpace(connectionString) == false && dbProviderFactory is not null)
                {
                    if (providerName.InvariantEquals(Constants.Persistance.Providers.Sqlite) == true)
                    {
                        return new Database(connectionString, DatabaseType.SQLite, dbProviderFactory);
                    }
                    else
                    {
                        return new Database(connectionString, DatabaseType.SqlServer2012, dbProviderFactory);
                    }
                }
            }

            return default;
        }
    }
}
