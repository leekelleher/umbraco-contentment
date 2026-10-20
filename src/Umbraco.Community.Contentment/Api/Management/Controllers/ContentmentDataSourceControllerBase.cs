// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Api.Management;

// Base controller for the endpoints that evaluate a data source, (e.g. SQL queries, file paths, URLs).
public abstract class ContentmentDataSourceControllerBase : ContentmentControllerBase
{
    private readonly IAuthorizationService _authorizationService;

    protected ContentmentDataSourceControllerBase(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    // Data source configuration is executed on the server.
    // Only users who are able to edit Data Types can supply their own configuration, e.g. the Data Type preview.
    // Everyone else must refer to a saved Data Type, so that its configuration is read from the server.
    protected async Task<bool> CanSupplyConfigurationAsync()
    {
        var result = await _authorizationService.AuthorizeAsync(User, AuthorizationPolicies.TreeAccessDataTypes);
        return result.Succeeded;
    }

    // Reads the first item of a Contentment configuration editor, (e.g. `dataSource`, `listEditor`) from a saved Data Type.
    protected static ConfigurationEditorItemRequestModel? GetConfigurationEditorItem(IJsonSerializer jsonSerializer, IDataType dataType, string alias)
    {
        if (dataType.ConfigurationData is Dictionary<string, object> configuration &&
            configuration.TryGetValue(alias, out var tmp) == true &&
            tmp is JsonArray array &&
            array.Count > 0 &&
            array[0] is JsonObject item)
        {
            return new ConfigurationEditorItemRequestModel
            {
                Key = item.GetValueAsString("key"),
                Value = jsonSerializer.Deserialize<Dictionary<string, object>>(item["value"]?.ToJsonString() ?? "{}") ?? [],
            };
        }

        return null;
    }

    // Set the current content values, so that `IContentmentContentContext` can access them.
    protected new void SetCurrentContentContextValues(
        string? contentId = null,
        string? parentId = null,
        string? variantId = null,
        string? propertyAlias = null,
        bool? isNew = false,
        Guid? contentTypeKey = null)
        => SetHttpContextItems(HttpContext, contentId, parentId, variantId, propertyAlias, isNew, contentTypeKey);

    // NOTE: Shared with the obsolete `ContentmentControllerBase.SetCurrentContentContextValues`, remove once that is removed in Contentment 8.0.
    internal static void SetHttpContextItems(
        HttpContext httpContext,
        string? contentId,
        string? parentId,
        string? variantId,
        string? propertyAlias,
        bool? isNew,
        Guid? contentTypeKey)
    {
        if (string.IsNullOrWhiteSpace(propertyAlias) == false)
        {
            httpContext.Items.Add("contentmentContextCurrentPropertyAlias", propertyAlias);
        }

        if (string.IsNullOrWhiteSpace(contentId) == false)
        {
            httpContext.Items.Add("contentmentContextCurrentContentId", contentId);
        }

        if (string.IsNullOrWhiteSpace(variantId) == false)
        {
            httpContext.Items.Add("contentmentContextCurrentContentVariantId", variantId);
        }

        if (string.IsNullOrWhiteSpace(parentId) == false)
        {
            httpContext.Items.Add("contentmentContextCurrentParentId", parentId);
        }

        if (contentTypeKey.HasValue == true && contentTypeKey.Value != Guid.Empty)
        {
            httpContext.Items.Add("contentmentContextCurrentContentTypeKey", contentTypeKey.Value);
        }

        httpContext.Items.Add("contentmentContextCurrentIsNew", isNew ?? false);
    }
}
