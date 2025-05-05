using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H2OpticaLogic
{
    public class DataCollection
    {
        public DateTime timestamp { get; }

        public double? temp {  get; set; }
        public double? pH { get; set; }

        public List<Sensor> activeFlowSensors { get; set; } = new List<Sensor>();
        public Dictionary<int, double?> flows { get; set; } = new Dictionary<int, double?>();

        public DataCollection(DateTime dt, double? temperature, double? ph)
        {
            timestamp = dt;

            if(temperature >= 0 && temperature < 200)
                temp = temperature;

            if(ph >= 0 && ph < 15)
                pH = ph;
        }
    }
}
