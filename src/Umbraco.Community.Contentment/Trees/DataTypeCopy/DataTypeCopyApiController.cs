/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using Umbraco.Core.Services;
using Umbraco.Web.Editors;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Umbraco.Community.Contentment.Trees
{
    [PluginController(Constants.Internals.PluginControllerName), IsBackOffice]
    public sealed class DataTypeCopyApiController : UmbracoAuthorizedJsonController
    {
        public HttpResponseMessage Copy(MoveOrCopy copy)
        {
            var result = Services.DataTypeService.Copy(copy.Id, copy.ParentId);

            if (result.Success)
            {
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result.Result, Encoding.UTF8, MediaTypeNames.Text.Plain);
                return response;
            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}
