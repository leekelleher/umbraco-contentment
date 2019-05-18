/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class SqlProviderConfigurationEditor : IDataProvider
    {
        public string Name => "SQL";

        public string Description => "Use a SQL Server database as the data source.";

        public string Icon => "icon-server-alt";

        [ConfigurationField("query", "SQL Query", "views/propertyeditors/textarea/textarea.html", Description = "Enter the SQL query.<br><br>You must return `label` and `value` columns names.")]
        public string Query { get; set; }

        [ConfigurationField(typeof(ConnectionStringConfigurationField))]
        public string ConnectionString { get; set; }

        public Dictionary<string, string> GetItems()
        {
            // TODO: Review this, make it bulletproof

            using (var database = new NPoco.Database(ConnectionString))
            {
                // SELECT macroAlias AS [value], macroName AS [label] FROM cmsMacro ORDER BY [label];
                var sql = Query.Replace("\r", "").Replace("\n", " ");
                var items = database.Fetch<LabelValueModel>(sql);

                return items.ToDictionary(x => x.value, x => x.label);
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
                View = IOHelper.ResolveUrl(DropdownDataEditor.DataEditorViewPath);
                Config = new Dictionary<string, object>
                {
                    { "allowEmpty", 0 },
                    { "items", connectionStrings.Select(x => new { label = x, value = x }) }
                };
            }
        }

        class LabelValueModel
        {
            public string label { get; set; }
            public string value { get; set; }
        }
    }
}
