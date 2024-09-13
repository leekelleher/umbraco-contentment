// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using System.Text.Json.Nodes;
using Umbraco.Cms.Core;
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
                configurationData["dataSource"] = MigrateDataSourceConfiguration(dataSource);
            }

            dataTypeDto.Configuration = _configurationEditorJsonSerializer.Serialize(configurationData);

            _ = Database.Update(dataTypeDto);
        }
    }

    public static JsonArray MigrateDataSourceConfiguration(JsonArray dataSource)
    {
        var item = dataSource[0]!;

        var key = item["key"]?.ToString();

        switch (key)
        {
            case "Umbraco.Community.Contentment.DataEditors.UmbracoContentDataListSource, Umbraco.Community.Contentment":
            {
                try
                {
                    var value = item["value"];
                    if (value is not null)
                    {
                        var parentNode = value["parentNode"]?.ToString();
                        if (string.IsNullOrWhiteSpace(parentNode) == false &&
                            UdiParser.TryParse(parentNode, out GuidUdi? udi) == true &&
                            udi?.Guid.Equals(Guid.Empty) == false)
                        {
                            value["parentNode"] = new JsonObject
                            {
                                ["originKey"] = udi.Guid.ToString(),
                                ["originAlias"] = "ByKey",
                            };

                            item["value"] = value;
                        }
                    }
                }
                catch { /* ¯\_(ツ)_/¯ */ }
                break;
            }

            case "Umbraco.Community.Contentment.DataEditors.UmbracoContentPropertiesDataListSource, Umbraco.Community.Contentment":
            {
                try
                {
                    var value = item["value"];
                    if (value is not null)
                    {
                        var contentTypes = value["contentType"]?.AsArray();
                        if (contentTypes?.Count > 0)
                        {
                            var uniques = contentTypes
                                .Select(x => UdiParser.TryParse(x?.ToString(), out GuidUdi? udi) == true ? udi.Guid : Guid.Empty)
                                .Where(x => x.Equals(Guid.Empty) == false);

                            value["contentType"] = string.Join(",", uniques);
                            item["value"] = value;
                        }
                    }
                }
                catch { /* ¯\_(ツ)_/¯ */ }
                break;
            }

            case "Umbraco.Community.Contentment.DataEditors.DataList.DataSources.UmbracoContentPropertyValueDataListSource, Umbraco.Community.Contentment":
            {
                item["key"] = "Umbraco.Community.Contentment.DataEditors.UmbracoContentPropertyValueDataListSource, Umbraco.Community.Contentment";

                try
                {
                    var value = item["value"];
                    if (value is not null)
                    {
                        var contentNode = value["contentNode"]?.ToString();
                        if (string.IsNullOrWhiteSpace(contentNode) == false &&
                            UdiParser.TryParse(contentNode, out GuidUdi? udi) == true &&
                            udi?.Guid.Equals(Guid.Empty) == false)
                        {
                            value["contentNode"] = new JsonObject
                            {
                                ["originKey"] = udi.Guid.ToString(),
                                ["originAlias"] = "ByKey",
                            };

                            item["value"] = value;
                        }
                    }
                }
                catch { /* ¯\_(ツ)_/¯ */ }
                break;
            }

            case "Umbraco.Community.Contentment.DataEditors.UmbracoDictionaryDataListSource, Umbraco.Community.Contentment":
            {
                try
                {
                    var value = item["value"];
                    if (value is not null)
                    {
                        var items = value["item"]?.AsArray();
                        if (items?.Count > 0)
                        {
                            var guid = items.FirstOrDefault()?["key"]?.ToString();
                            value["item"] = guid;
                            item["value"] = value;
                        }
                    }
                }
                catch { /* ¯\_(ツ)_/¯ */ }
                break;
            }

            case "Umbraco.Community.Contentment.DataEditors.UmbracoMembersDataListSource, Umbraco.Community.Contentment":
            {
                try
                {
                    var value = item["value"];
                    if (value is not null)
                    {
                        var memberTypes = value["memberType"]?.AsArray();
                        if (memberTypes?.Count > 0)
                        {
                            var uniques = memberTypes
                                .Select(x => UdiParser.TryParse(x?.ToString(), out GuidUdi? udi) == true ? udi.Guid : Guid.Empty)
                                .Where(x => x.Equals(Guid.Empty) == false);

                            value["memberType"] = string.Join(",", uniques);
                            item["value"] = value;
                        }
                    }
                }
                catch { /* ¯\_(ツ)_/¯ */ }
                break;
            }

            case "Umbraco.Community.Contentment.DataEditors.UmbracoUsersDataListSource, Umbraco.Community.Contentment":
            {
                try
                {
                    var value = item["value"];
                    if (value is not null)
                    {
                        var userGroups = value["userGroup"]?.AsArray();
                        if (userGroups?.Count > 0)
                        {
                            var aliases = userGroups.Select(x => x?.ToString());
                            value["userGroup"] = string.Join(",", aliases);
                            item["value"] = value;
                        }
                    }
                }
                catch { /* ¯\_(ツ)_/¯ */ }
                break;
            }

            default:
                break;
        }

        return dataSource;
    }
}
