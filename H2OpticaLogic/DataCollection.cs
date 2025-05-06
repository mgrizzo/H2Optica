using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace H2OpticaLogic
{
    public class DataCollection
    {
        public readonly DateTime timestamp;

        [JsonProperty("pH")]
        public double? pH {  get; set; }

        [JsonProperty("Temp")]
        public double? Temp { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JToken> FluxRaw { get; set; }
        public Dictionary<int, double> Flux { get; set; } = new Dictionary<int, double>();

        //Processo i dati raw
        public void ProcessFlux()
        {
            Flux.Clear();

            foreach (var entry in FluxRaw)
            {
                if (entry.Key.StartsWith("Flux") && int.TryParse(entry.Key.Substring(4), out int id))
                {
                    Flux[id] = (double)entry.Value;
                }
            }
        }
    }
}
