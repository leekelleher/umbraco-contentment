/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
#if NET472
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
#else
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    [PluginController(Constants.Internals.PluginControllerName), IsBackOffice]
    public sealed class DataListApiController : UmbracoAuthorizedJsonController
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly PropertyEditorCollection _propertyEditors;

        public DataListApiController(
            IDataTypeService dataTypeService,
            PropertyEditorCollection propertyEditors)
        {
            _dataTypeService = dataTypeService;
            _propertyEditors = propertyEditors;
        }

        [HttpPost]
#if NET472
        public HttpResponseMessage GetPreview([FromBody] JObject data)
#else
        public IActionResult GetPreview([FromBody] JObject data)
#endif
        {
            if (_propertyEditors.TryGet(DataListDataEditor.DataEditorAlias, out var propertyEditor) == true)
            {
                var config = data.ToObject<Dictionary<string, object>>();
                var alias = config.GetValueAs("alias", defaultValue: "preview");
                var configurationEditor = propertyEditor.GetConfigurationEditor();
                var valueEditorConfig = configurationEditor.ToValueEditor(config);
                var valueEditor = propertyEditor.GetValueEditor(config);

#if NET472
                return Request.CreateResponse(HttpStatusCode.OK, new { config = valueEditorConfig, view = valueEditor.View, alias });
#else
                return Ok(new { config = valueEditorConfig, view = valueEditor.View, alias });
#endif
            }

#if NET472
            return Request.CreateResponse(HttpStatusCode.NotFound);
#else
            return NotFound();
#endif
        }

        [HttpPost]
#if NET472
        public HttpResponseMessage GetDataSourceItems([FromBody] JObject data)
#else
        public IActionResult GetDataSourceItems([FromBody] JObject data)
#endif
        {
            if (_propertyEditors.TryGet(DataListDataEditor.DataEditorAlias, out var propertyEditor) == true)
            {
                var config = data.ToObject<Dictionary<string, object>>();
                var configurationEditor = propertyEditor.GetConfigurationEditor();
                var valueEditorConfig = configurationEditor.ToValueEditor(config);

#if NET472
                return Request.CreateResponse(HttpStatusCode.OK, new { config = valueEditorConfig });
#else
                return Ok(new { config = valueEditorConfig });
#endif
            }

#if NET472
            return Request.CreateResponse(HttpStatusCode.NotFound);
#else
            return NotFound();
#endif
        }

        [HttpGet, HttpPost]
#if NET472
        public HttpResponseMessage GetDataSourceItemsByDataTypeKey(Guid? key)
#else
        public IActionResult GetDataSourceItemsByDataTypeKey(Guid? key)
#endif
        {
            if (key.HasValue == true &&
                _dataTypeService.GetDataType(key.Value) is IDataType dataType &&
                dataType?.EditorAlias.InvariantEquals(DataListDataEditor.DataEditorAlias) == true &&
                dataType.Configuration is Dictionary<string, object> config &&
                _propertyEditors.TryGet(dataType.EditorAlias, out var propertyEditor) == true)
            {
                var configurationEditor = propertyEditor.GetConfigurationEditor();
                var valueEditorConfig = configurationEditor.ToValueEditor(config);

#if NET472
                return Request.CreateResponse(HttpStatusCode.OK, valueEditorConfig?["items"]);
#else
                return Ok(valueEditorConfig?["items"]);
#endif
            }

#if NET472
            return Request.CreateResponse(HttpStatusCode.NotFound);
#else
            return NotFound();
#endif
        }
    }
}
