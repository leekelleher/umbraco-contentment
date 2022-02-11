/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
#if NET472
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
#else
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;
#endif

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
#if NET472
        public HttpResponseMessage GetPreview([FromBody] JObject data)
#else
        public ActionResult GetPreview([FromBody] JObject data)
#endif
        {
            if (_propertyEditors.TryGet(DataListDataEditor.DataEditorAlias, out var propertyEditor) == true)
            {
                var config = data.ToObject<Dictionary<string, object>>();
                var alias = config.GetValueAs("alias", "preview");
                var configurationEditor = propertyEditor.GetConfigurationEditor();
                var valueEditorConfig = configurationEditor.ToValueEditor(config);
                var valueEditor = propertyEditor.GetValueEditor(config);

#if NET472
                return Request.CreateResponse(HttpStatusCode.OK, new { config = valueEditorConfig, view = valueEditor.View, alias });
#else
                return new ObjectResult(new { config = valueEditorConfig, view = valueEditor.View, alias });
#endif
            }

#if NET472
            return Request.CreateResponse(HttpStatusCode.NotFound);
#else
            return new NotFoundResult();
#endif
        }
    }
}
