using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Model.NextSoft
{
    public class ProductoWS : MasterWSClass
    {
        public async Task<object> getProducto(string codigoProducto)
        {
            string nombreServicio = "consultarproducto";

            var sendData = new { 
                token = this.apiToken,
                prodCodigo = codigoProducto
            };

            //string resultContent = await this.callService(sendData, nombreServicio);

            object dataResult = await this.callService(sendData, nombreServicio);


            return dataResult;
        }

        public async Task<object> crearProducto(object dataProducto)
        {
            string nombreServicio = "crearproducto";

            var sendData = new
            {
                token = this.apiToken,
                prod = dataProducto
            };

            object resultContent = await this.callService(sendData, nombreServicio);

            return resultContent;
        }

        public async Task<object> crearProductoLista(List<object> dataProductos)
        {
            string nombreServicio = "crearproducto";

            var sendData = new
            {
                token = this.apiToken,
                prods = dataProductos.ToArray()
            };

            object resultContent = await this.callService(sendData, nombreServicio);

            return resultContent;
        }
    }
}
