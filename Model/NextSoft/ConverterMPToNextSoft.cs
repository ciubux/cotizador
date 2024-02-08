using Model.ServiceReferencePSE;
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

            string nombreComercial = (obj.nombreComercial != null && !obj.nombreComercial.Trim().Equals("")) ? obj.nombreComercial : obj.nombreComercialSunat;
            if(nombreComercial == null || nombreComercial.Trim().Equals(""))
            {
                nombreComercial = obj.razonSocialSunat;
            }

            var item = new
            {
                tdi = ((int)obj.tipoDocumentoIdentidad).ToString(),
                numdoc = obj.ruc,
                razonsocial = obj.tipoDocumentoIdentidad == DocumentoVenta.TiposDocumentoIdentidad.RUC ? obj.razonSocialSunat : "",
                nomcomercial = obj.tipoDocumentoIdentidad == DocumentoVenta.TiposDocumentoIdentidad.RUC ? nombreComercial : "",
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

            //BORRAR
            //obj.clienteVer = obj.pedido.cliente;

            foreach(DocumentoDetalle movDet in obj.documentoDetalle)
            {
                int cantidadAtender = movDet.cantidadPorAtender; //movDet.cantidadPorAtender;

                if (cantidadAtender > 0 && 
                    (movDet.producto.tipoProducto == Producto.TipoProducto.Bien || 
                    movDet.producto.tipoProducto == Producto.TipoProducto.Comodato))
                {
                    string codFactor = "";
                    int nFactor = 1;
                    switch (movDet.idProductoPresentacion)
                    {
                        case 0:
                            codFactor = movDet.producto.codigoFactorUnidadMP;
                            nFactor = movDet.producto.equivalenciaAlternativa;
                            break;
                        case 1:
                            codFactor = movDet.producto.codigoFactorUnidadAlternativa;
                            nFactor = 1;
                            break;
                        case 2:
                            codFactor = movDet.producto.codigoFactorUnidadProveedor;
                            nFactor = movDet.producto.equivalenciaAlternativa * movDet.producto.equivalenciaProveedor;
                            break;
                        case 3:
                            codFactor = movDet.producto.codigoFactorUnidadConteo;
                            nFactor = 1;
                            break;

                    }
                    var det = new
                    {
                        almacen = obj.almacen.codigoAlmacenNextSoft,
                        numitem = numDet,
                        codprod = movDet.producto.sku,
                        descripcion = movDet.producto.descripcion,
                        cantidad = cantidadAtender,
                        notas = "",
                        //codfcv = codFactor, 
                        fcv = nFactor, // REVISAR
                        listaprecio = codListaPreciosLim,
                        //comprobante = "001-F001-22029",
                        peso = 1
                    };

                    listaDet.Add(det);
                    numDet++;
                }
            }

            string motivoTraslado = "001";
            switch(obj.motivoTraslado)
            {
                case GuiaRemision.motivosTraslado.Venta: motivoTraslado = "001"; break;
                case GuiaRemision.motivosTraslado.TrasladoInterno: motivoTraslado = "004"; break;
                case GuiaRemision.motivosTraslado.TransferenciaGratuitaEntregada: motivoTraslado = "001"; break;
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

            string codigoVendedor = obj.pedido.vendedor.codigoNextSoft;
            if (codigoVendedor == null || codigoVendedor.Trim().Equals(""))
            {
                codigoVendedor = "07885378";
            }

            Cliente cli = obj.clienteVer;
            if (cli == null) cli = obj.pedido.cliente;
            string rucComprador = obj.entregaTerceros ? obj.pedido.cliente.ruc : "";
            if (obj.entregaTerceros)
            {
                motivoTraslado = "014";
            }

            var item = new
            {
                sucursal = obj.almacen.codigoSucursalNextSoft,
                puntoventa = obj.almacen.codigoPuntoVentaNextSoft,
                ruccomprador = rucComprador,
                ruc = cli.ruc,
                direcccion = obj.direccionEntrega,
                ubigeo = obj.ubigeoEntrega.codigoSepPunto,
                exportacion = false,
                tdo = tipoDocumentoAlmacen,
                serie = obj.serieDocumento, // REVISAR
                numero = obj.numeroDocumento, // SALIDA
                fecemision = obj.fechaEmision.ToString("dd/MM/yyyy"),
                observaciones = obj.observaciones,
                //vendedor = "46124367",
                vendedor = codigoVendedor,
                ordencompra = obj.pedido != null && obj.pedido.numeroReferenciaCliente != null && 
                            !obj.pedido.numeroReferenciaCliente.Trim().Equals("") && !obj.pedido.numeroReferenciaCliente.Substring(0,2).Equals("IF") ? 
                                obj.pedido.numeroReferenciaCliente : "",
                motivotraslado = motivoTraslado,
                modalidadtraslado = "",
                fectraslado = obj.fechaTraslado.ToString("dd/MM/yyyy"),
                ructransportista = obj.transportista.ruc,
                placa = (obj.placaVehiculo == null || obj.placaVehiculo.Length < 6) ? "" : obj.placaVehiculo.Trim(),
                docconductor = (obj.transportista.brevete == null || obj.transportista.brevete.Length < 9) ? "" : obj.transportista.brevete.Substring(1,8), 
                licconductor = (obj.transportista.brevete == null || obj.transportista.brevete.Length < 9) ? "" : obj.transportista.brevete,
                nomconductor = obj.transportista.descripcion, 
                ubigeopartida = obj.almacen.ubigeo.codigoSepPunto, 
                ubigeollegada = obj.ubigeoEntrega.codigoSepPunto,
                partida = obj.almacen.direccion,
                llegada = obj.direccionEntrega,

                //usuario = nombreUsuario,
                usuario = "nextsoft",
                peso = 1,
                items = listaDet.ToArray()
            };


            return item;
        }

        public static object toGuiaConsulta(GuiaRemision obj, string apiToken, string apiRUC)
        {
            string tipoDocumentoAlmacen = "09";

            var item = new
            {
                Token = apiToken,
                Ruc = apiRUC,
                Tipo = tipoDocumentoAlmacen,
                Serie = obj.serieDocumento,
                Numero = obj.numeroDocumento,
                Usuario = "nextsoft",
                Archivo = true
            };

            return item;
        }

        public static object toCpe(DocumentoVenta obj)
        {
            List<object> listaDet = new List<object>();
            int numDet = 1;

            bool sinEntregaInmediata = true;
            string tipoDocumento = "001";
            string motivoNC = "";
            string nroGuiaRef = "";
            string nroFacturaRef = "";

            switch (obj.tipoDocumento)
            {
                case DocumentoVenta.TipoDocumento.Factura: {
                        tipoDocumento = "001";
                        nroGuiaRef = "009-" + obj.cPE_CABECERA_BE.NRO_GRE;
                        break;
                    }
                case DocumentoVenta.TipoDocumento.BoletaVenta: tipoDocumento = "003"; break;
                case DocumentoVenta.TipoDocumento.NotaCrédito:
                    {
                        tipoDocumento = "007";
                        nroFacturaRef = "001-" + obj.cPE_DOC_REF_BEList[0].NUM_SERIE_CPE_REF + "-"
                            + obj.cPE_DOC_REF_BEList[0].NUM_CORRE_CPE_REF;

                        if (!(obj.cPE_CABECERA_BE.NRO_GRE == null || obj.cPE_CABECERA_BE.NRO_GRE.Trim().Equals(""))) { 
                            sinEntregaInmediata = false;
                        }
                        
                        switch (obj.tipoNotaCredito)
                        {
                            case DocumentoVenta.TiposNotaCredito.AnulacionOperacion: motivoNC = "001"; break;
                            case DocumentoVenta.TiposNotaCredito.AnulacionErrorRUC: motivoNC = "002"; break;
                            case DocumentoVenta.TiposNotaCredito.DescuentoGlobal: motivoNC = "004"; break;
                            case DocumentoVenta.TiposNotaCredito.DescuentoItem: motivoNC = "005"; break;
                            case DocumentoVenta.TiposNotaCredito.DevolucionTotal: motivoNC = "006"; break;
                            case DocumentoVenta.TiposNotaCredito.DevolucionItem: motivoNC = "007"; break;
                            case DocumentoVenta.TiposNotaCredito.DisminucionValor: motivoNC = "010"; break;
                            case DocumentoVenta.TiposNotaCredito.CorreccionFechaPago: motivoNC = "013"; break;
                            
                        }
                        break;
                    }
                case DocumentoVenta.TipoDocumento.NotaDébito: tipoDocumento = "008"; break;
            }


            foreach (CPE_DETALLE_BE objDet in obj.cPE_DETALLE_BEList)
            {
                VentaDetalle vDet = obj.ventaDetalleList.ElementAt(numDet - 1);

                

                if (vDet.producto.tipoProducto == Producto.TipoProducto.Bien ||
                     vDet.producto.tipoProducto == Producto.TipoProducto.Comodato)
                {
                    string codFactor = "";
                    switch (vDet.ProductoPresentacion.IdProductoPresentacion)
                    {
                        case 0: codFactor = vDet.producto.codigoFactorUnidadMP; break;
                        case 1: codFactor = vDet.producto.codigoFactorUnidadAlternativa; break;
                        case 2: codFactor = vDet.producto.codigoFactorUnidadProveedor; break;
                        case 3: codFactor = vDet.producto.codigoFactorUnidadConteo; break;

                    }

                    var detAdd = new
                    {
                        almacen = obj.almacen.codigoAlmacenNextSoft, // PREGUNTAR
                        numitem = numDet,
                        codprod = vDet.producto.codigoNextSoft,
                        descripcion = objDet.TXT_DES_ITM,
                        cantidad = Decimal.ToInt32(decimal.Parse(objDet.CANT_UND_ITM)),
                        valorunitario = decimal.Parse(objDet.VAL_UNIT_ITM),
                        valorventa = decimal.Parse(objDet.VAL_VTA_ITM),
                        valorventatotal = decimal.Parse(objDet.VAL_VTA_ITM),
                        preciounitario = decimal.Parse(objDet.PRC_VTA_UND_ITM),
                        precioventa = decimal.Parse(objDet.PRC_VTA_ITEM),
                        precioventatotal = decimal.Parse(objDet.PRC_VTA_ITEM),
                        dcto = 0,
                        porcdcto = 0,
                        transgratuita = false,
                        igv = decimal.Parse(objDet.MNT_IGV_ITM),
                        porcigv = decimal.Parse(objDet.POR_IGV_ITM),
                        notas = "",
                        codfcv = codFactor, // PREGUNTAR
                        listaprecio = codListaPreciosLim,
                        combo = false,
                        fmtbebida = false,
                        guia = nroGuiaRef, // "009-G001-153763" PREGUNTAR
                        //itemguia = 0 // PREGUNTAR

                    };

                    listaDet.Add(detAdd);
                }

                if (vDet.producto.tipoProducto == Producto.TipoProducto.Servicio ||
                    vDet.producto.tipoProducto == Producto.TipoProducto.Descuento ||
                    vDet.producto.tipoProducto == Producto.TipoProducto.Cargo)
                {
                    var detAdd = new
                    {
                        numitem = numDet,
                        codserv = vDet.producto.codigoNextSoft,
                        descripcion = vDet.producto.descripcion,
                        cantidad = int.Parse(objDet.CANT_UND_ITM),
                        valorunitario = decimal.Parse(objDet.VAL_UNIT_ITM),
                        valorventa = decimal.Parse(objDet.VAL_VTA_ITM),
                        valorventatotal = decimal.Parse(objDet.VAL_VTA_ITM),
                        preciounitario = decimal.Parse(objDet.PRC_VTA_UND_ITM),
                        precioventa = decimal.Parse(objDet.PRC_VTA_ITEM),
                        precioventatotal = decimal.Parse(objDet.PRC_VTA_ITEM),
                        dcto = 0,
                        porcdcto = 0,
                        transgratuita = false,
                        igv = decimal.Parse(objDet.MNT_IGV_ITM),
                        porcigv = decimal.Parse(objDet.POR_IGV_ITM),
                        notas = "",
                        codfcv = "", // PREGUNTAR
                        fcv = 0, // PREGUNTAR
                        listaprecio = codListaPreciosLim,
                        combo = false,
                        fmtbebida = false,
                        guia = obj.cPE_CABECERA_BE.NRO_GRE // PREGUNTAR
                    };
                    listaDet.Add(detAdd);
                }

                //listaDet.Add(det);
                numDet++;
            }


            

            
            /*switch (obj.motivoTraslado)
            {
                case GuiaRemision.motivosTraslado.Venta: motivoTraslado = "004"; break;
                case GuiaRemision.motivosTraslado.DevolucionCompra: motivoTraslado = "002"; break;
                case GuiaRemision.motivosTraslado.TrasladoInterno: motivoTraslado = "010"; break;
            }*/

            string nombreUsuario = obj.usuario.email.Split('@')[0];
            nombreUsuario = nombreUsuario.Replace(".", "");

            string ubigeo = obj.cPE_CABECERA_BE.UBI_RCT;
            ubigeo = ubigeo == null || ubigeo.Length < 6 ? "" : ubigeo.Substring(0, 2) + "." + ubigeo.Substring(2, 2) + "." + ubigeo.Substring(4, 2);

            string formaPago = "001";
            switch (obj.tipoPago)
            {
                case DocumentoVenta.TipoPago.Crédito7: formaPago = "004"; break;
                case DocumentoVenta.TipoPago.Crédito15: formaPago = "005"; break;
                case DocumentoVenta.TipoPago.Crédito20:
                case DocumentoVenta.TipoPago.Crédito21: formaPago = "006"; break;
                case DocumentoVenta.TipoPago.Crédito30: formaPago = "007"; break;
                case DocumentoVenta.TipoPago.Crédito45: formaPago = "009"; break;
                case DocumentoVenta.TipoPago.Crédito60: formaPago = "010"; break;
                case DocumentoVenta.TipoPago.Crédito90: formaPago = "011"; break;
            }


            var item = new
            {
                sucursal = obj.almacen.codigoSucursalNextSoft,
                puntoventa = obj.almacen.codigoPuntoVentaNextSoft,
                ruc = obj.cPE_CABECERA_BE.NRO_DOC_RCT,
                direcccion = obj.cPE_CABECERA_BE.DIR_DES_RCT,
                ubigeo = ubigeo,
                exportacion = 0,
                fpg = formaPago,
                tdo = tipoDocumento,
                serie = obj.cPE_CABECERA_BE.SERIE,
                //numero = int.Parse(obj.cPE_CABECERA_BE.CORRELATIVO).ToString(),
                mnd = "001",
                fecemision = cambiarFormatoFechaV1(obj.cPE_CABECERA_BE.FEC_EMI),
                fecvencimiento = cambiarFormatoFechaV1(obj.cPE_CABECERA_BE.FEC_VCTO),
                notas = obj.cPE_CABECERA_BE.OBS_CPE,
                valorg = decimal.Parse(obj.cPE_CABECERA_BE.MNT_TOT_GRV),
                valorng = decimal.Parse(obj.cPE_CABECERA_BE.MNT_TOT_INF) + decimal.Parse(obj.cPE_CABECERA_BE.MNT_TOT_EXR),
                dscglobal = 0,
                igv = decimal.Parse(obj.cPE_CABECERA_BE.MNT_TOT_IMP),
                porcigv = Constantes.IGV*100,
                det = "",
                sinentregainmediata = sinEntregaInmediata,
                porcdet = 0,
                valorreganticipo = 0,
                vendedor = "07885378",
                numref = nroFacturaRef,
                ordencompra = obj.cPE_CABECERA_BE.NRO_ORD_COM,
                montocuota = obj.cPE_CABECERA_BE.MDP_MNT_PDT_PAG, // PREGUNTAR
                fechacuota = obj.cPE_CABECERA_BE.FEC_VCTO, // PREGUNTAR
                motivonc = motivoNC,
                motivond = "",
                porcretencion = 0,
                usuario = nombreUsuario,
                items = listaDet.ToArray()
            };

            return item;
        }

        public static object toProducto(Producto obj)
        {
            List<object> preciosList = new List<object>();
            string tipoProd = "001";

            switch (obj.tipoProducto)
            {
                case Producto.TipoProducto.Bien: tipoProd = "001"; break;
            }


            preciosList.Add(new {
                tipofcv = obj.codigoFactorUnidadMP,
                factor = obj.equivalenciaUnidadEstandarUnidadConteo,
                principal = 1
            });

            if (obj.equivalenciaAlternativa > 1) { 
                preciosList.Add(new
                {
                    tipofcv = obj.codigoFactorUnidadAlternativa,
                    factor = obj.equivalenciaUnidadEstandarUnidadConteo / obj.equivalenciaAlternativa,
                    principal = 0
                });
            }


            if (obj.equivalenciaProveedor > 1)
            {
                preciosList.Add(new
                {
                    tipofcv = obj.codigoFactorUnidadProveedor,
                    factor = obj.equivalenciaUnidadEstandarUnidadConteo * obj.equivalenciaProveedor,
                    principal = 0
                });
            }

            var item = new
            {
                codigo = obj.sku,
                familia = obj.sku.Substring(0,3),
                tprProd = tipoProd,
                unmMenor = obj.codigoFactorUnidadAlternativa,
                unmMayor = obj.codigoFactorUnidadProveedor,
                factor = obj.equivalenciaAlternativa * obj.equivalenciaProveedor,
                codAlternativo = obj.skuProveedor,
                descripcion = obj.descripcion,
                usuario = "nextsoft",
                almacen = "",
                items = preciosList.ToArray()
            };

            return item;
        }

        public static List<object> toProductoList(List<Producto> lista)
        {
            List<object> listaResult = new List<object>();

            foreach (Producto obj in lista)
            {
                listaResult.Add(ConverterMPToNextSoft.toProducto(obj));
            }

            return listaResult;
        }
        /* CONVIERTE UNA FECHA EN STRING DE AÑO-MES-DIA A DIA/MES/AÑO */
        private static string cambiarFormatoFechaV1(string fecha) {
            string[] aFecha = fecha.Split('-');

            string result = "";

            if(aFecha.Length == 3)
            {
                result = aFecha[2] + "/" + aFecha[1] + "/" + aFecha[0];
            }

            return result;
        }
    }
}
