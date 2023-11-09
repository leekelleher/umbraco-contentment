/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

/* NOTE: This code file is ONLY for Umbraco v10/v11/v12, .NET Core 6.0/7.0. */

#if NET6_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NPoco;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed partial class SqlDataListSource : IDataListSource
    {
        private readonly string _codeEditorMode;
        private readonly IEnumerable<DataListItem> _connectionStrings;
        private readonly IIOHelper _ioHelper;
        private readonly IConfiguration _configuration;
        private readonly IDbProviderFactoryCreator _dbProviderFactoryCreator;
        private readonly IScopeProvider _scopeProvider;

        public SqlDataListSource(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration,
            IDbProviderFactoryCreator dbProviderFactoryCreator,
            IScopeProvider scopeProvider,
            IIOHelper ioHelper)
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
            _ioHelper = ioHelper;
        }

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
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
                        item.Description = result.Values.ElementAtOrDefault(2).TryConvertTo<string>().ResultOr(string.Empty);

                    if (result.Values.Count > 3)
                        item.Icon = result.Values.ElementAtOrDefault(3).TryConvertTo<string>().ResultOr(string.Empty);

                    if (result.Values.Count > 4)
                        item.Disabled = result.Values.ElementAtOrDefault(4).TryConvertTo<bool>().ResultOr(false);

                    yield return item;
                }

                scope.Complete();
            }
        }

        private IDatabase GetDatabase(string connectionStringName)
        {
            if (connectionStringName != UmbConstants.System.UmbracoConnectionName)
            {
                var connectionString = _configuration.GetUmbracoConnectionString(connectionStringName, out var providerName);

                if (string.IsNullOrWhiteSpace(providerName) == true)
                {
                    providerName = Constants.Persistance.Providers.SqlServer;
                }

                var dbProviderFactory = _dbProviderFactoryCreator.CreateFactory(providerName);

                if (providerName.InvariantEquals(Constants.Persistance.Providers.Sqlite) == true)
                {
                    return new Database(connectionString, DatabaseType.SQLite, dbProviderFactory);
                }
                else
                {
                    return new Database(connectionString, DatabaseType.SqlServer2012, dbProviderFactory);
                }
            }

            return default;
        }
    }
}
#endif
