
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using Model.NextSoft;
using System.IO;
using Model.UTILES;
using NPOI.SS.UserModel;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BusinessLayer
{
    public class NextSoftBL
    {
        public async Task<ServiceResponse> validarProductos(List<String> skus, List<int> factores)
        {
            ServiceResponse res = new ServiceResponse();

            ProductoWS ws = new ProductoWS();
            ws.urlApi = Constantes.NEXTSOFT_API_URL;
            ws.apiToken = Constantes.NEXTSOFT_API_TOKEN;

            object result = await ws.validarProductoTecnicaLista(ConverterMPToNextSoft.toProductoValidarList(skus, factores));

            
            JObject dataResult = (JObject)result;
            res.code = dataResult["validarproductosResult"]["codigo"].Value<int>();
            res.message = "";

            JArray productos = (JArray)dataResult["validarproductosResult"]["Productos"];

            // Recorrer cada objeto del arreglo
            foreach (JObject producto in productos)
            {
                string codigoMP = producto["CodigoMP"].ToString();
                bool existeFactor = producto["ExisteFactor"].Value<bool>();
                bool existeProducto = producto["ExisteProducto"].Value<bool>();
                int factorMP = producto["FactorMP"].Value<int>();

                if (!existeFactor || !existeProducto)
                {
                    res.message = res.message + "El producto " + codigoMP + " no esta correctamente registrado. ";
                    res.code = 1;
                }
            }

            //res.code = 0;
            //res.message = "OK";
            return res;
        }
    }
}
