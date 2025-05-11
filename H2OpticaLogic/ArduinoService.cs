using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;

namespace H2OpticaLogic
{
    public class ArduinoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _arduinoBaseURL;

        public ArduinoService(string hostname)
        {
            _arduinoBaseURL = $"http://{hostname}/";

            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(5)
            };
        }

        //Metodi
        public async Task<DataCollection> GetLatestReadingAsync()
        {
            try
            {
                Logger.Log("Trying to get async json response...");

                HttpResponseMessage response = await _httpClient.GetAsync(_arduinoBaseURL);
                response.EnsureSuccessStatusCode(); //Se ha esito negativo restituisce un'eccezione

                string json = await response.Content.ReadAsStringAsync();
                DataCollection data = JsonConvert.DeserializeObject<DataCollection>(json);
                data.Flows.ProcessFlux();

                return data;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                Logger.Log("Returning 'null'...");
                return null;
            }
        }
    }
}
