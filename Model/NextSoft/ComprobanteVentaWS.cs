using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Model.NextSoft
{
    public class ComprobanteVentaWS : MasterWSClass
    {
        public async Task<object> crearComprobanteVenta(object dataVenta)
        {
            string nombreServicio = "crearcomprobante";

            var sendData = new { 
                token = this.apiToken,
                comprobante = dataVenta
            };

            object resultContent = await this.callService(sendData, nombreServicio);

            return resultContent;
        }

        
    }
}
