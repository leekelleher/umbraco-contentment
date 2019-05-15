/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Entities;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.Models.ContentEditing;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class UmbracoEntityPickerValueConverter : PropertyValueConverterBase
    {
        private readonly IdkMap _idkMap;
        private readonly ServiceContext _services;

        public UmbracoEntityPickerValueConverter(ServiceContext services, IdkMap idkMap)
            : base()
        {
            _services = services;
            _idkMap = idkMap;
        }

        public override bool IsConverter(PublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(UmbracoEntityPickerDataEditor.DataEditorAlias);

        public override PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType) => PropertyCacheLevel.Element;

        public override Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            if (propertyType.DataType.Configuration is Dictionary<string, object> config &&
                config.ContainsKey(Constants.Conventions.ConfigurationEditors.EntityType) &&
                Enum.TryParse(config[Constants.Conventions.ConfigurationEditors.EntityType].ToString(), out UmbracoEntityTypes entityType))
            {
                switch (entityType)
                {
                    case UmbracoEntityTypes.DataType:
                        return typeof(IEnumerable<IDataType>);

                    case UmbracoEntityTypes.Document:
                        return typeof(IEnumerable<IContent>);

                    case UmbracoEntityTypes.DocumentType:
                        return typeof(IEnumerable<IContentType>);

                    case UmbracoEntityTypes.Macro:
                        return typeof(IEnumerable<IMacro>);

                    case UmbracoEntityTypes.Media:
                        return typeof(IEnumerable<IMedia>);

                    case UmbracoEntityTypes.MediaType:
                        return typeof(IEnumerable<IMediaType>);

                    case UmbracoEntityTypes.Member:
                        return typeof(IEnumerable<IMember>);

                    case UmbracoEntityTypes.MemberType:
                        return typeof(IEnumerable<IMemberType>);

                    case UmbracoEntityTypes.Template:
                        return typeof(IEnumerable<ITemplate>);

                    case UmbracoEntityTypes.DictionaryItem:
                    case UmbracoEntityTypes.Domain:
                    case UmbracoEntityTypes.Language:
                    case UmbracoEntityTypes.MemberGroup:
                    case UmbracoEntityTypes.PropertyType:
                    case UmbracoEntityTypes.PropertyGroup:
                    case UmbracoEntityTypes.Stylesheet:
                    case UmbracoEntityTypes.User:
                    default:
                        return typeof(IEnumerable<IEntity>);
                }
            }

            return typeof(IEnumerable<IEntity>);
        }

        public override object ConvertSourceToIntermediate(IPublishedElement owner, PublishedPropertyType propertyType, object source, bool preview)
        {
            if (source is string value)
            {
                return JsonConvert.DeserializeObject<UmbracoEntityPickerModel>(value);
            }

            return base.ConvertSourceToIntermediate(owner, propertyType, source, preview);
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, PublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            if (inter is UmbracoEntityPickerModel model)
            {
                switch (model.EntityType)
                {
                    case UmbracoEntityTypes.DataType:
                        {
                            var ids = model.Items.Select(_idkMap.GetIdForUdi).Where(x => x.Success).Select(x => x.Result).ToArray();
                            return _services.DataTypeService.GetAll(ids);
                        }

                    case UmbracoEntityTypes.Document:
                        return _services.ContentService.GetByIds(model.Items.Select(x => x.Guid));

                    case UmbracoEntityTypes.DocumentType:
                        {
                            var ids = model.Items.Select(_idkMap.GetIdForUdi).Where(x => x.Success).Select(x => x.Result).ToArray();
                            return _services.ContentTypeService.GetAll(ids);
                        }

                    case UmbracoEntityTypes.Macro:
                        return _services.MacroService.GetAll(model.Items.Select(x => x.Guid).ToArray());

                    case UmbracoEntityTypes.Media:
                        return _services.MediaService.GetByIds(model.Items.Select(x => x.Guid));

                    case UmbracoEntityTypes.MediaType:
                        {
                            var ids = model.Items.Select(_idkMap.GetIdForUdi).Where(x => x.Success).Select(x => x.Result).ToArray();
                            return _services.MediaTypeService.GetAll(ids);
                        }

                    case UmbracoEntityTypes.Member:
                        {
                            var ids = model.Items.Select(_idkMap.GetIdForUdi).Where(x => x.Success).Select(x => x.Result).ToArray();
                            return _services.MemberService.GetAllMembers(ids);
                        }

                    case UmbracoEntityTypes.MemberType:
                        {
                            var ids = model.Items.Select(_idkMap.GetIdForUdi).Where(x => x.Success).Select(x => x.Result).ToArray();
                            return _services.MemberTypeService.GetAll(ids);
                        }

                    case UmbracoEntityTypes.Template:
                        // NOTE: Can't use `FileService.GetTemplates`, as it only accepts `string[]` aliases.
                        return _services.EntityService.GetAll(UmbracoObjectTypes.Template, model.Items.Select(x => x.Guid).ToArray());

                    case UmbracoEntityTypes.DictionaryItem:
                    case UmbracoEntityTypes.Domain:
                    case UmbracoEntityTypes.Language:
                    case UmbracoEntityTypes.MemberGroup:
                    case UmbracoEntityTypes.PropertyType:
                    case UmbracoEntityTypes.PropertyGroup:
                    case UmbracoEntityTypes.Stylesheet:
                    case UmbracoEntityTypes.User:
                    default:
                        return Enumerable.Empty<IEntity>();
                }
            }

            return base.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, inter, preview);
        }
    }
}
