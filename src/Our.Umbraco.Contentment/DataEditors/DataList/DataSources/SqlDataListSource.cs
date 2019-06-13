/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
#if !DEBUG
    // TODO: IsWorkInProgress - Under development.
    [global::Umbraco.Core.Composing.HideFromTypeFinder]
#endif
    internal class SqlDataListSource : IDataListSource
    {
        public string Name => "SQL";

        public string Description => "Use a SQL Server database as the data source.";

        public string Icon => "icon-server-alt";

        [ConfigurationField(typeof(NotesConfigurationField))]
        public string Notes { get; set; }

        [ConfigurationField(typeof(QueryConfigurationField))]
        public string Query { get; set; }

        [ConfigurationField(typeof(ConnectionStringConfigurationField))]
        public string ConnectionString { get; set; }

        public IEnumerable<DataListItem> GetItems()
        {
            // TODO: [LK:2019-06-06] Look to do a better way of querying the db.
            //// https://github.com/schotime/NPoco/blob/ec4d3d7808c8ce413b2d61f756d6d7277039c98d/src/NPoco/Database.cs
            //using (var cmd = database.CreateCommand(null, System.Data.CommandType.Text, sql))
            //{
            //}

            using (var database = new NPoco.Database(ConnectionString))
            {
                // SELECT macroAlias AS [value], macroName AS [name] FROM cmsMacro ORDER BY [name];
                var sql = Query.Replace("\r", "").Replace("\n", " ").Replace("\t", " ");

                // I'm not happy about using this class to deserialize the SQL data,
                // Look at alternatives, so we can remove this class.
                // Maybe we will end up using classic ADO type queries, to give us more control?
                var items = database.Fetch<DataListItem>(sql);

                return items;
            }
        }

        class NotesConfigurationField : ConfigurationField
        {
            public NotesConfigurationField()
            {
                Key = NotesConfigurationEditor.Notes;
                Name = nameof(NotesConfigurationEditor.Notes);
                View = IOHelper.ResolveUrl(NotesDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>
                {
                    // TODO: [LK:2019-06-07] Revise the SQL notes. We'll probably do 4 columns, anything that can serialize with a `DataListItem`.
                    { NotesConfigurationEditor.Notes, @"<p class=""alert alert-success""><strong>A note about your SQL query.</strong><br>
Your SQL query should be designed to return 2 columns, these will be used as name/value pairs in the data list.<br>
If more columns are returned, then only the first 2 columns will be used.</p>" }
                };
                HideLabel = true;
            }
        }

        class QueryConfigurationField : ConfigurationField
        {
            public QueryConfigurationField()
            {
                // NOTE: Umbraco doesn't ship with SQL mode, so we check if its been added manually, otherwise defautls to Razor.
                var mode = System.IO.File.Exists(IOHelper.MapPath("~/umbraco/lib/ace-builds/src-min-noconflict/mode-sql.js"))
                    ? "sql"
                    : "razor";

                Key = "query";
                Name = "SQL Query";
                Description = "Enter the SQL query.";
                View = IOHelper.ResolveUrl(CodeEditorDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>
                {
                    { CodeEditorConfigurationEditor.Mode, mode },
                };
            }
        }

        class ConnectionStringConfigurationField : ConfigurationField
        {
            public ConnectionStringConfigurationField()
            {
                var connectionStrings = new List<string>();
                foreach (ConnectionStringSettings connString in ConfigurationManager.ConnectionStrings)
                {
                    connectionStrings.Add(connString.Name);
                }

                Key = "connString";
                Name = "Connection String";
                Description = "Enter the connection string.";
                View = IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>
                {
                    { DropdownListConfigurationEditor.AllowEmpty, Constants.Values.False },
                    { DropdownListConfigurationEditor.Items, connectionStrings.Select(x => new { name = x, value = x }) }
                };
            }
        }
    }
}
