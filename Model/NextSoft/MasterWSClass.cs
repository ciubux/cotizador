using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Model.NextSoft
{
    public class MasterWSClass 
    {
        public string urlApi { get; set; }
        public string apiToken { get; set; }

        public string urlApiWeb { get; set; }
        public string apiWebToken { get; set; }
        public string apiWebRUC { get; set; }

        protected async Task<object> callService(object sendData, string  nombreServicio) {

            var httpClient = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(sendData), Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(this.fullUrl(nombreServicio), content);
            string resultContent = await result.Content.ReadAsStringAsync();

            object dataResult = JsonConvert.DeserializeObject(resultContent);
            return dataResult;
        }

        protected async Task<object> callServiceWeb(object sendData, string nombreServicio)
        {

            var httpClient = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(sendData), Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(this.fullUrlWeb(nombreServicio), content);
            string resultContent = await result.Content.ReadAsStringAsync();

            object dataResult = JsonConvert.DeserializeObject(resultContent);
            return dataResult;
        }

        protected string fullUrl(string urlService)
        {
            return urlApi + urlService;
        }
        protected string fullUrlWeb(string urlService)
        {
            return urlApiWeb + urlService;
        }
    }
}
