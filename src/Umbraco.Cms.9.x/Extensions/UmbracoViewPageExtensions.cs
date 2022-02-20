using Microsoft.AspNetCore.Html;
using Umbraco.Cms.Web.Common.Views;

namespace Umbraco.Extensions
{
    public static class UmbracoViewPageExtensions
    {
        public static IHtmlContent RenderSection(this UmbracoViewPage viewPage, string name, IHtmlContent defaultContents)
        {
            return viewPage.IsSectionDefined(name) == true
                 ? viewPage.RenderSection(name)
                 : defaultContents;
        }
    }
}
