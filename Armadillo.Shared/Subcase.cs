using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Armadillo.Shared
{
    public class Subcase
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("customer")]
        public string Customer { get; set; }
        
        [JsonProperty("owner")]
        public string Owner { get; set; }
        
        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("level")]
        public string Level { get; set; }
        
        [JsonProperty("loaded")]
        public DateTime Loaded { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }
        
        [JsonProperty("lastUpdate")]
        public DateTime LastUpdate { get; set; }                
    }
}
