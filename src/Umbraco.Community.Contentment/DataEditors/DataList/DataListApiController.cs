/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace Umbraco.Community.Contentment.DataEditors
{
    [PluginController(Constants.Internals.PluginControllerName), IsBackOffice]
    public sealed class DataListApiController : UmbracoAuthorizedJsonController
    {
        private readonly PropertyEditorCollection _propertyEditors;

        public DataListApiController(PropertyEditorCollection propertyEditors)
        {
            _propertyEditors = propertyEditors;
        }

        [HttpPost]
        public ActionResult GetPreview([FromBody] JObject data)
        {
            var config = data.ToObject<Dictionary<string, object>>();

            if (_propertyEditors.TryGet(DataListDataEditor.DataEditorAlias, out var propertyEditor) == true)
            {
                var alias = config.GetValueAs("alias", "preview");
                var configurationEditor = propertyEditor.GetConfigurationEditor();
                var valueEditorConfig = configurationEditor.ToValueEditor(config);
                var valueEditor = propertyEditor.GetValueEditor(config);

                return new ObjectResult(new { config = valueEditorConfig, view = valueEditor.View, alias });
            }

            return new NotFoundResult();
        }
    }
}
