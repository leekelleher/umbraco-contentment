#if NET472
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
#else
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.Services.Implement
{
    public class ContentmentContextAccessor : IContentmentContextAccessor
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

#if NET472
        public ContentmentContextAccessor(
            IUmbracoContextAccessor umbracoContextAccessor)
        {
            _umbracoContextAccessor = umbracoContextAccessor;

        }
#else
        private readonly IRequestAccessor _requestAccessor;

        public ContentmentContextAccessor(
            IRequestAccessor requestAccessor,
            IUmbracoContextAccessor umbracoContextAccessor)
        {
            _requestAccessor = requestAccessor;
            _umbracoContextAccessor = umbracoContextAccessor;

        }
#endif


        public int? GetCurrentContentId(out bool isParent)
        {
            var umbracoContext = _umbracoContextAccessor.GetRequiredUmbracoContext();
            isParent = false;

            // NOTE: First we check for "id" (if on a content page), then "parentId" (if editing an element).
#if NET472
            if (int.TryParse(umbracoContext.HttpContext.Request.QueryString.Get("id"), out var currentId) == true)
#else
                if (int.TryParse(_requestAccessor.GetQueryStringValue("id"), out var currentId) == true)
#endif
            {
                return currentId;
            }
#if NET472
            else if (int.TryParse(umbracoContext.HttpContext.Request.QueryString.Get("parentId"), out var parentId) == true)
#else
                else if (int.TryParse(_requestAccessor.GetQueryStringValue("parentId"), out var parentId) == true)
#endif
            {
                isParent = true;
                return parentId;
            }

            return default;

        }

        public IPublishedContent GetCurrentContent(out bool isParent)
        {
            var currentContentId = GetCurrentContentId(out isParent);

            if (currentContentId.HasValue == false)
            {
                return null;
            }
            
            var umbracoContext = _umbracoContextAccessor.GetRequiredUmbracoContext();
            
            if (umbracoContext == null)
            {
                return null;
            }

            return umbracoContext.Content.GetById(true, currentContentId.Value);
        }

    }
}
