using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;
using Model.EXCEPTION;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Model.CONFIGCLASSES;

namespace DataLayer
{
    public class MovimientoALmacenDAL : DaoBase
    {
        public MovimientoALmacenDAL(IDalSettings settings) : base(settings)
        {
        }

        public MovimientoALmacenDAL() : this(new CotizadorSettings())
        {
        }

        public DocumentoVenta obtenerResumenConsolidadoAtenciones(List<Guid> idMovimientoAlmacenList)
        {
            var objCommand = GetSqlCommand("ps_resumenConsolidadoAtenciones");


            
            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("idMovimientoAlmacenList", typeof(Guid)));

            // populate DataTable from your List here
            foreach (var id in idMovimientoAlmacenList)
                tvp.Rows.Add(id);
            
            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idMovimientoAlmacenList", tvp);
            // these next lines are important to map the C# DataTable object to the correct SQL User Defined Type
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";

            //InputParameterAdd.Varchar(objCommand, "idProductoList", tvparam);


            //InputParameterAdd.Varchar(objCommand, "idMovimientoAlmacenList", idMovimientoAlmacenList);
            DataSet dataSet = ExecuteDataSet(objCommand);

            DataTable documentoVentaDataTable = dataSet.Tables[0];
            DataTable productoDataTable = dataSet.Tables[1];
            

            DocumentoVenta documentoVenta = new DocumentoVenta();
            documentoVenta.ventaDetalleList = new List<VentaDetalle>();

            foreach (DataRow row in documentoVentaDataTable.Rows)
            {
                VentaDetalle ventaDetalle = new VentaDetalle();
                ventaDetalle.producto = new Producto();
                ventaDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                ventaDetalle.producto.sku = Converter.GetString(row, "sku");
                ventaDetalle.producto.descripcion = Converter.GetString(row, "descripcion");

                ventaDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");

                if (ventaDetalle.esPrecioAlternativo)
                {
                    ventaDetalle.ProductoPresentacion = new ProductoPresentacion();
                    ventaDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                }
                
                

                ventaDetalle.producto.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");

                //  ventaDetalle.sumCantidad =  Converter.GetDecimal(row, "sum_cantidad");
                ventaDetalle.sumCantidadUnidadAlternativa = Converter.GetDecimal(row, "sum_cantidad_unidad_alternativa");


                ventaDetalle.producto.unidad = Converter.GetString(row, "unidad");
                ventaDetalle.sumCantidadUnidadEstandar = Converter.GetDecimal(row, "sum_cantidad_unidad_estandar");




                ventaDetalle.sumPrecioNeto = Converter.GetDecimal(row, "sum_precio_neto");
                ventaDetalle.sumPrecioUnitario = Converter.GetDecimal(row, "sum_precio_unitario");

                documentoVenta.ventaDetalleList.Add(ventaDetalle);
            }



            Guid idProductoActual = Guid.Empty;

            List<Producto> productoList = new List<Producto>();
            Producto producto = new Producto();
            foreach (DataRow row in productoDataTable.Rows)
            {
                Guid idProducto = Converter.GetGuid(row, "id_producto");
                if (idProducto != idProductoActual)
                {
                    idProductoActual = idProducto;
                    producto = new Producto();
                    producto.idProducto = idProducto;
                    producto.ProductoPresentacionList = new List<ProductoPresentacion>();
                    productoList.Add(producto);
                }
                ProductoPresentacion productoPresentacion = new ProductoPresentacion();
                productoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                productoPresentacion.Presentacion = Converter.GetString(row, "presentacion");
                productoPresentacion.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion");
                producto.ProductoPresentacionList.Add(productoPresentacion);
            }
          

            foreach (VentaDetalle ventaDetalle in documentoVenta.ventaDetalleList)
            {
                Producto productoTmp = productoList.Where(p => p.idProducto == ventaDetalle.producto.idProducto).FirstOrDefault();
                if (productoTmp != null)
                {
                    ventaDetalle.producto.ProductoPresentacionList = productoTmp.ProductoPresentacionList;
                    foreach (ProductoPresentacion productoPresentacion in ventaDetalle.producto.ProductoPresentacionList)
                    {
                        productoPresentacion.Cantidad = Convert.ToInt32(ventaDetalle.sumCantidadUnidadEstandar * productoPresentacion.Equivalencia);
                    }
                }
            }

            return documentoVenta;
        }


        public List<GuiaRemision> obtenerDetalleConsolidadoAtenciones(List<Guid> idMovimientoAlmacenList, Dictionary<String,int> mostrarUnidadAlternativaList)
        {
            var objCommand = GetSqlCommand("ps_detalleConsolidadoAtenciones");
            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("idMovimientoAlmacenList", typeof(Guid)));
            foreach (var id in idMovimientoAlmacenList)
                tvp.Rows.Add(id);
            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idMovimientoAlmacenList", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";

            DataSet dataSet = ExecuteDataSet(objCommand);

            DataTable guiaRemisionDataTable = dataSet.Tables[0];
            DataTable productoDataTable = dataSet.Tables[1];

            List<GuiaRemision> guiaRemisionList = new List<GuiaRemision>();

            Guid idMovimientoAlmacenActual = Guid.Empty;
            GuiaRemision guiaRemision = null;

            int i = 0;
            foreach (DataRow row in guiaRemisionDataTable.Rows)
            {
               
                Guid idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");


                if (idMovimientoAlmacen != idMovimientoAlmacenActual)
                {
                    i = 0;
                    idMovimientoAlmacenActual = idMovimientoAlmacen;
                    guiaRemision = new GuiaRemision();
                    guiaRemision.idMovimientoAlmacen = idMovimientoAlmacenActual;
                    guiaRemisionList.Add(guiaRemision);

                    guiaRemision.documentoDetalle = new List<DocumentoDetalle>();
                    guiaRemision.pedido = new Pedido();
                    guiaRemision.pedido.numeroPedido = Converter.GetInt(row, "numero_pedido");
                    guiaRemision.pedido.numeroGrupoPedido = Converter.GetInt(row, "numero_grupo_pedido");
                    guiaRemision.pedido.numeroRequerimiento = Converter.GetString(row, "numero_requerimiento");
                    guiaRemision.serieDocumento = Converter.GetString(row, "serie_documento");
                    guiaRemision.numeroDocumento = Converter.GetInt(row, "numero_documento");
                    guiaRemision.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
                    guiaRemision.direccionEntrega  = Converter.GetString(row, "direccion_entrega");
                    guiaRemision.ubigeoEntrega = new Ubigeo();
                    guiaRemision.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");
                    guiaRemision.ubigeoEntrega.Provincia = Converter.GetString(row, "provincia");
                    guiaRemision.ubigeoEntrega.Departamento = Converter.GetString(row, "departamento");
                    guiaRemision.observaciones = Converter.GetString(row, "observaciones");
                    guiaRemision.pedido.direccionEntrega = new DireccionEntrega();
                    guiaRemision.pedido.direccionEntrega.idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega");
                    guiaRemision.pedido.direccionEntrega.codigo = Converter.GetInt(row, "direccion_entrega_codigo");
                    guiaRemision.pedido.direccionEntrega.nombre = Converter.GetString(row, "direccion_entrega_nombre");
                    guiaRemision.pedido.direccionEntrega.codigoCliente = Converter.GetString(row, "direccion_entrega_codigo_cliente");
                    guiaRemision.pedido.direccionEntrega.contacto = Converter.GetString(row, "direccion_entrega_contacto");
                    guiaRemision.pedido.direccionEntrega.telefono = Converter.GetString(row, "direccion_entrega_telefono");

                }

                DocumentoDetalle documentoDetalle = new DocumentoDetalle();
                documentoDetalle.producto = new Producto();
                documentoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                documentoDetalle.producto.sku = Converter.GetString(row, "sku");
                documentoDetalle.producto.unidad = Converter.GetString(row, "producto_unidad");
              //  documentoDetalle.ProductoPresentacion.Equivalencia = Converter.GetInt(row, "equivalencia");

                //Boolean mostrarUnidadAlternativa = false;
                documentoDetalle.esPrecioAlternativo = false;


                if (mostrarUnidadAlternativaList != null)
                {
                    int idProductoPresentacion = mostrarUnidadAlternativaList[documentoDetalle.producto.sku];
                    
                    if(idProductoPresentacion > 0 )
                    {
                        documentoDetalle.ProductoPresentacion = new ProductoPresentacion();
                        documentoDetalle.ProductoPresentacion.IdProductoPresentacion = idProductoPresentacion;
                        documentoDetalle.esPrecioAlternativo = true;
                    }
                }

                if (documentoDetalle.esPrecioAlternativo)
                {
                    //Esto es temporal, Se convierte a false el flag para que no se recalcule
                    documentoDetalle.esPrecioAlternativo = false;
                    documentoDetalle.unidad = Converter.GetString(row, "unidad_alternativa");
                    documentoDetalle.cantidadDecimal = Converter.GetDecimal(row, "cantidad_alternativa");
                    
                    /*  if (documentoDetalle.esPrecioAlternativo)
                          documentoDetalle.precioNeto = (Converter.GetDecimal(row, "precio_neto_alternativo") * documentoDetalle.ProductoPresentacion.Equivalencia);
                      else*/
                    
                    documentoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto_alternativo");
                }
                else {
                    documentoDetalle.unidad = Converter.GetString(row, "unidad");
                    documentoDetalle.cantidadDecimal = Converter.GetDecimal(row, "cantidad");
                    /*if (documentoDetalle.esPrecioAlternativo)
                        documentoDetalle.precioNeto = (Converter.GetDecimal(row, "precio_neto") * documentoDetalle.ProductoPresentacion.Equivalencia);
                    else*/
                        documentoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                } 

                guiaRemision.documentoDetalle.Add(documentoDetalle);
                i++;
            }




            return guiaRemisionList;
        }


        public void GuardarRespuestaNextSys(Guid idGuiaRemision, int success, string resultText)
        {
            var objCommand = GetSqlCommand("pu_guia_enviada_nextsys");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", idGuiaRemision);
            InputParameterAdd.Int(objCommand, "success", success);
            InputParameterAdd.Varchar(objCommand, "respuesta", resultText);
            ExecuteNonQuery(objCommand);
        }


        public void UpdateMarcaNoEntregado(GuiaRemision guiaRemision)
        {
            var objCommand = GetSqlCommand("pu_guiaRemisionMarcaNoEntregado");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", guiaRemision.idMovimientoAlmacen);
            InputParameterAdd.Int(objCommand, "noEntregado", guiaRemision.estaNoEntregado?1:0);
            InputParameterAdd.Guid(objCommand, "idUsuario", guiaRemision.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }

        public void UpdateAjusteEstadoAprobado(GuiaRemision guiaRemision)
        {
            var objCommand = GetSqlCommand("pu_CambiarAprobarAjusteAlmacen");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", guiaRemision.idMovimientoAlmacen);
            InputParameterAdd.Int(objCommand, "ajusteAprobado", guiaRemision.ajusteAprobado);
            InputParameterAdd.Guid(objCommand, "idUsuario", guiaRemision.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }

        
        public void InsertMovimientoAlmacenSalida(GuiaRemision guiaRemision)
        {

            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pi_movimientoAlmacenSalida");
            InputParameterAdd.DateTime(objCommand, "fechaEmision", guiaRemision.fechaEmision);
            InputParameterAdd.DateTime(objCommand, "fechaTraslado", guiaRemision.fechaTraslado);
            InputParameterAdd.Varchar(objCommand, "serieDocumento", guiaRemision.serieDocumento); //puede ser null
            InputParameterAdd.BigInt(objCommand, "numeroDocumento", guiaRemision.numeroDocumento); //puede ser null

            if (guiaRemision.notaIngresoAExtornar == null)
            {
                InputParameterAdd.Guid(objCommand, "idPedido", guiaRemision.pedido.idPedido);
            }
            else
            {
                InputParameterAdd.Guid(objCommand, "idPedido", null);
            }

            InputParameterAdd.Int(objCommand, "atencionParcial", guiaRemision.atencionParcial ? 1 : 0);
            InputParameterAdd.Int(objCommand, "ultimaAtencionParcial", guiaRemision.ultimaAtencionParcial ? 1 : 0);
            InputParameterAdd.Guid(objCommand, "idSedeOrigen", guiaRemision.ciudadOrigen.idCiudad);
            InputParameterAdd.Char(objCommand, "ubigeoEntrega", guiaRemision.pedido.ubigeoEntrega.Id);
            InputParameterAdd.Varchar(objCommand, "direccionEntrega", guiaRemision.pedido.direccionEntrega.descripcion);
            InputParameterAdd.Char(objCommand, "motivoTraslado", ((char)guiaRemision.motivoTraslado).ToString());
            InputParameterAdd.Varchar(objCommand, "observaciones", guiaRemision.observaciones);
            InputParameterAdd.Varchar(objCommand, "certificadoInscripcion", guiaRemision.certificadoInscripcion);
            InputParameterAdd.Guid(objCommand, "idUsuario", guiaRemision.usuario.idUsuario);
            InputParameterAdd.Guid(objCommand, "idAlmacen", guiaRemision.idAlmacen);
            InputParameterAdd.Int(objCommand, "estado", (int)guiaRemision.seguimientoMovimientoAlmacenSalida.estado);
            InputParameterAdd.Varchar(objCommand, "observacionSeguimiento", guiaRemision.seguimientoMovimientoAlmacenSalida.observacion);
            InputParameterAdd.Int(objCommand, "entregaATerceros", guiaRemision.entregaTerceros ? 1 : 0);
            InputParameterAdd.Guid(objCommand, "idClienteTercero", guiaRemision.idClienteTerceros);

            if (guiaRemision.esGuiaDiferida)
            {
                InputParameterAdd.VarcharEmpty(objCommand, "nombreTransportista", "");
                InputParameterAdd.VarcharEmpty(objCommand, "rucTransportista", "");
                InputParameterAdd.VarcharEmpty(objCommand, "breveteTransportista", "");
                InputParameterAdd.VarcharEmpty(objCommand, "direccionTransportista", "");
                InputParameterAdd.VarcharEmpty(objCommand, "placaVehiculo", "");
            } else
            {
                InputParameterAdd.Guid(objCommand, "idTransportista", guiaRemision.transportista.idTransportista);
                InputParameterAdd.Varchar(objCommand, "nombreTransportista", guiaRemision.transportista.descripcion);
                InputParameterAdd.Varchar(objCommand, "rucTransportista", guiaRemision.transportista.ruc);
                InputParameterAdd.Varchar(objCommand, "breveteTransportista", guiaRemision.transportista.brevete);
                InputParameterAdd.Varchar(objCommand, "direccionTransportista", guiaRemision.transportista.direccion);
                InputParameterAdd.Varchar(objCommand, "placaVehiculo", guiaRemision.placaVehiculo);
            }

            if (guiaRemision.notaIngresoAExtornar != null)
            {
                InputParameterAdd.Guid(objCommand, "idMovimientoAlmacenExtornado", guiaRemision.notaIngresoAExtornar.idMovimientoAlmacen);

                /*Si la atención es parcial quiere decir que es una devolución parcial*/
                if (guiaRemision.atencionParcial)
                {
                    InputParameterAdd.Int(objCommand, "motivoExtorno", (int)GuiaRemision.MotivosExtornoNotaIngreso.DevolucionItem);
                }
                else
                {
                    InputParameterAdd.Int(objCommand, "motivoExtorno", (int)GuiaRemision.MotivosExtornoNotaIngreso.DevolucionTotal);
                }
                InputParameterAdd.Varchar(objCommand, "sustentoExtorno", guiaRemision.sustentoExtorno);
            }
            else
            {
                InputParameterAdd.Guid(objCommand, "idMovimientoAlmacenExtornado", null);
                InputParameterAdd.Int(objCommand, "motivoExtorno", null);
                InputParameterAdd.Varchar(objCommand, "sustentoExtorno", null);
            }


            OutputParameterAdd.UniqueIdentifier(objCommand, "idMovimientoAlmacen");
            OutputParameterAdd.UniqueIdentifier(objCommand, "idVenta");
            OutputParameterAdd.BigInt(objCommand, "numeroMovimientoAlmacen");
            OutputParameterAdd.BigInt(objCommand, "numeroVenta");
            OutputParameterAdd.Int(objCommand, "siguienteNumeroGuiaRemision");
            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);
            ExecuteNonQuery(objCommand);

            guiaRemision.idMovimientoAlmacen = (Guid)objCommand.Parameters["@idMovimientoAlmacen"].Value;
            guiaRemision.numero = (Int64)objCommand.Parameters["@numeroMovimientoAlmacen"].Value;
            int siguienteNumeroGuiaRemision = (int)objCommand.Parameters["@siguienteNumeroGuiaRemision"].Value;


            guiaRemision.venta = new Venta();
            guiaRemision.venta.idVenta = (Guid)objCommand.Parameters["@idVenta"].Value;
            guiaRemision.venta.numero = (Int64)objCommand.Parameters["@numeroVenta"].Value;

            guiaRemision.guiaRemisionValidacion = new GuiaRemisionValidacion();

            guiaRemision.guiaRemisionValidacion.tipoErrorValidacion = (GuiaRemisionValidacion.TiposErrorValidacion)(int)objCommand.Parameters["@tipoError"].Value;
            guiaRemision.guiaRemisionValidacion.descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;

            if (guiaRemision.guiaRemisionValidacion.tipoErrorValidacion == GuiaRemisionValidacion.TiposErrorValidacion.NoExisteError)
            {

                if (guiaRemision.numeroDocumento != siguienteNumeroGuiaRemision)
                {
                    throw new DuplicateNumberDocumentException();
                }

                this.InsertMovimientoAlmacenDetalle(guiaRemision);
            }


            objCommand = GetSqlCommand("pu_venta");
            InputParameterAdd.Guid(objCommand, "idVenta", guiaRemision.venta.idVenta);
            InputParameterAdd.Varchar(objCommand, "observaciones", "Se crea Transacción");
            ExecuteNonQuery(objCommand);

            this.Commit();

        }

        public GuiaRemision InsertMovimientoAlmacenSalidaDesdeGuiaDiferida(GuiaRemision guiaRemision, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pi_movimientoAlmacenSalidaDesdeGuiaDiferida");
            InputParameterAdd.Guid(objCommand, "idGuiaDiferida", guiaRemision.idMovimientoAlmacen);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);


            InputParameterAdd.Guid(objCommand, "idTransportista", guiaRemision.transportista.idTransportista);
            InputParameterAdd.Varchar(objCommand, "nombreTransportista", guiaRemision.transportista.descripcion);
            InputParameterAdd.Varchar(objCommand, "rucTransportista", guiaRemision.transportista.ruc);
            InputParameterAdd.Varchar(objCommand, "breveteTransportista", guiaRemision.transportista.brevete);
            InputParameterAdd.Varchar(objCommand, "direccionTransportista", guiaRemision.transportista.direccion);
            InputParameterAdd.Varchar(objCommand, "placaVehiculo", guiaRemision.placaVehiculo);
            InputParameterAdd.Varchar(objCommand, "observaciones", guiaRemision.observaciones);

            OutputParameterAdd.UniqueIdentifier(objCommand, "idMovimientoAlmacen");
            OutputParameterAdd.Int(objCommand, "siguienteNumeroGuiaRemision");
            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);
            ExecuteNonQuery(objCommand);

            GuiaRemision guia = new GuiaRemision();
            guia.idMovimientoAlmacen = (Guid)objCommand.Parameters["@idMovimientoAlmacen"].Value;

            guia = this.SelectGuiaRemision(guia);

            guia.guiaRemisionValidacion = new GuiaRemisionValidacion();
            
            guia.guiaRemisionValidacion.tipoErrorValidacion = (GuiaRemisionValidacion.TiposErrorValidacion)(int)objCommand.Parameters["@tipoError"].Value;
            guia.guiaRemisionValidacion.descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;

            if(guia.guiaRemisionValidacion.tipoErrorValidacion == GuiaRemisionValidacion.TiposErrorValidacion.NoExisteError)
            {                

            }


            return guia;

        }


        public void InsertMovimientoAlmacenEntrada(NotaIngreso notaIngreso)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pi_movimientoAlmacenEntrada");
            InputParameterAdd.DateTime(objCommand, "fechaEmision", notaIngreso.fechaEmision);
            InputParameterAdd.DateTime(objCommand, "fechaTraslado", notaIngreso.fechaTraslado);
            InputParameterAdd.Varchar(objCommand, "serieDocumento", notaIngreso.serieDocumento); //puede ser null
            InputParameterAdd.BigInt(objCommand, "numeroDocumento", notaIngreso.numeroDocumento); //puede ser null

            if (notaIngreso.pedido != null && notaIngreso.pedido.idPedido != null)
            {
                InputParameterAdd.Guid(objCommand, "idPedido", notaIngreso.pedido.idPedido);
            }

            /*
            if (notaIngreso.guiaRemisionAExtornar == null && notaIngreso.guiaRemisionAIngresar == null)
            {
                InputParameterAdd.Guid(objCommand, "idPedido", notaIngreso.pedido.idPedido);
            }
            else
            {
                InputParameterAdd.Guid(objCommand, "idPedido", null);
            }*/


            InputParameterAdd.Int(objCommand, "atencionParcial", notaIngreso.atencionParcial ? 1 : 0);
            InputParameterAdd.Int(objCommand, "ultimaAtencionParcial", notaIngreso.ultimaAtencionParcial ? 1 : 0);
            InputParameterAdd.Guid(objCommand, "idAlmacen", notaIngreso.idAlmacen);
            InputParameterAdd.Guid(objCommand, "idSedeDestino", notaIngreso.ciudadDestino.idCiudad);
            InputParameterAdd.Char(objCommand, "ubigeoEntrega", notaIngreso.pedido.ubigeoEntrega.Id);
            InputParameterAdd.Varchar(objCommand, "direccionEntrega", notaIngreso.pedido.direccionEntrega.descripcion);
            InputParameterAdd.Char(objCommand, "motivoTraslado", ((char)notaIngreso.motivoTraslado).ToString());
            InputParameterAdd.Varchar(objCommand, "serieGuiaReferencia", notaIngreso.serieGuiaReferencia);
            InputParameterAdd.Int(objCommand, "numeroGuiaReferencia", notaIngreso.numeroGuiaReferencia);
            InputParameterAdd.Varchar(objCommand, "serieDocumentoVentaReferencia", notaIngreso.serieDocumentoVentaReferencia);
            InputParameterAdd.Int(objCommand, "numeroDocumentoVentaReferencia", notaIngreso.numeroDocumentoVentaReferencia);
            InputParameterAdd.Int(objCommand, "tipoDocumentoVentaReferencia", (int)notaIngreso.tipoDocumentoVentaReferencia);
            InputParameterAdd.Varchar(objCommand, "observaciones", notaIngreso.observaciones);
            InputParameterAdd.Guid(objCommand, "idUsuario", notaIngreso.usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "estado", (int)notaIngreso.seguimientoMovimientoAlmacenEntrada.estado);
            InputParameterAdd.Varchar(objCommand, "observacionSeguimiento", notaIngreso.seguimientoMovimientoAlmacenEntrada.observacion);

            if (notaIngreso.guiaRemisionAExtornar != null)
            {
                InputParameterAdd.Guid(objCommand, "idMovimientoAlmacenExtornado", notaIngreso.guiaRemisionAExtornar.idMovimientoAlmacen);
                InputParameterAdd.Int(objCommand, "motivoExtorno", (int)notaIngreso.motivoExtornoGuiaRemision);
                InputParameterAdd.Varchar(objCommand, "sustentoExtorno", notaIngreso.sustentoExtorno);
            }
            else
            {
                InputParameterAdd.Guid(objCommand, "idMovimientoAlmacenExtornado", null);
                InputParameterAdd.Int(objCommand, "motivoExtorno", null);
                InputParameterAdd.Varchar(objCommand, "sustentoExtorno", null);
            }


            if (notaIngreso.guiaRemisionAIngresar != null)
            {
                InputParameterAdd.Guid(objCommand, "idMovimientoAlmacenIngresado", notaIngreso.guiaRemisionAIngresar.idMovimientoAlmacen);
            }
            else
            {
                InputParameterAdd.Guid(objCommand, "idMovimientoAlmacenIngresado", null);
            }



            OutputParameterAdd.UniqueIdentifier(objCommand, "idMovimientoAlmacen");
            OutputParameterAdd.UniqueIdentifier(objCommand, "idVenta");
            OutputParameterAdd.Int(objCommand, "siguienteNumeroNotaIngreso");
            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);
            ExecuteNonQuery(objCommand);

            notaIngreso.idMovimientoAlmacen = (Guid)objCommand.Parameters["@idMovimientoAlmacen"].Value;
            int siguienteNumeroNotaIngreso = (int)objCommand.Parameters["@siguienteNumeroNotaIngreso"].Value;

            notaIngreso.notaIngresoValidacion = new NotaIngresoValidacion();

            notaIngreso.notaIngresoValidacion.tipoErrorValidacion = (NotaIngresoValidacion.TiposErrorValidacion)(int)objCommand.Parameters["@tipoError"].Value;
            notaIngreso.notaIngresoValidacion.descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;

            

            if (notaIngreso.notaIngresoValidacion.tipoErrorValidacion == NotaIngresoValidacion.TiposErrorValidacion.NoExisteError)
            {
                if (notaIngreso.numeroDocumento != siguienteNumeroNotaIngreso)
                {
                    throw new DuplicateNumberDocumentException();
                }

                notaIngreso.venta = new Venta();
                notaIngreso.venta.idVenta = (Guid)objCommand.Parameters["@idVenta"].Value;

                this.InsertMovimientoAlmacenDetalle(notaIngreso);
            }


            objCommand = GetSqlCommand("pu_venta");
            InputParameterAdd.Guid(objCommand, "idVenta", notaIngreso.venta.idVenta);
            InputParameterAdd.Varchar(objCommand, "observaciones", "Se crea Transacción");
            ExecuteNonQuery(objCommand);

            if (notaIngreso.pedido != null && notaIngreso.pedido.idPedido != null)
            {
                PedidoDAL pedidoDal = new PedidoDAL();
                pedidoDal.ActualizarEstadoIngresoPedido(notaIngreso.pedido.idPedido, notaIngreso.usuario.idUsuario);
            }

            this.Commit();
        }



        public void InsertMovimientoAlmacenDetalle(MovimientoAlmacen movimientoAlmacen)
        {
            Venta venta = movimientoAlmacen.venta;
            if (venta == null)
            {
                venta = new Venta();
                
            }

            foreach (DocumentoDetalle documentoDetalle in movimientoAlmacen.documentoDetalle)
            {
                if(documentoDetalle.cantidadPorAtender > 0)
                { 
                    var objCommand = GetSqlCommand("pi_movimientoAlmacenDetalle");
                    InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", movimientoAlmacen.idMovimientoAlmacen);        
                    InputParameterAdd.Guid(objCommand, "idVenta", venta.idVenta);
                    InputParameterAdd.Guid(objCommand, "idUsuario", movimientoAlmacen.usuario.idUsuario);
                    InputParameterAdd.Guid(objCommand, "idPedido", movimientoAlmacen.pedido.idPedido);
                    InputParameterAdd.Char(objCommand, "tipoPedido", ((char)movimientoAlmacen.pedido.tipoPedido).ToString());
                    InputParameterAdd.Guid(objCommand, "idProducto", documentoDetalle.producto.idProducto);
                    InputParameterAdd.Int(objCommand, "cantidad", documentoDetalle.cantidadPorAtender);
                    InputParameterAdd.VarcharEmpty(objCommand, "observaciones", documentoDetalle.observacion == null ? "" : documentoDetalle.observacion);
                    InputParameterAdd.Int(objCommand, "tipoProducto", (int)documentoDetalle.producto.tipoProducto);
                    OutputParameterAdd.UniqueIdentifier(objCommand, "idMovimientoAlmacenDetalle");
                    ExecuteNonQuery(objCommand);
                    Guid idMovimientoAlmacenDetalle = (Guid)objCommand.Parameters["@idMovimientoAlmacenDetalle"].Value;
                }
            }
        }

        public void InsertAjusteAlmacenDetalle(MovimientoAlmacen movimientoAlmacen)
        {
            foreach (DocumentoDetalle documentoDetalle in movimientoAlmacen.documentoDetalle)
            {
                
                var objCommand = GetSqlCommand("pi_ajusteAlmacenDetalle");
                InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", movimientoAlmacen.idMovimientoAlmacen);
                InputParameterAdd.Guid(objCommand, "idUsuario", movimientoAlmacen.usuario.idUsuario);
                InputParameterAdd.Guid(objCommand, "idProducto", documentoDetalle.producto.idProducto);
                InputParameterAdd.Int(objCommand, "cantidad", documentoDetalle.cantidad);
                InputParameterAdd.Int(objCommand, "esUnidadAlternativa", documentoDetalle.esPrecioAlternativo ? 1 : 0);

                int cantidadConteo = documentoDetalle.cantidad * documentoDetalle.producto.equivalenciaUnidadEstandarUnidadConteo;
                if (documentoDetalle.ProductoPresentacion != null)
                {
                    if (documentoDetalle.ProductoPresentacion.IdProductoPresentacion == 1)
                    {
                        cantidadConteo = cantidadConteo / documentoDetalle.producto.equivalenciaAlternativa;
                    }
                    if (documentoDetalle.ProductoPresentacion.IdProductoPresentacion == 2)
                    {
                        cantidadConteo = cantidadConteo * documentoDetalle.producto.equivalenciaProveedor;
                    }

                    if (documentoDetalle.ProductoPresentacion.IdProductoPresentacion == 3)
                    {
                        cantidadConteo = documentoDetalle.cantidad;
                    }
                    InputParameterAdd.Decimal(objCommand, "equivalencia", documentoDetalle.ProductoPresentacion.Equivalencia);
                    InputParameterAdd.Int(objCommand, "idProductoPresentacion", documentoDetalle.ProductoPresentacion.IdProductoPresentacion);
                } else
                {
                    InputParameterAdd.Decimal(objCommand, "equivalencia", 0);
                    InputParameterAdd.Int(objCommand, "idProductoPresentacion", 0);
                }
                

                InputParameterAdd.Int(objCommand, "cantidadUnidadConteo", cantidadConteo);
                
                InputParameterAdd.Varchar(objCommand, "observaciones", documentoDetalle.observacion);
                InputParameterAdd.Varchar(objCommand, "unidad", documentoDetalle.unidad);
                InputParameterAdd.Varchar(objCommand, "unidadConteo", documentoDetalle.producto.unidadConteo);
                InputParameterAdd.Int(objCommand, "tipoProducto", (int)documentoDetalle.producto.tipoProducto);
                OutputParameterAdd.UniqueIdentifier(objCommand, "idMovimientoAlmacenDetalle");
                ExecuteNonQuery(objCommand);
                Guid idMovimientoAlmacenDetalle = (Guid)objCommand.Parameters["@idMovimientoAlmacenDetalle"].Value;
            }
        }

        public void InsertAjusteAlmacen(GuiaRemision obj)
        {

            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pi_ajusteAlmacen");
            InputParameterAdd.DateTime(objCommand, "fechaEmision", obj.fechaEmision);
            InputParameterAdd.DateTime(objCommand, "fechaTraslado", obj.fechaTraslado);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.usuario.idUsuario);
            InputParameterAdd.Guid(objCommand, "idCierreStock", obj.idCierreStock);
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);
            InputParameterAdd.Varchar(objCommand, "observaciones", obj.observaciones);

            InputParameterAdd.Guid(objCommand, "idSedeOrigen", obj.ciudadOrigen.idCiudad);
            InputParameterAdd.Char(objCommand, "motivoTraslado", ((char)obj.motivoTraslado).ToString());
            InputParameterAdd.Char(objCommand, "tipoMovimiento", ((char)obj.tipoMovimiento).ToString());
            InputParameterAdd.Int(objCommand, "idMotivoAjuste", obj.motivoAjuste.idMotivoAjusteAlmacen);
            InputParameterAdd.Int(objCommand, "ajusteAprobado", obj.ajusteAprobado);

            OutputParameterAdd.UniqueIdentifier(objCommand, "idMovimientoAlmacen");

            ExecuteNonQuery(objCommand);

            obj.idMovimientoAlmacen = (Guid)objCommand.Parameters["@idMovimientoAlmacen"].Value;

            this.InsertAjusteAlmacenDetalle(obj);

            this.Commit();
        }


        public void AnularMovimientoAlmacen(MovimientoAlmacen movimientoAlmacen)
        {
            var objCommand = GetSqlCommand("pu_anularMovimientoAlmacen");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", movimientoAlmacen.idMovimientoAlmacen);
            InputParameterAdd.Varchar(objCommand, "comentarioAnulado", movimientoAlmacen.comentarioAnulado);
            InputParameterAdd.Guid(objCommand, "idUsuario", movimientoAlmacen.usuario.idUsuario);
            ExecuteNonQuery(objCommand);

        }

        public void GuiaFicticiaPedidoRelacionado(Guid idGuiaOriginal, Guid idUsuario, out Guid idGuiaFic, out Guid idVentaFic, 
                out DocumentoVenta.TiposErrorValidacion TipoError, out string descripcionError)
        {
            var objCommand = GetSqlCommand("pi_guiaFicticiaPedidoRelacionadoDesdeGuiaAtencion");
            
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", idGuiaOriginal);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            OutputParameterAdd.UniqueIdentifier(objCommand, "idGuiaFic");
            OutputParameterAdd.UniqueIdentifier(objCommand, "idVentaFic");
            OutputParameterAdd.Int(objCommand, "tipoError");
            OutputParameterAdd.Varchar(objCommand, "descripcionError", 500);
            ExecuteNonQuery(objCommand);
            idGuiaFic = (Guid)objCommand.Parameters["@idGuiaFic"].Value;
            idVentaFic = (Guid)objCommand.Parameters["@idVentaFic"].Value;
            TipoError = (DocumentoVenta.TiposErrorValidacion)(int)objCommand.Parameters["@tipoError"].Value;
            descripcionError = (String)objCommand.Parameters["@descripcionError"].Value;
        }


        public void UpdateMovimientoAlmacenSalida(Pedido pedido)
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


        }
    

        public void InsertPedidoDetalle(PedidoDetalle pedidoDetalle, Usuario usuario)
        {
            var objCommand = GetSqlCommand("pi_pedidoDetalle");
            InputParameterAdd.Guid(objCommand, "idPedido", pedidoDetalle.idPedido);
            InputParameterAdd.Guid(objCommand, "idProducto", pedidoDetalle.producto.idProducto);
            InputParameterAdd.Int(objCommand, "cantidad", pedidoDetalle.cantidad);
            //Siempre se almacena el precio sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "precioSinIGV", pedidoDetalle.producto.precioSinIgv);
            //Siempre se almacena el costo sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "costoSinIGV", pedidoDetalle.producto.costoSinIgv);
            InputParameterAdd.Decimal(objCommand, "equivalencia", pedidoDetalle.ProductoPresentacion.Equivalencia);
            InputParameterAdd.Varchar(objCommand, "unidad", pedidoDetalle.unidad);
            InputParameterAdd.Decimal(objCommand, "porcentajeDescuento", pedidoDetalle.porcentajeDescuento);
            InputParameterAdd.Decimal(objCommand, "precioNetoEquivalente", pedidoDetalle.precioNeto);
            InputParameterAdd.Int(objCommand, "esPrecioAlternativo", pedidoDetalle.esPrecioAlternativo?1:0);
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
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


        public Cotizacion obtenerProductosAPartirdePreciosRegistrados(Cotizacion cotizacion, String familia, String proveedor, Usuario usuario)
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
                CotizacionDetalle cotizacionDetalle = new CotizacionDetalle(usuario.visualizaCostos, usuario.visualizaMargen);
                cotizacionDetalle.producto = new Producto();


                //No se cuenta con IdCotizacionDetalle
                cotizacionDetalle.cantidad = 1;
                cotizacionDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                cotizacionDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                cotizacionDetalle.flete = Converter.GetDecimal(row, "flete");


                cotizacionDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                cotizacionDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");


    
                //if (cotizacionDetalle.esPrecioAlternativo)
               // {
                    cotizacionDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * cotizacionDetalle.ProductoPresentacion.Equivalencia;
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




        public GuiaRemision SelectGuiaRemision(GuiaRemision guiaRemision)
        {
           
            var objCommand = GetSqlCommand("ps_guiaRemision");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", guiaRemision.idMovimientoAlmacen);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable guiaRemisionDataTable = dataSet.Tables[0];
            DataTable guiaRemisionDetalleDataTable = dataSet.Tables[1];

            DataTable transaccionListDataTable = dataSet.Tables[2];

            //Datos de la cotizacion
            foreach (DataRow row in guiaRemisionDataTable.Rows)
            {
                //DATOS DE LA GUIA
                guiaRemision.serieDocumento = Converter.GetString(row, "serie_documento");
                guiaRemision.numeroDocumento = Converter.GetLong(row, "numero_documento");
                guiaRemision.idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");
                guiaRemision.fechaTraslado = Converter.GetDateTime(row, "fecha_traslado");
                guiaRemision.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
                guiaRemision.atencionParcial = Converter.GetBool(row, "atencion_parcial");
                guiaRemision.ultimaAtencionParcial = Converter.GetBool(row, "ultima_atencion_parcial");
                guiaRemision.observaciones  = Converter.GetString(row, "observaciones");
                guiaRemision.estaAnulado = Converter.GetBool(row, "anulado");
                guiaRemision.estaFacturado = Converter.GetBool(row, "facturado");
                guiaRemision.motivoTraslado = (GuiaRemision.motivosTraslado) Char.Parse(Converter.GetString(row, "motivo_traslado"));
                guiaRemision.direccionEntrega = Converter.GetString(row, "direccion_entrega");
                guiaRemision.idMovimientoRelacionado = Converter.GetGuid(row, "id_movimiento_relacionado");
                guiaRemision.facturaPedidoRelacionado = Converter.GetInt(row, "factura_pedido_relacionado") == 1 ? true : false;

                //PEDIDO
                guiaRemision.pedido = new Pedido();
                guiaRemision.pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                guiaRemision.pedido.numeroPedido = Converter.GetLong(row, "numero");
                guiaRemision.pedido.direccionEntrega = new DireccionEntrega();
                guiaRemision.pedido.direccionEntrega.descripcion = Converter.GetString(row, "direccion_entrega");  
                guiaRemision.pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                guiaRemision.pedido.facturaUnica = Converter.GetInt(row, "factura_unica") == 1 ? true : false;
                guiaRemision.pedido.numeroGrupoPedido = Converter.GetLong(row, "numero_grupo");
                guiaRemision.pedido.numeroPedidoRelacionado = Converter.GetLong(row, "numero_pedido_relacionado");

                //UBIGEO
                guiaRemision.pedido.ubigeoEntrega = new Ubigeo();
                guiaRemision.pedido.ubigeoEntrega.Id = Converter.GetString(row, "id_ubigeo_entrega");
                guiaRemision.pedido.ubigeoEntrega.Departamento = Converter.GetString(row, "departamento");
                guiaRemision.pedido.ubigeoEntrega.Provincia = Converter.GetString(row, "provincia");
                guiaRemision.pedido.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");
                guiaRemision.ubigeoEntrega = guiaRemision.pedido.ubigeoEntrega;

                guiaRemision.pedido.vendedor = new Vendedor();
                guiaRemision.pedido.vendedor.idVendedor = Converter.GetInt(row, "id_vendedor");
                guiaRemision.pedido.vendedor.codigoNextSoft = Converter.GetString(row, "codigo_nextsoft_vendedor");

                guiaRemision.pedido.entregaATerceros = Converter.GetInt(row, "entrega_terceros") == 1 ? true : false;
                if (guiaRemision.pedido.entregaATerceros)
                {
                    guiaRemision.pedido.idClienteTercero = Converter.GetGuid(row, "id_cliente_rel");
                    guiaRemision.pedido.nombreClienteTercero = Converter.GetString(row, "nombre_cliente_rel");
                }

                //CLIENTE
                guiaRemision.pedido.cliente = new Cliente();
                guiaRemision.pedido.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                guiaRemision.pedido.cliente.codigo = Converter.GetString(row, "codigo");
                guiaRemision.pedido.cliente.razonSocial = Converter.GetString(row, "razon_social");
                guiaRemision.pedido.cliente.ruc = Converter.GetString(row, "ruc");
                guiaRemision.pedido.cliente.domicilioLegal = Converter.GetString(row, "domicilio_legal");

                guiaRemision.pedido.seguimientoCrediticioPedido = new SeguimientoCrediticioPedido();
                guiaRemision.pedido.seguimientoCrediticioPedido.estado = (SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido)Converter.GetInt(row, "estado_seguimiento_crediticio_pedido");

                try
                {
                    guiaRemision.pedido.cliente.configuraciones = JsonConvert.DeserializeObject<ClienteConfiguracion>(Converter.GetString(row, "cliente_configuraciones"));
                    if (guiaRemision.pedido.cliente.configuraciones == null) guiaRemision.pedido.cliente.configuraciones = new ClienteConfiguracion();
                }
                catch (Exception ex)
                {
                    guiaRemision.pedido.cliente.configuraciones = new ClienteConfiguracion();
                }

                guiaRemision.pedido.empresaRelacionada = new Empresa();
                guiaRemision.pedido.empresaRelacionada.idEmpresa = Converter.GetInt(row, "id_empresa_rel");
                guiaRemision.pedido.empresaRelacionada.codigo = Converter.GetString(row, "codigo_empresa_rel");
                guiaRemision.pedido.empresaRelacionada.facturacionHabilitada = Converter.GetInt(row, "facturacion_habilitada_empresa_rel") == 1;

                Empresa.EntornoFacturacion emrelenfac;
                if (Enum.TryParse<Empresa.EntornoFacturacion>(Converter.GetString(row, "entorno_facturacion_empresa_rel"), true, out emrelenfac))
                {
                    guiaRemision.pedido.empresaRelacionada.entornoFacturacion = emrelenfac;
                }
                else
                {
                    guiaRemision.pedido.empresaRelacionada.entornoFacturacion = Empresa.EntornoFacturacion.NINGUNO;
                }


                //USUARIO
                guiaRemision.usuario = new Usuario();
                guiaRemision.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                guiaRemision.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                //SEDE
                guiaRemision.ciudadOrigen = new Ciudad();
                guiaRemision.ciudadOrigen.idCiudad = Converter.GetGuid(row, "id_ciudad");
                guiaRemision.ciudadOrigen.nombre = Converter.GetString(row, "nombre_ciudad");
                guiaRemision.ciudadOrigen.direccionPuntoPartida = Converter.GetString(row, "direccion_punto_partida");
                guiaRemision.ciudadOrigen.esProvincia = Converter.GetBool(row, "es_provincia");
                guiaRemision.ciudadOrigen.sede = Converter.GetString(row, "sede");
                //ALMACEN
                guiaRemision.almacen = new Almacen();
                guiaRemision.almacen.idAlmacen = Converter.GetGuid(row, "id_almacen");
                guiaRemision.almacen.direccion = Converter.GetString(row, "direccion_almacen");
                guiaRemision.almacen.ubigeo = new Ubigeo();
                guiaRemision.almacen.ubigeo.Id = Converter.GetString(row, "ubigeo_almacen");
                guiaRemision.almacen.codigoSucursalNextSoft = Converter.GetString(row, "codigo_sucursal_nextsoft_almacen");
                guiaRemision.almacen.codigoPuntoVentaNextSoft = Converter.GetString(row, "codigo_punto_venta_nextsoft_almacen");
                guiaRemision.almacen.codigoAlmacenNextSoft = Converter.GetString(row, "codigo_almacen_nextsoft_almacen");

                //TRANSPORTISTA
                guiaRemision.transportista = new Transportista();
                guiaRemision.transportista.brevete = Converter.GetString(row, "brevete_transportista");
                guiaRemision.transportista.direccion = Converter.GetString(row, "direccion_transportista");
                guiaRemision.transportista.ruc = Converter.GetString(row, "ruc_transportista");
                guiaRemision.transportista.descripcion = Converter.GetString(row, "nombre_transportista");

                //ADICIONALES
                guiaRemision.placaVehiculo = Converter.GetString(row, "placa_vehiculo");
                guiaRemision.certificadoInscripcion = Converter.GetString(row, "certificado_inscripcion");

                guiaRemision.ingresado = Converter.GetBool(row, "ingresado");

                if (Converter.GetInt(row, "motivo_extorno") > 0)
                {
                    guiaRemision.motivoExtornoNotaIngreso = (GuiaRemision.MotivosExtornoNotaIngreso)Converter.GetInt(row, "motivo_extorno");

                    guiaRemision.sustentoExtorno = Converter.GetString(row, "sustento_extorno");

                    guiaRemision.notaIngresoAExtornar = new NotaIngreso();
                    guiaRemision.notaIngresoAExtornar.idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen_extornado");
                    guiaRemision.notaIngresoAExtornar.serieDocumento = Converter.GetString(row, "movimiento_almacen_extorno_serie_documento");
                    guiaRemision.notaIngresoAExtornar.numeroDocumento = Converter.GetInt(row, "movimiento_almacen_extorno_numero_documento");
                }

                guiaRemision.ingresado = Converter.GetBool(row, "ingresado");
                guiaRemision.tipoExtorno = (GuiaRemision.TiposExtorno)Converter.GetInt(row, "tipo_extorno");
            }


            guiaRemision.documentoDetalle = new List<DocumentoDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in guiaRemisionDetalleDataTable.Rows)
            {
                DocumentoDetalle documentoDetalle = new DocumentoDetalle();
                documentoDetalle.idDocumentoDetalle = Converter.GetGuid(row, "id_movimiento_almacen_detalle");
                documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                documentoDetalle.cantidadPorAtender = documentoDetalle.cantidad;
                documentoDetalle.cantidadPermitida = documentoDetalle.cantidad;
                documentoDetalle.cantidadGuiada = 0;
                documentoDetalle.cantidadTotalAtencion = Converter.GetInt(row, "cantidad_pedido");

                documentoDetalle.ProductoPresentacion = new ProductoPresentacion();
                documentoDetalle.ProductoPresentacion.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion");
                documentoDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");

                documentoDetalle.unidad = Converter.GetString(row, "unidad");
                documentoDetalle.producto = new Producto();
                documentoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                documentoDetalle.producto.sku = Converter.GetString(row, "sku");
                documentoDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                documentoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                documentoDetalle.producto.descripcionLarga = Converter.GetString(row, "descripcion_larga");
                documentoDetalle.producto.ventaRestringida = (Producto.TipoVentaRestringida)Converter.GetInt(row, "descontinuado");
                documentoDetalle.producto.motivoRestriccion = Converter.GetString(row, "motivo_restriccion");
                documentoDetalle.producto.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia_producto");
                documentoDetalle.producto.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor_producto");
                documentoDetalle.producto.codigoNextSoft = Converter.GetString(row, "codigo_nextsoft");
                documentoDetalle.producto.codigoFactorUnidadMP = Converter.GetString(row, "codigo_factor_unidad_mp");
                documentoDetalle.producto.codigoFactorUnidadAlternativa = Converter.GetString(row, "codigo_factor_unidad_alternativa");
                documentoDetalle.producto.codigoFactorUnidadProveedor = Converter.GetString(row, "codigo_factor_unidad_proveedor");
                documentoDetalle.producto.codigoFactorUnidadConteo = Converter.GetString(row, "codigo_factor_unidad_conteo");

                guiaRemision.documentoDetalle.Add(documentoDetalle);
            }

            guiaRemision.transaccionList = new List<Transaccion>();
            foreach (DataRow row in transaccionListDataTable.Rows)
            {
                Transaccion transaccion = new Transaccion();
                transaccion.documentoVenta = new DocumentoVenta();
                transaccion.documentoVenta.serie = Converter.GetString(row, "SERIE");
                transaccion.documentoVenta.numero = Converter.GetString(row, "CORRELATIVO");
                transaccion.documentoVenta.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
                guiaRemision.transaccionList.Add(transaccion);
            }

            return guiaRemision;
        }

        public List<MovimientoAlmacen> SelectMovimientosAlmacenExtornantes(MovimientoAlmacen movimientoAlmacen)
        {
            List<MovimientoAlmacen> MovimientoAlmacenExtornantesList = new List<MovimientoAlmacen>();
            var objCommand = GetSqlCommand("ps_movimientosAlmacenExtornantes");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", movimientoAlmacen.idMovimientoAlmacen);
            DataTable dataTable = Execute(objCommand);
            foreach (DataRow row in dataTable.Rows)
            {
                Guid idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");
                if (movimientoAlmacen.idMovimientoAlmacen != idMovimientoAlmacen)
                {

                    movimientoAlmacen = new MovimientoAlmacen();
                    movimientoAlmacen.documentoDetalle = new List<DocumentoDetalle>();
                    movimientoAlmacen.serieDocumento = Converter.GetString(row, "serie_documento");
                    movimientoAlmacen.numeroDocumento = Converter.GetInt(row, "numero_documento");
                    movimientoAlmacen.idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");
                    movimientoAlmacen.fechaTraslado = Converter.GetDateTime(row, "fecha_traslado");
                    movimientoAlmacen.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
                    movimientoAlmacen.venta = new Venta();
                    movimientoAlmacen.venta.documentoVenta = new DocumentoVenta();
                    movimientoAlmacen.venta.documentoVenta.idDocumentoVenta = Converter.GetGuid(row, "id_documento_venta");
                    movimientoAlmacen.venta.documentoVenta.serie = Converter.GetString(row, "cpe_serie");
                    movimientoAlmacen.venta.documentoVenta.numero = Converter.GetString(row, "cpe_numero");

                    if (row["cpe_fecha_emision"] == DBNull.Value)
                        movimientoAlmacen.venta.documentoVenta.fechaEmision = null;
                    else
                        movimientoAlmacen.venta.documentoVenta.fechaEmision = Converter.GetDateTime(row, "cpe_fecha_emision");
                    MovimientoAlmacenExtornantesList.Add(movimientoAlmacen);

                }

                DocumentoDetalle documentoDetalle = new DocumentoDetalle();
                documentoDetalle.idDocumentoDetalle = Converter.GetGuid(row, "id_movimiento_almacen_detalle");
                documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                documentoDetalle.producto = new Producto();
                documentoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                documentoDetalle.producto.sku = Converter.GetString(row, "sku");
                documentoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                documentoDetalle.unidad = Converter.GetString(row, "unidad");
                documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                
                movimientoAlmacen.documentoDetalle.Add(documentoDetalle);


            }
            return MovimientoAlmacenExtornantesList;
        }



        public List<GuiaRemision> SelectGuiasRemision(GuiaRemision guiaRemision)
        {
            List<GuiaRemision> guiaRemisionList = new List<GuiaRemision>();

            var objCommand = GetSqlCommand("ps_guiasRemision");
            InputParameterAdd.BigInt(objCommand, "numeroDocumento", guiaRemision.numeroDocumento);
            InputParameterAdd.Guid(objCommand, "idCiudad", guiaRemision.ciudadOrigen.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", guiaRemision.pedido.cliente.idCliente);
            InputParameterAdd.Int(objCommand, "idGrupoCliente", guiaRemision.pedido.idGrupoCliente);
            InputParameterAdd.Bit(objCommand, "buscaSedesGrupoCliente", guiaRemision.pedido.buscarSedesGrupoCliente);
            InputParameterAdd.Guid(objCommand, "idUsuario", guiaRemision.usuario.idUsuario);
            InputParameterAdd.DateTime(objCommand, "fechaTrasladoDesde", guiaRemision.fechaTrasladoDesde);
            InputParameterAdd.DateTime(objCommand, "fechaTrasladoHasta", guiaRemision.fechaTrasladoHasta);
            InputParameterAdd.Int(objCommand, "anulado", guiaRemision.estaAnulado ? 1 : 0);
            InputParameterAdd.Int(objCommand, "facturado", guiaRemision.estaFacturado ? 1 : 0);
            InputParameterAdd.BigInt(objCommand, "numeroPedido", guiaRemision.pedido.numeroPedido);
            InputParameterAdd.BigInt(objCommand, "numeroGrupoPedido", guiaRemision.pedido.numeroGrupoPedido);
            InputParameterAdd.Int(objCommand, "estadoFiltro", (int)guiaRemision.estadoFiltro);
            InputParameterAdd.Char(objCommand, "motivoTraslado", ((Char)guiaRemision.motivoTrasladoBusqueda).ToString());
            InputParameterAdd.Varchar(objCommand, "sku", guiaRemision.sku);
            InputParameterAdd.Varchar(objCommand, "nroFactura", guiaRemision.cpeNro);
            InputParameterAdd.Int(objCommand, "idAsesor", guiaRemision.responsableComercial.idVendedor);

            DataTable dataTable = Execute(objCommand);           

            foreach (DataRow row in dataTable.Rows)
            {
                //GUIA
                guiaRemision = new GuiaRemision();
                guiaRemision.serieDocumento = Converter.GetString(row, "serie_documento");
                guiaRemision.numeroDocumento = Converter.GetLong(row, "numero_documento");
                guiaRemision.idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");
                guiaRemision.fechaTraslado = Converter.GetDateTime(row, "fecha_traslado");
                guiaRemision.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
                guiaRemision.atencionParcial = Converter.GetBool(row, "atencion_parcial");
                guiaRemision.ultimaAtencionParcial = Converter.GetBool(row, "ultima_atencion_parcial");
                guiaRemision.estaAnulado = Converter.GetBool(row, "anulado");
                guiaRemision.estaFacturado = Converter.GetBool(row, "facturado");
                guiaRemision.estaNoEntregado = Converter.GetBool(row, "no_entregado");
                guiaRemision.motivoTraslado = (GuiaRemision.motivosTraslado)Char.Parse(Converter.GetString(row, "motivo_traslado"));
                guiaRemision.idMovimientoRelacionado = Converter.GetGuid(row, "id_movimiento_relacionado");

                guiaRemision.cpeNro = Converter.GetString(row, "serie_factura") + "-" + Converter.GetString(row, "correlativo_factura");
                guiaRemision.cpeFechaEmision = Converter.GetString(row, "fecha_emision_factura");
                guiaRemision.cpeSubTotal = Converter.GetString(row, "subtotal_factura");
                guiaRemision.cpeTotal = Converter.GetString(row, "total_factura");

                //PEDIDO
                guiaRemision.pedido = new Pedido();
                guiaRemision.pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                guiaRemision.pedido.numeroPedido = Converter.GetLong(row, "numero_pedido");
                guiaRemision.pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                guiaRemision.pedido.entregaATerceros = Converter.GetInt(row, "entrega_terceros") == 1 ? true : false;
                if (guiaRemision.pedido.entregaATerceros)
                {
                    guiaRemision.pedido.nombreClienteTercero = Converter.GetString(row, "nombre_cliente_rel");
                }

                //CLIENTE
                guiaRemision.pedido.cliente = new Cliente();
                guiaRemision.pedido.cliente.codigo = Converter.GetString(row, "codigo");
                guiaRemision.pedido.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                guiaRemision.pedido.cliente.razonSocial = Converter.GetString(row, "razon_social");
                guiaRemision.pedido.cliente.ruc = Converter.GetString(row, "ruc");
                //USUARIO
                guiaRemision.usuario = new Usuario();
                guiaRemision.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                guiaRemision.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                //SEDE
                guiaRemision.ciudadOrigen = new Ciudad();
                guiaRemision.ciudadOrigen.idCiudad = Converter.GetGuid(row, "id_ciudad");
                guiaRemision.ciudadOrigen.nombre = Converter.GetString(row, "nombre_ciudad");
                //ESTADO
                guiaRemision.seguimientoMovimientoAlmacenSalida = new SeguimientoMovimientoAlmacenSalida();
                guiaRemision.seguimientoMovimientoAlmacenSalida.estado = (SeguimientoMovimientoAlmacenSalida.estadosSeguimientoMovimientoAlmacenSalida)Converter.GetInt(row, "estado_movimiento_almacen");
                guiaRemision.seguimientoMovimientoAlmacenSalida.observacion = Converter.GetString(row, "observacion_seguimiento");
                guiaRemision.seguimientoMovimientoAlmacenSalida.usuario = new Usuario();
                guiaRemision.seguimientoMovimientoAlmacenSalida.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento");
                guiaRemision.seguimientoMovimientoAlmacenSalida.usuario.nombre = Converter.GetString(row, "usuario_seguimiento");

                guiaRemision.tipoExtorno = (GuiaRemision.TiposExtorno)Converter.GetInt(row, "tipo_extorno");

                guiaRemisionList.Add(guiaRemision);
            }
            return guiaRemisionList;
        }


        public List<GuiaRemision> SelectGuiasRemisionGrupoCliente(GuiaRemision guiaRemision)
        {
            List<GuiaRemision> guiaRemisionList = new List<GuiaRemision>();

            var objCommand = GetSqlCommand("ps_guiasRemisionGrupoCliente");
            //InputParameterAdd.BigInt(objCommand, "numeroDocumento", guiaRemision.numeroDocumento);
            //InputParameterAdd.Guid(objCommand, "idCiudad", guiaRemision.ciudadOrigen.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", guiaRemision.pedido.cliente.idCliente);
            InputParameterAdd.Guid(objCommand, "idUsuario", guiaRemision.usuario.idUsuario);
            InputParameterAdd.BigInt(objCommand, "numeroPedido", guiaRemision.pedido.numeroPedido);
            InputParameterAdd.BigInt(objCommand, "numeroGrupoPedido", guiaRemision.pedido.numeroGrupoPedido);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", guiaRemision.pedido.numeroReferenciaCliente);
            InputParameterAdd.DateTime(objCommand, "fechaTrasladoDesde", new DateTime(guiaRemision.fechaTrasladoDesde.Year, guiaRemision.fechaTrasladoDesde.Month, guiaRemision.fechaTrasladoDesde.Day, 0, 0, 0));
            InputParameterAdd.DateTime(objCommand, "fechaTrasladoHasta", new DateTime(guiaRemision.fechaTrasladoHasta.Year, guiaRemision.fechaTrasladoHasta.Month, guiaRemision.fechaTrasladoHasta.Day, 23, 59, 59));
            InputParameterAdd.Int(objCommand, "anulado", guiaRemision.estaAnulado ? 1 : 0);
            InputParameterAdd.Int(objCommand, "facturado", guiaRemision.estaFacturado ? 1 : 0);
            //InputParameterAdd.BigInt(objCommand, "numeroPedido", guiaRemision.pedido.numeroPedido);
            DataTable dataTable = Execute(objCommand);



            foreach (DataRow row in dataTable.Rows)
            {
                //GUIA
                guiaRemision = new GuiaRemision();
                guiaRemision.serieDocumento = Converter.GetString(row, "serie_documento");
                guiaRemision.numeroDocumento = Converter.GetLong(row, "numero_documento");
                guiaRemision.idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");
                guiaRemision.fechaTraslado = Converter.GetDateTime(row, "fecha_traslado");
                guiaRemision.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
                guiaRemision.atencionParcial = Converter.GetBool(row, "atencion_parcial");
                guiaRemision.ultimaAtencionParcial = Converter.GetBool(row, "ultima_atencion_parcial");
                guiaRemision.estaAnulado = Converter.GetBool(row, "anulado");
                guiaRemision.estaFacturado = Converter.GetBool(row, "facturado");
                guiaRemision.estaNoEntregado = Converter.GetBool(row, "no_entregado");

                //PEDIDO
                guiaRemision.pedido = new Pedido();
                guiaRemision.pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                guiaRemision.pedido.numeroPedido = Converter.GetLong(row, "numero_pedido");
                guiaRemision.pedido.numeroGrupoPedido = Converter.GetLong(row, "numero_grupo_pedido");

                //CLIENTE
                guiaRemision.pedido.cliente = new Cliente();
                guiaRemision.pedido.cliente.codigo = Converter.GetString(row, "codigo");
                guiaRemision.pedido.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                guiaRemision.pedido.cliente.razonSocial = Converter.GetString(row, "razon_social");
                guiaRemision.pedido.cliente.ruc = Converter.GetString(row, "ruc");
                guiaRemision.pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");

                guiaRemision.pedido.direccionEntrega = new DireccionEntrega();
                guiaRemision.pedido.direccionEntrega.nombre = Converter.GetString(row, "nombre_direccion_entrega");
                if (guiaRemision.pedido.direccionEntrega.nombre == null) { guiaRemision.pedido.direccionEntrega.nombre = ""; }
                //USUARIO
                guiaRemision.usuario = new Usuario();
                guiaRemision.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                guiaRemision.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                //SEDE
                guiaRemision.ciudadOrigen = new Ciudad();
                guiaRemision.ciudadOrigen.idCiudad = Converter.GetGuid(row, "id_ciudad");
                guiaRemision.ciudadOrigen.nombre = Converter.GetString(row, "nombre_ciudad");
                //ESTADO
                guiaRemision.seguimientoMovimientoAlmacenSalida = new SeguimientoMovimientoAlmacenSalida();
                guiaRemision.seguimientoMovimientoAlmacenSalida.estado = (SeguimientoMovimientoAlmacenSalida.estadosSeguimientoMovimientoAlmacenSalida)Converter.GetInt(row, "estado_movimiento_almacen");
                guiaRemision.seguimientoMovimientoAlmacenSalida.observacion = Converter.GetString(row, "observacion_seguimiento");
                guiaRemision.seguimientoMovimientoAlmacenSalida.usuario = new Usuario();
                guiaRemision.seguimientoMovimientoAlmacenSalida.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento");
                guiaRemision.seguimientoMovimientoAlmacenSalida.usuario.nombre = Converter.GetString(row, "usuario_seguimiento");


                guiaRemisionList.Add(guiaRemision);
            }
            return guiaRemisionList;
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


        public GuiaRemision SelectAjusteAlmacen(GuiaRemision guiaRemision)
        {

            var objCommand = GetSqlCommand("ps_ajusteAlmacen");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", guiaRemision.idMovimientoAlmacen);
            InputParameterAdd.Guid(objCommand, "idUsuario", guiaRemision.usuario.idUsuario);
            DataSet dataSet = ExecuteDataSet(objCommand);

            DataTable guiaRemisionDataTable = dataSet.Tables[0];
            DataTable guiaRemisionDetalleDataTable = dataSet.Tables[1];

            //Datos de la cotizacion
            foreach (DataRow row in guiaRemisionDataTable.Rows)
            {
                //DATOS DE LA GUIA
                guiaRemision.serieDocumento = Converter.GetString(row, "serie_documento");
                guiaRemision.numeroDocumento = Converter.GetLong(row, "numero_documento");
                guiaRemision.idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");
                guiaRemision.fechaTraslado = Converter.GetDateTime(row, "fecha_traslado");
                guiaRemision.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
                guiaRemision.observaciones = Converter.GetString(row, "observaciones");
                guiaRemision.estaAnulado = Converter.GetBool(row, "anulado");
                //guiaRemision.motivoTraslado = (GuiaRemision.motivosTraslado)Char.Parse(Converter.GetString(row, "motivo_traslado"));
                guiaRemision.tipoMovimiento = (GuiaRemision.tiposMovimiento)Char.Parse(Converter.GetString(row, "tipo_movimiento"));
                guiaRemision.FechaRegistro = Converter.GetDateTime(row, "fecha_creacion");
                guiaRemision.ajusteAprobado = Converter.GetInt(row, "ajuste_aprobado");

                guiaRemision.motivoAjuste = new MotivoAjusteAlmacen();
                guiaRemision.motivoAjuste.idMotivoAjusteAlmacen = Converter.GetInt(row, "id_motivo_ajuste_stock");
                guiaRemision.motivoAjuste.descripcion = Converter.GetString(row, "descripcion_motivo");
                guiaRemision.motivoAjuste.tipo = Converter.GetInt(row, "tipo_motivo");

                //USUARIO
                guiaRemision.usuario = new Usuario();
                guiaRemision.usuario.idUsuario = Converter.GetGuid(row, "usuario_creacion");
                guiaRemision.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                guiaRemision.usuario.email = Converter.GetString(row, "email_usuario");
                //SEDE
                guiaRemision.ciudadOrigen = new Ciudad();
                guiaRemision.ciudadOrigen.idCiudad = Converter.GetGuid(row, "id_ciudad");
                guiaRemision.ciudadOrigen.nombre = Converter.GetString(row, "nombre_ciudad"); 
            }


            guiaRemision.documentoDetalle = new List<DocumentoDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in guiaRemisionDetalleDataTable.Rows)
            {
                DocumentoDetalle documentoDetalle = new DocumentoDetalle();
                documentoDetalle.idDocumentoDetalle = Converter.GetGuid(row, "id_movimiento_almacen_detalle");
                documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                documentoDetalle.unidad = Converter.GetString(row, "unidad");
                documentoDetalle.observacion = Converter.GetString(row, "observaciones");

                documentoDetalle.ProductoPresentacion = new ProductoPresentacion();
                documentoDetalle.ProductoPresentacion.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion");
                documentoDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");

                documentoDetalle.producto = new Producto();
                documentoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                documentoDetalle.producto.sku = Converter.GetString(row, "sku");
                documentoDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                documentoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                documentoDetalle.producto.descripcionLarga = Converter.GetString(row, "descripcion_larga");
                documentoDetalle.producto.equivalenciaUnidadEstandarUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_estandar_unidad_conteo");
                documentoDetalle.producto.ventaRestringida = (Producto.TipoVentaRestringida)Converter.GetInt(row, "descontinuado");
                documentoDetalle.producto.motivoRestriccion = Converter.GetString(row, "motivo_restriccion");
                guiaRemision.documentoDetalle.Add(documentoDetalle);
            }

            
            return guiaRemision;
        }

        public List<GuiaRemision> SelectAjustesAlmacen(GuiaRemision guiaRemision)
        {
            List<GuiaRemision> guiaRemisionList = new List<GuiaRemision>();

            var objCommand = GetSqlCommand("ps_ajustesAlmacen");
            InputParameterAdd.Guid(objCommand, "idUsuario", guiaRemision.usuario.idUsuario);
            InputParameterAdd.DateTime(objCommand, "fechaEmisionDesde", guiaRemision.fechaEmisionDesde);
            InputParameterAdd.DateTime(objCommand, "fechaEmisionHasta", guiaRemision.fechaEmisionHasta);
            //InputParameterAdd.Int(objCommand, "estado", guiaRemision.Estado);

            InputParameterAdd.Guid(objCommand, "idSedeOrigen", guiaRemision.ciudadOrigen.idCiudad);
            //InputParameterAdd.Char(objCommand, "motivoTraslado", ((char)guiaRemision.motivoTraslado).ToString());
            InputParameterAdd.Char(objCommand, "tipoMovimiento", ((char)guiaRemision.tipoMovimiento).ToString());
            InputParameterAdd.Int(objCommand, "idMotivoAjuste", guiaRemision.motivoAjuste.idMotivoAjusteAlmacen);
            InputParameterAdd.Int(objCommand, "ajusteAprobado", guiaRemision.ajusteAprobado);


            DataTable dataTable = Execute(objCommand);

            //Datos de la cotizacion
            foreach (DataRow row in dataTable.Rows)
            {
                guiaRemision = new GuiaRemision();
                //DATOS DE LA GUIA
                guiaRemision.serieDocumento = Converter.GetString(row, "serie_documento");
                guiaRemision.numeroDocumento = Converter.GetLong(row, "numero_documento");
                guiaRemision.idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");
                guiaRemision.fechaTraslado = Converter.GetDateTime(row, "fecha_traslado");
                guiaRemision.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
                guiaRemision.observaciones = Converter.GetString(row, "observaciones");
                guiaRemision.estaAnulado = Converter.GetBool(row, "anulado");
                //guiaRemision.motivoTraslado = (GuiaRemision.motivosTraslado)Char.Parse(Converter.GetString(row, "motivo_traslado"));
                guiaRemision.tipoMovimiento = (GuiaRemision.tiposMovimiento)Char.Parse(Converter.GetString(row, "tipo_movimiento"));
                guiaRemision.FechaRegistro = Converter.GetDateTime(row, "fecha_creacion");
                guiaRemision.ajusteAprobado = Converter.GetInt(row, "ajuste_aprobado");

                guiaRemision.motivoAjuste = new MotivoAjusteAlmacen();
                guiaRemision.motivoAjuste.idMotivoAjusteAlmacen = Converter.GetInt(row, "id_motivo_ajuste_stock");
                guiaRemision.motivoAjuste.descripcion = Converter.GetString(row, "descripcion_motivo");
                guiaRemision.motivoAjuste.tipo = Converter.GetInt(row, "tipo_motivo");

                //USUARIO
                guiaRemision.usuario = new Usuario();
                guiaRemision.usuario.idUsuario = Converter.GetGuid(row, "usuario_creacion");
                guiaRemision.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                guiaRemision.usuario.email = Converter.GetString(row, "email_usuario");
                //SEDE
                guiaRemision.ciudadOrigen = new Ciudad();
                guiaRemision.ciudadOrigen.idCiudad = Converter.GetGuid(row, "id_ciudad");
                guiaRemision.ciudadOrigen.nombre = Converter.GetString(row, "nombre_ciudad");

                guiaRemisionList.Add(guiaRemision);
            }

            return guiaRemisionList;
        }

        public Guid getIdDocumentoVentaRelacionado(Guid idGuiaRemision)
        {

            var objCommand = GetSqlCommand("ps_idDocumentoVentaMovRelacionado");
            InputParameterAdd.Guid(objCommand, "idMov", idGuiaRemision);
            DataTable dataTable = Execute(objCommand);
            Guid idDocumentoVenta = Guid.Empty;

            foreach (DataRow row in dataTable.Rows)
            {
                idDocumentoVenta = Converter.GetGuid(row, "id_documento_venta");
            }


            return idDocumentoVenta;
        }



        /*NOTAS DE INGRESO*/


        public NotaIngreso SelectNotaIngreso(NotaIngreso notaIngreso)
        {

            var objCommand = GetSqlCommand("ps_notaIngreso");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", notaIngreso.idMovimientoAlmacen);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable notaIngresoDataTable = dataSet.Tables[0];
            DataTable notaIngresoDetalleDataTable = dataSet.Tables[1];

            //Datos de la cotizacion
            foreach (DataRow row in notaIngresoDataTable.Rows)
            {
                //DATOS DE LA GUIA
                notaIngreso.serieDocumento = Converter.GetString(row, "serie_documento");
                notaIngreso.numeroDocumento = Converter.GetLong(row, "numero_documento");
                notaIngreso.idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");
                notaIngreso.fechaTraslado = Converter.GetDateTime(row, "fecha_traslado");
                notaIngreso.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
            //    notaIngreso.atencionParcial = Converter.GetBool(row, "atencion_parcial");
            //    notaIngreso.ultimaAtencionParcial = Converter.GetBool(row, "ultima_atencion_parcial");
                notaIngreso.observaciones = Converter.GetString(row, "observaciones");
                notaIngreso.estaAnulado = Converter.GetBool(row, "anulado");
                notaIngreso.estaFacturado = Converter.GetBool(row, "facturado");
                notaIngreso.serieGuiaReferencia = Converter.GetString(row, "serie_guia_referencia");
                notaIngreso.numeroGuiaReferencia = Converter.GetInt(row, "numero_guia_referencia");
                notaIngreso.serieDocumentoVentaReferencia = Converter.GetString(row, "serie_documento_venta_referencia");
                notaIngreso.numeroDocumentoVentaReferencia = Converter.GetInt(row, "numero_documento_venta_referencia");
                notaIngreso.tipoDocumentoVentaReferencia = (NotaIngreso.TiposDocumentoVentaReferencia)Converter.GetInt(row, "tipo_documento_venta_referencia");
                
                notaIngreso.motivoTraslado = (NotaIngreso.motivosTraslado)Char.Parse(Converter.GetString(row, "motivo_traslado"));


                //PEDIDO
                notaIngreso.pedido = new Pedido();
                notaIngreso.pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                notaIngreso.pedido.numeroPedido = Converter.GetLong(row, "numero");
                notaIngreso.pedido.direccionEntrega = new DireccionEntrega();
                notaIngreso.pedido.direccionEntrega.descripcion = Converter.GetString(row, "direccion_entrega");
                notaIngreso.pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                //UBIGEO
                notaIngreso.pedido.ubigeoEntrega = new Ubigeo();
                notaIngreso.pedido.ubigeoEntrega.Departamento = Converter.GetString(row, "departamento");
                notaIngreso.pedido.ubigeoEntrega.Provincia = Converter.GetString(row, "provincia");
                notaIngreso.pedido.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

                //CLIENTE
                notaIngreso.pedido.cliente = new Cliente();
                notaIngreso.pedido.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                notaIngreso.pedido.cliente.codigo = Converter.GetString(row, "codigo");
                notaIngreso.pedido.cliente.razonSocial = Converter.GetString(row, "razon_social");
                notaIngreso.pedido.cliente.ruc = Converter.GetString(row, "ruc");
                notaIngreso.pedido.cliente.domicilioLegal = Converter.GetString(row, "domicilio_legal");
                //USUARIO
                notaIngreso.usuario = new Usuario();
                notaIngreso.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                notaIngreso.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                //SEDE
                notaIngreso.ciudadDestino = new Ciudad();
                notaIngreso.ciudadDestino.idCiudad = Converter.GetGuid(row, "id_ciudad");
                notaIngreso.ciudadDestino.nombre = Converter.GetString(row, "nombre_ciudad");
                notaIngreso.ciudadDestino.direccionPuntoLlegada = Converter.GetString(row, "direccion_punto_llegada");
                notaIngreso.ciudadDestino.esProvincia = Converter.GetBool(row, "es_provincia");
                notaIngreso.ciudadDestino.sede = Converter.GetString(row, "sede");
                //ALMACEN
                notaIngreso.almacen = new Almacen();
                notaIngreso.almacen.idAlmacen = Converter.GetGuid(row, "id_almacen");
                notaIngreso.almacen.direccion = Converter.GetString(row, "direccion_almacen");
                notaIngreso.almacen.ubigeo = new Ubigeo();
                notaIngreso.almacen.ubigeo.Id = Converter.GetString(row, "ubigeo_almacen");
                notaIngreso.almacen.codigoSucursalNextSoft = Converter.GetString(row, "codigo_sucursal_nextsoft_almacen");
                notaIngreso.almacen.codigoPuntoVentaNextSoft = Converter.GetString(row, "codigo_punto_venta_nextsoft_almacen");
                notaIngreso.almacen.codigoAlmacenNextSoft = Converter.GetString(row, "codigo_almacen_nextsoft_almacen");

                //TRANSPORTISTA
                notaIngreso.transportista = new Transportista();
                notaIngreso.transportista.brevete = Converter.GetString(row, "brevete_transportista");
                notaIngreso.transportista.direccion = Converter.GetString(row, "direccion_transportista");
                notaIngreso.transportista.ruc = Converter.GetString(row, "ruc_transportista");
                notaIngreso.transportista.descripcion = Converter.GetString(row, "nombre_transportista");

                //ADICIONALES
                notaIngreso.placaVehiculo = Converter.GetString(row, "placa_vehiculo");

                if (Converter.GetInt(row, "motivo_extorno") > 0)
                {
                    notaIngreso.motivoExtornoGuiaRemision = (NotaIngreso.MotivosExtornoGuiaRemision)Converter.GetInt(row, "motivo_extorno");
                             
                    notaIngreso.sustentoExtorno = Converter.GetString(row, "sustento_extorno");

                    notaIngreso.guiaRemisionAExtornar = new GuiaRemision();
                    notaIngreso.guiaRemisionAExtornar.idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen_extornado");
                    notaIngreso.guiaRemisionAExtornar.serieDocumento = Converter.GetString(row, "movimiento_almacen_extorno_serie_documento");
                    notaIngreso.guiaRemisionAExtornar.numeroDocumento = Converter.GetInt(row, "movimiento_almacen_extorno_numero_documento");
                }

                if (notaIngreso.motivoTraslado == NotaIngreso.motivosTraslado.TrasladoInterno)
                {
                    notaIngreso.guiaRemisionAIngresar = new GuiaRemision();
                    notaIngreso.guiaRemisionAIngresar.idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen_ingresado");
                    notaIngreso.guiaRemisionAIngresar.serieDocumento = Converter.GetString(row, "movimiento_almacen_ingresado_serie_documento");
                    notaIngreso.guiaRemisionAIngresar.numeroDocumento = Converter.GetInt(row, "movimiento_almacen_ingresado_numero_documento");
                }

                //  notaIngreso.certificadoInscripcion = Converter.GetString(row, "certificado_inscripcion");
                notaIngreso.tipoExtorno = (NotaIngreso.TiposExtorno)Converter.GetInt(row, "tipo_extorno");
            }


            notaIngreso.documentoDetalle = new List<DocumentoDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in notaIngresoDetalleDataTable.Rows)
            {
                DocumentoDetalle documentoDetalle = new DocumentoDetalle();
                documentoDetalle.idDocumentoDetalle = Converter.GetGuid(row, "id_movimiento_almacen_detalle");
                documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                documentoDetalle.unidad = Converter.GetString(row, "unidad");
                documentoDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");

                if (documentoDetalle.esPrecioAlternativo)
                {
                    documentoDetalle.ProductoPresentacion = new ProductoPresentacion();
                    documentoDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                    documentoDetalle.ProductoPresentacion.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion");
                }


                documentoDetalle.producto = new Producto();
                documentoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                documentoDetalle.producto.sku = Converter.GetString(row, "sku");
                documentoDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                documentoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");

                documentoDetalle.producto.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia_alternativa");
                documentoDetalle.producto.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");

                documentoDetalle.producto.tipoProducto = (Producto.TipoProducto)Converter.GetInt(row, "tipo_producto"); 
                documentoDetalle.producto.codigoNextSoft = Converter.GetString(row, "codigo_nextsoft");
                documentoDetalle.producto.codigoFactorUnidadMP = Converter.GetString(row, "codigo_factor_unidad_mp");
                documentoDetalle.producto.codigoFactorUnidadAlternativa = Converter.GetString(row, "codigo_factor_unidad_alternativa");
                documentoDetalle.producto.codigoFactorUnidadProveedor = Converter.GetString(row, "codigo_factor_unidad_proveedor");
                documentoDetalle.producto.codigoFactorUnidadConteo = Converter.GetString(row, "codigo_factor_unidad_conteo");

                notaIngreso.documentoDetalle.Add(documentoDetalle);
            }

            return notaIngreso;
        }

 

        public List<NotaIngreso> SelectNotasIngreso(NotaIngreso notaIngreso)
        {
            List<NotaIngreso> notaIngresoList = new List<NotaIngreso>();

            var objCommand = GetSqlCommand("ps_notasIngreso");
            InputParameterAdd.BigInt(objCommand, "numeroDocumento", notaIngreso.numeroDocumento);
            InputParameterAdd.Guid(objCommand, "idCiudad", notaIngreso.ciudadDestino.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", notaIngreso.pedido.cliente.idCliente);
            InputParameterAdd.Int(objCommand, "idGrupoCliente", notaIngreso.pedido.idGrupoCliente);
            InputParameterAdd.Bit(objCommand, "buscaSedesGrupoCliente", notaIngreso.pedido.buscarSedesGrupoCliente);
            InputParameterAdd.Guid(objCommand, "idUsuario", notaIngreso.usuario.idUsuario);
            InputParameterAdd.DateTime(objCommand, "fechaEmisionDesde", notaIngreso.fechaEmisionDesde);
            InputParameterAdd.DateTime(objCommand, "fechaEmisionHasta", notaIngreso.fechaEmisionHasta);
            //InputParameterAdd.DateTime(objCommand, "fechaTrasladoDesde", notaIngreso.fechaTrasladoDesde);
            //InputParameterAdd.DateTime(objCommand, "fechaTrasladoHasta", notaIngreso.fechaTrasladoHasta);
            InputParameterAdd.Int(objCommand, "anulado", notaIngreso.estaAnulado ? 1 : 0);
            InputParameterAdd.Int(objCommand, "numeroGuiaReferencia", notaIngreso.numeroGuiaReferencia);
            InputParameterAdd.BigInt(objCommand, "numeroPedido", notaIngreso.pedido.numeroPedido);
            InputParameterAdd.Char(objCommand, "motivoTraslado", ((Char)notaIngreso.motivoTrasladoBusqueda).ToString());
            InputParameterAdd.Int(objCommand, "estadoFiltro", (int) notaIngreso.estadoFiltro);
            InputParameterAdd.Varchar(objCommand, "sku", notaIngreso.sku);
            DataTable dataTable = Execute(objCommand);



            foreach (DataRow row in dataTable.Rows)
            {
                //GUIA
                notaIngreso = new NotaIngreso();
                notaIngreso.serieDocumento = Converter.GetString(row, "serie_documento");
                notaIngreso.numeroDocumento = Converter.GetLong(row, "numero_documento");
                notaIngreso.idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");
                notaIngreso.fechaTraslado = Converter.GetDateTime(row, "fecha_traslado");
                notaIngreso.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
           //     notaIngreso.atencionParcial = Converter.GetBool(row, "atencion_parcial");
           //     notaIngreso.ultimaAtencionParcial = Converter.GetBool(row, "ultima_atencion_parcial");
                notaIngreso.estaAnulado = Converter.GetBool(row, "anulado");
                notaIngreso.estaFacturado = Converter.GetBool(row, "facturado");
                notaIngreso.estaNoEntregado = Converter.GetBool(row, "no_entregado");
                notaIngreso.motivoTraslado = (NotaIngreso.motivosTraslado)Char.Parse(Converter.GetString(row, "motivo_traslado"));
                //PEDIDO
                notaIngreso.pedido = new Pedido();
                notaIngreso.pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                notaIngreso.pedido.numeroPedido = Converter.GetLong(row, "numero_pedido");
                //CLIENTE
                notaIngreso.pedido.cliente = new Cliente();
                notaIngreso.pedido.cliente.codigo = Converter.GetString(row, "codigo");
                notaIngreso.pedido.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                notaIngreso.pedido.cliente.razonSocial = Converter.GetString(row, "razon_social");
                notaIngreso.pedido.cliente.ruc = Converter.GetString(row, "ruc");
                //USUARIO
                notaIngreso.usuario = new Usuario();
                notaIngreso.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                notaIngreso.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                //SEDE
                notaIngreso.ciudadDestino = new Ciudad();
                notaIngreso.ciudadDestino.idCiudad = Converter.GetGuid(row, "id_ciudad");
                notaIngreso.ciudadDestino.nombre = Converter.GetString(row, "nombre_ciudad");
                //ESTADO
                notaIngreso.seguimientoMovimientoAlmacenSalida = new SeguimientoMovimientoAlmacenSalida();
                notaIngreso.seguimientoMovimientoAlmacenSalida.estado = (SeguimientoMovimientoAlmacenSalida.estadosSeguimientoMovimientoAlmacenSalida)Converter.GetInt(row, "estado_movimiento_almacen");
                notaIngreso.seguimientoMovimientoAlmacenSalida.observacion = Converter.GetString(row, "observacion_seguimiento");
                notaIngreso.seguimientoMovimientoAlmacenSalida.usuario = new Usuario();
                notaIngreso.seguimientoMovimientoAlmacenSalida.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento");
                notaIngreso.seguimientoMovimientoAlmacenSalida.usuario.nombre = Converter.GetString(row, "usuario_seguimiento");


                notaIngreso.tipoExtorno = (GuiaRemision.TiposExtorno)Converter.GetInt(row, "tipo_extorno");

                notaIngresoList.Add(notaIngreso);
            }
            return notaIngresoList;
        }


        public List<DocumentoDetalle> SelectMovimientoAlmacenCantidadesExtornadas(MovimientoAlmacen movimientoAlmacen)
        {
            var objCommand = GetSqlCommand("ps_movimientoAlmacenCantidadesExtornadas");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", movimientoAlmacen.idMovimientoAlmacen);
            DataTable dataTable = Execute(objCommand);
            List<DocumentoDetalle> documentoDetalleList = new List<DocumentoDetalle>();
            foreach (DataRow row in dataTable.Rows)
            {
                DocumentoDetalle documentoDetalle = new DocumentoDetalle();
                documentoDetalle.producto = new Producto();
                documentoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                documentoDetalleList.Add(documentoDetalle);
            }
            return documentoDetalleList;
        }

        public Guid SelectMovimientoAlmacenIdCpeCabeceraBe(MovimientoAlmacen movimientoAlmacen)
        {
            var objCommand = GetSqlCommand("ps_movimientoAlmacenObtenerIdCPECabeceraBE");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", movimientoAlmacen.idMovimientoAlmacen);
            DataTable dataTable = Execute(objCommand);
            List<DocumentoDetalle> documentoDetalleList = new List<DocumentoDetalle>();
            Guid idCpeCabeceraBe = Guid.Empty;
            foreach (DataRow row in dataTable.Rows)
            {
                idCpeCabeceraBe =  Converter.GetGuid(row, "id_cpe_cabecera_be");
            }
            return idCpeCabeceraBe;
        }


    }
}
