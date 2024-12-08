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

        [Obsolete("To be removed in Contentment 7.0")]
        public static class Views
        {
            [Obsolete("To be removed in Contentment 7.0")]
            public const string Buttons = DataEditors.ButtonsDataListEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string Bytes = DataEditors.BytesDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string CascadingDropdownList = DataEditors.CascadingDropdownListDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string CheckboxList = DataEditors.ButtonsDataListEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string CodeEditor = DataEditors.CodeEditorDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string ConfigurationEditor = DataEditors.ConfigurationEditorDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string ContentBlocks = DataEditors.ContentBlocksDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string ContentPicker = DataEditors.ContentPickerDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string DataList = DataEditors.DataListDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string DataPicker = DataEditors.DataPickerDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string DataTable = DataEditors.DataTableDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string DictionaryPicker = DataEditors.DictionaryPickerDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string DropdownList = DataEditors.DropdownListDataListEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string EditorNotes = DataEditors.EditorNotesDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string IconPicker = DataEditors.IconPickerDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string ItemPicker = DataEditors.ItemPickerDataListEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string ListItems = DataEditors.ListItemsDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string MacroPicker = DataEditors.MacroPickerDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string Notes = DataEditors.NotesDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string NumberInput = DataEditors.NumberInputDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string RadioButtonList = DataEditors.RadioButtonListDataListEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string RenderMacro = DataEditors.RenderMacroDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string SocialLinks = DataEditors.SocialLinksDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string Tags = DataEditors.TagsDataListEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string TemplatedLabel = DataEditors.TemplatedLabelDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string TemplatedList = DataEditors.TemplatedListDataListEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string TextboxList = DataEditors.TextboxListDataEditor.DataEditorViewPath;

            [Obsolete("To be removed in Contentment 7.0")]
            public const string TextInput = DataEditors.TextInputDataEditor.DataEditorViewPath;
        }

        public static class UiAliases
        {
            public const string Buttons = DataEditors.ButtonsDataListEditor.DataEditorUiAlias;

            public const string Bytes = DataEditors.BytesDataEditor.DataEditorUiAlias;

            public const string CascadingDropdownList = DataEditors.CascadingDropdownListDataEditor.DataEditorUiAlias;

            public const string CheckboxList = DataEditors.ButtonsDataListEditor.DataEditorUiAlias;

            public const string CodeEditor = DataEditors.CodeEditorDataEditor.DataEditorUiAlias;

            public const string ConfigurationEditor = DataEditors.ConfigurationEditorDataEditor.DataEditorUiAlias;

            public const string ContentBlocks = DataEditors.ContentBlocksDataEditor.DataEditorUiAlias;

            public const string ContentPicker = DataEditors.ContentPickerDataEditor.DataEditorUiAlias;

            public const string DataList = DataEditors.DataListDataEditor.DataEditorUiAlias;

            public const string DataPicker = DataEditors.DataPickerDataEditor.DataEditorUiAlias;

            public const string DataTable = DataEditors.DataTableDataEditor.DataEditorUiAlias;

            public const string DictionaryPicker = DataEditors.DictionaryPickerDataEditor.DataEditorUiAlias;

            public const string DropdownList = DataEditors.DropdownListDataListEditor.DataEditorUiAlias;

            public const string EditorNotes = DataEditors.EditorNotesDataEditor.DataEditorUiAlias;

            public const string IconPicker = DataEditors.IconPickerDataEditor.DataEditorUiAlias;

            public const string ItemPicker = DataEditors.ItemPickerDataListEditor.DataEditorUiAlias;

            public const string ListItems = DataEditors.ListItemsDataEditor.DataEditorUiAlias;

            public const string Notes = DataEditors.NotesDataEditor.DataEditorUiAlias;

            public const string NumberInput = DataEditors.NumberInputDataEditor.DataEditorUiAlias;

            public const string RadioButtonList = DataEditors.RadioButtonListDataListEditor.DataEditorUiAlias;

            public const string SocialLinks = DataEditors.SocialLinksDataEditor.DataEditorUiAlias;

            public const string Tags = DataEditors.TagsDataListEditor.DataEditorUiAlias;

            public const string TemplatedLabel = DataEditors.TemplatedLabelDataEditor.DataEditorUiAlias;

            public const string TemplatedList = DataEditors.TemplatedListDataListEditor.DataEditorUiAlias;

            public const string TextboxList = DataEditors.TextboxListDataEditor.DataEditorUiAlias;

            public const string TextInput = DataEditors.TextInputDataEditor.DataEditorUiAlias;
        }
    }
}
