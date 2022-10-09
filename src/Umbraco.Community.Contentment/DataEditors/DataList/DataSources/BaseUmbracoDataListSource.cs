using System.Collections.Generic;

#if NET472
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;
#else
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
#endif

namespace Umbraco.Community.Contentment.DataEditors
{
    public abstract class BaseUmbracoContentDataListSource : IDataListSource
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

#if NET472
        public BaseUmbracoContentDataListSource(
            IUmbracoContextAccessor umbracoContextAccessor)
        {
            _umbracoContextAccessor = umbracoContextAccessor;

        }
#else
        private readonly IRequestAccessor _requestAccessor;

        public BaseUmbracoContentDataListSource(
            IRequestAccessor requestAccessor,
            IUmbracoContextAccessor umbracoContextAccessor)
        {
            _requestAccessor = requestAccessor;
            _umbracoContextAccessor = umbracoContextAccessor;
                   
        }
#endif


        protected int? ContextContentId
        {
            get
            {
                var umbracoContext = _umbracoContextAccessor.GetRequiredUmbracoContext();
                isContextContentParent = false;

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
                    isContextContentParent = true;
                    return parentId;
                }

                return default(int?);
            }
        }

        protected IPublishedContent GetContextContent()
        {
            var umbracoContext = _umbracoContextAccessor.GetRequiredUmbracoContext();
            if (ContextContentId.HasValue == false || umbracoContext == null)
            {
                return null;
            }

            return umbracoContext.Content.GetById(true, ContextContentId.Value);
        }

        private bool isContextContentParent;
        protected bool IsContextContentParent
        {
            get
            {
                return ContextContentId.HasValue && isContextContentParent;
            }
        }


        public abstract Dictionary<string, object> DefaultValues { get; }

        public abstract IEnumerable<ConfigurationField> Fields { get; }

        public virtual string Group { get; } = Constants.Conventions.DataSourceGroups.Umbraco;

        public abstract OverlaySize OverlaySize { get; }

        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract string Icon { get; }

        public abstract IEnumerable<DataListItem> GetItems(Dictionary<string, object> config);
    }
}
