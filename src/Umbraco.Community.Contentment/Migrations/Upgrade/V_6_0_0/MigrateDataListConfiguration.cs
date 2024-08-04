// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.Dtos;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Migrations.Upgrade.V_6_0_0;

internal sealed class MigrateDataListConfiguration : MigrationBase
{
    public const string State = "{contentment-data-list-config}";

    private readonly IConfigurationEditorJsonSerializer _configurationEditorJsonSerializer;

    public MigrateDataListConfiguration(IMigrationContext context, IConfigurationEditorJsonSerializer configurationEditorJsonSerializer) : base(context)
    {
        _configurationEditorJsonSerializer = configurationEditorJsonSerializer;
    }

    protected override void Migrate()
    {
        var sql = Sql()
            .Select<DataTypeDto>()
            .From<DataTypeDto>()
            .Where<DataTypeDto>(x => x.EditorAlias.InvariantEquals(DataListDataEditor.DataEditorAlias) == true);

        var dataTypeDtos = Database.Fetch<DataTypeDto>(sql);

        foreach (var dataTypeDto in dataTypeDtos)
        {
            var configurationData = string.IsNullOrWhiteSpace(dataTypeDto.Configuration) == false
                ? _configurationEditorJsonSerializer
                    .Deserialize<Dictionary<string, object?>>(dataTypeDto.Configuration)?
                    .Where(item => item.Value is not null)
                    .ToDictionary(item => item.Key, item => item.Value!) ?? []
                : [];

            if (configurationData.TryGetValueAs("dataSource", out JsonArray? dataSource) == true &&
               dataSource?.Count > 0)
            {
                var item = dataSource[0]!;

                var key = item["key"]?.ToString();

                if (key == "Umbraco.Community.Contentment.DataEditors.DataList.DataSources.UmbracoContentPropertyValueDataListSource, Umbraco.Community.Contentment")
                {
                    item["key"] = "Umbraco.Community.Contentment.DataEditors.UmbracoContentPropertyValueDataListSource, Umbraco.Community.Contentment";
                }

                configurationData["dataSource"] = dataSource;
            }

            dataTypeDto.Configuration = _configurationEditorJsonSerializer.Serialize(configurationData);

            _ = Database.Update(dataTypeDto);
        }
    }
}
