/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

/* NOTE: This code file is ONLY the base partial class.
 * For the actual SQL logic for .NET Framework or .NET Core,
 * please see the .NET472, .NET5_0 or .NET6_0 code files. */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#if NET472
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using UmbConstants = Umbraco.Core.Constants;
#else
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public sealed partial class SqlDataListSource : IDataListSource, IDataPickerSource
    {
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

        public Dictionary<string, object> DefaultValues => new Dictionary<string, object>()
        {
            { "query", $"SELECT\r\n\t[text],\r\n\t[uniqueId]\r\nFROM\r\n\t[umbracoNode]\r\nWHERE\r\n\t[nodeObjectType] = '{UmbConstants.ObjectTypes.Strings.Document}'\r\n\tAND\r\n\t[level] = 1\r\nORDER BY\r\n\t[sortOrder] ASC\r\n\r\n-- This is an example query that will select all the content nodes that are at level 1.\r\n;" },
            { "connectionString", UmbConstants.System.UmbracoConnectionName }
        };

        public Task<IEnumerable<DataListItem>> GetItemsAsync(Dictionary<string, object> config, IEnumerable<string> values)
        {
            var items = GetItems(config);

            if (items?.Any() == true)
            {
                return Task.FromResult(items.Where(x => values.Contains(x.Value) == true));
            }

            return Task.FromResult(Enumerable.Empty<DataListItem>());
        }

        public Task<PagedResult<DataListItem>> SearchAsync(Dictionary<string, object> config, int pageNumber = 1, int pageSize = 12, string query = "")
        {
            var items = default(IEnumerable<DataListItem>);

            if (string.IsNullOrWhiteSpace(query) == true)
            {
                items = GetItems(config);
            }
            else
            {
                items = GetItems(config)?.Where(x => x.Name?.InvariantContains(query) == true || x.Value?.InvariantStartsWith(query) == true);
            }

            if (items?.Any() == true)
            {
                var offset = (pageNumber - 1) * pageSize;
                return Task.FromResult(new PagedResult<DataListItem>(items.Count(), pageNumber, pageSize)
                {
                    Items = items.Skip(offset).Take(pageSize),
                });
            }

            return Task.FromResult(new PagedResult<DataListItem>(-1, pageNumber, pageSize));
        }
    }
}
