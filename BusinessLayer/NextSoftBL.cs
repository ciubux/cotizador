
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
            int validados = 0;
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
                } else
                {
                    validados++;
                }
            }

            if (validados < skus.Count)
            {
                res.message = res.message + "Hay productos que NextSys no ha indicado el estado de la homologación. ";
                res.code = 1;
            }

            //res.code = 0;
            //res.message = "OK";
            return res;
        }

        public async Task<List<List<string>>> productosHomologados()
        {
            ServiceResponse res = new ServiceResponse();

            ProductoWS ws = new ProductoWS();
            ws.urlApi = Constantes.NEXTSOFT_API_URL;
            ws.apiToken = Constantes.NEXTSOFT_API_TOKEN;

            object result = await ws.productosHomologados();


            JObject dataResult = (JObject)result;
            res.code = dataResult["productosintegracionResult"]["codigo"].Value<int>();
            res.message = dataResult["productosintegracionResult"]["Mensaje"].Value<string>();

            JArray productos = (JArray)dataResult["productosintegracionResult"]["Productos"];

            /* "codigoMP": "HB0Q00",
                "codigoTP": "02.04.03.0002",
                "factorMP": 1,
                "factorTP": 1,
                "formatoMP": "ROLLO",
                "formatoTP": "PAQUETE",
                "productoMP": "Sabanilla Súper Plus blanca 1 ply, rollo x 100 mts",
                "productoTP": "SABANILLA SUPER PLUS BLANCA X 100M X 2 ROLL"
            */
            // Recorrer cada objeto del arreglo
            List<List<string>> lista = new List<List<string>>();
            List<string> item;

            foreach (JObject producto in productos)
            {
                item = new List<string>
                {
                    producto["codigoMP"].ToString(),
                    producto["productoMP"].ToString(),
                     producto["formatoMP"].ToString(),
                    producto["factorMP"].ToString(),
                    producto["codigoTP"].ToString(),
                    producto["productoTP"].ToString(),
                    producto["formatoTP"].ToString(),
                    producto["factorTP"].ToString()
                };

                lista.Add(item);
            }

            //res.code = 0;
            //res.message = "OK";
            return lista;
        }
    }
}
