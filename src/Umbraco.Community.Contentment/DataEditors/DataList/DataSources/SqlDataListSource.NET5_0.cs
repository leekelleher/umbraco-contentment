/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

/* NOTE: This code file is ONLY for Umbraco v9, .NET Core 5.0.
 * For the .NET Core 6.0 version, please see file `SqlDataListSource.NET6_0.cs`. */

#if NET5_0
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Umbraco.Cms.Core.IO;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed partial class SqlDataListSource : IDataListSource
    {
        private readonly string _codeEditorMode;
        private readonly IEnumerable<DataListItem> _connectionStrings;
        private readonly IIOHelper _ioHelper;
        private readonly IConfiguration _configuration;

        public SqlDataListSource(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration,
            IIOHelper ioHelper)
        {
            // NOTE: Umbraco doesn't ship with SqlServer mode, so we check if its been added manually, otherwise defautls to Razor.
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

            var connectionString = _configuration.GetConnectionString(connectionStringName);
            if (string.IsNullOrWhiteSpace(connectionString) == true)
            {
                return items;
            }

            // TODO: [v9] [LK:2021-05-07] Review SQLCE
            // NOTE: SQLCE uses a different connection/command. I'm trying to keep this as generic as possible, without resorting to using NPoco. [LK]
            // I've tried digging around Umbraco's `IUmbracoDatabase` layer, but I couldn't get my head around it.
            // At the end of the day, if the user has SQLCE configured, it'd be nice for them to query it.
            // But I don't want to add an assembly dependency (for SQLCE) to Contentment itself. I'd like to leverage Umbraco's code.

            items.AddRange(GetSqlItems<SqlConnection, SqlCommand>(query, connectionString));

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
#endif
