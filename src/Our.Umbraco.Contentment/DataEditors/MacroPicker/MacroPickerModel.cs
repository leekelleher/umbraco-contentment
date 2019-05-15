using System.Collections.Generic;
using Newtonsoft.Json;

namespace Our.Umbraco.Contentment.DataEditors
{
    internal class MacroPickerModel
    {
        [JsonProperty("udi")]
        public string Udi { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("params")]
        public Dictionary<string, object> Parameters { get; set; }
    }
}
