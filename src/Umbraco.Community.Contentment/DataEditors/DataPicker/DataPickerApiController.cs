/* Copyright � 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;

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
        public async Task<IActionResult> GetItems([FromQuery] int id, Guid dataTypeKey, [FromBody] string[] values)
        {
            if (_lookup.TryGetValue(dataTypeKey, out var cached) == true)
            {
                var result = (await cached.Item1.GetItemsAsync(cached.Item2, values)).DistinctBy(x => x.Value).ToDictionary(x => x.Value);

                return Ok(result);
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
                    var config1 = item1?["value"]?.ToObject<Dictionary<string, object>>()!;

                    _lookup.TryAdd(dataTypeKey, (source1, config1));

                    var result = (await source1.GetItemsAsync(config1, values)).DistinctBy(x => x.Value).ToDictionary(x => x.Value);

                    return Ok(result);
                }
            }

            return NotFound($"Unable to locate data source for data type: '{dataTypeKey}'");
        }

        [HttpGet]
        public async Task<IActionResult> Search(int id, Guid dataTypeKey, int pageNumber = 1, int pageSize = 12, string query = "")
        {
            if (_lookup.TryGetValue(dataTypeKey, out var cached) == true)
            {
                var results = await cached.Item1.SearchAsync(cached.Item2, pageNumber, pageSize, HttpUtility.UrlDecode(query));

                return Ok(results);
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
                    var config1 = item1?["value"]?.ToObject<Dictionary<string, object>>()!;

                    _lookup.TryAdd(dataTypeKey, (source1, config1));

                    var results = await source1?.SearchAsync(config1, pageNumber, pageSize, HttpUtility.UrlDecode(query));

                    return Ok(results);
                }
            }

            return NotFound($"Unable to locate data source for data type: '{dataTypeKey}'");
        }

        // NOTE: The internal cache is cleared from `ContentmentDataTypeNotificationHandler` [LK]
        internal static void ClearCache(Guid dataTypeKey)
        {
            _lookup.Remove(dataTypeKey);
        }
    }
}
