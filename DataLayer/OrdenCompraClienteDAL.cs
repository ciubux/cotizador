
using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Model;
using System.Linq;

namespace DataLayer
{
    public class OrdenCompraClienteDAL : DaoBase
    {
        public OrdenCompraClienteDAL(IDalSettings settings) : base(settings)
        {
        }

        public OrdenCompraClienteDAL() : this(new CotizadorSettings())
        {
        }

        #region OrdenCompraClientes de Venta

        public void InsertOrdenCompraCliente(OrdenCompraCliente occ)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pi_ordenCompraCliente");

            if (occ.cotizacion.idCotizacion == Guid.Empty)
                InputParameterAdd.Guid(objCommand, "idCotizacion", null); //puede ser null
            else
                InputParameterAdd.Guid(objCommand, "idCotizacion", occ.cotizacion.idCotizacion); //puede ser null

            InputParameterAdd.Guid(objCommand, "idCiudad", occ.ciudad.idCiudad);
            //InputParameterAdd.Guid(objCommand, "idCliente", occ.cliente.idCliente);
            InputParameterAdd.Int(objCommand, "idClienteSunat", occ.clienteSunat.idClienteSunat);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", occ.numeroReferenciaCliente); //puede ser null

            
            InputParameterAdd.DateTime(objCommand, "fechaSolicitud", occ.fechaSolicitud);
            
            DateTime dtTmp = DateTime.Now;
            

            InputParameterAdd.Guid(objCommand, "idSolicitante", occ.solicitante.idSolicitante);
            InputParameterAdd.Varchar(objCommand, "contactoOrdenCompraCliente", occ.solicitante.nombre);  //puede ser null
            InputParameterAdd.Varchar(objCommand, "telefonoContactoOrdenCompraCliente", occ.solicitante.telefono);  //puede ser null
            InputParameterAdd.Varchar(objCommand, "correoContactoOrdenCompraCliente", occ.solicitante.correo);  //puede ser null


            InputParameterAdd.SmallInt(objCommand, "incluidoIGV", (short)(occ.incluidoIGV?1:0));
            InputParameterAdd.Decimal(objCommand, "tasaIGV", Constantes.IGV);
            InputParameterAdd.Decimal(objCommand, "igv", occ.montoIGV);
            InputParameterAdd.Decimal(objCommand, "total", occ.montoTotal);
            InputParameterAdd.Varchar(objCommand, "observaciones", occ.observaciones);  //puede ser null
            InputParameterAdd.Guid(objCommand, "idUsuario", occ.usuario.idUsuario);


            //InputParameterAdd.Decimal(objCommand, "otrosCargos", occ.otrosCargos);
            //InputParameterAdd.Varchar(objCommand, "numeroRequerimiento", occ.numeroRequerimiento);

            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            OutputParameterAdd.BigInt(objCommand, "numero");
            ExecuteNonQuery(objCommand);

            occ.idOrdenCompraCliente = (Guid)objCommand.Parameters["@newId"].Value;
            occ.numeroOrdenCompraCliente = (Int64)objCommand.Parameters["@numero"].Value;


            foreach (OrdenCompraClienteDetalle occDetalle in occ.detalleList)
            {
                occDetalle.idOrdenCompraCliente = occ.idOrdenCompraCliente;
                this.InsertOrdenCompraClienteDetalle(occDetalle, occ.usuario);
            }

            foreach (ArchivoAdjunto occAdjunto in occ.AdjuntoList)
            {
                //occAdjunto.idOrdenCompraCliente = occ.idOrdenCompraCliente;
                //this.InsertOrdenCompraClienteAdjunto(occAdjunto);
            }
            this.Commit();
        }
        

        public void UpdateOrdenCompraCliente(OrdenCompraCliente occ)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pu_ordenCompraCliente");

            InputParameterAdd.Guid(objCommand, "idOrdenCompraCliente", occ.idOrdenCompraCliente); //puede ser null

            //if (occ.cotizacion.idCotizacion == Guid.Empty)
            //    InputParameterAdd.Guid(objCommand, "idCotizacion", null); //puede ser null
            //else
            //    InputParameterAdd.Guid(objCommand, "idCotizacion", occ.cotizacion.idCotizacion); //puede ser null

            InputParameterAdd.Guid(objCommand, "idCiudad", occ.ciudad.idCiudad);
            InputParameterAdd.Int(objCommand, "idClienteSunat", occ.clienteSunat.idClienteSunat);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", occ.numeroReferenciaCliente); //puede ser null


            InputParameterAdd.DateTime(objCommand, "fechaSolicitud", occ.fechaSolicitud);
            

            InputParameterAdd.Guid(objCommand, "idSolicitante", occ.solicitante.idSolicitante);
            InputParameterAdd.Varchar(objCommand, "contactoOrdenCompraCliente", occ.solicitante.nombre);  //puede ser null
            InputParameterAdd.Varchar(objCommand, "telefonoContactoOrdenCompraCliente", occ.solicitante.telefono);  //puede ser null
            InputParameterAdd.Varchar(objCommand, "correoContactoOrdenCompraCliente", occ.solicitante.correo);  //puede ser null


            InputParameterAdd.SmallInt(objCommand, "incluidoIGV", (short)(occ.incluidoIGV ? 1 : 0));
            InputParameterAdd.Decimal(objCommand, "tasaIGV", Constantes.IGV);
            InputParameterAdd.Decimal(objCommand, "igv", occ.montoIGV);
            InputParameterAdd.Decimal(objCommand, "total", occ.montoTotal);
            InputParameterAdd.Varchar(objCommand, "observaciones", occ.observaciones);  //puede ser null
            InputParameterAdd.Guid(objCommand, "idUsuario", occ.usuario.idUsuario);


            ExecuteNonQuery(objCommand);


            foreach (OrdenCompraClienteDetalle occDetalle in occ.detalleList)
            {
                occDetalle.idOrdenCompraCliente = occ.idOrdenCompraCliente;
                //occDetalle.usuario = occ.usuario;
                this.InsertOrdenCompraClienteDetalle(occDetalle, occ.usuario);
            }
            
            this.Commit();
        }           
    
        #endregion


        #region General


        public void InsertOrdenCompraClienteAdjunto(Guid idOrdenCompraCliente, ArchivoAdjunto occAdjunto)
        {
            var objCommand = GetSqlCommand("pi_ordenCompraClienteAdjunto");
            InputParameterAdd.Guid(objCommand, "idOrdenCompraCliente", idOrdenCompraCliente);
            InputParameterAdd.Guid(objCommand, "idArchivoAdjunto", occAdjunto.idArchivoAdjunto);
            InputParameterAdd.Varchar(objCommand, "nombre", occAdjunto.nombre);
            InputParameterAdd.VarBinary(objCommand, "adjunto", occAdjunto.adjunto);

            InputParameterAdd.Guid(objCommand, "idUsuario", occAdjunto.usuario.idUsuario);
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            ExecuteNonQuery(objCommand);

            occAdjunto.idArchivoAdjunto = (Guid)objCommand.Parameters["@newId"].Value;
        }

        public void InsertOrdenCompraClienteDetalle(OrdenCompraClienteDetalle occDetalle, Usuario usuario)
        {
            var objCommand = GetSqlCommand("pi_ordenCompraClienteDetalle");
            InputParameterAdd.Guid(objCommand, "idOrdenCompraCliente", occDetalle.idOrdenCompraCliente);
            InputParameterAdd.Guid(objCommand, "idProducto", occDetalle.producto.idProducto);
            InputParameterAdd.Int(objCommand, "cantidad", occDetalle.cantidad);
            //Siempre se almacena el precio sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "precioSinIGV", occDetalle.producto.precioSinIgv);
            //Siempre se almacena el costo sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "costoSinIGV", occDetalle.producto.costoSinIgv);
            if(occDetalle.esPrecioAlternativo)
                InputParameterAdd.Decimal(objCommand, "equivalencia", occDetalle.ProductoPresentacion.Equivalencia);            
            else
                InputParameterAdd.Decimal(objCommand, "equivalencia", 1);
            InputParameterAdd.Varchar(objCommand, "unidad", occDetalle.unidad);
            InputParameterAdd.Decimal(objCommand, "porcentajeDescuento", occDetalle.porcentajeDescuento);
            InputParameterAdd.Decimal(objCommand, "precioNeto", occDetalle.precioNeto);
            InputParameterAdd.Int(objCommand, "esPrecioAlternativo", occDetalle.esPrecioAlternativo?1:0);
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            InputParameterAdd.Decimal(objCommand, "flete", occDetalle.flete);
            InputParameterAdd.Varchar(objCommand, "observaciones", occDetalle.observacion);
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            if (occDetalle.esPrecioAlternativo)
            {
                InputParameterAdd.Int(objCommand, "idProductoPresentacion", occDetalle.ProductoPresentacion.IdProductoPresentacion);
            }
            else
            {
                InputParameterAdd.Int(objCommand, "idProductoPresentacion", 0);
            }

            InputParameterAdd.Int(objCommand, "indicadorAprobacion", (int)occDetalle.indicadorAprobacion);

            ExecuteNonQuery(objCommand);
          
            occDetalle.idOrdenCompraClienteDetalle = (Guid)objCommand.Parameters["@newId"].Value;
        }

        public OrdenCompraCliente ProgramarOrdenCompraCliente(OrdenCompraCliente occ, Usuario usuario)
        {
            var objCommand = GetSqlCommand("pu_programarOrdenCompraCliente");

            InputParameterAdd.Guid(objCommand, "idOrdenCompraCliente", occ.idOrdenCompraCliente);
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            InputParameterAdd.DateTime(objCommand, "fechaProgramacion", occ.fechaProgramacion);
            InputParameterAdd.Varchar(objCommand, "comentarioProgramacion", occ.comentarioProgramacion);
            ExecuteNonQuery(objCommand);
            return occ;
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
                
                cotizacionDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");

                if (cotizacionDetalle.esPrecioAlternativo)
                {
                    cotizacionDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                }



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


        public Int64 SelectSiguienteNumeroGrupoOrdenCompraCliente()
        {
            var objCommand = GetSqlCommand("ps_siguienteNumeroGrupoOrdenCompraCliente");
            OutputParameterAdd.BigInt(objCommand, "numeroGrupo");
            ExecuteNonQuery(objCommand);
            return (Int64)objCommand.Parameters["@numeroGrupo"].Value;
        }

        public List<OrdenCompraCliente> SelectOrdenCompraClientes(OrdenCompraCliente occ)
        {
            var objCommand = GetSqlCommand("ps_ordenesCompraCliente");
            InputParameterAdd.BigInt(objCommand, "numero", occ.numeroOrdenCompraCliente);
            InputParameterAdd.Int(objCommand, "idClienteSunat", occ.clienteSunat.idClienteSunat);
            InputParameterAdd.Guid(objCommand, "idUsuario", occ.usuario.idUsuario);
            InputParameterAdd.Guid(objCommand, "idUsuarioBusqueda", occ.usuarioBusqueda.idUsuario);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", occ.numeroReferenciaCliente);
            
            InputParameterAdd.DateTime(objCommand, "fechaCreacionDesde", new DateTime(occ.fechaCreacionDesde.Year, occ.fechaCreacionDesde.Month, occ.fechaCreacionDesde.Day, 0, 0, 0));
            InputParameterAdd.DateTime(objCommand, "fechaCreacionHasta", new DateTime(occ.fechaCreacionHasta.Year, occ.fechaCreacionHasta.Month, occ.fechaCreacionHasta.Day, 23, 59, 59));
            InputParameterAdd.Varchar(objCommand, "sku", occ.sku);
            DataTable dataTable = Execute(objCommand);

            List<OrdenCompraCliente> occList = new List<OrdenCompraCliente>();


            foreach (DataRow row in dataTable.Rows)
            {
                occ = new OrdenCompraCliente();
                occ.numeroOrdenCompraCliente = Converter.GetLong(row, "numero_occ");
                occ.idOrdenCompraCliente = Converter.GetGuid(row, "id_orden_compra_cliente");
                occ.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                occ.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");

                occ.FechaRegistro = Converter.GetDateTime(row, "fecha_registro");
                //occ.FechaRegistro = occ.FechaRegistro.AddHours(-5);
                /*if (row["fecha_programacion"] == DBNull.Value)
                    occ.fechaProgramacion = null;
                else*/
                occ.incluidoIGV = Converter.GetBool(row, "incluido_igv");

                occ.montoIGV = Converter.GetDecimal(row, "igv");
                occ.montoTotal = Converter.GetDecimal(row, "total");
                occ.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, occ.montoTotal - occ.montoIGV));

                occ.observaciones = Converter.GetString(row, "observaciones");

                occ.clienteSunat = new ClienteSunat();
                occ.clienteSunat.idClienteSunat = Converter.GetInt(row, "id_cliente_sunat");
                occ.clienteSunat.razonSocial = Converter.GetString(row, "razon_social");
                occ.clienteSunat.ruc = Converter.GetString(row, "ruc");

                occ.usuario = new Usuario();
                occ.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                occ.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");

                //  cotizacion.usuario_aprobador = new Usuario();
                //  cotizacion.usuario_aprobador.nombre = Converter.GetString(row, "nombre_usuario_aprobador");

                occ.ciudad = new Ciudad();
                occ.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                occ.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");


                occList.Add(occ);
            }
            return occList;
        }


        public OrdenCompraCliente SelectOrdenCompraCliente(OrdenCompraCliente occ, Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_ordenCompraCliente");
            InputParameterAdd.Guid(objCommand, "idOrdenCompraCliente", occ.idOrdenCompraCliente);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable occDataTable = dataSet.Tables[0];
            DataTable occDetalleDataTable = dataSet.Tables[1];
            DataTable occSedesDataTable = dataSet.Tables[2];

            //DataTable movimientoAlmacenDataTable = dataSet.Tables[2];
            //DataTable solicitanteDataTable = dataSet.Tables[3];

            //   DataTable dataTable = Execute(objCommand);
            //Datos de la cotizacion
            foreach (DataRow row in occDataTable.Rows)
            {
                occ.numeroOrdenCompraCliente = Converter.GetLong(row, "numero_occ");
                occ.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");

                occ.fechaEntregaExtendida = Converter.GetDateTimeNullable(row, "fecha_entrega_extendida");


                occ.incluidoIGV = Converter.GetBool(row, "incluido_igv");
                occ.montoIGV = Converter.GetDecimal(row, "igv");
                occ.montoTotal = Converter.GetDecimal(row, "total");
                occ.observaciones = Converter.GetString(row, "observaciones");
                occ.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, occ.montoTotal - occ.montoIGV));
                occ.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                occ.direccionEntrega = new DireccionEntrega();
                occ.direccionEntrega.idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega");
                occ.direccionEntrega.descripcion = Converter.GetString(row, "direccion_entrega");
                occ.direccionEntrega.contacto = Converter.GetString(row, "contacto_entrega");
                occ.direccionEntrega.telefono = Converter.GetString(row, "telefono_contacto_entrega");
                occ.direccionEntrega.codigoCliente = Converter.GetString(row, "direccion_entrega_codigo_cliente");
                occ.direccionEntrega.codigoMP = Converter.GetString(row, "direccion_entrega_codigo_mp");
                occ.direccionEntrega.nombre = Converter.GetString(row, "direccion_entrega_nombre");

                occ.solicitante = new Solicitante();
                occ.solicitante.idSolicitante = Converter.GetGuid(row, "id_solicitante");

                occ.esPagoContado = Converter.GetBool(row, "es_pago_contado");

                occ.contactoOrdenCompraCliente = Converter.GetString(row, "contacto_pedido");
                occ.telefonoContactoOrdenCompraCliente = Converter.GetString(row, "telefono_contacto_pedido");
                occ.correoContactoOrdenCompraCliente = Converter.GetString(row, "correo_contacto_pedido");

                occ.numeroRequerimiento = Converter.GetString(row, "numero_requerimiento");

                occ.solicitante.nombre = occ.contactoOrdenCompraCliente;
                occ.solicitante.telefono = occ.telefonoContactoOrdenCompraCliente;
                occ.solicitante.correo = occ.correoContactoOrdenCompraCliente;

                occ.fechaProgramacion = Converter.GetDateTime(row, "fecha_programacion");
                occ.observacionesFactura = Converter.GetString(row, "observaciones_factura");
                occ.observacionesGuiaRemision = Converter.GetString(row, "observaciones_guia_remision");

               
                occ.otrosCargos = Converter.GetDecimal(row, "otros_cargos");

                occ.FechaRegistro = Converter.GetDateTime(row, "fecha_registro");
                occ.FechaRegistro = occ.FechaRegistro.AddHours(-5);

                /* occ.venta = new Venta();
                   occ.venta.igv = Converter.GetDecimal(row, "igv_venta");
                   occ.venta.subTotal = Converter.GetDecimal(row, "sub_total_venta");
                   occ.venta.total = Converter.GetDecimal(row, "total_venta");
                   occ.venta.idVenta = Converter.GetGuid(row, "id_Venta");*/

                occ.ubigeoEntrega = new Ubigeo();
                occ.ubigeoEntrega.Id = Converter.GetString(row, "ubigeo_entrega");
                occ.ubigeoEntrega.Departamento = Converter.GetString(row, "departamento");
                occ.ubigeoEntrega.Provincia = Converter.GetString(row, "provincia");
                occ.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

                occ.ciudad = new Ciudad();
                occ.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                occ.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");
                occ.ciudad.idClienteRelacionado = Converter.GetGuid(row, "id_cliente_ciudad");

                occ.cotizacion = new Cotizacion();
                occ.cotizacion.idCotizacion = Converter.GetGuid(row, "id_cotizacion");
                occ.cotizacion.codigo = Converter.GetLong(row, "cotizacion_codigo");
                occ.cotizacion.tipoCotizacion = (Cotizacion.TiposCotizacion)Converter.GetInt(row, "tipo_cotizacion");

                occ.clienteSunat = new ClienteSunat();
                occ.clienteSunat.idClienteSunat = Converter.GetInt(row, "id_cliente_sunat");
                occ.clienteSunat.razonSocial = Converter.GetString(row, "razon_social");
                occ.clienteSunat.ruc = Converter.GetString(row, "ruc");

                occ.usuario = new Usuario(); 
                occ.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                occ.usuario.cargo = Converter.GetString(row, "cargo");
                occ.usuario.contacto = Converter.GetString(row, "contacto_usuario");
                occ.usuario.email = Converter.GetString(row, "email");

            }


            occ.detalleList = new List<OrdenCompraClienteDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in occDetalleDataTable.Rows)
            {
                OrdenCompraClienteDetalle occDetalle = new OrdenCompraClienteDetalle(usuario.visualizaCostos, usuario.visualizaMargen);

                occDetalle.producto = new Producto();

                occDetalle.idOrdenCompraClienteDetalle = Converter.GetGuid(row, "id_orden_compra_cliente_detalle");
                occDetalle.cantidad = Converter.GetInt(row, "cantidad");
                occDetalle.cantidadPendienteAtencion = Converter.GetInt(row, "cantidadPendienteAtencion");
                occDetalle.cantidadPorAtender = Converter.GetInt(row, "cantidadPendienteAtencion");

                occDetalle.cantidadPermitida = Converter.GetInt(row, "cantidad_permitida");
                occDetalle.observacionRestriccion = Converter.GetString(row, "comentario_retencion");

                occDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");

                occDetalle.flete = Converter.GetDecimal(row, "flete");


                //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                occDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                occDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar

                if (occDetalle.esPrecioAlternativo)
                {
                    /* occDetalle.ProductoPresentacion = new ProductoPresentacion();
                     occDetalle.ProductoPresentacion.Equivalencia = occDetalle.producto.equivalencia;
                     */
                    occDetalle.ProductoPresentacion = new ProductoPresentacion();
                    occDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                    occDetalle.ProductoPresentacion.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion");
                    occDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * occDetalle.ProductoPresentacion.Equivalencia;
                }
                else
                {
                    occDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                }

                occDetalle.unidad = Converter.GetString(row, "unidad");

                occDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                occDetalle.producto.sku = Converter.GetString(row, "sku");
                occDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                occDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                occDetalle.producto.proveedor = Converter.GetString(row, "proveedor");
                occDetalle.producto.tipoProducto = (Producto.TipoProducto)Converter.GetInt(row, "tipo_producto");
                occDetalle.producto.ventaRestringida = (Producto.TipoVentaRestringida)Converter.GetInt(row, "descontinuado");
                occDetalle.producto.motivoRestriccion = Converter.GetString(row, "motivo_restriccion");

                occDetalle.producto.image = Converter.GetBytes(row, "imagen");

                occDetalle.porcentajeDescuento = Converter.GetDecimal(row, "porcentaje_descuento");

                occDetalle.observacion = Converter.GetString(row, "observaciones");


                PrecioClienteProducto precioClienteProducto = new PrecioClienteProducto();

                precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto_vigente");
                precioClienteProducto.flete = Converter.GetDecimal(row, "flete_vigente");
                precioClienteProducto.precioUnitario = Converter.GetDecimal(row, "precio_unitario_vigente");
                precioClienteProducto.equivalencia = Converter.GetInt(row, "equivalencia_vigente");

                precioClienteProducto.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");

                precioClienteProducto.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");

                /*   if (row["fecha_fin_vigencia"] == DBNull.Value)
                   {
                       precioClienteProducto.fechaFinVigencia = null;
                   }
                   else
                   {*/
                precioClienteProducto.fechaFinVigencia = Converter.GetDateTimeNullable(row, "fecha_fin_vigencia");
                //}

                //  precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                precioClienteProducto.cliente = new Cliente();
                precioClienteProducto.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                occDetalle.producto.precioClienteProducto = precioClienteProducto;


                occDetalle.precioUnitarioVenta = Converter.GetDecimal(row, "precio_unitario_venta");
                occDetalle.idVentaDetalle = Converter.GetGuid(row, "id_venta_detalle");

                occDetalle.indicadorAprobacion = (OrdenCompraClienteDetalle.IndicadorAprobacion)Converter.GetShort(row, "indicador_aprobacion");

                occ.detalleList.Add(occDetalle);
            }

            occ.sedesClienteSunat = new List<Ciudad>();
            //Detalle de la cotizacion
            foreach (DataRow row in occSedesDataTable.Rows)
            {
                Ciudad occSede = new Ciudad();

                occSede.idCiudad = Converter.GetGuid(row, "id_ciudad");
                occSede.nombre = Converter.GetString(row, "nombre");
                occSede.idClienteRelacionado = Converter.GetGuid(row, "id_cliente");

                occ.sedesClienteSunat.Add(occSede);
            }

            List<DireccionEntrega> direccionEntregaList = new List<DireccionEntrega>();

            
            List<Solicitante> solicitanteList = new List<Solicitante>();

            //foreach (DataRow row in solicitanteDataTable.Rows)
            //{
            //    Solicitante obj = new Solicitante
            //    {
            //        idSolicitante = Converter.GetGuid(row, "id_solicitante"),
            //        nombre = Converter.GetString(row, "nombre"),
            //        telefono = Converter.GetString(row, "telefono"),
            //        correo = Converter.GetString(row, "correo")
            //    };
            //    solicitanteList.Add(obj);
            //}

            //occ.cliente.solicitanteList = solicitanteList;


            occ.guiaRemisionList = new List<GuiaRemision>();

            GuiaRemision movimientoAlmacen = new GuiaRemision();
            movimientoAlmacen.idMovimientoAlmacen = Guid.Empty;

            //foreach (DataRow row in movimientoAlmacenDataTable.Rows)
            //{
            //    Guid idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");
            //    if (movimientoAlmacen.idMovimientoAlmacen != idMovimientoAlmacen)
            //    {
            //        //Si no coincide con el anterior se crea un nuevo movimiento Almacen
            //        movimientoAlmacen = new GuiaRemision();
            //        movimientoAlmacen.idMovimientoAlmacen = idMovimientoAlmacen;
            //        movimientoAlmacen.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
            //        movimientoAlmacen.fechaTraslado = Converter.GetDateTime(row, "fecha_traslado");
            //        movimientoAlmacen.numeroDocumento = Converter.GetInt(row, "numero_documento");
            //        movimientoAlmacen.serieDocumento = Converter.GetString(row, "serie_documento");
            //        movimientoAlmacen.documentoDetalle = new List<DocumentoDetalle>();

            //        movimientoAlmacen.documentoVenta = new DocumentoVenta();
            //        movimientoAlmacen.documentoVenta.idDocumentoVenta = Converter.GetGuid(row, "id_documento_venta");
            //        movimientoAlmacen.documentoVenta.serie = Converter.GetString(row, "SERIE");
            //        movimientoAlmacen.documentoVenta.numero = Converter.GetString(row, "CORRELATIVO");


            //        movimientoAlmacen.documentoVenta.fechaEmision = Converter.GetDateTimeNullable(row, "fecha_emision_factura");

            //        occ.guiaRemisionList.Add(movimientoAlmacen);
            //    }

            //    DocumentoDetalle documentoDetalle = new DocumentoDetalle();
            //    documentoDetalle.idDocumentoDetalle = Converter.GetGuid(row, "id_movimiento_almacen_detalle");
            //    documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");
            //    documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");
            //    documentoDetalle.producto = new Producto();
            //    documentoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
            //    documentoDetalle.producto.sku = Converter.GetString(row, "sku");
            //    documentoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
            //    documentoDetalle.unidad = Converter.GetString(row, "unidad");
            //    documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");

            //    movimientoAlmacen.documentoDetalle.Add(documentoDetalle);
            //}

            return occ;
        }

        public OrdenCompraCliente SelectOrdenCompraClienteParaEditar(OrdenCompraCliente occ, Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_occParaEditar");
            InputParameterAdd.Guid(objCommand, "idOrdenCompraCliente", occ.idOrdenCompraCliente);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable occDataTable = dataSet.Tables[0];
            DataTable occDetalleDataTable = dataSet.Tables[1];
            DataTable direccionEntregaDataTable = dataSet.Tables[2];
            DataTable movimientoAlmacenDataTable = dataSet.Tables[3];
            DataTable occAdjuntoDataTable = dataSet.Tables[4];
            DataTable solicitanteDataTable = dataSet.Tables[5];

            //   DataTable dataTable = Execute(objCommand);
            //Datos de la cotizacion
            foreach (DataRow row in occDataTable.Rows)
            {
                occ.numeroOrdenCompraCliente = Converter.GetLong(row, "numero");
                occ.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                occ.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
                occ.fechaEntregaHasta = Converter.GetDateTime(row, "fecha_entrega_hasta");
                occ.horaEntregaDesde = Converter.GetString(row, "hora_entrega_desde");
                occ.horaEntregaHasta = Converter.GetString(row, "hora_entrega_hasta");
                occ.horaEntregaAdicionalDesde = Converter.GetString(row, "hora_entrega_adicional_desde");
                occ.horaEntregaAdicionalHasta = Converter.GetString(row, "hora_entrega_adicional_hasta");
                occ.incluidoIGV = Converter.GetBool(row, "incluido_igv");
                occ.montoIGV = Converter.GetDecimal(row, "igv");
                occ.montoTotal = Converter.GetDecimal(row, "total");
                occ.observaciones = Converter.GetString(row, "observaciones");
                occ.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, occ.montoTotal - occ.montoIGV));
                occ.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                occ.direccionEntrega = new DireccionEntrega();
                occ.direccionEntrega.idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega");
                occ.direccionEntrega.descripcion = Converter.GetString(row, "direccion_entrega");
                occ.direccionEntrega.contacto = Converter.GetString(row, "contacto_entrega");
                occ.direccionEntrega.telefono = Converter.GetString(row, "telefono_contacto_entrega");
                occ.direccionEntrega.codigoCliente = Converter.GetString(row, "direccion_entrega_codigo_cliente");
                occ.direccionEntrega.codigoMP = Converter.GetString(row, "direccion_entrega_codigo_mp");
                occ.direccionEntrega.nombre = Converter.GetString(row, "direccion_entrega_nombre");

                occ.solicitante = new Solicitante();
                occ.solicitante.idSolicitante = Converter.GetGuid(row, "id_solicitante");

                occ.contactoOrdenCompraCliente = Converter.GetString(row, "contacto_occ");
                occ.telefonoContactoOrdenCompraCliente = Converter.GetString(row, "telefono_contacto_occ");
                occ.correoContactoOrdenCompraCliente = Converter.GetString(row, "correo_contacto_occ");
                occ.numeroRequerimiento = Converter.GetString(row, "numero_requerimiento");
                occ.esPagoContado = Converter.GetBool(row, "es_pago_contado");

                occ.solicitante.nombre = occ.contactoOrdenCompraCliente;
                occ.solicitante.telefono = occ.telefonoContactoOrdenCompraCliente;
                occ.solicitante.correo = occ.correoContactoOrdenCompraCliente;

                occ.fechaProgramacion = Converter.GetDateTime(row, "fecha_programacion");
                occ.observacionesFactura = Converter.GetString(row, "observaciones_factura");
                occ.observacionesGuiaRemision = Converter.GetString(row, "observaciones_guia_remision");

                occ.claseOrdenCompraCliente = (OrdenCompraCliente.ClasesOrdenCompraCliente)Char.Parse(Converter.GetString(row, "tipo"));
                if (occ.claseOrdenCompraCliente == OrdenCompraCliente.ClasesOrdenCompraCliente.Venta)
                    occ.tipoOrdenCompraCliente = (OrdenCompraCliente.tiposOrdenCompraCliente)Char.Parse(Converter.GetString(row, "tipo_occ"));
                else if (occ.claseOrdenCompraCliente == OrdenCompraCliente.ClasesOrdenCompraCliente.Compra)
                    occ.tipoOrdenCompraClienteCompra = (OrdenCompraCliente.tiposOrdenCompraClienteCompra)Char.Parse(Converter.GetString(row, "tipo_occ"));
                else
                    occ.tipoOrdenCompraClienteAlmacen = (OrdenCompraCliente.tiposOrdenCompraClienteAlmacen)Char.Parse(Converter.GetString(row, "tipo_occ"));



                occ.otrosCargos = Converter.GetDecimal(row, "otros_cargos");

                occ.FechaRegistro = Converter.GetDateTime(row, "fecha_registro");
                occ.FechaRegistro = occ.FechaRegistro.AddHours(-5);

                /* occ.venta = new Venta();
                   occ.venta.igv = Converter.GetDecimal(row, "igv_venta");
                   occ.venta.subTotal = Converter.GetDecimal(row, "sub_total_venta");
                   occ.venta.total = Converter.GetDecimal(row, "total_venta");
                   occ.venta.idVenta = Converter.GetGuid(row, "id_Venta");*/

                occ.ubigeoEntrega = new Ubigeo();
                occ.ubigeoEntrega.Id = Converter.GetString(row, "ubigeo_entrega");
                occ.ubigeoEntrega.Departamento = Converter.GetString(row, "departamento");
                occ.ubigeoEntrega.Provincia = Converter.GetString(row, "provincia");
                occ.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");


                occ.cotizacion = new Cotizacion();
                occ.cotizacion.idCotizacion = Converter.GetGuid(row, "id_cotizacion");
                occ.cotizacion.codigo = Converter.GetLong(row, "cotizacion_codigo");

                occ.cliente = new Cliente();
                occ.cliente.codigo = Converter.GetString(row, "codigo");
                occ.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                occ.cliente.razonSocial = Converter.GetString(row, "razon_social");
                occ.cliente.ruc = Converter.GetString(row, "ruc");
                occ.cliente.ciudad = new Ciudad();
                occ.cliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad_cliente");
                occ.cliente.ciudad.nombre = Converter.GetString(row, "nombre_ciudad_cliente");
                occ.cliente.razonSocialSunat = Converter.GetString(row, "razon_social_sunat");
                occ.cliente.direccionDomicilioLegalSunat = Converter.GetString(row, "direccion_domicilio_legal_sunat");
                occ.cliente.correoEnvioFactura = Converter.GetString(row, "correo_envio_factura");
                occ.cliente.plazoCredito = Converter.GetString(row, "plazo_credito");
                occ.cliente.tipoPagoFactura = (DocumentoVenta.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                occ.cliente.formaPagoFactura = (DocumentoVenta.FormaPago)Converter.GetInt(row, "forma_pago_factura");
                occ.cliente.habilitadoModificarDireccionEntrega = Converter.GetBool(row, "habilitado_modificar_direccion_entrega");
                occ.cliente.tipoLiberacionCrediticia = (Persona.TipoLiberacionCrediticia)Converter.GetInt(row, "estado_liberacion_creditica");

                occ.ciudad = new Ciudad();
                occ.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                occ.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

             /*   occ.usuario = new Usuario();
                occ.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                occ.usuario.cargo = Converter.GetString(row, "cargo");
                occ.usuario.contacto = Converter.GetString(row, "contacto_usuario");
                occ.usuario.email = Converter.GetString(row, "email");*/

            }


            occ.detalleList = new List<OrdenCompraClienteDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in occDetalleDataTable.Rows)
            {
                OrdenCompraClienteDetalle occDetalle = new OrdenCompraClienteDetalle(usuario.visualizaCostos, usuario.visualizaMargen);

                occDetalle.producto = new Producto();

                occDetalle.idOrdenCompraClienteDetalle = Converter.GetGuid(row, "id_occ_detalle");
                occDetalle.cantidad = Converter.GetInt(row, "cantidad");
                occDetalle.cantidadPendienteAtencion = Converter.GetInt(row, "cantidadPendienteAtencion");
                occDetalle.cantidadPorAtender = Converter.GetInt(row, "cantidadPendienteAtencion");
                //occDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                occDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                occDetalle.flete = Converter.GetDecimal(row, "flete");


                //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                occDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                occDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");
                occDetalle.producto.tipoProducto = (Producto.TipoProducto) Converter.GetInt(row, "tipo_producto");

                //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar

                if (occDetalle.esPrecioAlternativo)
                {
                    occDetalle.ProductoPresentacion = new ProductoPresentacion();
                    occDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                    occDetalle.ProductoPresentacion.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion");
                    occDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * occDetalle.ProductoPresentacion.Equivalencia;
                }
                else
                {
                    occDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                }

                occDetalle.unidad = Converter.GetString(row, "unidad");

                occDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                occDetalle.producto.sku = Converter.GetString(row, "sku");
                occDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                occDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                occDetalle.producto.proveedor = Converter.GetString(row, "proveedor");
                occDetalle.producto.ventaRestringida = (Producto.TipoVentaRestringida) Converter.GetInt(row, "descontinuado");
                occDetalle.producto.motivoRestriccion = Converter.GetString(row, "motivo_restriccion");

                occDetalle.producto.image = Converter.GetBytes(row, "imagen");

                occDetalle.porcentajeDescuento = Converter.GetDecimal(row, "porcentaje_descuento");

                occDetalle.observacion = Converter.GetString(row, "observaciones");


                PrecioClienteProducto precioClienteProducto = new PrecioClienteProducto();

                precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto_vigente");
                precioClienteProducto.flete = Converter.GetDecimal(row, "flete_vigente");
                precioClienteProducto.precioUnitario = Converter.GetDecimal(row, "precio_unitario_vigente");
                precioClienteProducto.equivalencia = Converter.GetInt(row, "equivalencia_vigente");

                precioClienteProducto.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");

                precioClienteProducto.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");

                /*if (row["fecha_fin_vigencia"] == DBNull.Value)
                {
                    precioClienteProducto.fechaFinVigencia = null;
                }
                else
                {*/
                precioClienteProducto.fechaFinVigencia = Converter.GetDateTimeNullable(row, "fecha_fin_vigencia");
                //}

                //  precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                precioClienteProducto.cliente = new Cliente();
                precioClienteProducto.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                occDetalle.producto.precioClienteProducto = precioClienteProducto;


                occDetalle.precioUnitarioVenta = Converter.GetDecimal(row, "precio_unitario_venta");
                occDetalle.idVentaDetalle = Converter.GetGuid(row, "id_venta_detalle");

                occ.detalleList.Add(occDetalle);
            }

            List<DireccionEntrega> direccionEntregaList = new List<DireccionEntrega>();

            foreach (DataRow row in direccionEntregaDataTable.Rows)
            {
                DireccionEntrega obj = new DireccionEntrega
                {
                    idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega"),
                    descripcion = Converter.GetString(row, "descripcion"),
                    contacto = Converter.GetString(row, "contacto"),
                    telefono = Converter.GetString(row, "telefono"),
                    ubigeo = new Ubigeo { Id = Converter.GetString(row, "ubigeo") },
                    nombre = Converter.GetString(row, "nombre"),
                    codigoCliente = Converter.GetString(row, "codigoCliente"),
                    codigoMP = Converter.GetString(row, "codigoMP")  
                };
                direccionEntregaList.Add(obj);
            }

            occ.cliente.direccionEntregaList = direccionEntregaList;


            List<Solicitante> solicitanteList = new List<Solicitante>();

            foreach (DataRow row in solicitanteDataTable.Rows)
            {
                Solicitante obj = new Solicitante
                {
                    idSolicitante = Converter.GetGuid(row, "id_solicitante"),
                    nombre = Converter.GetString(row, "nombre"),
                    telefono = Converter.GetString(row, "telefono"),
                    correo = Converter.GetString(row, "correo")
                };
                solicitanteList.Add(obj);
            }

            occ.cliente.solicitanteList = solicitanteList;


            occ.guiaRemisionList = new List<GuiaRemision>();

            GuiaRemision movimientoAlmacen = new GuiaRemision();
            movimientoAlmacen.idMovimientoAlmacen = Guid.Empty;

            foreach (DataRow row in movimientoAlmacenDataTable.Rows)
            {
                Guid idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");
                if (movimientoAlmacen.idMovimientoAlmacen != idMovimientoAlmacen)
                {
                    //Si no coincide con el anterior se crea un nuevo movimiento Almacen
                    movimientoAlmacen = new GuiaRemision();
                    movimientoAlmacen.idMovimientoAlmacen = idMovimientoAlmacen;
                    movimientoAlmacen.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
                    movimientoAlmacen.fechaTraslado = Converter.GetDateTime(row, "fecha_traslado");
                    movimientoAlmacen.numeroDocumento = Converter.GetInt(row, "numero_documento");
                    movimientoAlmacen.serieDocumento = Converter.GetString(row, "serie_documento");
                    movimientoAlmacen.documentoDetalle = new List<DocumentoDetalle>();

                    movimientoAlmacen.documentoVenta = new DocumentoVenta();
                    movimientoAlmacen.documentoVenta.idDocumentoVenta = Converter.GetGuid(row, "id_documento_venta");
                    movimientoAlmacen.documentoVenta.serie = Converter.GetString(row, "SERIE");
                    movimientoAlmacen.documentoVenta.numero = Converter.GetString(row, "CORRELATIVO");



                    /*if (row["fecha_emision_factura"] == DBNull.Value)
                        movimientoAlmacen.documentoVenta.fechaEmision = null;
                    else*/
                    movimientoAlmacen.documentoVenta.fechaEmision = Converter.GetDateTimeNullable(row, "fecha_emision_factura");



                    occ.guiaRemisionList.Add(movimientoAlmacen);
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


            /*mad.id_movimiento_almacen_detalle, mad.cantidad, 
mad.unidad, pr.id_producto, pr.sku, pr.descripcion*/

            occ.AdjuntoList = new List<ArchivoAdjunto>();
            //Detalle de la cotizacion
            foreach (DataRow row in occAdjuntoDataTable.Rows)
            {
                ArchivoAdjunto occAdjunto = new ArchivoAdjunto();
                occAdjunto.idArchivoAdjunto = Converter.GetGuid(row, "id_archivo_adjunto");
                occAdjunto.adjunto = Converter.GetBytes(row, "adjunto");
                occAdjunto.nombre = Converter.GetString(row, "nombre");
                occ.AdjuntoList.Add(occAdjunto);
            }

            return occ;
        }

       

        #endregion        
         

        public List<Guid> SelectOrdenCompraClientesSinAtencion()
        {
            List<Guid> occIds = new List<Guid>();

            var objCommand = GetSqlCommand("ps_occs_sin_atencion");
            DataTable dataTable = Execute(objCommand);
            
            foreach (DataRow row in dataTable.Rows)
            {
                Guid idOrdenCompraCliente = Converter.GetGuid(row, "id_occ");
                
                occIds.Add(idOrdenCompraCliente);
            }
            
            return occIds;
        }

        

        
    }
}
