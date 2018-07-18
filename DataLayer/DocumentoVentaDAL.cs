using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;
using Model.DTO;
using Model.ServiceReferencePSE;

namespace DataLayer
{
    public class DocumentoVentaDAL : DaoBase
    {
        public DocumentoVentaDAL(IDalSettings settings) : base(settings)
        {
        }

        public DocumentoVentaDAL() : this(new CotizadorSettings())
        {
        }


        public List<DocumentoVenta> SelectDocumentosVentaPorProcesar()
        {
            List<DocumentoVenta> documentoVentaList = new List<DocumentoVenta>();

            var objCommand = GetSqlCommand("ps_documentosVentaPorProcesar");
            DataTable dataTable = Execute(objCommand);

            foreach (DataRow row in dataTable.Rows)
            {
                DocumentoVenta documentoVenta = new DocumentoVenta();
                documentoVenta.idDocumentoVenta = Converter.GetGuid(row, "id_documento_venta"); 
                documentoVenta.serie = Converter.GetString(row, "SERIE");
                documentoVenta.numero = Converter.GetString(row, "CORRELATIVO");
                documentoVenta.estadoDocumentoSunat = (DocumentoVenta.EstadosDocumentoSunat)Converter.GetInt(row, "estado");
                documentoVenta.tipoDocumento = (DocumentoVenta.TipoDocumento) Converter.GetInt(row, "tipo_documento");
                
                documentoVentaList.Add(documentoVenta); 
            }
            return documentoVentaList;
        }


        public void anularDocumentoVenta(DocumentoVenta documentoVenta)
        {
            var objCommand = GetSqlCommand("pu_solicitarAnulacionDocumentoVenta");
            InputParameterAdd.Guid(objCommand, "idDocumentoVenta", documentoVenta.idDocumentoVenta);
            InputParameterAdd.Varchar(objCommand, "comentarioAnulado", documentoVenta.comentarioAnulado);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoVenta.usuario.idUsuario);
            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);

            ExecuteNonQuery(objCommand);
            documentoVenta.tipoErrorSolicitudAnulacion = (DocumentoVenta.TiposErrorSolicitudAnulacion)(int)objCommand.Parameters["@tipoError"].Value;
            documentoVenta.descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;
        }

        public void aprobarAnulacionDocumentoVenta(DocumentoVenta documentoVenta)
        {
            var objCommand = GetSqlCommand("pu_aprobarAnulacionFactura");
            InputParameterAdd.Guid(objCommand, "idDocumentoVenta", documentoVenta.idDocumentoVenta);
            InputParameterAdd.Varchar(objCommand, "comentarioAprobacionAnulacion", documentoVenta.comentarioAprobacionAnulacion);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoVenta.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }

        

        public void aprobarAnularDocumentoVenta(DocumentoVenta documentoVenta)
        {
            var objCommand = GetSqlCommand("pu_aprobarAnularFactura");
            InputParameterAdd.Guid(objCommand, "idDocumentoVenta", documentoVenta.idDocumentoVenta);
            InputParameterAdd.Varchar(objCommand, "comentarioAnulado", documentoVenta.comentarioAnulado);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoVenta.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }


        public void InsertarDocumentoVenta(DocumentoVenta documentoVenta)
        {
            var objCommand = GetSqlCommand("pi_documentoVenta");     
            InputParameterAdd.Guid(objCommand, "idVenta", documentoVenta.venta.idVenta);
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", documentoVenta.venta.guiaRemision.idMovimientoAlmacen);
            InputParameterAdd.Int(objCommand, "tipoDocumento", (int)documentoVenta.tipoDocumento);
            InputParameterAdd.DateTime(objCommand, "fechaEmision", documentoVenta.fechaEmision);
            InputParameterAdd.DateTime(objCommand, "fechaVencimiento", documentoVenta.fechaVencimiento);
            InputParameterAdd.Int(objCommand, "tipoPago", (int)documentoVenta.tipoPago);
            InputParameterAdd.Int(objCommand, "formaPago", (int)documentoVenta.formaPago);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoVenta.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "serie", documentoVenta.serie);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", null);
            InputParameterAdd.Guid(objCommand, "idDocumentoVentaReferencia", Guid.Empty);
            InputParameterAdd.Varchar(objCommand, "observaciones", documentoVenta.observaciones);
            InputParameterAdd.Varchar(objCommand, "codigoCliente", documentoVenta.cliente.codigo);

            OutputParameterAdd.UniqueIdentifier(objCommand, "idDocumentoVenta");
            OutputParameterAdd.UniqueIdentifier(objCommand, "idVentaSalida");
            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError",500);       

            ExecuteNonQuery(objCommand);

            documentoVenta.idDocumentoVenta = (Guid)objCommand.Parameters["@idDocumentoVenta"].Value;
            
            documentoVenta.venta.idVenta = (Guid)objCommand.Parameters["@idVentaSalida"].Value;

            documentoVenta.tiposErrorValidacion =  (DocumentoVenta.TiposErrorValidacion)(int)objCommand.Parameters["@tipoError"].Value;
            documentoVenta.descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;
        }

        public void InsertarDocumentoVentaNotaCredito(DocumentoVenta documentoVenta)
        {
            var objCommand = GetSqlCommand("pi_documentoVentaNotaCreditoDebito");
            InputParameterAdd.Guid(objCommand, "idVenta", documentoVenta.venta.idVenta);
            InputParameterAdd.Int(objCommand, "tipoDocumento", (int)documentoVenta.tipoDocumento);
            InputParameterAdd.DateTime(objCommand, "fechaEmision", documentoVenta.fechaEmision);
            InputParameterAdd.DateTime(objCommand, "fechaVencimiento", documentoVenta.fechaVencimiento);
            InputParameterAdd.Int(objCommand, "tipoPago", (int)documentoVenta.tipoPago);
            InputParameterAdd.Int(objCommand, "formaPago", (int)documentoVenta.formaPago);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoVenta.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "serie", documentoVenta.serie);
            InputParameterAdd.Guid(objCommand, "idDocumentoReferenciaVenta", documentoVenta.venta.documentoReferencia.idDocumentoReferenciaVenta);
            InputParameterAdd.Varchar(objCommand, "observaciones", documentoVenta.observaciones==null?"": documentoVenta.observaciones);
            InputParameterAdd.Varchar(objCommand, "codigoCliente", documentoVenta.cliente.codigo);

            OutputParameterAdd.UniqueIdentifier(objCommand, "idDocumentoVenta");
            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);

            ExecuteNonQuery(objCommand);

            documentoVenta.idDocumentoVenta = (Guid)objCommand.Parameters["@idDocumentoVenta"].Value;
            documentoVenta.tiposErrorValidacion = (DocumentoVenta.TiposErrorValidacion)(int)objCommand.Parameters["@tipoError"].Value;
            documentoVenta.descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;
        }


        public void UpdateSiguienteNumeroFactura(DocumentoVenta documentoVenta)
        {
            var objCommand = GetSqlCommand("pu_siguienteNumeroFactura");

            InputParameterAdd.Guid(objCommand, "idVenta", documentoVenta.venta.idVenta);
            InputParameterAdd.Guid(objCommand, "idDocumentoVenta", documentoVenta.idDocumentoVenta);
            InputParameterAdd.Varchar(objCommand, "serie", documentoVenta.serie.Substring(1,3));
            InputParameterAdd.Guid(objCommand, "idPedido", documentoVenta.venta.pedido.idPedido);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoVenta.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }


        public void UpdateSiguienteNumeroNotaCredito(DocumentoVenta documentoVenta)
        {
            var objCommand = GetSqlCommand("pu_siguienteNumeroNotaCredito");

            InputParameterAdd.Guid(objCommand, "idVenta", documentoVenta.venta.idVenta);
            InputParameterAdd.Guid(objCommand, "idDocumentoVenta", documentoVenta.idDocumentoVenta);
            InputParameterAdd.Varchar(objCommand, "serie", documentoVenta.serie.Substring(2, 2));
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoVenta.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }


        public void UpdateRespuestaDocumentoVenta(DocumentoVenta documentoVenta)
        {
            var objCommand = GetSqlCommand("pu_documentoVenta");
            InputParameterAdd.Guid(objCommand, "idDocumentoVenta", documentoVenta.idDocumentoVenta);
            InputParameterAdd.Varchar(objCommand, "CODIGO", documentoVenta.cPE_RESPUESTA_BE.CODIGO);
            InputParameterAdd.Varchar(objCommand, "COD_ESTD_SUNAT", documentoVenta.cPE_RESPUESTA_BE.COD_ESTD_SUNAT);
            InputParameterAdd.Varchar(objCommand, "DESCRIPCION", documentoVenta.cPE_RESPUESTA_BE.DESCRIPCION);
            InputParameterAdd.Varchar(objCommand, "DETALLE", documentoVenta.cPE_RESPUESTA_BE.DETALLE);
            InputParameterAdd.Varchar(objCommand, "NUM_CPE", documentoVenta.cPE_RESPUESTA_BE.NUM_CPE);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoVenta.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }


        public void insertEstadoDocumentoVenta(DocumentoVenta documentoVenta)
        {
            var objCommand = GetSqlCommand("pi_documentoVentaEstado");
            InputParameterAdd.Guid(objCommand, "idDocumentoVenta", documentoVenta.idDocumentoVenta);
            InputParameterAdd.Varchar(objCommand, "CODIGO", documentoVenta.rPTA_BE.CODIGO);
            InputParameterAdd.Varchar(objCommand, "DESCRIPCION", documentoVenta.rPTA_BE.DESCRIPCION);
            InputParameterAdd.Varchar(objCommand, "DETALLE", documentoVenta.rPTA_BE.DETALLE);
            InputParameterAdd.Varchar(objCommand, "ESTADO", documentoVenta.rPTA_BE.ESTADO);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoVenta.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }

        public void UpdatePedido(Pedido pedido)
        {
            var objCommand = GetSqlCommand("pu_pedido");

            InputParameterAdd.Guid(objCommand, "idPedido", pedido.idPedido);
            InputParameterAdd.BigInt(objCommand, "numeroGrupo", pedido.numeroGrupoPedido); //puede ser null

            if (pedido.cotizacion.idCotizacion == Guid.Empty)
                InputParameterAdd.Guid(objCommand, "idCotizacion", null); //puede ser null
            else
                InputParameterAdd.Guid(objCommand, "idCotizacion", pedido.cotizacion.idCotizacion); //puede ser null

            InputParameterAdd.Guid(objCommand, "idCiudad", pedido.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", pedido.cliente.idCliente);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", pedido.numeroReferenciaCliente); //puede ser null
            InputParameterAdd.Guid(objCommand, "idDireccionEntrega", pedido.direccionEntrega.idDireccionEntrega); //puede ser null
            InputParameterAdd.Varchar(objCommand, "direccionEntrega", pedido.direccionEntrega.descripcion);  //puede ser null
            InputParameterAdd.Varchar(objCommand, "contactoEntrega", pedido.direccionEntrega.contacto); //puede ser null
            InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", pedido.direccionEntrega.telefono); //puede ser null
            InputParameterAdd.DateTime(objCommand, "fechaSolicitud", pedido.fechaSolicitud);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", pedido.fechaEntregaDesde.Value);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", pedido.fechaEntregaHasta.Value);

            DateTime dtTmp = DateTime.Now;
            String[] horaEntregaDesdeArray = pedido.horaEntregaDesde.Split(':');
            DateTime horaEntregaDesde = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaDesdeArray[0]), Int32.Parse(horaEntregaDesdeArray[1]), 0);
            String[] horaEntregaHastaArray = pedido.horaEntregaHasta.Split(':');
            DateTime horaEntregaHasta = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaHastaArray[0]), Int32.Parse(horaEntregaHastaArray[1]), 0);
            InputParameterAdd.DateTime(objCommand, "horaEntregaDesde", horaEntregaDesde);
            InputParameterAdd.DateTime(objCommand, "horaEntregaHasta", horaEntregaHasta);
            InputParameterAdd.Varchar(objCommand, "contactoPedido", pedido.contactoPedido);  //puede ser null
            InputParameterAdd.Varchar(objCommand, "telefonoContactoPedido", pedido.telefonoContactoPedido);  //puede ser null
            InputParameterAdd.Varchar(objCommand, "correoContactoPedido", pedido.correoContactoPedido);  //puede ser null
            InputParameterAdd.SmallInt(objCommand, "incluidoIGV", (short)(pedido.incluidoIGV ? 1 : 0));
            InputParameterAdd.Decimal(objCommand, "tasaIGV", Constantes.IGV);
            InputParameterAdd.Decimal(objCommand, "igv", pedido.montoIGV);
            InputParameterAdd.Decimal(objCommand, "total", pedido.montoTotal);
            InputParameterAdd.Varchar(objCommand, "observaciones", pedido.observaciones);  //puede ser null
            InputParameterAdd.Guid(objCommand, "idUsuario", pedido.usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "estado", (int)pedido.seguimientoPedido.estado);
            InputParameterAdd.Varchar(objCommand, "observacionSeguimientoPedido", pedido.seguimientoPedido.observacion);
            InputParameterAdd.Varchar(objCommand, "ubigeoEntrega", pedido.ubigeoEntrega.Id);

            ExecuteNonQuery(objCommand);


            foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
            {
                pedidoDetalle.idPedido = pedido.idPedido;
                this.InsertPedidoDetalle(pedidoDetalle);
            }
        }
    

        public void InsertPedidoDetalle(PedidoDetalle pedidoDetalle)
        {
            var objCommand = GetSqlCommand("pi_pedidoDetalle");
            InputParameterAdd.Guid(objCommand, "idPedido", pedidoDetalle.idPedido);
            InputParameterAdd.Guid(objCommand, "idProducto", pedidoDetalle.producto.idProducto);
            InputParameterAdd.Int(objCommand, "cantidad", pedidoDetalle.cantidad);
            //Siempre se almacena el precio sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "precioSinIGV", pedidoDetalle.producto.precioSinIgv);
            //Siempre se almacena el costo sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "costoSinIGV", pedidoDetalle.producto.costoSinIgv);
            InputParameterAdd.Decimal(objCommand, "equivalencia", pedidoDetalle.producto.equivalencia);
            InputParameterAdd.Varchar(objCommand, "unidad", pedidoDetalle.unidad);
            InputParameterAdd.Decimal(objCommand, "porcentajeDescuento", pedidoDetalle.porcentajeDescuento);
            InputParameterAdd.Decimal(objCommand, "precioNetoEquivalente", pedidoDetalle.precioNeto);
            InputParameterAdd.Int(objCommand, "esPrecioAlternativo", pedidoDetalle.esPrecioAlternativo?1:0);
            InputParameterAdd.Guid(objCommand, "idUsuario", pedidoDetalle.usuario.idUsuario);
            InputParameterAdd.Decimal(objCommand, "flete", pedidoDetalle.flete);
            InputParameterAdd.Varchar(objCommand, "observaciones", pedidoDetalle.observacion);
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            ExecuteNonQuery(objCommand);
          
            pedidoDetalle.idPedidoDetalle = (Guid)objCommand.Parameters["@newId"].Value;
        }

        public Cotizacion aprobarCotizacion(Cotizacion cotizacion)
        {
            var objCommand = GetSqlCommand("pu_aprobarCotizacion");

            InputParameterAdd.BigInt(objCommand, "codigo", cotizacion.codigo);
            InputParameterAdd.Guid(objCommand, "idUsuario", cotizacion.usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "estado", (int)cotizacion.seguimientoCotizacion.estado);
            InputParameterAdd.Varchar(objCommand, "observacion", cotizacion.seguimientoCotizacion.observacion);

            ExecuteNonQuery(objCommand);
            return cotizacion;

        }


        public Cotizacion obtenerProductosAPartirdePreciosRegistrados(Cotizacion cotizacion, String familia, String proveedor)
        {
            var objCommand = GetSqlCommand("ps_generarPlantillaCotizacion");
            InputParameterAdd.Guid(objCommand, "idCliente", cotizacion.cliente.idCliente);
            InputParameterAdd.DateTime(objCommand, "fecha", cotizacion.fechaPrecios);
            InputParameterAdd.Varchar(objCommand, "familia", familia);
            InputParameterAdd.Varchar(objCommand, "proveedor", proveedor);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable cotizacionDataTable = dataSet.Tables[0];
            DataTable cotizacionDetalleDataTable = dataSet.Tables[1];

            //DataTable dataTable = Execute(objCommand);
            //Datos de la cotizacion
            foreach (DataRow row in cotizacionDataTable.Rows)
            {

               //No se cuenta con IdCotizacion
         /*       cotizacion.fecha = DateTime.Now;
                cotizacion.fechaLimiteValidezOferta = DateTime.Now.AddDays(Constantes.PLAZO_OFERTA_DIAS);
                cotizacion.fechaInicioVigenciaPrecios = null;
                cotizacion.fechaFinVigenciaPrecios = null;
                cotizacion.incluidoIgv = false;
                cotizacion.considerarCantidades = Cotizacion.OpcionesConsiderarCantidades.Cantidades;
                cotizacion.mostrarValidezOfertaEnDias = 1;
                cotizacion.flete = 0;
                cotizacion.igv = Constantes.IGV;
                cotizacion.contacto = Converter.GetString(row, "contacto");
                cotizacion.observaciones = Constantes.OBSERVACION;
                cotizacion.mostrarCodigoProveedor = true;
                cotizacion.fechaModificacion = DateTime.Now;


                ///Falta agregar la búsqueda con Grupo
                cotizacion.grupo = new Grupo();
                Guid idCliente = cotizacion.cliente.idCliente;
                cotizacion.cliente = new Cliente();
                cotizacion.cliente.codigo = Converter.GetString(row, "codigo");
                cotizacion.cliente.idCliente = idCliente;
                cotizacion.cliente.razonSocial = Converter.GetString(row, "razon_social");
                cotizacion.cliente.ruc = Converter.GetString(row, "ruc");


                cotizacion.ciudad = new Ciudad();
                cotizacion.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                cotizacion.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");
                cotizacion.seguimientoCotizacion = new SeguimientoCotizacion();
                */
            }


            cotizacion.cotizacionDetalleList = new List<CotizacionDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in cotizacionDetalleDataTable.Rows)
            {
                CotizacionDetalle cotizacionDetalle = new CotizacionDetalle();
                cotizacionDetalle.producto = new Producto();


                //No se cuenta con IdCotizacionDetalle
                cotizacionDetalle.cantidad = 1;
                cotizacionDetalle.producto.equivalencia = Convert.ToInt32(Converter.GetDecimal(row, "equivalencia"));
                cotizacionDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                cotizacionDetalle.flete = Converter.GetDecimal(row, "flete");


                cotizacionDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                cotizacionDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");


    
                //if (cotizacionDetalle.esPrecioAlternativo)
               // {
                    cotizacionDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * cotizacionDetalle.producto.equivalencia;
                    cotizacionDetalle.porcentajeDescuento = 100 - (cotizacionDetalle.precioNeto * 100 / cotizacionDetalle.producto.precioSinIgv);
                /*}
                else
                {
                    cotizacionDetalle.precioNetoEquivalente = Converter.GetDecimal(row, "precio_neto");
                    cotizacionDetalle.porcentajeDescuento = 100 - (cotizacionDetalle.producto.precioSinIgv * 100 / cotizacionDetalle.precioNetoEquivalente);
                }*/


                cotizacionDetalle.flete = Converter.GetDecimal(row, "flete");
                cotizacionDetalle.unidad = Converter.GetString(row, "unidad");
                cotizacionDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                cotizacionDetalle.producto.sku = Converter.GetString(row, "sku");
                cotizacionDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                cotizacionDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                cotizacionDetalle.producto.proveedor = Converter.GetString(row, "proveedor");
                cotizacionDetalle.producto.image = Converter.GetBytes(row, "imagen");

                
                cotizacionDetalle.observacion = null; 

                cotizacion.cotizacionDetalleList.Add(cotizacionDetalle);
            }

            //POR REVISAR
            //  cotizacion.montoTotal = cotizacion.cotizacionDetalleList.AsEnumerable().Sum(o => o.subTotal);
            //  cotizacion.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacion.montoTotal / (1 + cotizacion.igv)));
            //cotizacion.montoIGV = cotizacion.montoTotal - cotizacion.montoSubTotal;

            return cotizacion;
        }




        public DocumentoVenta SelectDocumentoVenta(DocumentoVenta documentoVenta)
        {

            var objCommand = GetSqlCommand("ps_documentoVenta");
            InputParameterAdd.Guid(objCommand, "idDocumentoVenta", documentoVenta.idDocumentoVenta);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable cpeCabeceraBETable = dataSet.Tables[0];
            DataTable cpeDetalleBETable = dataSet.Tables[1];
            DataTable cpeDatAdicBETable = dataSet.Tables[2];
            DataTable cpeDocRefBETable = dataSet.Tables[3];


            //Se obtienen todas las columnas de la tabla 
            var columnasCabecera = cpeCabeceraBETable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
            var columnnasDetalle = cpeDetalleBETable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
            var columnnasDatAdic = cpeDatAdicBETable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
            var columnnasDocRef = cpeDocRefBETable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

            documentoVenta.cPE_CABECERA_BE = new CPE_CABECERA_BE();
            documentoVenta.cPE_DETALLE_BEList = new List<CPE_DETALLE_BE>();
            documentoVenta.cPE_DAT_ADIC_BEList = new List<CPE_DAT_ADIC_BE>();
            documentoVenta.cPE_DOC_REF_BEList = new List<CPE_DOC_REF_BE>();
            documentoVenta.cPE_ANTICIPO_BEList = new List<CPE_ANTICIPO_BE>();
            documentoVenta.cPE_FAC_GUIA_BEList = new List<CPE_FAC_GUIA_BE>();
            documentoVenta.cPE_DOC_ASOC_BEList = new List<CPE_DOC_ASOC_BE>();


            foreach (DataRow row in cpeCabeceraBETable.Rows)
            {

                documentoVenta.solicitadoAnulacion = Converter.GetBool(row, "SOLICITUD_ANULACION");
                documentoVenta.permiteAnulacion = Converter.GetBool(row, "permite_anulacion");


                foreach (String column in columnasCabecera)
                {
                   

                    if (!column.Equals("id_cpe_cabecera_be") && 
                        !column.Equals("estado") &&
                        !column.Equals("usuario_creacion") &&
                        !column.Equals("usuario_modificacion") &&
                        !column.Equals("fecha_creacion") &&
                        !column.Equals("fecha_modificacion") &&
                        !column.Equals("ESTADO_SUNAT") &&
                        !column.Equals("CODIGO") &&
                        !column.Equals("COD_ESTD_SUNAT") &&
                        !column.Equals("DESCRIPCION") &&
                        !column.Equals("DETALLE") &&
                        !column.Equals("NUM_CPE") &&
                        !column.Equals("SOLICITUD_ANULACION") &&
                        !column.Equals("ENVIADO_A_EOL") &&
                        !column.Equals("AMBIENTE_PRODUCCION") &&
                        !column.Equals("id_venta") &&
                        !column.Equals("COMENTARIO_SOLICITUD_ANULACION") &&
                        !column.Equals("COMENTARIO_APROBACION_ANULACION") &&
                        !column.Equals("permite_anulacion") 
                        )
                    {
                        documentoVenta.cPE_CABECERA_BE.GetType().GetProperty(column).SetValue(documentoVenta.cPE_CABECERA_BE, Converter.GetString(row, column));
                    }
                }
            }

            foreach (DataRow row in cpeDetalleBETable.Rows)
            {
                CPE_DETALLE_BE cPE_DETALLE_BE = new CPE_DETALLE_BE();
                foreach (String column in columnnasDetalle)
                {
                    if (!column.Equals("id_cpe_detalle_be") && !column.Equals("id_cpe_cabecera_be") && !column.Equals("estado"))
                    {
                        cPE_DETALLE_BE.GetType().GetProperty(column).SetValue(cPE_DETALLE_BE, Converter.GetString(row, column));
                    }
                }
                documentoVenta.cPE_DETALLE_BEList.Add(cPE_DETALLE_BE);

            }


            foreach (DataRow row in cpeDatAdicBETable.Rows)
            {
                CPE_DAT_ADIC_BE cPE_DAT_ADIC_BE = new CPE_DAT_ADIC_BE();
                foreach (String column in columnnasDatAdic)
                {
                    if (!column.Equals("id_cpe_dat_adic_be") && !column.Equals("id_cpe_cabecera_be") )
                    {
                        cPE_DAT_ADIC_BE.GetType().GetProperty(column).SetValue(cPE_DAT_ADIC_BE, Converter.GetString(row, column));
                    }
                }
                documentoVenta.cPE_DAT_ADIC_BEList.Add(cPE_DAT_ADIC_BE);

            }

            foreach (DataRow row in cpeDocRefBETable.Rows)
            {
                CPE_DOC_REF_BE cPE_DOC_REF_BE = new CPE_DOC_REF_BE();
                foreach (String column in columnnasDocRef)
                {
                    if (!column.Equals("id_cpe_doc_ref_be") && !column.Equals("id_cpe_cabecera_be"))
                    {
                        cPE_DOC_REF_BE.GetType().GetProperty(column).SetValue(cPE_DOC_REF_BE, Converter.GetString(row, column));
                    }
                }
                documentoVenta.cPE_DOC_REF_BEList.Add(cPE_DOC_REF_BE);

            }
            


            return documentoVenta;
        }

        public List<DocumentoVenta> SelectDocumentosVenta(DocumentoVenta documentoVenta)
        {

            List<DocumentoVenta> facturaList = new List<DocumentoVenta>();

            var objCommand = GetSqlCommand("ps_facturas");
            if (!documentoVenta.numero.Equals("0"))
            {
                InputParameterAdd.Varchar(objCommand, "numero", documentoVenta.numero.PadLeft(8, '0'));
            }
            else
            {
                InputParameterAdd.Varchar(objCommand, "numero", String.Empty);
            }

            InputParameterAdd.Guid(objCommand, "idCliente", documentoVenta.cliente.idCliente);
            InputParameterAdd.Guid(objCommand, "idCiudad", documentoVenta.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idUsuario", documentoVenta.usuario.idUsuario);
            InputParameterAdd.DateTime(objCommand, "fechaDesde", new DateTime(documentoVenta.fechaEmisionDesde.Year, documentoVenta.fechaEmisionDesde.Month, documentoVenta.fechaEmisionDesde.Day, 0, 0, 0));
            InputParameterAdd.DateTime(objCommand, "fechaHasta", new DateTime(documentoVenta.fechaEmisionHasta.Year, documentoVenta.fechaEmisionHasta.Month, documentoVenta.fechaEmisionHasta.Day, 23, 59, 59)); 
            InputParameterAdd.Int(objCommand, "soloSolicitudAnulacion", documentoVenta.solicitadoAnulacion?1:0);
            InputParameterAdd.Int(objCommand, "estado", (int)documentoVenta.estadoDocumentoSunatBusqueda);
            InputParameterAdd.BigInt(objCommand, "numeroPedido", documentoVenta.pedido.numeroPedido);
            InputParameterAdd.BigInt(objCommand, "numeroGuiaRemision", documentoVenta.guiaRemision.numeroDocumento);

           


            //   InputParameterAdd.Int(objCommand, "estado", (int)pedido.seguimientoPedido.estado);
            DataTable dataTable = Execute(objCommand);

            List<Pedido> pedidoList = new List<Pedido>();

            foreach (DataRow row in dataTable.Rows)
            {
                documentoVenta = new DocumentoVenta();
                documentoVenta.idDocumentoVenta = Converter.GetGuid(row, "id_documento_venta");
                //documentoVenta.cPE_CABECERA_BE = new CPE_CABECERA_BE();
                documentoVenta.serie = Converter.GetString(row, "SERIE");
                documentoVenta.numero = Converter.GetString(row, "CORRELATIVO");
                documentoVenta.total = Converter.GetDecimal(row, "MNT_TOT_PRC_VTA");

                documentoVenta.pedido = new Pedido();
                documentoVenta.pedido.numeroPedido = Converter.GetLong(row, "numero");
                
                documentoVenta.guiaRemision = new GuiaRemision();
                documentoVenta.guiaRemision.serieDocumento = Converter.GetString(row, "serie_documento");
                documentoVenta.guiaRemision.numeroDocumento = Converter.GetLong(row, "numero_documento");

                documentoVenta.tipoDocumento = (DocumentoVenta.TipoDocumento)Converter.GetInt(row, "TIP_CPE");



                documentoVenta.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
                documentoVenta.estadoDocumentoSunat = (DocumentoVenta.EstadosDocumentoSunat)Converter.GetInt(row, "estado");
                





                documentoVenta.usuario = new Usuario();
                documentoVenta.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                documentoVenta.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");

                documentoVenta.cliente = new Cliente();
                documentoVenta.cliente.codigo = Converter.GetString(row, "codigo");
                documentoVenta.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                documentoVenta.cliente.razonSocial = Converter.GetString(row, "razon_social");
                documentoVenta.cliente.ruc = Converter.GetString(row, "ruc");

                documentoVenta.ciudad = new Ciudad();
                documentoVenta.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                documentoVenta.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                documentoVenta.solicitadoAnulacion = Converter.GetBool(row, "solicitud_anulacion");
                documentoVenta.comentarioSolicitudAnulacion = Converter.GetString(row, "comentario_solicitud_anulacion");
                documentoVenta.comentarioSolicitudAnulacion = documentoVenta.comentarioSolicitudAnulacion == null ? String.Empty : documentoVenta.comentarioSolicitudAnulacion;
                documentoVenta.comentarioAprobacionAnulacion = Converter.GetString(row, "comentario_aprobacion_anulacion");
                documentoVenta.comentarioAprobacionAnulacion = documentoVenta.comentarioAprobacionAnulacion == null ? String.Empty : documentoVenta.comentarioAprobacionAnulacion;

                documentoVenta.permiteAnulacion = Converter.GetBool(row, "permite_anulacion");

                facturaList.Add(documentoVenta);
            }
            return facturaList;
        }


        public void insertSeguimientoPedido(Pedido pedido)
        {
            var objCommand = GetSqlCommand("pi_seguimiento_pedido");

            InputParameterAdd.Guid(objCommand, "idPedido", pedido.idPedido);
            InputParameterAdd.Guid(objCommand, "idUsuario", pedido.usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "estado", (int)pedido.seguimientoPedido.estado);
            InputParameterAdd.Varchar(objCommand, "observacion", pedido.seguimientoPedido.observacion);
            InputParameterAdd.DateTime(objCommand, "fechaModificacion", pedido.fechaModificacion);
         //   OutputParameterAdd.DateTime(objCommand, "fechaModificacionActual");
            ExecuteNonQuery(objCommand);

         //   DateTime fechaModifiacionActual = (DateTime)objCommand.Parameters["@fechaModificacionActual"].Value;

/*
            DateTime date1 = new DateTime(fechaModifiacionActual.Year, fechaModifiacionActual.Month, fechaModifiacionActual.Day, fechaModifiacionActual.Hour, fechaModifiacionActual.Minute, fechaModifiacionActual.Second);
            DateTime date2 = new DateTime(cotizacion.fechaModificacion.Year, cotizacion.fechaModificacion.Month, cotizacion.fechaModificacion.Day, cotizacion.fechaModificacion.Hour, cotizacion.fechaModificacion.Minute, cotizacion.fechaModificacion.Second);

            int result = DateTime.Compare(date1, date2);
            if (result != 0)
            {
                //No se puede actualizar la cotización si las fechas son distintas
                throw new Exception("CotizacionDesactualizada");
            }

    */
        }
        
    }
}
