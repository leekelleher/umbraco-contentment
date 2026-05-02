using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade.V_15_0_0.LocalLinks;

namespace Umbraco.Community.Contentment.Migrations.Upgrade.V_6_1_3;

[Obsolete("To be removed in Contentment 7.0, as `ITypedLocalLinkProcessor` will be removed in Umbraco 18.0.")]
public sealed class ConvertLocalLinkComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder
            .Services
                .AddSingleton<ITypedLocalLinkProcessor, ContentBlocksLocalLinkProcessor>()
         ;
    }
}
