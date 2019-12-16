/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Umbraco.Community.Contentment.Composing
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    internal sealed class ContentmentRunComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition
                .Components()
                    .Append<ContentmentComponent>()
            ;

#if !DEBUG
            composition
                .DataEditors()
                    .Exclude<CardsDataEditor>()
                    .Exclude<CascadingDropdownListDataEditor>()
                    .Exclude<CheckboxDataEditor>()
                    .Exclude<CheckboxListDataEditor>()
                    .Exclude<CodeEditorDataEditor>()
                    .Exclude<ConfigurationEditorDataEditor>()
                    .Exclude<ContentBlocksDataEditor>()
                    .Exclude<DataTableDataEditor>()
                    .Exclude<DropdownListDataEditor>()
                    .Exclude<ItemPickerDataEditor>()
                    .Exclude<MacroPickerDataEditor>()
                    .Exclude<RadioButtonListDataEditor>()
                    .Exclude<TextInputDataEditor>()
                    .Exclude<TogglesDataEditor>()
            ;

            composition
                .PropertyValueConverters()
                    .Remove<CascadingDropdownListValueConverter>()
                    .Remove<CheckboxValueConverter>()
                    .Remove<CheckboxListValueConverter>()
                    .Remove<CodeEditorValueConverter>()
                    .Remove<ConfigurationEditorValueConverter>()
                    .Remove<ContentBlocksValueConverter>()
                    .Remove<DataTableValueConverter>()
                    .Remove<DropdownListValueConverter>()
                    .Remove<ItemPickerValueConverter>()
                    .Remove<MacroPickerValueConverter>()
                    .Remove<RadioButtonListValueConverter>()
            ;
#endif
        }
    }
}
