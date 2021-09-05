using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace API.Models {
    public class ChessStats {
        [JsonProperty("Bullet", NullValueHandling = NullValueHandling.Ignore)] // don't return null values
        public Dictionary<string, ResultStats> Bullet { get; set; }
        
        [JsonProperty("Blitz", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, ResultStats> Blitz { get; set; }
        
        [JsonProperty("Rapid", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, ResultStats> Rapid { get; set; }
        
        [JsonProperty("Daily", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, ResultStats> Daily { get; set; }

        public ChessStats() {
            Bullet = new Dictionary<string, ResultStats>();
            Blitz = new Dictionary<string, ResultStats>();
            Rapid = new Dictionary<string, ResultStats>();
            Daily = new Dictionary<string, ResultStats>();
        }
    }
}