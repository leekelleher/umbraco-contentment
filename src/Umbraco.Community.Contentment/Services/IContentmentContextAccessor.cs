#if NET472
using Umbraco.Core.Models.PublishedContent;
#else
using Umbraco.Cms.Core.Models.PublishedContent;
#endif

namespace Umbraco.Community.Contentment.Services
{
    public interface IContentmentContextAccessor
    {
        /// <summary>
        /// Retrieves the current Umbraco node (or parent node). If you only need the ID, using <cref>GetCurrentContentId</cref> is more efficient.
        /// </summary>
        /// <param name="isParent">Whether the returned IPublishedContent belongs to the parent page.</param>
        /// <returns>Current IPublishedContent.</returns>
        IPublishedContent GetCurrentContent(out bool isParent);

        /// <summary>
        /// Retrieves the ID for the current Umbraco node (or parent node).
        /// </summary>
        /// <param name="isParent">Whether the returned ID belongs to the parent page.</param>
        /// <returns>Current Umbraco node ID.</returns>
        int? GetCurrentContentId(out bool isParent);
    }
}
