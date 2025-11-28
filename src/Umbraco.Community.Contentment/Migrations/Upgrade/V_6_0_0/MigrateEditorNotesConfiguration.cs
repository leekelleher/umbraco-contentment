// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.Dtos;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Migrations.Upgrade.V_6_0_0;

internal sealed class MigrateEditorNotesConfiguration : MigrationBase
{
    public const string State = "{contentment-editor-notes-config}";

    private readonly IConfigurationEditorJsonSerializer _configurationEditorJsonSerializer;

    public MigrateEditorNotesConfiguration(IMigrationContext context, IConfigurationEditorJsonSerializer configurationEditorJsonSerializer) : base(context)
    {
        _configurationEditorJsonSerializer = configurationEditorJsonSerializer;
    }

    protected override void Migrate()
    {
        var sql = Sql()
            .Select<DataTypeDto>()
            .From<DataTypeDto>()
            .Where<DataTypeDto>(x =>
                x.EditorAlias.InvariantEquals(EditorNotesDataEditor.DataEditorAlias) == true ||
                x.EditorAlias.InvariantEquals(NotesDataEditor.DataEditorAlias) == true);

        var dataTypeDtos = Database.Fetch<DataTypeDto>(sql);

        foreach (var dataTypeDto in dataTypeDtos)
        {
            var configurationData = string.IsNullOrWhiteSpace(dataTypeDto.Configuration) == false
                ? _configurationEditorJsonSerializer
                    .Deserialize<Dictionary<string, object?>>(dataTypeDto.Configuration)?
                    .Where(item => item.Value is not null)
                    .ToDictionary(item => item.Key, item => item.Value!) ?? []
                : [];

            if (configurationData.TryGetValueAs("alertType", out string? alertType) == true)
            {
                configurationData["alertType"] = alertType switch
                {
                    "alert alert-form" => "default",
                    "alert alert-info" => "default",
                    "alert alert-success" => "positive",
                    "alert alert-warning" => "warning",
                    "alert alert-error" => "danger",
                    "well" => "default",
                    _ => "",
                };
            }

            if (configurationData.TryGetValueAs("message", out string? message) == true &&
               message?.DetectIsJson() is false)
            {
                configurationData["message"] = new RichTextEditorValue { Markup = message, Blocks = null };
            }

            dataTypeDto.Configuration = _configurationEditorJsonSerializer.Serialize(configurationData);

            _ = Database.Update(dataTypeDto);
        }
    }
}
