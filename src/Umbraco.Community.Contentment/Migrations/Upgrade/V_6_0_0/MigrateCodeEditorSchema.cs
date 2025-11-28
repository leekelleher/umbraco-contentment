// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.Dtos;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Migrations.Upgrade.V_6_0_0;

internal sealed class MigrateCodeEditorSchema : MigrationBase
{
    public const string State = "{contentment-code-editor-schema}";

    public MigrateCodeEditorSchema(IMigrationContext context) : base(context) { }

    protected override void Migrate()
    {
        var sql = Sql()
            .Select<DataTypeDto>()
            .From<DataTypeDto>()
            .Where<DataTypeDto>(x => x.EditorAlias.InvariantEquals(CodeEditorDataEditor.DataEditorAlias) == true);

        var dataTypeDtos = Database.Fetch<DataTypeDto>(sql);

        foreach (var dataTypeDto in dataTypeDtos)
        {
            if (dataTypeDto.EditorUiAlias.InvariantEquals(ContentmentConstants.PropertyEditors.UiAliases.CodeEditor) == true)
            {
                dataTypeDto.EditorAlias = UmbConstants.PropertyEditors.Aliases.PlainString;

                _ = Database.Update(dataTypeDto);
            }
        }
    }
}
