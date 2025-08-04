using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Cotizador.Models;
using Model;
using Model.NextSoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cotizador.Controllers
{
    public class DocumentoExternoController : Controller
    {
        public async System.Threading.Tasks.Task<string> DescargarFacturaTC(string serie, string correlativo)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            int success = 0;

            if (!usuario.codigoEmpresa.Equals(Constantes.EMPRESA_CODIGO_TECNICA))
            {
                return "";
            }

            ComprobanteVentaWS wsDoc = new ComprobanteVentaWS();
            wsDoc.urlApi = Constantes.NEXTSOFT_API_URL;
            wsDoc.apiToken = Constantes.NEXTSOFT_API_TOKEN;

            object result = await wsDoc.descargarFacturaTC(serie, correlativo);


            JObject dataResult = (JObject) result;
            int codigo = dataResult["consultacomprobantetpResult"]["Codigo"].Value<int>();

            string resultText = JsonConvert.SerializeObject(result);

            //int codigo = 1; var result = new { codigo = "PRUEBA" };

            MovimientoAlmacenBL bl = new MovimientoAlmacenBL();
            if (codigo == 0)
            {
                success = 1;
            }

            return JsonConvert.SerializeObject(new { success = success, result = result });
        }
    }
}
