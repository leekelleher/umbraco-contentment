// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
#if NET10_0_OR_GREATER
using Microsoft.OpenApi;
#else
using Microsoft.OpenApi.Models;
#endif
using Swashbuckle.AspNetCore.SwaggerGen;
using Umbraco.Cms.Api.Management.OpenApi;

namespace Umbraco.Community.Contentment.Api.Management;

internal class ConfigureContentmentSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.SwaggerDoc(
            Constants.Internals.ProjectAlias,
            new OpenApiInfo
            {
                Title = $"{Constants.Internals.ProjectName} Management API",
                Version = "Latest",
            }
        );

        options.OperationFilter<ContentmentApiOperationSecurityFilter>();
    }
}

internal class ContentmentApiOperationSecurityFilter : BackOfficeSecurityRequirementsOperationFilterBase
{
    protected override string ApiName => Constants.Internals.ProjectAlias;
}
