using System;
using Newtonsoft.Json;

namespace Armadillo.Shared
{
    public class Product
    {
        [JsonProperty("id")]
        public string Id { get; set;}

        [JsonProperty("name")]
        public string Name { get; set;}
        
        [JsonProperty("subcases")]
        public Subcase[] Subcases { get; set; }
        
        [JsonProperty("error")]
        public string Error { get; set; }
        
        [JsonProperty("reportlink")]
        public string ReportLink { get; set; }
        
        [JsonProperty("loaded")]
        public DateTime Loaded
        { 
            get
            {
                return Subcases.Length == 0 ? DateTime.MinValue : Subcases[0].Loaded;
            }
        }


        public override string ToString()
        {
            return String.Format("{0}: {1} subcases", Name, Subcases == null ? 0 : Subcases.Length);
        }
    }
}