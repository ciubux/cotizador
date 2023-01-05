using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.NextSoft
{
    public static class ConverterMPToNextSoft
    {
        static string codListaPreciosLim = "0001";
        static string codListaPreciosProv = "0002";
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
                    ubigeo = obj.ubigeo.codigoSepPunto
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

            List<object> direcciones = new List<object>();
            direcciones.Add(direccion);
 
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
                listaprecios = codListaPreciosLim,
                fpg = formaPag,
                direccion = direcciones
            };


            return item;
        }

        public static object toGuia(GuiaRemision obj)
        {
            List<object> listaDet = new List<object>();
            int numDet = 1;

            foreach(DocumentoDetalle movDet in obj.documentoDetalle)
            {
                string codFactor = "";
                switch(movDet.idProductoPresentacion)
                {
                    case 0: codFactor = movDet.producto.codigoFactorUnidadMP; break;
                    case 1: codFactor = movDet.producto.codigoFactorUnidadAlternativa; break;
                    case 2: codFactor = movDet.producto.codigoFactorUnidadProveedor; break;
                    case 3: codFactor = movDet.producto.codigoFactorUnidadConteo; break;

                }
                var det = new {
                    almacen = obj.almacen.codigoAlmacenNextSoft,
                    numitem = numDet,
                    codprod = movDet.producto.codigoNextSoft, 
                    descripcion = movDet.producto.descripcion,
                    cantidad = movDet.cantidad,
                    notas = "",
                    codfcv = codFactor, 
                    //fcv = "1", // REVISAR
                    listaprecio = codListaPreciosLim,
                    peso = 1
                };

                listaDet.Add(det);
                numDet++;
            }

            string motivoTraslado = "006";
            switch(obj.motivoTraslado)
            {
                case GuiaRemision.motivosTraslado.Venta: motivoTraslado = "001"; break;
                case GuiaRemision.motivosTraslado.TrasladoInterno: motivoTraslado = "004"; break;
            }

            string tipoDocumentoAlmacen = "009";
            /*switch (obj.motivoTraslado)
            {
                case GuiaRemision.motivosTraslado.Venta: motivoTraslado = "004"; break;
                case GuiaRemision.motivosTraslado.DevolucionCompra: motivoTraslado = "002"; break;
                case GuiaRemision.motivosTraslado.TrasladoInterno: motivoTraslado = "010"; break;
            }*/

            string nombreUsuario = obj.usuario.email.Split('@')[0];
            nombreUsuario = nombreUsuario.Replace(".", "");

            var direccion = new
            {
                descripcion = obj.direccionEntrega,
                ubigeo = obj.ubigeoEntrega.codigoSepPunto
            };

            var item = new
            {
                sucursal = obj.almacen.codigoSucursalNextSoft,
                puntoventa = obj.almacen.codigoPuntoVentaNextSoft,
                ruc = obj.pedido.cliente.ruc,
                direcccion = obj.direccionEntrega, 
                ubigeo = obj.ubigeoEntrega.codigoSepPunto,
                exportacion = false,
                tdo = tipoDocumentoAlmacen, 
                serie = "G" + obj.serieDocumento, // REVISAR
                numero = obj.numeroDocumento, // SALIDA
                fecemision = obj.fechaEmision.ToString("dd/MM/yyyy"),
                observaciones = obj.observaciones,
                vendedor = obj.pedido.vendedor.codigoNextSoft,
                ordencompra = obj.pedido != null && obj.pedido.numeroReferenciaCliente != null && obj.pedido.numeroReferenciaCliente.Trim().Equals("") ? obj.pedido.numeroReferenciaCliente : "sin orden de compra",
                motivotraslado = motivoTraslado, 
                modalidadtraslado = "", 
                fectraslado = obj.fechaTraslado.ToString("dd/MM/yyyy"),
                ructransportista = obj.transportista.ruc, 
                placa = obj.placaVehiculo, 
                docconductor = obj.transportista.ruc, 
                licconductor = obj.transportista.brevete,
                nomconductor = obj.transportista.descripcion, 
                ubigeopartida = obj.almacen.ubigeo.codigoSepPunto, 
                ubigeollegada = obj.ubigeoEntrega.codigoSepPunto,
                partida = obj.almacen.direccion,
                llegada = obj.direccionEntrega,
                usuario = nombreUsuario,
                peso = 1,
                items = listaDet.ToArray()
            };


            return item;
        }
    }
}
