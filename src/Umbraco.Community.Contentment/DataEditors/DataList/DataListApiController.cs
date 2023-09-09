/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;

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
        public IActionResult GetPreview([FromBody] JObject data)
        {
            if (_propertyEditors.TryGet(DataListDataEditor.DataEditorAlias, out var propertyEditor) == true)
            {
                var config = data.ToObject<Dictionary<string, object>>();
                var alias = config?.GetValueAs("alias", defaultValue: "preview") ?? "preview";
                var configurationEditor = propertyEditor.GetConfigurationEditor();
                var valueEditorConfig = configurationEditor.ToValueEditor(config);
                var valueEditor = propertyEditor.GetValueEditor(config);

                return Ok(new { config = valueEditorConfig, view = valueEditor.View, alias });
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult GetDataSourceItems([FromBody] JObject data)
        {
            if (_propertyEditors.TryGet(DataListDataEditor.DataEditorAlias, out var propertyEditor) == true)
            {
                var config = data.ToObject<Dictionary<string, object>>();
                var configurationEditor = propertyEditor.GetConfigurationEditor();
                var valueEditorConfig = configurationEditor.ToValueEditor(config);

                return Ok(new { config = valueEditorConfig });
            }

            return NotFound();
        }

        [HttpGet, HttpPost]
        public IActionResult GetDataSourceItemsByDataTypeKey(Guid? key)
        {
            if (key.HasValue == true &&
                _dataTypeService.GetDataType(key.Value) is IDataType dataType &&
                dataType?.EditorAlias.InvariantEquals(DataListDataEditor.DataEditorAlias) == true &&
#if NET8_0_OR_GREATER
                dataType.ConfigurationObject is Dictionary<string, object> config &&
#else
                dataType.Configuration is Dictionary<string, object> config &&
#endif
                _propertyEditors.TryGet(dataType.EditorAlias, out var propertyEditor) == true)
            {
                var configurationEditor = propertyEditor.GetConfigurationEditor();
                var valueEditorConfig = configurationEditor.ToValueEditor(config);

                return Ok(valueEditorConfig?["items"]);
            }

            return NotFound();
        }
    }
}
