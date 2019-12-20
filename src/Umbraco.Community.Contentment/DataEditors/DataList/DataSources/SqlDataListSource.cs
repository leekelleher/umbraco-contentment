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
        public string Name => "SQL Data";

        public string Description => "Use a SQL Server database as the data source.";

        public string Icon => "icon-server-alt";

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>
        {
            { ConnectionStringConfigurationField.ConnectionString, Core.Constants.System.UmbracoConnectionName }
        };

        [ConfigurationField(typeof(SqlNotesConfigurationField))]
        public string Notes { get; set; }

        [ConfigurationField(typeof(QueryConfigurationField))]
        public string Query { get; set; }

        [ConfigurationField(typeof(ConnectionStringConfigurationField))]
        public string ConnectionString { get; set; }

        public IEnumerable<DataListItem> GetItems()
        {
            var items = new List<DataListItem>();

            if (string.IsNullOrWhiteSpace(Query) || string.IsNullOrWhiteSpace(ConnectionString))
                return items;

            var query = Query.Replace("\r", string.Empty).Replace("\n", " ").Replace("\t", " ");
            var settings = ConfigurationManager.ConnectionStrings[ConnectionString];

            if (settings == null)
                return items;

            // NOTE: SQLCE uses a different connection/command. I'm trying to keep this as generic as possible, without resorting to using NPoco. [LK]
            if (settings.ProviderName.InvariantEquals("System.Data.SqlServerCe.4.0"))
            {
                items.AddRange(GetSqlItems<SqlCeConnection, SqlCeCommand>(query, settings.ConnectionString));
            }
            else
            {
                items.AddRange(GetSqlItems<SqlConnection, SqlCommand>(query, settings.ConnectionString));
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

        class SqlNotesConfigurationField : NotesConfigurationField
        {
            public SqlNotesConfigurationField()
                : base(@"<div class=""alert alert-info"">
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
</div>", true)
            { }
        }

        class QueryConfigurationField : ConfigurationField
        {
            internal const string Query = "query";

            public QueryConfigurationField()
            {
                // NOTE: Umbraco doesn't ship with SQL mode, so we check if its been added manually, otherwise defautls to Razor.
                var mode = System.IO.File.Exists(IOHelper.MapPath("~/umbraco/lib/ace-builds/src-min-noconflict/mode-sql.js"))
                    ? "sql"
                    : "razor";

                Key = Query;
                Name = "SQL query";
                Description = "Enter your SQL query.";
                View = IOHelper.ResolveUrl(CodeEditorDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>
                {
                    { CodeEditorConfigurationEditor.Mode, mode },
                };
            }
        }

        class ConnectionStringConfigurationField : ConfigurationField
        {
            internal const string ConnectionString = "connectionString";

            public ConnectionStringConfigurationField()
            {
                var items = new List<DataListItem>();
                foreach (ConnectionStringSettings connString in ConfigurationManager.ConnectionStrings)
                {
                    items.Add(new DataListItem { Name = connString.Name, Value = connString.Name });
                }

                Key = ConnectionString;
                Name = "Connection string";
                Description = "Enter the connection string.";
                View = IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>
                {
                    { AllowEmptyConfigurationField.AllowEmpty, Constants.Values.False },
                    { DropdownListConfigurationEditor.Items, items },
                };
            }
        }
    }
}
