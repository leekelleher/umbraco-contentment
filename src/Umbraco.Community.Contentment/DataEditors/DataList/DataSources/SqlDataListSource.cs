/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed class SqlDataListSource : IDataListSource
    {
        private readonly ConfigurationField[] _fields;
        public SqlDataListSource()
        {
            // NOTE: Umbraco doesn't ship with SQL mode, so we check if its been added manually, otherwise defautls to Razor.
            var codeEditorMode = System.IO.File.Exists(IOHelper.MapPath("~/umbraco/lib/ace-builds/src-min-noconflict/mode-sql.js"))
                ? "sql"
                : "razor";

            var connStrings = new List<DataListItem>();
            foreach (ConnectionStringSettings connString in ConfigurationManager.ConnectionStrings)
            {
                connStrings.Add(new DataListItem { Name = connString.Name, Value = connString.Name });
            }

            _fields = new ConfigurationField[]
            {
                new NotesConfigurationField(@"<div class=""alert alert-info"">
<p><strong>A note about your SQL query.</strong></p>
<p>Your SQL query should be designed to return a minimum of 2 columns, (and a maximum of 4 columns). These columns will be used to populate the List Editor items.</p>
<p>The columns will be mapped in the following order:</p>
<ol>
<li><strong>Name</strong> <em>(e.g. item's label)</em></li>
<li><strong>Value</strong></li>
<li>Description <em>(optional)</em></li>
<li>Icon <em>(optional)</em></li>
</ol>
<p>If you need assistance with SQL syntax, please refer to this resource: <a href=""https://www.w3schools.com/sql/"" target=""_blank""><strong>w3schools.com/sql</strong></a>.</p>
</div>", true),
                new ConfigurationField
                {
                    Key = "query",
                    Name = "SQL query",
                    Description = "Enter your SQL query.",
                    View = IOHelper.ResolveUrl(CodeEditorDataEditor.DataEditorViewPath),
                    Config = new Dictionary<string, object>
                    {
                        { CodeEditorConfigurationEditor.Mode, codeEditorMode },
                    }
                },
                new ConfigurationField
                {
                    Key = "connectionString",
                    Name = "Connection string",
                    Description = "Enter the connection string.",
                    View = IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath),
                    Config = new Dictionary<string, object>
                    {
                        { AllowEmptyConfigurationField.AllowEmpty, Constants.Values.False },
                        { DropdownListConfigurationEditor.Items, connStrings },
                    }
                }
            };
        }

        public string Name => "SQL Data";

        public string Description => "Use a SQL Server database as the data source.";

        public string Icon => "icon-server-alt";

        public IEnumerable<ConfigurationField> Fields => _fields;

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { "query", $"SELECT\r\n\t[text],\r\n\t[uniqueId]\r\nFROM\r\n\t[umbracoNode]\r\nWHERE\r\n\t[nodeObjectType] = '{Core.Constants.ObjectTypes.Strings.Document}'\r\n\tAND\r\n\t[level] = 1\r\nORDER BY\r\n\t[sortOrder] ASC\r\n;" },
            { "connectionString", Core.Constants.System.UmbracoConnectionName }
        };

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var items = new List<DataListItem>();

            var query = config.GetValueAs("query", string.Empty);
            var connectionString = config.GetValueAs("connectionString", string.Empty);

            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(connectionString))
                return items;

            var sql = query.Replace("\r", string.Empty).Replace("\n", " ").Replace("\t", " ");
            var settings = ConfigurationManager.ConnectionStrings[connectionString];

            if (settings == null)
                return items;

            // NOTE: SQLCE uses a different connection/command. I'm trying to keep this as generic as possible, without resorting to using NPoco. [LK]
            if (settings.ProviderName.InvariantEquals("System.Data.SqlServerCe.4.0"))
            {
                items.AddRange(GetSqlItems<SqlCeConnection, SqlCeCommand>(sql, settings.ConnectionString));
            }
            else
            {
                items.AddRange(GetSqlItems<SqlConnection, SqlCommand>(sql, settings.ConnectionString));
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
                    while (reader.Read())
                    {
                        if (reader.FieldCount > 0)
                        {
                            var item = new DataListItem
                            {
                                Name = reader[0].ToString()
                            };

                            item.Value = reader.FieldCount > 1 ? reader[1].ToString() : item.Name;

                            if (reader.FieldCount > 2)
                                item.Description = reader[2].ToString();

                            if (reader.FieldCount > 3)
                                item.Icon = reader[3].ToString();

                            yield return item;
                        }
                    }
                }
            }
        }
    }
}
