using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace H2OpticaLogic
{
    public class Sensor
    {
        public int SensorID { get; set; }
        public string SensorName { get; set; }
        public double? SensorLimit { get; set; }

        public Sensor()
        {
            SensorID = 0;
            SensorName = "Nuovo Sensore";
            SensorLimit = null;
        }
        public Sensor(int sensorID, string sensorName, double? sensorLimit)
        {
            SensorID = sensorID;
            SensorName = sensorName;
            SensorLimit = sensorLimit;
        }
        public Sensor(int sensorID, double? sensorLimit)
        {
            SensorID = sensorID;
            SensorName = "Nuovo Sensore";
            SensorLimit = sensorLimit;
        }
        public Sensor(int sensorID, string sensorName)
        {
            SensorID = sensorID;
            SensorName = sensorName;
            SensorLimit = null;
        }

        public override bool Equals(object obj)
        {
            if (obj is Sensor sens)
            {
                return this.SensorID == sens.SensorID && this.SensorName == sens.SensorName && this.SensorLimit == sens.SensorLimit;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hashSensorID = SensorID.GetHashCode();
            int hashSensorName = SensorName?.GetHashCode() ?? 0;
            int hashSensorLimit = SensorLimit?.GetHashCode() ?? 0;

            return hashSensorID ^ hashSensorName ^ hashSensorLimit;
        }
    }

    public class DataCollection
    {
        public WaterStats Stats { get; set; }

        public FlowCollection Flows { get; set; }

        public override bool Equals(object obj)
        {
            if(obj == null || this.GetType() != obj.GetType())
                return false;

            DataCollection other = obj as DataCollection;
            return Stats.Equals(other.Stats) && Flows.Equals(other.Flows);
        }
        public override int GetHashCode()
        {
            int hash = 17;

            // Calcola l'hash code per 'Stats' (WaterStats)
            hash = hash * 23 + (Stats != null ? Stats.GetHashCode() : 0);

            // Calcola l'hash code per 'Flows' (FlowCollection)
            hash = hash * 23 + (Flows != null ? Flows.GetHashCode() : 0);

            return hash;
        }
    }

    public class FlowCollection
    {
        [JsonExtensionData]
        public Dictionary<string, JToken> FluxRaw { get; set; }
        public Dictionary<int, double> Flux { get; set; } = new Dictionary<int, double>();

        public double CalculateTotalVolume()
        {
            return this.Flux.Values.Sum();
        }

        public double GetLatestVolume(int sensorId)
        {
            return Flux[sensorId];
        }
        public void ResetDailyData()
        {
            List<int> keys = Flux.Keys.ToList();

            foreach (int key in keys)
            {
                Flux[key] = 0.0;
            }
        }

        //Processo i dati raw
        public void ProcessFlux()
        {
            Flux.Clear();

            foreach (KeyValuePair<string,JToken> entry in FluxRaw)
            {
                if (entry.Key.StartsWith("Flux") && int.TryParse(entry.Key.Substring(4), out int id))
                {
                    Flux[id] = (double)entry.Value;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if(obj == null || this.GetType() != obj.GetType()) 
                return false;

            FlowCollection other = obj as FlowCollection;

            return Flux.SequenceEqual(other.Flux);
        }
        public override int GetHashCode()
        {
            return Flux != null ? Flux.GetHashCode() : 0;
        }
    }

    public class WaterStats
    {
        [JsonProperty("pH")]
        public double? pH { get; set; }

        [JsonProperty("Temp")]
        public double? Temp { get; set; }

        public WaterStats()
        {
            pH = 7.0;
            Temp = 0.0;
        }

        public override bool Equals(object obj)
        {
            if(obj == null || this.GetType() != obj.GetType())
                return false;

            WaterStats other = obj as WaterStats;

            return this.pH == other.pH && this.Temp == other.Temp;
        }
        public override int GetHashCode()
        {
            // Calcola un hash code manualmente
            int hash = 17;
            hash = hash * 23 + (pH.HasValue ? pH.Value.GetHashCode() : 0); // Gestisce i nulli
            hash = hash * 23 + (Temp.HasValue ? Temp.Value.GetHashCode() : 0); // Gestisce i nulli
            return hash;
        }
    }
}
