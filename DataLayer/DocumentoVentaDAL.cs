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

        public void InsertarDocumentoVenta(DocumentoVenta documentoVenta)
        {
            var objCommand = GetSqlCommand("pi_documento_venta");

            InputParameterAdd.Guid(objCommand, "idVenta", documentoVenta.venta.idVenta);
            InputParameterAdd.Guid(objCommand, "idPedido", documentoVenta.venta.pedido.idPedido);
            InputParameterAdd.Int(objCommand, "tipoDocumento", (int)documentoVenta.tipoDocumento);
            InputParameterAdd.DateTime(objCommand, "fechaEmision", documentoVenta.fechaEmision);
            InputParameterAdd.DateTime(objCommand, "fechaVencimiento", documentoVenta.fechaVencimiento);
            InputParameterAdd.Int(objCommand, "tipoPago", (int)documentoVenta.tipoPago);
            InputParameterAdd.Int(objCommand, "formaPago", (int)documentoVenta.formaPago);
            InputParameterAdd.Varchar(objCommand, "correoEnvio", documentoVenta.correoEnvio);
            InputParameterAdd.Varchar(objCommand, "correoCopia", documentoVenta.correoCopia);
            InputParameterAdd.Varchar(objCommand, "correoOculto", documentoVenta.correoOculto);
            InputParameterAdd.Guid(objCommand, "idDocumentoVentaReferencia", Guid.Empty);
            OutputParameterAdd.UniqueIdentifier(objCommand, "idDocumentoVenta");
            ExecuteNonQuery(objCommand);

            documentoVenta.idDocumentoVenta = (Guid)objCommand.Parameters["@idDocumentoVenta"].Value;

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
            InputParameterAdd.Decimal(objCommand, "precioNetoEquivalente", pedidoDetalle.precioNetoEquivalente);
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
                    cotizacionDetalle.porcentajeDescuento = 100 - (cotizacionDetalle.precioNetoEquivalente * 100 / cotizacionDetalle.producto.precioSinIgv);
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

            //Se obtienen todas las columnas de la tabla 
            var columnasCabecera = cpeCabeceraBETable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
            var columnnasDetalle = cpeDetalleBETable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

            documentoVenta.cPE_CABECERA_BE = new CPE_CABECERA_BE();
            documentoVenta.cPE_DETALLE_BEList = new List<CPE_DETALLE_BE>();
            documentoVenta.cPE_DAT_ADIC_BEList = new List<CPE_DAT_ADIC_BE>();
            documentoVenta.cPE_DOC_REF_BEList = new List<CPE_DOC_REF_BE>();
            documentoVenta.cPE_ANTICIPO_BEList = new List<CPE_ANTICIPO_BE>();
            documentoVenta.cPE_FAC_GUIA_BEList = new List<CPE_FAC_GUIA_BE>();
            documentoVenta.cPE_DOC_ASOC_BEList = new List<CPE_DOC_ASOC_BE>();


            foreach (DataRow row in cpeCabeceraBETable.Rows)
            {
                foreach (String column in columnasCabecera)
                {
                    if (!column.Equals("id_cpe_cabecera_be") && !column.Equals("estado"))
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
            return documentoVenta;
        }

        public List<DocumentoVenta> SelectDocumentosVenta(DocumentoVenta documentoVenta)
        {

            List<DocumentoVenta> facturaList = new List<DocumentoVenta>();

            var objCommand = GetSqlCommand("ps_documentosVenta");
            /*   InputParameterAdd.BigInt(objCommand, "numero", documentoVenta.numero);
               InputParameterAdd.BigInt(objCommand, "numeroGrupo", pedido.numeroGrupoPedido);
               InputParameterAdd.Guid(objCommand, "idCliente", pedido.cliente.idCliente);
               InputParameterAdd.Guid(objCommand, "idCiudad", pedido.ciudad.idCiudad);
               InputParameterAdd.Guid(objCommand, "idUsuario", pedido.usuarioBusqueda.idUsuario);

               InputParameterAdd.DateTime(objCommand, "fechaSolicitudDesde", pedido.fechaSolicitudDesde);
               InputParameterAdd.DateTime(objCommand, "fechaSolicitudHasta", pedido.fechaSolicitudHasta);

               InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", pedido.fechaEntregaDesde);
               InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", pedido.fechaEntregaHasta);


               InputParameterAdd.Int(objCommand, "estado", (int)pedido.seguimientoPedido.estado);
               DataTable dataTable = Execute(objCommand);

               List<Pedido> pedidoList = new List<Pedido>();

               foreach (DataRow row in dataTable.Rows)
               {
                   pedido = new Pedido();
                   pedido.numeroPedido = Converter.GetLong(row, "numero_pedido");
                   pedido.numeroGrupoPedido = Converter.GetLong(row, "numero_grupo_pedido");
                   pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                   pedido.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                   pedido.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
                   pedido.fechaEntregaHasta = Converter.GetDateTime(row, "fecha_entrega_hasta");
                   pedido.horaEntregaDesde = Converter.GetString(row, "hora_entrega_desde");
                   pedido.horaEntregaHasta = Converter.GetString(row, "hora_entrega_hasta");
                   pedido.incluidoIGV = Converter.GetBool(row, "incluido_igv");

                   pedido.montoIGV = Converter.GetDecimal(row, "igv");
                   pedido.montoTotal = Converter.GetDecimal(row, "total");
                   pedido.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedido.montoTotal - pedido.montoIGV));

                   pedido.observaciones = Converter.GetString(row, "observaciones");

                   pedido.cliente = new Cliente();
                   pedido.cliente.codigo = Converter.GetString(row, "codigo");
                   pedido.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                   pedido.cliente.razonSocial = Converter.GetString(row, "razon_social");
                   pedido.cliente.ruc = Converter.GetString(row, "ruc");

                   pedido.usuario = new Usuario();
                   pedido.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                   pedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");

                   //  cotizacion.usuario_aprobador = new Usuario();
                   //  cotizacion.usuario_aprobador.nombre = Converter.GetString(row, "nombre_usuario_aprobador");

                   pedido.ciudad = new Ciudad();
                   pedido.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                   pedido.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                   pedido.seguimientoPedido = new SeguimientoPedido();
                   pedido.seguimientoPedido.estado = (SeguimientoPedido.estadosSeguimientoPedido)Converter.GetInt(row, "estado_seguimiento");
                   pedido.seguimientoPedido.observacion = Converter.GetString(row, "observacion_seguimiento");
                   pedido.seguimientoPedido.usuario = new Usuario();
                   pedido.seguimientoPedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento");
                   pedido.seguimientoPedido.usuario.nombre = Converter.GetString(row, "usuario_seguimiento");

                   pedidoList.Add(pedido);
               }
               return pedidoList;*/
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
