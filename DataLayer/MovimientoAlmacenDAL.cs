using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;
using Model.EXCEPTION;

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

        public void InsertMovimientoAlmacenSalida(GuiaRemision guiaRemision)
        {
            var objCommand = GetSqlCommand("pi_movimientoAlmacenSalida");
            InputParameterAdd.DateTime(objCommand, "fechaEmision", guiaRemision.fechaEmision);
            InputParameterAdd.DateTime(objCommand, "fechaTraslado", guiaRemision.fechaTraslado);
            InputParameterAdd.Varchar(objCommand, "serieDocumento", guiaRemision.serieDocumento); //puede ser null
            InputParameterAdd.BigInt(objCommand, "numeroDocumento", guiaRemision.numeroDocumento); //puede ser null
            InputParameterAdd.Guid(objCommand, "idPedido", guiaRemision.pedido.idPedido);
            InputParameterAdd.Int(objCommand, "atencionParcial", guiaRemision.atencionParcial ? 1 : 0);
            InputParameterAdd.Int(objCommand, "ultimaAtencionParcial", guiaRemision.ultimaAtencionParcial ? 1 : 0);
            InputParameterAdd.Guid(objCommand, "idSedeOrigen", guiaRemision.ciudadOrigen.idCiudad);
            InputParameterAdd.Char(objCommand, "ubigeoEntrega", guiaRemision.pedido.ubigeoEntrega.Id);
            InputParameterAdd.Varchar(objCommand, "direccionEntrega", guiaRemision.pedido.direccionEntrega.descripcion);
            InputParameterAdd.Char(objCommand, "motivoTraslado", ((char)guiaRemision.motivoTraslado).ToString());
            InputParameterAdd.Guid(objCommand, "idTransportista", guiaRemision.transportista.idTransportista);
            InputParameterAdd.Varchar(objCommand, "nombreTransportista", guiaRemision.transportista.descripcion);
            InputParameterAdd.Varchar(objCommand, "rucTransportista", guiaRemision.transportista.ruc);
            InputParameterAdd.Varchar(objCommand, "breveteTransportista", guiaRemision.transportista.brevete);
            InputParameterAdd.Varchar(objCommand, "direccionTransportista", guiaRemision.transportista.direccion);
            InputParameterAdd.Varchar(objCommand, "placaVehiculo", guiaRemision.placaVehiculo);
            InputParameterAdd.Varchar(objCommand, "observaciones", guiaRemision.observaciones);
            InputParameterAdd.Varchar(objCommand, "certificadoInscripcion", guiaRemision.certificadoInscripcion);
            InputParameterAdd.Guid(objCommand, "idUsuario", guiaRemision.usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "estado", (int)guiaRemision.seguimientoMovimientoAlmacenSalida.estado);
            InputParameterAdd.Varchar(objCommand, "observacionSeguimiento", guiaRemision.seguimientoMovimientoAlmacenSalida.observacion);



            OutputParameterAdd.UniqueIdentifier(objCommand, "idMovimientoAlmacen");
            OutputParameterAdd.UniqueIdentifier(objCommand, "idVenta");
            OutputParameterAdd.BigInt(objCommand, "numeroMovimientoAlmacen");
            OutputParameterAdd.BigInt(objCommand, "numeroVenta");
            OutputParameterAdd.Int(objCommand, "siguienteNumeroGuiaRemision");
            ExecuteNonQuery(objCommand);

            guiaRemision.idMovimientoAlmacen = (Guid)objCommand.Parameters["@idMovimientoAlmacen"].Value;
            guiaRemision.numero = (Int64)objCommand.Parameters["@numeroMovimientoAlmacen"].Value;
            int siguienteNumeroGuiaRemision = (int)objCommand.Parameters["@siguienteNumeroGuiaRemision"].Value;
            

            guiaRemision.venta = new Venta();
            guiaRemision.venta.idVenta = (Guid)objCommand.Parameters["@idVenta"].Value;
            guiaRemision.venta.numero = (Int64)objCommand.Parameters["@numeroVenta"].Value;
            this.InsertMovimientoAlmacenDetalle(guiaRemision);

            if (guiaRemision.numeroDocumento != siguienteNumeroGuiaRemision)
            {
                throw new DuplicateNumberDocumentException();
            }


        }


        public void InsertMovimientoAlmacenDetalle(GuiaRemision guiaRemision)
        {

            foreach (DocumentoDetalle documentoDetalle in guiaRemision.pedido.documentoDetalle)
            {
                if(documentoDetalle.cantidadPorAtender > 0)
                { 
                    var objCommand = GetSqlCommand("pi_movimientoAlmacenDetalleSalida");
                    InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", guiaRemision.idMovimientoAlmacen);
                    InputParameterAdd.Guid(objCommand, "idVenta", guiaRemision.venta.idVenta);
                    InputParameterAdd.Guid(objCommand, "idUsuario", guiaRemision.usuario.idUsuario);
                    InputParameterAdd.Guid(objCommand, "idPedido", guiaRemision.pedido.idPedido);
                    InputParameterAdd.Char(objCommand, "tipoPedido", ((char)guiaRemision.pedido.tipoPedido).ToString());
                    InputParameterAdd.Guid(objCommand, "idProducto", documentoDetalle.producto.idProducto);
                    InputParameterAdd.Int(objCommand, "cantidad", documentoDetalle.cantidadPorAtender);
                    InputParameterAdd.Varchar(objCommand, "observaciones", documentoDetalle.observacion);
                    OutputParameterAdd.UniqueIdentifier(objCommand, "idMovimientoAlmacenDetalle");
                    ExecuteNonQuery(objCommand);
                    Guid idMovimientoAlmacenDetalle = (Guid)objCommand.Parameters["@idMovimientoAlmacenDetalle"].Value;
                }
            }
        
      
        }



        public void AnularMovimientoAlmacen(MovimientoAlmacen movimientoAlmacen)
        {
            var objCommand = GetSqlCommand("pu_anularMovimientoAlmacen");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", movimientoAlmacen.idMovimientoAlmacen);
            InputParameterAdd.Varchar(objCommand, "comentarioAnulado", movimientoAlmacen.comentarioAnulado);
            InputParameterAdd.Guid(objCommand, "idUsuario", movimientoAlmacen.usuario.idUsuario);
            ExecuteNonQuery(objCommand);

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




        public GuiaRemision SelectGuiaRemision(GuiaRemision guiaRemision)
        {
           
            var objCommand = GetSqlCommand("ps_guiaRemision");
            InputParameterAdd.Guid(objCommand, "idMovimientoAlmacen", guiaRemision.idMovimientoAlmacen);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable pedidoDataTable = dataSet.Tables[0];
            DataTable pedidoDetalleDataTable = dataSet.Tables[1];
    
            //Datos de la cotizacion
            foreach (DataRow row in pedidoDataTable.Rows)
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
                guiaRemision.motivoTraslado = (GuiaRemision.motivosTraslado) Char.Parse(Converter.GetString(row, "motivo_traslado"));

              

                //PEDIDO
                guiaRemision.pedido = new Pedido();
                guiaRemision.pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                guiaRemision.pedido.numeroPedido = Converter.GetLong(row, "numero");
                guiaRemision.pedido.direccionEntrega = new DireccionEntrega();
                guiaRemision.pedido.direccionEntrega.descripcion = Converter.GetString(row, "direccion_entrega");  
                guiaRemision.pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                //UBIGEO
                guiaRemision.pedido.ubigeoEntrega = new Ubigeo();
                guiaRemision.pedido.ubigeoEntrega.Departamento = Converter.GetString(row, "departamento");
                guiaRemision.pedido.ubigeoEntrega.Provincia = Converter.GetString(row, "provincia");
                guiaRemision.pedido.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

                //CLIENTE
                guiaRemision.pedido.cliente = new Cliente();
                guiaRemision.pedido.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                guiaRemision.pedido.cliente.codigo = Converter.GetString(row, "codigo");
                guiaRemision.pedido.cliente.razonSocial = Converter.GetString(row, "razon_social");
                guiaRemision.pedido.cliente.ruc = Converter.GetString(row, "ruc");
                guiaRemision.pedido.cliente.domicilioLegal = Converter.GetString(row, "domicilio_legal");
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
                //TRANSPORTISTA
                guiaRemision.transportista = new Transportista();
                guiaRemision.transportista.brevete = Converter.GetString(row, "brevete_transportista");
                guiaRemision.transportista.direccion = Converter.GetString(row, "direccion_transportista");
                guiaRemision.transportista.ruc = Converter.GetString(row, "ruc_transportista");
                guiaRemision.transportista.descripcion = Converter.GetString(row, "nombre_transportista");

                //ADICIONALES
                guiaRemision.placaVehiculo = Converter.GetString(row, "placa_vehiculo");
                guiaRemision.certificadoInscripcion = Converter.GetString(row, "certificado_inscripcion");
                
            }


            guiaRemision.documentoDetalle = new List<DocumentoDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in pedidoDetalleDataTable.Rows)
            {
                DocumentoDetalle documentoDetalle = new DocumentoDetalle();
                documentoDetalle.idDocumentoDetalle = Converter.GetGuid(row, "id_movimiento_almacen_detalle");
                documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                documentoDetalle.unidad = Converter.GetString(row, "unidad");
                documentoDetalle.producto = new Producto();
                documentoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                documentoDetalle.producto.sku = Converter.GetString(row, "sku");
                documentoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                guiaRemision.documentoDetalle.Add(documentoDetalle);
            }        
               
            return guiaRemision;
        }

        public List<GuiaRemision> SelectGuiasRemision(GuiaRemision guiaRemision)
        {
            List<GuiaRemision> guiaRemisionList = new List<GuiaRemision>();
            
            var objCommand = GetSqlCommand("ps_guiasRemision");
            InputParameterAdd.BigInt(objCommand, "numeroDocumento", guiaRemision.numeroDocumento);
            InputParameterAdd.Guid(objCommand, "idCiudad", guiaRemision.ciudadOrigen.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", guiaRemision.pedido.cliente.idCliente);
            InputParameterAdd.Guid(objCommand, "idUsuario", guiaRemision.usuario.idUsuario);
            InputParameterAdd.DateTime(objCommand, "fechaTrasladoDesde", guiaRemision.fechaTrasladoDesde);
            InputParameterAdd.DateTime(objCommand, "fechaTrasladoHasta", guiaRemision.fechaTrasladoHasta);
            InputParameterAdd.Int(objCommand, "anulado", guiaRemision.estaAnulado?1:0);
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

                //PEDIDO
                guiaRemision.pedido = new Pedido();
                guiaRemision.pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                guiaRemision.pedido.numeroPedido = Converter.GetLong(row, "numero_pedido");
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
        
    }
}
