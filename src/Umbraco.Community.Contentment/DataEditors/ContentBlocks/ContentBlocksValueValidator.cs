/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472 == false
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ContentBlocksValueValidator : ComplexEditorValidator
    {
        public ContentBlocksValueValidator(IPropertyValidationService propertyValidationService)
            : base(propertyValidationService)
        { }

        protected override IEnumerable<ElementTypeValidationModel> GetElementTypeValidation(object value)
        {
            // TODO: [LK:2022-09-01] Take a look at `ComplexEditorValidator`
            // https://github.com/umbraco/Umbraco-CMS/blob/v10/contrib/src/Umbraco.Infrastructure/PropertyEditors/ComplexEditorValidator.cs
            // https://github.com/umbraco/Umbraco-CMS/blob/v10/contrib/src/Umbraco.Infrastructure/PropertyEditors/BlockEditorPropertyEditor.cs#L278
            // https://github.com/umbraco/Umbraco-CMS/blob/v10/contrib/src/Umbraco.Infrastructure/PropertyEditors/NestedContentPropertyEditor.cs#L342

            return Enumerable.Empty<ElementTypeValidationModel>();
        }
    }
}
#endif
