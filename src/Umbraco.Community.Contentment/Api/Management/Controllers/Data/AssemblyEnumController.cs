// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using System.Reflection;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Strings;
using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.Api.Management;

[ApiVersion("1.0")]
[ContentmentVersionedApiBackOfficeRoute("data")]
public sealed class AssemblyEnumController : ContentmentControllerBase
{
    internal const string GetAssembliesUrl = "/umbraco/management/api/v1/contentment/data/assemblies";
    internal const string GetEnumsUrl = "/umbraco/management/api/v1/contentment/data/enums?assembly={0}";

    private readonly IShortStringHelper _shortStringHelper;

    public AssemblyEnumController(IShortStringHelper shortStringHelper)
    {
        _shortStringHelper = shortStringHelper;
    }

    [HttpGet("assemblies", Name = "GetAssembliesData")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetAssemblies()
    {
        // NOTE: A placeholder async task, until I get async working throughout the codebase. ¯\_(ツ)_/¯
        await Task.Run(() => { });

        var options = GetAssemblyNames();

        var result = options.Select(x => new DataListItem { Name = x, Value = x });

        return Ok(result);
    }

    [HttpGet("enums", Name = "GetEnumsData")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetEnums(string assembly)
    {
        // NOTE: A placeholder async task, until I get async working throughout the codebase. ¯\_(ツ)_/¯
        await Task.Run(() => { });

        // NOTE: Only assemblies that are offered by the `GetAssemblies` endpoint can be loaded.
        if (string.IsNullOrWhiteSpace(assembly) == true || GetAssemblyNames().Contains(assembly) == false)
        {
            return BadRequest("Unable to find the assembly.");
        }

        var options = new SortedDictionary<string, DataListItem>();

        Type[] types;

        try
        {
            types = Assembly.Load(assembly).GetTypes();
        }
        catch (Exception ex) when (ex is FileLoadException or FileNotFoundException or BadImageFormatException or ReflectionTypeLoadException)
        {
            return BadRequest("Unable to load the assembly.");
        }

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

    private static SortedSet<string> GetAssemblyNames()
    {
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
                catch (FileNotFoundException) { /* ┌( ಠ_ಠ)┘ */ }
                catch (TypeLoadException) { /* ¯\_(ツ)_/¯ */ }

                if (hasEnums == false)
                {
                    continue;
                }

                _ = options.Add(assemblyName.Name);
            }
        }

        return options;
    }
}
