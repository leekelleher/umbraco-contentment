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
                ContentmentConstants.PropertyEditors.Aliases.Bytes => "Umb.Contentment.PropertyEditorUi.Bytes",
                ContentmentConstants.PropertyEditors.Aliases.CodeEditor => "Umb.Contentment.PropertyEditorUi.CodeEditor",
                ContentmentConstants.PropertyEditors.Aliases.ContentBlocks => "Umb.Contentment.PropertyEditorUi.ContentBlocks",
                ContentmentConstants.PropertyEditors.Aliases.DataList => "Umb.Contentment.PropertyEditorUi.DataList",
                ContentmentConstants.PropertyEditors.Aliases.DataPicker => "Umb.Contentment.PropertyEditorUi.DataPicker",
                ContentmentConstants.PropertyEditors.Aliases.EditorNotes => "Umb.Contentment.PropertyEditorUi.EditorNotes",
                ContentmentConstants.PropertyEditors.Aliases.IconPicker => "Umb.Contentment.PropertyEditorUi.IconPicker",
                ContentmentConstants.PropertyEditors.Aliases.ListItems => "Umb.Contentment.PropertyEditorUi.ListItems",
                ContentmentConstants.PropertyEditors.Aliases.Notes => "Umb.Contentment.PropertyEditorUi.Notes",
                ContentmentConstants.PropertyEditors.Aliases.NumberInput => "Umb.Contentment.PropertyEditorUi.NumberInput",
                ContentmentConstants.PropertyEditors.Aliases.RenderMacro => "Umb.Contentment.PropertyEditorUi.RenderMacro",
                ContentmentConstants.PropertyEditors.Aliases.SocialLinks => "Umb.Contentment.PropertyEditorUi.SocialLinks",
                ContentmentConstants.PropertyEditors.Aliases.TemplatedLabel => "Umb.Contentment.PropertyEditorUi.TemplatedLabel",
                ContentmentConstants.PropertyEditors.Aliases.TextboxList => "Umb.Contentment.PropertyEditorUi.TextboxList",
                ContentmentConstants.PropertyEditors.Aliases.TextInput => "Umb.Contentment.PropertyEditorUi.TextInput",
                _ => null
            };

            if (dataTypeDto.EditorUiAlias is not null)
            {
                _ = Database.Update(dataTypeDto);
            }
        }
    }
}
