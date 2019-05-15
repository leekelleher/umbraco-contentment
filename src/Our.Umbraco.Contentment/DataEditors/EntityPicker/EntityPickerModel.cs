using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Web.Models.ContentEditing;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class EntityPickerModel
    {
        [JsonProperty(Constants.Conventions.ConfigurationEditors.EntityType)]
        public UmbracoEntityTypes EntityType { get; set; }

        [JsonProperty(Constants.Conventions.ConfigurationEditors.Items)]
        public GuidUdi[] Items { get; set; }
    }
}
