using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.NextSoft
{
    public static class ConverterMPToNextSoft
    {
        public static object toCliente(Cliente obj)
        {
            object direccion;
            string nombres = "";
            string apepat = "";
            string apemat = "";

            if (obj.tipoDocumentoIdentidad == DocumentoVenta.TiposDocumentoIdentidad.RUC)
            {
                direccion = new
                {
                    descripcion = obj.direccionDomicilioLegalSunat,
                    ubigeo = obj.ubigeo.Id.Substring(0, 2) + "." + obj.ubigeo.Id.Substring(2, 2) + "." + obj.ubigeo.Id.Substring(4, 2)
                };
            } else
            {
                direccion = new
                {
                    descripcion = "",
                    ubigeo = ""
                };

                string[] parts = obj.nombreCliente.Split(' ');
                switch (parts.Length)
                {
                    case 1: nombres = obj.nombreCliente; apepat = obj.nombreCliente; break;
                    case 2: nombres = parts[0]; apepat = parts[1]; break;
                    case 3: nombres = parts[0]; apepat = parts[1]; apemat = parts[2]; break;
                    case 4: nombres = parts[0] + " " + parts[1]; apepat = parts[2]; apemat = parts[3]; break;
                    default: nombres = parts[0] + " " + parts[1]; apepat = parts[2]; apemat = obj.nombreCliente.Replace(parts[0] + " " + parts[1] + " " + parts[2] + " ", ""); break;
                } 
            }

            string formaPag = "001";
            /*
            switch(obj.tipoPagoFactura)
            {
                case DocumentoVenta.TipoPago.Contado: formaPag = ""; break;
                case DocumentoVenta.TipoPago.Crédito1: formaPag = ""; break
                case DocumentoVenta.TipoPago.Crédito7: formaPag = ""; break;
                case DocumentoVenta.TipoPago.Crédito15: formaPag = ""; break;
                case DocumentoVenta.TipoPago.Crédito20: formaPag = ""; break;
                case DocumentoVenta.TipoPago.Crédito21: formaPag = ""; break;
                case DocumentoVenta.TipoPago.Crédito25: formaPag = ""; break;
                case DocumentoVenta.TipoPago.Crédito30: formaPag = ""; break;
                case DocumentoVenta.TipoPago.Crédito45: formaPag = ""; break;
                case DocumentoVenta.TipoPago.Crédito60: formaPag = ""; break;
                case DocumentoVenta.TipoPago.Crédito90: formaPag = ""; break;
                case DocumentoVenta.TipoPago.Crédito120: formaPag = ""; break;
                case DocumentoVenta.TipoPago.NoAsignado: formaPag = ""; break;
            }
            */
            
            

            var item = new
            {
                tdi = ((int)obj.tipoDocumentoIdentidad).ToString(),
                numdoc = obj.ruc,
                razonsocial = obj.tipoDocumentoIdentidad == DocumentoVenta.TiposDocumentoIdentidad.RUC ? obj.razonSocialSunat : "",
                nomcomercial = obj.tipoDocumentoIdentidad == DocumentoVenta.TiposDocumentoIdentidad.RUC ? obj.nombreComercial : "",
                paterno = apepat,
                materno = apemat,
                nombres = nombres,
                nodomiciliado = obj.tipoDocumentoIdentidad == DocumentoVenta.TiposDocumentoIdentidad.Carnet ? true : false,
                email = obj.correoEnvioFactura == null ? "" : obj.correoEnvioFactura,
                vendedor = obj.responsableComercial.codigoNextSoft,
                listaprecios = "0001",
                fpg = formaPag,
                direccion = direccion
            };


            return item;
        }
    }
}
