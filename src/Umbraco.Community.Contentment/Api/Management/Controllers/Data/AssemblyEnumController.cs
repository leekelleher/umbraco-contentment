// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using System.Reflection;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Core.Strings;
using Umbraco.Community.Contentment;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Extensions;

namespace Umbraco.Cms.Api.Management.Controllers.Contentment;

[ApiVersion("1.0")]
[VersionedApiBackOfficeRoute($"{Constants.Internals.ProjectAlias}/data")]
public sealed class AssemblyEnumController : ContentmentControllerBase
{
    internal const string GetAssembliesUrl = "/umbraco/management/api/v1/contentment/data/assemblies";
    internal const string GetEnumsUrl = "/umbraco/management/api/v1/contentment/data/enums?assembly={0}";

    private readonly IShortStringHelper _shortStringHelper;

    public AssemblyEnumController(IShortStringHelper shortStringHelper)
    {
        _shortStringHelper = shortStringHelper;
    }

    [HttpGet("assemblies")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(IEnumerable<DataListItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAssemblies()
    {
        // NOTE: A placeholder async task, until I get async working throughout the codebase. ¯\_(ツ)_/¯
        await Task.Run(() => { });

        var options = new SortedSet<string>(StringComparer.InvariantCultureIgnoreCase);

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        if (assemblies?.Length > 0)
        {
            foreach (var assembly in assemblies)
            {
                var assemblyName = assembly.GetName();
                if (string.IsNullOrWhiteSpace(assemblyName.Name) == true ||
                    options.Contains(assemblyName.Name) == true ||
                    assembly.IsDynamic == true)
                {
                    continue;
                }

                var hasEnums = false;
                try
                {
                    var exportedTypes = assembly.GetExportedTypes();
                    if (exportedTypes != null)
                    {
                        foreach (var exportedType in exportedTypes)
                        {
                            if (exportedType.IsEnum == true)
                            {
                                hasEnums = true;
                                break;
                            }
                        }
                    }
                }
                catch (FileLoadException) { /* (╯°□°）╯︵ ┻━┻ */ }
                catch (TypeLoadException) { /* ¯\_(ツ)_/¯ */ }

                if (hasEnums == false)
                {
                    continue;
                }

                _ = options.Add(assemblyName.Name);
            }
        }

        var result = options.Select(x => new DataListItem { Name = x, Value = x });

        return Ok(result);
    }

    [HttpGet("enums")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(IEnumerable<DataListItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEnums(string assembly)
    {
        // NOTE: A placeholder async task, until I get async working throughout the codebase. ¯\_(ツ)_/¯
        await Task.Run(() => { });

        var options = new SortedDictionary<string, DataListItem>();

        var types = Assembly.Load(assembly).GetTypes();

        foreach (var type in types)
        {
            if (type.IsEnum == false || string.IsNullOrWhiteSpace(type.FullName) == true)
            {
                continue;
            }

            options.Add(type.FullName, new DataListItem
            {
                Name = type.Name.SplitPascalCasing(_shortStringHelper),
                Value = type.FullName,
                Description = type.FullName,
            });
        }

        var result = options.Values;

        return Ok(result);
    }
}
