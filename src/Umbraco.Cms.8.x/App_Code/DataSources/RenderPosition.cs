using Umbraco.Community.Contentment.DataEditors;

namespace Umbraco.Web.PublishedModels
{
    public enum RenderPosition
    {
        [DataListItem(Name = "Head", Description = "Inside the &lt;head&gt; tags")]
        Head = 2,

        [DataListItem(Name = "Body (start)", Description = "After the opening &lt;body&gt; tag")]
        BodyOpen = 1,

        [DataListItem(Name = "Body (end)", Description = "Before the closing &lt;/body&gt; tag")]
        BodyClose = 0
    }
}
