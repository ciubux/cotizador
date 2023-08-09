using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Model.NextSoft
{
    public class GuiaWS : MasterWSClass
    {
        public async Task<object> crearGuia(object dataGuia)
        {
            string nombreServicio = "crearguia";

            var sendData = new { 
                token = this.apiToken,
                guia = dataGuia
            };

            object resultContent = await this.callService(sendData, nombreServicio);

            return resultContent;
        }

        public async Task<object> consultarGuia(object sendData)
        {
            string nombreServicio = "ConsultarComprobante";


            object resultContent = await this.callServiceWeb(sendData, nombreServicio);

            return resultContent;
        }

    }
}
