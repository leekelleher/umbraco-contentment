// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.Dtos;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Migrations.Upgrade.V_6_0_0;

internal sealed class MigrateNotesConfiguration : MigrationBase
{
    public const string State = "{contentment-notes-config}";

    private readonly IConfigurationEditorJsonSerializer _configurationEditorJsonSerializer;

    public MigrateNotesConfiguration(IMigrationContext context, IConfigurationEditorJsonSerializer configurationEditorJsonSerializer) : base(context)
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

            if (configurationData.TryGetValueAs("notes", out string? notes) == true &&
                notes?.DetectIsJson() is false)
            {
                configurationData["notes"] = new RichTextEditorValue { Markup = notes, Blocks = null };
            }

            dataTypeDto.Configuration = _configurationEditorJsonSerializer.Serialize(configurationData);

            _ = Database.Update(dataTypeDto);
        }
    }
}
