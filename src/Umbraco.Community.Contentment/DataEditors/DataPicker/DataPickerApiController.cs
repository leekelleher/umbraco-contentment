/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
#if NET472
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
#else
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    [PluginController(Constants.Internals.PluginControllerName), IsBackOffice]
    public sealed class DataPickerApiController : UmbracoAuthorizedJsonController
    {
        private readonly IDataTypeService _dataTypeService;
        private readonly ConfigurationEditorUtility _utility;

        internal static readonly Dictionary<Guid, (IDataPickerSource, Dictionary<string, object>)> _lookup = new Dictionary<Guid, (IDataPickerSource, Dictionary<string, object>)>();

        public DataPickerApiController(
            IDataTypeService dataTypeService,
            ConfigurationEditorUtility utility)
        {
            _dataTypeService = dataTypeService;
            _utility = utility;
        }

        [HttpPost]
#if NET472
        public async Task<HttpResponseMessage> GetItems(Guid dataTypeKey, [FromBody] string[] values)
#else
        public async Task<IActionResult> GetItems(Guid dataTypeKey, [FromBody] string[] values)
#endif
        {
            if (_lookup.TryGetValue(dataTypeKey, out var cached) == true)
            {
                var result = (await cached.Item1.GetItemsAsync(cached.Item2, values)).DistinctBy(x => x.Value).ToDictionary(x => x.Value);
#if NET472
                return Request.CreateResponse(HttpStatusCode.OK, result);
#else
                return Ok(result);
#endif
            }
            else if (_dataTypeService.GetDataType(dataTypeKey) is IDataType dataType &&
                dataType?.EditorAlias.InvariantEquals(DataPickerDataEditor.DataEditorAlias) == true &&
                dataType.Configuration is Dictionary<string, object> dataTypeConfig &&
                dataTypeConfig.TryGetValue(DataPickerConfigurationEditor.DataSource, out var tmp1) == true &&
                tmp1 is JArray array1 &&
                array1.Count > 0 &&
                array1[0] is JObject item1)
            {
                var source1 = _utility.GetConfigurationEditor<IDataPickerSource>(item1.Value<string>("key"));
                if (source1 != null)
                {
#if NET472
                    var config1 = item1?["value"]?.ToObject<Dictionary<string, object>>();
#else
                    var config1 = item1?["value"]?.ToObject<Dictionary<string, object>>()!;
#endif

                    _lookup.TryAdd(dataTypeKey, (source1, config1));
                    var result = (await source1.GetItemsAsync(config1, values)).DistinctBy(x => x.Value).ToDictionary(x => x.Value);
#if NET472
                    return Request.CreateResponse(HttpStatusCode.OK, result);
#else
                    return Ok(result);
#endif
                }
            }

#if NET472
            return Request.CreateResponse(HttpStatusCode.NotFound, $"Unable to locate data source for data type: '{dataTypeKey}'");
#else
            return NotFound($"Unable to locate data source for data type: '{dataTypeKey}'");
#endif
        }

        [HttpGet]
#if NET472
        public async Task<HttpResponseMessage> Search(Guid dataTypeKey, int pageNumber = 1, int pageSize = 12, string query = "")
#else
        public async Task<IActionResult> Search(Guid dataTypeKey, int pageNumber = 1, int pageSize = 12, string query = "")
#endif
        {
            var totalPages = -1;

            if (_lookup.TryGetValue(dataTypeKey, out var cached) == true)
            {
                var results = await cached.Item1.SearchAsync(cached.Item2, pageNumber, pageSize, HttpUtility.UrlDecode(query));

#if NET472
                return Request.CreateResponse(HttpStatusCode.OK, results);
#else
                return Ok(results);
#endif
            }
            else if (_dataTypeService.GetDataType(dataTypeKey) is IDataType dataType &&
                dataType?.EditorAlias.InvariantEquals(DataPickerDataEditor.DataEditorAlias) == true &&
                dataType.Configuration is Dictionary<string, object> dataTypeConfig &&
                dataTypeConfig.TryGetValue(DataPickerConfigurationEditor.DataSource, out var tmp1) == true &&
                tmp1 is JArray array1 &&
                array1.Count > 0 &&
                array1[0] is JObject item1)
            {
                var source1 = _utility.GetConfigurationEditor<IDataPickerSource>(item1.Value<string>("key"));
                if (source1 != null)
                {
#if NET472
                    var config1 = item1?["value"]?.ToObject<Dictionary<string, object>>();
#else
                    var config1 = item1?["value"]?.ToObject<Dictionary<string, object>>()!;
#endif

                    _lookup.TryAdd(dataTypeKey, (source1, config1));

                    var results = await source1?.SearchAsync(config1, pageNumber, pageSize, HttpUtility.UrlDecode(query));
#if NET472
                    return Request.CreateResponse(HttpStatusCode.OK, results);
#else
                    return Ok(results);
#endif
                }
            }

#if NET472
            return Request.CreateResponse(HttpStatusCode.NotFound, $"Unable to locate data source for data type: '{dataTypeKey}'");
#else
            return NotFound($"Unable to locate data source for data type: '{dataTypeKey}'");
#endif
        }

        // NOTE: The internal cache is cleared from `ContentmentDataTypeNotificationHandler` [LK]
        internal static void ClearCache(Guid dataTypeKey)
        {
            _lookup.Remove(dataTypeKey);
        }
    }
}
