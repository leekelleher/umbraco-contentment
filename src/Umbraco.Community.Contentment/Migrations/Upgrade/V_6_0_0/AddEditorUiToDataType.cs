/* Copyright © 2013-present Umbraco.
 * This Source Code has been derived from Umbraco CMS.
 * https://github.com/umbraco/Umbraco-CMS/blob/release-14.0.0/src/Umbraco.Infrastructure/Migrations/Upgrade/V_14_0_0/AddEditorUiToDataType.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2024 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.Dtos;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Migrations.Upgrade.V_6_0_0;

internal sealed class AddEditorUiToDataType : MigrationBase
{
    public const string State = "{contentment-editorui-datatype-schema}";

    public AddEditorUiToDataType(IMigrationContext context) : base(context) { }

    protected override void Migrate()
    {
        var sql = Sql()
            .Select<DataTypeDto>()
            .AndSelect<NodeDto>()
            .From<DataTypeDto>()
            .InnerJoin<NodeDto>()
            .On<DataTypeDto, NodeDto>(left => left.NodeId, right => right.NodeId)
            .Where<DataTypeDto>(x => x.EditorAlias.StartsWith(Constants.Internals.DataEditorAliasPrefix) && x.EditorUiAlias == x.EditorAlias);

        var dataTypeDtos = Database.Fetch<DataTypeDto>(sql);

        foreach (var dataTypeDto in dataTypeDtos)
        {
            dataTypeDto.EditorUiAlias = dataTypeDto.EditorAlias switch
            {
                ContentmentConstants.PropertyEditors.Aliases.Bytes => ContentmentConstants.PropertyEditors.UiAliases.Bytes,
                ContentmentConstants.PropertyEditors.Aliases.CodeEditor => ContentmentConstants.PropertyEditors.UiAliases.CodeEditor,
                ContentmentConstants.PropertyEditors.Aliases.ContentBlocks => ContentmentConstants.PropertyEditors.UiAliases.ContentBlocks,
                ContentmentConstants.PropertyEditors.Aliases.DataList => ContentmentConstants.PropertyEditors.UiAliases.DataList,
                ContentmentConstants.PropertyEditors.Aliases.DataPicker => ContentmentConstants.PropertyEditors.UiAliases.DataPicker,
                ContentmentConstants.PropertyEditors.Aliases.EditorNotes => ContentmentConstants.PropertyEditors.UiAliases.EditorNotes,
                ContentmentConstants.PropertyEditors.Aliases.IconPicker => ContentmentConstants.PropertyEditors.UiAliases.IconPicker,
                ContentmentConstants.PropertyEditors.Aliases.ListItems => ContentmentConstants.PropertyEditors.UiAliases.ListItems,
                ContentmentConstants.PropertyEditors.Aliases.Notes => ContentmentConstants.PropertyEditors.UiAliases.Notes,
                ContentmentConstants.PropertyEditors.Aliases.NumberInput => ContentmentConstants.PropertyEditors.UiAliases.NumberInput,
                ContentmentConstants.PropertyEditors.Aliases.RenderMacro => Constants.Internals.DataEditorUiAliasPrefix + "RenderMacro",
                ContentmentConstants.PropertyEditors.Aliases.SocialLinks => ContentmentConstants.PropertyEditors.UiAliases.SocialLinks,
                ContentmentConstants.PropertyEditors.Aliases.TemplatedLabel => ContentmentConstants.PropertyEditors.UiAliases.TemplatedLabel,
                ContentmentConstants.PropertyEditors.Aliases.TextboxList => ContentmentConstants.PropertyEditors.UiAliases.TextboxList,
                ContentmentConstants.PropertyEditors.Aliases.TextInput => ContentmentConstants.PropertyEditors.UiAliases.TextInput,
                _ => null
            };

            if (dataTypeDto.EditorUiAlias is not null)
            {
                _ = Database.Update(dataTypeDto);
            }
        }
    }
}
