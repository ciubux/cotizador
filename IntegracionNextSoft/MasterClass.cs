using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IntegracionNextSoft
{
    public class MasterClass 
    {
        public string urlApi { get; set; }
        public string apiToken { get; set; }

        protected async Task<String> callService(object sendData, string  nombreServicio) {

            var httpClient = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(sendData), Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(this.fullUrl(nombreServicio), content);
            string resultContent = await result.Content.ReadAsStringAsync();

            return resultContent;
        }

        protected string fullUrl(string urlService)
        {
            return urlApi + urlService;
        }
    }
}
