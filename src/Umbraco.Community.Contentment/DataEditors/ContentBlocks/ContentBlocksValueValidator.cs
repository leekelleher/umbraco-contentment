/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Validation;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class ContentBlocksValueValidator : ComplexEditorValidator
    {
        private readonly Lazy<Dictionary<Guid, IContentType>> _elementTypes;

        public ContentBlocksValueValidator(
            Lazy<Dictionary<Guid, IContentType>> elementTypes,
            IPropertyValidationService propertyValidationService)
            : base(propertyValidationService)
        {
            _elementTypes = elementTypes;
        }
        protected override IEnumerable<ElementTypeValidationModel> GetElementTypeValidation(object? value, PropertyValidationContext validationContext)
        {
            if (value is JArray array && array.Any() == true)
            {
                var blocks = array.ToObject<IEnumerable<ContentBlock>>();
                if (blocks?.Any() == true)
                {
                    foreach (var block in blocks)
                    {
                        if (block != null &&
                            _elementTypes.Value.TryGetValue(block.ElementType, out var elementType) == true)
                        {
                            var elementValidation = new ElementTypeValidationModel(elementType.Alias, block.Key);

                            foreach (var propertyType in elementType.CompositionPropertyTypes)
                            {
                                if (block.Value.TryGetValue(propertyType.Alias, out var bpv) == true)
                                {
                                    // TODO: [LK] Review what the `JsonPath` parameter should be here.
                                    elementValidation.AddPropertyTypeValidation(new PropertyTypeValidationModel(propertyType, bpv, string.Empty));
                                }
                            }

                            yield return elementValidation;
                        }
                    }
                }
            }
        }
    }
}
