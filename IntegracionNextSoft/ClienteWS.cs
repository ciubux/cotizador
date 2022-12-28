using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IntegracionNextSoft
{
    public class ClienteWS : MasterClass
    {
        public async Task<String> crearCliente(object dataCliente)
        {
            string nombreServicio = "crearcliente";

            var sendData = new { 
                token = this.apiToken,
                cliente = dataCliente
            };

            string resultContent = await this.callService(sendData, nombreServicio);

            return resultContent;
        }

        
    }
}
