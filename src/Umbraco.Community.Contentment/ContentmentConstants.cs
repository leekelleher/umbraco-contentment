/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Umbraco.Community.Contentment;

public static class ContentmentConstants
{
    public static class PropertyEditors
    {
        public static class Aliases
        {
            public const string Bytes = DataEditors.BytesDataEditor.DataEditorAlias;

            public const string CodeEditor = DataEditors.CodeEditorDataEditor.DataEditorAlias;

            public const string ConfigurationEditor = DataEditors.ConfigurationEditorDataEditor.DataEditorAlias;

            public const string ContentBlocks = DataEditors.ContentBlocksDataEditor.DataEditorAlias;

            public const string DataList = DataEditors.DataListDataEditor.DataEditorAlias;

            public const string DataPicker = DataEditors.DataPickerDataEditor.DataEditorAlias;

            public const string DataTable = DataEditors.DataTableDataEditor.DataEditorAlias;

            public const string EditorNotes = DataEditors.EditorNotesDataEditor.DataEditorAlias;

            public const string IconPicker = DataEditors.IconPickerDataEditor.DataEditorAlias;

            public const string ListItems = DataEditors.ListItemsDataEditor.DataEditorAlias;

            public const string Notes = DataEditors.NotesDataEditor.DataEditorAlias;

            public const string NumberInput = DataEditors.NumberInputDataEditor.DataEditorAlias;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string RenderMacro = DataEditors.RenderMacroDataEditor.DataEditorAlias;

            public const string SocialLinks = DataEditors.SocialLinksDataEditor.DataEditorAlias;

            public const string TemplatedLabel = DataEditors.TemplatedLabelDataEditor.DataEditorAlias;

            public const string TextboxList = DataEditors.TextboxListDataEditor.DataEditorAlias;

            public const string TextInput = DataEditors.TextInputDataEditor.DataEditorAlias;
        }

        public static class ConfigurationKeys
        {
            public const string AddButtonLabelKey = Constants.Conventions.ConfigurationFieldAliases.AddButtonLabelKey;

            public const string DefaultValue = Constants.Conventions.ConfigurationFieldAliases.DefaultValue;

            public const string Items = Constants.Conventions.ConfigurationFieldAliases.Items;

            public const string OverlayView = Constants.Conventions.ConfigurationFieldAliases.OverlayView;
        }

        public static class Groups
        {
            public const string Code = Constants.Conventions.PropertyGroups.Code;

            public const string Display = Constants.Conventions.PropertyGroups.Display;
        }

        // TODO: [LK:2024-12-06] Figure out if this is still needed?
        public static class Views
        {
            public const string Buttons = DataEditors.ButtonsDataListEditor.DataEditorViewPath;

            public const string Bytes = DataEditors.BytesDataEditor.DataEditorViewPath;

            public const string CascadingDropdownList = DataEditors.CascadingDropdownListDataEditor.DataEditorViewPath;

            public const string CheckboxList = DataEditors.ButtonsDataListEditor.DataEditorViewPath;

            public const string CodeEditor = DataEditors.CodeEditorDataEditor.DataEditorViewPath;

            public const string ConfigurationEditor = DataEditors.ConfigurationEditorDataEditor.DataEditorViewPath;

            public const string ContentBlocks = DataEditors.ContentBlocksDataEditor.DataEditorViewPath;

            public const string ContentPicker = DataEditors.ContentPickerDataEditor.DataEditorViewPath;

            public const string DataList = DataEditors.DataListDataEditor.DataEditorViewPath;

            public const string DataPicker = DataEditors.DataPickerDataEditor.DataEditorViewPath;

            public const string DataTable = DataEditors.DataTableDataEditor.DataEditorViewPath;

            public const string DictionaryPicker = DataEditors.DictionaryPickerDataEditor.DataEditorViewPath;

            public const string DropdownList = DataEditors.DropdownListDataListEditor.DataEditorViewPath;

            public const string EditorNotes = DataEditors.EditorNotesDataEditor.DataEditorViewPath;

            public const string IconPicker = DataEditors.IconPickerDataEditor.DataEditorViewPath;

            public const string ItemPicker = DataEditors.ItemPickerDataListEditor.DataEditorViewPath;

            public const string ListItems = DataEditors.ListItemsDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string MacroPicker = DataEditors.MacroPickerDataEditor.DataEditorViewPath;

            public const string Notes = DataEditors.NotesDataEditor.DataEditorViewPath;

            public const string NumberInput = DataEditors.NumberInputDataEditor.DataEditorViewPath;

            public const string RadioButtonList = DataEditors.RadioButtonListDataListEditor.DataEditorViewPath;

            public const string RenderMacro = DataEditors.RenderMacroDataEditor.DataEditorViewPath;

            public const string SocialLinks = DataEditors.SocialLinksDataEditor.DataEditorViewPath;

            public const string Tags = DataEditors.TagsDataListEditor.DataEditorViewPath;

            public const string TemplatedLabel = DataEditors.TemplatedLabelDataEditor.DataEditorViewPath;

            public const string TemplatedList = DataEditors.TemplatedListDataListEditor.DataEditorViewPath;

            public const string TextboxList = DataEditors.TextboxListDataEditor.DataEditorViewPath;

            public const string TextInput = DataEditors.TextInputDataEditor.DataEditorViewPath;
        }


        // TODO: [LK:2024-12-06] Add the constants for PropertyEditorUiAlias(es).
    }
}
