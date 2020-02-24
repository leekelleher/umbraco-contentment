/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if DEBUG
using Umbraco.Community.Contentment.DataEditors;
#endif
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Umbraco.Community.Contentment.Composing
{
    [ComposeAfter(typeof(ContentmentBootComposer))]
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    internal sealed class ContentmentRunComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition
                .Components()
                    .Append<ContentmentComponent>()
            ;

#if DEBUG
            composition
                .DataEditors()
                    .Add<CheckboxListDataEditor>()
                    .Add<CodeEditorDataEditor>()
                    .Add<ConfigurationEditorDataEditor>()
                    .Add<ContentBlocksDataEditor>()
                    .Add<DataTableDataEditor>()
                    .Add<DropdownListDataEditor>()
                    .Add<ItemPickerDataEditor>()
                    .Add<MacroPickerDataEditor>()
                    .Add<NumberInputDataEditor>()
                    .Add<RadioButtonListDataEditor>()
                    .Add<TextInputDataEditor>()
            ;

            composition
                .PropertyValueConverters()
                    .Append<CheckboxListValueConverter>()
                    .Append<CodeEditorValueConverter>()
                    .Append<ConfigurationEditorValueConverter>()
                    .Append<ContentBlocksValueConverter>()
                    .Append<DataTableValueConverter>()
                    .Append<DropdownListValueConverter>()
                    .Append<ItemPickerValueConverter>()
                    .Append<MacroPickerValueConverter>()
                    .Append<NumberInputValueConverter>()
                    .Append<RadioButtonListValueConverter>()
                    .Append<TextInputValueConverter>()
            ;
#endif
        }
    }
}
