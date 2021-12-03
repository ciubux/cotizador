using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Data.SqlClient;

namespace DataLayer
{
    public class UsuarioDAL : DaoBase
    {
        public UsuarioDAL(IDalSettings settings) : base(settings)
        {
        }

        public UsuarioDAL() : this(new CotizadorSettings())
        {
        }

        public List<Usuario> searchUsuarios(String textoBusqueda)
        {
            var objCommand = GetSqlCommand("ps_usuarios_search");
            InputParameterAdd.Varchar(objCommand, "textoBusqueda", textoBusqueda);
            DataTable dataTable = Execute(objCommand);

            List<Usuario> usuarioList = new List<Usuario>();

            foreach (DataRow row in dataTable.Rows)
            {
                Usuario usuario = new Usuario();
                usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                usuario.email = Converter.GetString(row, "email");
                usuario.nombre = Converter.GetString(row, "nombre");
                usuarioList.Add(usuario);
            }

            return usuarioList;
        }

        public void updateCotizacionSerializada(Usuario usuario,String cotizacionSerializada)
        {
            var objCommand = GetSqlCommand("pu_cotizacionSerializada");
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "cotizacionSerializada", -1, cotizacionSerializada);
            ExecuteNonQuery(objCommand);
        }

        public void updatePedidoSerializado(Usuario usuario, String pedidoSerializado)
        {
            var objCommand = GetSqlCommand("pu_pedidoSerializado");
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "pedidoSerializado", -1, pedidoSerializado);
            ExecuteNonQuery(objCommand);
        }


        public List<Usuario> selectUsuarios()
        {
            var objCommand = GetSqlCommand("ps_usuarios");
            DataTable dataTable = Execute(objCommand);
            List<Usuario> usuarioList = new List<Usuario>();

            foreach (DataRow row in dataTable.Rows)
            {
                Usuario usuario = new Usuario();
                usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                usuario.email = Converter.GetString(row, "email");
                usuario.nombre = Converter.GetString(row, "nombre");
                usuarioList.Add(usuario);
            }

            return usuarioList;
        }

        public List<Usuario> selectUsuariosPorPermiso(Permiso permiso, int tipo = 1)
        {
            var objCommand = GetSqlCommand("ps_usuariosPorPermiso");
            InputParameterAdd.Int(objCommand, "idPermiso", permiso.idPermiso);
            InputParameterAdd.Int(objCommand, "tipo", tipo);
            DataTable dataTable = Execute(objCommand);
            List<Usuario> usuarioList = new List<Usuario>();

            foreach (DataRow row in dataTable.Rows)
            {
                Usuario usuario = new Usuario();
                usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                usuario.email = Converter.GetString(row, "email");
                usuario.nombre = Converter.GetString(row, "nombre");
                usuarioList.Add(usuario);
            }

            return usuarioList;
        }

        public List<Usuario> selectUsuariosPorPermiso(String codigoPermiso, int tipo = 1)
        {
            var objCommand = GetSqlCommand("ps_usuariosPorPermisoCodigo");
            InputParameterAdd.Char(objCommand, "codigo", codigoPermiso);
            InputParameterAdd.Int(objCommand, "tipo", tipo);
            DataTable dataTable = Execute(objCommand);
            List<Usuario> usuarioList = new List<Usuario>();

            foreach (DataRow row in dataTable.Rows)
            {
                Usuario usuario = new Usuario();
                usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                usuario.email = Converter.GetString(row, "email");
                usuario.nombre = Converter.GetString(row, "nombre");
                usuarioList.Add(usuario);
            }

            return usuarioList;
        }


        public void updatePermiso(List<Usuario> usuarioLit, Permiso permiso, Usuario usuario)
        {
            var objCommand = GetSqlCommand("pu_usuarioPermiso");
            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("idUsuarioList", typeof(Guid)));

            // populate DataTable from your List here
            foreach (var id in usuarioLit)
                tvp.Rows.Add(id.idUsuario);

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idUsuarioList", tvp);
            // these next lines are important to map the C# DataTable object to the correct SQL User Defined Type
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";

            InputParameterAdd.Int(objCommand, "idPermiso", permiso.idPermiso);
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }

        public Usuario getUsuario(Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_usuario_get");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            DataTable dataTable = Execute(objCommand);

            Usuario obj = new Usuario();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idUsuario = Converter.GetGuid(row, "id_usuario");
                obj.cargo = Converter.GetString(row, "cargo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.email = Converter.GetString(row, "email");
                obj.contacto = Converter.GetString(row, "contacto");
            }

            return obj;
        }


        public Usuario getUsuarioLogin(Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_usuario_b");
            InputParameterAdd.Varchar(objCommand, "email", usuario.email);
            InputParameterAdd.Varchar(objCommand, "password", usuario.password);
            InputParameterAdd.Varchar(objCommand, "ip_login", usuario.ipAddress);
            DataSet dataSet = ExecuteDataSet(objCommand);

            DataTable dataTableUsuario = dataSet.Tables[0];

            foreach (DataRow row in dataTableUsuario.Rows)
            {
                usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                usuario.cargo = Converter.GetString(row, "cargo");
                usuario.nombre = Converter.GetString(row, "nombre");
                usuario.contacto = Converter.GetString(row, "contacto");

                usuario.firmaImagen = Converter.GetBytes(row, "firma_imagen");

                //Cotizaciones
                usuario.maximoPorcentajeDescuentoAprobacion = Converter.GetDecimal(row, "maximo_porcentaje_descuento_aprobacion");
                usuario.cotizacionSerializada = Converter.GetString(row, "cotizacion_serializada");

                /*        usuario.apruebaCotizacionesLima = Converter.GetBool(row, "aprueba_cotizaciones_lima");
                        usuario.apruebaCotizacionesProvincias = Converter.GetBool(row, "aprueba_cotizaciones_provincias");
                        usuario.creaCotizaciones = Converter.GetBool(row, "crea_cotizaciones");
                        usuario.creaCotizacionesProvincias = Converter.GetBool(row, "crea_cotizaciones_provincias");
                        usuario.creaCotizacionesGrupales = Converter.GetBool(row, "crea_cotizaciones_grupales");
                        usuario.apruebaCotizacionesGrupales = Converter.GetBool(row, "aprueba_cotizaciones_grupales");
                        usuario.visualizaCotizaciones = Converter.GetBool(row, "visualiza_cotizaciones");
                        */
                //Pedidos
                /*          usuario.tomaPedidos = Converter.GetBool(row, "toma_pedidos");
                          usuario.realizaCargaMasivaPedidos = Converter.GetBool(row, "realiza_carga_masiva_pedidos");

                          usuario.apruebaPedidosLima = Converter.GetBool(row, "aprueba_pedidos_lima");
                          usuario.apruebaPedidosProvincias = Converter.GetBool(row, "aprueba_pedidos_provincias");
                          usuario.pedidoSerializado = Converter.GetString(row, "pedido_serializado");
                          usuario.tomaPedidosProvincias = Converter.GetBool(row, "toma_pedidos_provincias");
                          usuario.programaPedidos = Converter.GetBool(row, "programa_pedidos");
                          usuario.visualizaPedidosLima = Converter.GetBool(row, "visualiza_pedidos_lima");
                          usuario.visualizaPedidosProvincias = Converter.GetBool(row, "visualiza_pedidos_provincias");
                          usuario.bloqueaPedidos = Converter.GetBool(row, "bloquea_pedidos");
                          usuario.liberaPedidos = Converter.GetBool(row, "libera_pedidos");
                          usuario.visualizaCostos = Converter.GetBool(row, "visualiza_costos");
                          */
                //Guia
                /*           usuario.creaGuias = Converter.GetBool(row, "crea_guias");
                           usuario.administraGuiasLima = Converter.GetBool(row, "administra_guias_lima");
                           usuario.administraGuiasProvincias = Converter.GetBool(row, "administra_guias_provincias");
                           usuario.visualizaGuias = Converter.GetBool(row, "visualiza_guias_remision");

                           //DOCUMENTOS VENTA
                           usuario.creaDocumentosVenta = Converter.GetBool(row, "crea_documentos_venta");
                           usuario.creaFacturaConsolidadaMultiregional = Converter.GetBool(row, "crea_factura_consolidada_multiregional");
                           usuario.creaFacturaConsolidadaLocal = Converter.GetBool(row, "crea_factura_consolidada_local");
                           usuario.visualizaGuiasPendienteFacturacion = Converter.GetBool(row, "visualiza_guias_pendientes_facturacion");

                           usuario.administraDocumentosVentaLima = Converter.GetBool(row, "administra_documentos_venta_lima");
                           usuario.administraDocumentosVentaProvincias = Converter.GetBool(row, "administra_documentos_venta_provincias");
                           usuario.visualizaDocumentosVenta = Converter.GetBool(row, "visualiza_documentos_venta");
                           usuario.apruebaAnulaciones = Converter.GetBool(row, "aprueba_anulaciones");
                           */
                usuario.sedeMP = new Ciudad();
                usuario.sedeMP.idCiudad = Converter.GetGuid(row, "id_ciudad");

                /*       usuario.modificaMaestroClientes = Converter.GetBool(row, "modifica_maestro_clientes");
                       usuario.modificaMaestroProductos = Converter.GetBool(row, "modifica_maestro_productos");

                       usuario.modificaSubDistribuidor = Converter.GetBool(row, "modifica_subdistribuidor");
                       usuario.modificaOrigen = Converter.GetBool(row, "modifica_origen");

                       usuario.creaNotasCredito = Converter.GetBool(row, "crea_notas_credito");
                       usuario.creaNotasDebito = Converter.GetBool(row, "crea_notas_debito");
                       usuario.realizaRefacturacion = Converter.GetBool(row, "realiza_refacturacion");

                       usuario.visualizaMargen = Converter.GetBool(row, "visualiza_margen");
                       usuario.confirmaStock = Converter.GetBool(row, "confirma_stock");
                       */
                usuario.esCliente = Converter.GetBool(row, "es_cliente");
                /*
                                usuario.tomaPedidosCompra = Converter.GetBool(row, "toma_pedidos_compra");
                                usuario.tomaPedidosAlmacen = Converter.GetBool(row, "toma_pedidos_almacen");
                                usuario.apruebaPlazoCredito = Converter.GetBool(row, "define_plazo_credito");
                                usuario.apruebaMontoCredito = Converter.GetBool(row, "define_monto_credito");
                                usuario.modificaResponsableComercial = Converter.GetBool(row, "define_responsable_comercial");
                                usuario.modificaSupervisorComercial = Converter.GetBool(row, "define_supervisor_comercial");
                                usuario.modificaAsistenteAtencionCliente = Converter.GetBool(row, "define_asistente_atencion_cliente");
                                usuario.modificaResponsablePortafolio = Converter.GetBool(row, "define_responsable_portafolio");
                                usuario.modificaPedidoVentaFechaEntregaHasta = Converter.GetBool(row, "modifica_pedido_venta_fecha_entrega_hasta");
                                usuario.modificaCanales = Converter.GetBool(row, "modifica_canales");
                                //usuario.modificaProducto = Converter.GetBool(row, "modifica_producto");
                                usuario.bloqueaClientes = Converter.GetBool(row, "bloquea_clientes");
                                usuario.modificaPedidoFechaEntregaExtendida = Converter.GetBool(row, "modifica_pedido_fecha_entrega_extendida");
                                usuario.modificaNegociacionMultiregional = Converter.GetBool(row, "modifica_negociacion_multiregional");
                                usuario.buscaSedesGrupoCliente = Converter.GetBool(row, "busca_sedes_grupo_cliente");

                                usuario.modificaGrupoClientes = Converter.GetBool(row, "modifica_grupo_clientes");

                                usuario.visualizaGrupoClientes = Converter.GetBool(row, "visualiza_grupo_cliente");
                                usuario.visualizaClientes = Converter.GetBool(row, "visualiza_clientes");
                                usuario.visualizaProductos = Converter.GetBool(row, "visualiza_productos");
                                usuario.visualizaSubDistribuidores = Converter.GetBool(row, "visualiza_subdistribuidores");
                                usuario.visualizaOrigenes = Converter.GetBool(row, "visualiza_origenes");
                                usuario.realizaCargaMasivaCliente = Converter.GetBool(row, "realiza_carga_masiva_clientes");
                                usuario.realizaCargaMasivaProductos = Converter.GetBool(row, "realiza_carga_masiva_productos");
                                usuario.eliminaCotizacionesAceptadas = Converter.GetBool(row, "elimina_cotizaciones_aceptadas");
                                */
            }

            DataTable dataTableParametros = dataSet.Tables[1];

            foreach (DataRow row in dataTableParametros.Rows)
            {
                String codigoParametro = Converter.GetString(row, "codigo");
                String valorParametro = Converter.GetString(row, "valor");

                switch (codigoParametro)
                {
                    case "IGV":
                        Constantes.IGV = Decimal.Parse(valorParametro); break;
                    case "SIMBOLO_SOL":
                        Constantes.SIMBOLO_SOL = valorParametro; break;
                    case "PLAZO_OFERTA_DIAS":
                        Constantes.PLAZO_OFERTA_DIAS = int.Parse(valorParametro); break;
                    case "PORCENTAJE_MAX_APROBACION":
                        Constantes.PORCENTAJE_MAX_APROBACION = Decimal.Parse(valorParametro); break;
                    case "DEBUG":
                        Constantes.DEBUG = int.Parse(valorParametro); break;
                    case "DIAS_MAX_BUSQUEDA_PRECIOS":
                        Constantes.DIAS_MAX_BUSQUEDA_PRECIOS = int.Parse(valorParametro); break;
                    case "OBSERVACION":
                        Constantes.OBSERVACION_E = valorParametro; break;
                    case "MILISEGUNDOS_AUTOGUARDADO":
                        Constantes.MILISEGUNDOS_AUTOGUARDADO = int.Parse(valorParametro); break;
                    case "DIAS_MAX_VIGENCIA_PRECIOS_COTIZACION":
                        Constantes.DIAS_MAX_VIGENCIA_PRECIOS_COTIZACION = int.Parse(valorParametro); break;
                    case "DIAS_MAX_VIGENCIA_PRECIOS_PEDIDO":
                        Constantes.DIAS_MAX_VIGENCIA_PRECIOS_PEDIDO = int.Parse(valorParametro); break;
                    case "VARIACION_PRECIO_ITEM_PEDIDO":
                        Constantes.VARIACION_PRECIO_ITEM_PEDIDO = Decimal.Parse(valorParametro); break;
                    case "USER_EOL_TEST":
                        Constantes.USER_EOL_TEST = valorParametro; break;
                    case "PASSWORD_EOL_TEST":
                        Constantes.PASSWORD_EOL_TEST = valorParametro; break;
                    case "ENDPOINT_ADDRESS_EOL_TEST":
                        Constantes.ENDPOINT_ADDRESS_EOL_TEST = valorParametro; break;
                    case "MAIL_COMUNICACION_FACTURAS":
                        Constantes.MAIL_COMUNICACION_FACTURAS = valorParametro; break;
                    case "PASSWORD_MAIL_COMUNICACION_FACTURAS":
                        Constantes.PASSWORD_MAIL_COMUNICACION_FACTURAS = valorParametro; break;
                    case "MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS":
                        Constantes.MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS = valorParametro; break;
                    case "PASSWORD_MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS":
                        Constantes.PASSWORD_MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS = valorParametro; break;
                    case "ID_VENDEDOR_POR_ASIGNAR":
                        Constantes.ID_VENDEDOR_POR_ASIGNAR = int.Parse(valorParametro); break;
                    case "DIAS_DEFECTO_VALIDEZ_OFERTA_COTIZACION_PUNTUAL":
                        Constantes.DIAS_DEFECTO_VALIDEZ_OFERTA_COTIZACION_PUNTUAL = int.Parse(valorParametro); break;
                    case "DIAS_MAX_VALIDEZ_OFERTA_COTIZACION_PUNTUAL":
                        Constantes.DIAS_MAX_VALIDEZ_OFERTA_COTIZACION_PUNTUAL = int.Parse(valorParametro); break;

                    case "HORA_CORTE_CREDITOS_LIMA":
                        {

                            DateTime horaCorte = new DateTime();
                            String[] horaArray =  valorParametro.Split(':');
                            horaCorte = new DateTime(horaCorte.Year, horaCorte.Month, horaCorte.Day,
                                Int32.Parse( horaArray[0]),
                                Int32.Parse(horaArray[1]),
                                Int32.Parse(horaArray[2])
                            );

                            Constantes.HORA_CORTE_CREDITOS_LIMA = horaCorte; break;
                        }

                    case "USER_EOL_PROD":
                        Constantes.USER_EOL_PROD = valorParametro; break;
                    case "PASSWORD_EOL_PROD":
                        Constantes.PASSWORD_EOL_PROD = valorParametro; break;
                    case "ENDPOINT_ADDRESS_EOL_PROD":
                        Constantes.ENDPOINT_ADDRESS_EOL_PROD = valorParametro; break;
                    case "DESCARGAR_XML":
                        Constantes.DESCARGAR_XML = int.Parse(valorParametro); break;
                    case "CPE_CABECERA_BE_ID":
                        Constantes.CPE_CABECERA_BE_ID = valorParametro; break;
                    case "CPE_CABECERA_BE_COD_GPO":
                        Constantes.CPE_CABECERA_BE_COD_GPO = valorParametro; break;

                    case "RUC_MP":
                        Constantes.RUC_MP = valorParametro; break;
                    case "RAZON_SOCIAL_MP":
                        Constantes.RAZON_SOCIAL_MP = valorParametro; break;
                    case "DIRECCION_MP":
                        Constantes.DIRECCION_MP = valorParametro; break;
                    case "TELEFONO_MP":
                        Constantes.TELEFONO_MP = valorParametro; break;
                    case "WEB_MP":
                        Constantes.WEB_MP = valorParametro; break;


                    case "AMBIENTE_EOL":
                        {
                            Constantes.AMBIENTE_EOL = valorParametro;
                            //  Constantes.AMBIENTE_EOL = "TEST";
                        }
                        break; //


                }

            }


            //Permisos de usuario
            if (usuario.idUsuario != null && usuario.idUsuario != Guid.Empty)
            {
                DataTable dataTablePermisos = dataSet.Tables[10];
                usuario.permisoList = new List<Permiso>();

                foreach (DataRow row in dataTablePermisos.Rows)
                {
                    Permiso permiso = new Permiso();
                    permiso.idPermiso = Converter.GetInt(row, "id_Permiso");
                    permiso.codigo = Converter.GetString(row, "codigo");
                    permiso.descripcion_corta = Converter.GetString(row, "descripcion_corta");
                    permiso.descripcion_larga = Converter.GetString(row, "descripcion_larga");
                    usuario.permisoList.Add(permiso);
                }
            }


            /*Si es usuario aprobador se recupera la lista de usuarios a los cuales puede aprobar cotizaciones*/
            if (usuario.idUsuario != null && usuario.idUsuario != Guid.Empty)
            {
                DataTable dataTableUsuariosCreaCotizacion = dataSet.Tables[2];
                List<Usuario> usuarioList = new List<Usuario>();

                foreach (DataRow row in dataTableUsuariosCreaCotizacion.Rows)
                {
                    Usuario usuarioTmp = new Usuario();
                    usuarioTmp.idUsuario = Converter.GetGuid(row, "id_usuario");
                    usuarioTmp.nombre = Converter.GetString(row, "nombre");
                    usuarioTmp.sedeMP = new Ciudad();
                    usuarioTmp.sedeMP.esProvincia = Converter.GetBool(row, "es_provincia");
                    usuarioList.Add(usuarioTmp);
                }
                usuario.usuarioCreaCotizacionList = usuarioList;
            }

            if (usuario.apruebaPedidos || ( usuario.idUsuario != null && usuario.idUsuario != Guid.Empty))
            {
                DataTable dataTableUsuariosTomaPedido = dataSet.Tables[3];
                List<Usuario> usuarioList = new List<Usuario>();

                foreach (DataRow row in dataTableUsuariosTomaPedido.Rows)
                {
                    Usuario usuarioTmp = new Usuario();
                    usuarioTmp.idUsuario = Converter.GetGuid(row, "id_usuario");
                    usuarioTmp.nombre = Converter.GetString(row, "nombre");
                    usuarioTmp.sedeMP = new Ciudad();
                    usuarioTmp.sedeMP.esProvincia = Converter.GetBool(row, "es_provincia");
                    usuarioList.Add(usuarioTmp);
                }
                usuario.usuarioTomaPedidoList = usuarioList;
            }

            if (usuario.administraGuias)
            {
                DataTable dataTableUsuariosAdministraGuias = dataSet.Tables[4];
                List<Usuario> usuarioList = new List<Usuario>();

                foreach (DataRow row in dataTableUsuariosAdministraGuias.Rows)
                {
                    Usuario usuarioTmp = new Usuario();
                    usuarioTmp.idUsuario = Converter.GetGuid(row, "id_usuario");
                    usuarioTmp.nombre = Converter.GetString(row, "nombre");
                    usuarioTmp.sedeMP = new Ciudad();
                    usuarioTmp.sedeMP.esProvincia = Converter.GetBool(row, "es_provincia");
                    usuarioList.Add(usuarioTmp);
                }
                usuario.usuarioCreaGuiaList = usuarioList;
            }

            if (usuario.administraDocumentosVenta)
            {
                DataTable dataTableUsuariosAdministraDocumentosVenta = dataSet.Tables[5];
                List<Usuario> usuarioList = new List<Usuario>();

                foreach (DataRow row in dataTableUsuariosAdministraDocumentosVenta.Rows)
                {
                    Usuario usuarioTmp = new Usuario();
                    usuarioTmp.idUsuario = Converter.GetGuid(row, "id_usuario");
                    usuarioTmp.nombre = Converter.GetString(row, "nombre");
                    usuarioTmp.sedeMP = new Ciudad();
                    usuarioTmp.sedeMP.esProvincia = Converter.GetBool(row, "es_provincia");
                    usuarioList.Add(usuarioTmp);
                }
                usuario.usuarioCreaDocumentoVentaList = usuarioList;
            }

            if (usuario.idUsuario != null && usuario.idUsuario != Guid.Empty)
            {
                DataTable dataTableClientes = dataSet.Tables[6];
                List<Cliente> clienteList = new List<Cliente>();

                foreach (DataRow row in dataTableClientes.Rows)
                {
                    Cliente cliente = new Cliente();
                    cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                    cliente.razonSocial = Converter.GetString(row, "razon_social");
                    cliente.codigo = Converter.GetString(row, "codigo");
                    cliente.ruc = Converter.GetString(row, "ruc");
                    cliente.nombreComercial = Converter.GetString(row, "nombre_comercial");
                    clienteList.Add(cliente);
                }
                usuario.clienteList = clienteList;


                DataTable dataTableVendedores = dataSet.Tables[7];
                List<Vendedor> vendedorList = new List<Vendedor>();

                foreach (DataRow row in dataTableVendedores.Rows)
                {
                    Vendedor vendedor = new Vendedor();
                    vendedor.idVendedor = Converter.GetInt(row, "id_vendedor");
                    vendedor.descripcion = Converter.GetString(row, "descripcion");
                    vendedor.codigo = Converter.GetString(row, "codigo");
                    vendedor.esResponsableComercial = Converter.GetBool(row, "es_responsable_comercial");
                    vendedor.esAsistenteServicioCliente = Converter.GetBool(row, "es_asistente_servicio_cliente");
                    vendedor.esResponsablePortafolio = Converter.GetBool(row, "es_responsable_portafolio");
                    vendedor.esSupervisorComercial = Converter.GetBool(row, "es_supervisor_comercial");
                    vendedor.usuario = new Usuario();
                    vendedor.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                    vendedor.idSupervisorComercial = Converter.GetInt(row, "id_supervisor_comercial");
                    vendedorList.Add(vendedor);

                    if (vendedor.usuario.idUsuario == usuario.idUsuario)
                    {
                        usuario.esVendedor = true;
                    }
                }
                usuario.vendedorList = vendedorList;

            }

            if (usuario.idUsuario != null && usuario.idUsuario != Guid.Empty)
            {
                DataTable dataTableProductosDescuento = dataSet.Tables[8];
                Constantes.DESCUENTOS_LIST = new List<Producto>();

                foreach (DataRow row in dataTableProductosDescuento.Rows)
                {
                    Producto producto = new Producto();
                    producto.idProducto = Converter.GetGuid(row, "id_Producto");
                    producto.sku = Converter.GetString(row, "sku");
                    producto.descripcion = Converter.GetString(row, "descripcion");
                    Constantes.DESCUENTOS_LIST.Add(producto);
                }
            }

            if (usuario.idUsuario != null && usuario.idUsuario != Guid.Empty)
            {
                DataTable dataTableProductosCargos = dataSet.Tables[9];
                Constantes.CARGOS_LIST = new List<Producto>();

                foreach (DataRow row in dataTableProductosCargos.Rows)
                {
                    Producto producto = new Producto();
                    producto.idProducto = Converter.GetGuid(row, "id_Producto");
                    producto.sku = Converter.GetString(row, "sku");
                    producto.descripcion = Converter.GetString(row, "descripcion");
                    Constantes.CARGOS_LIST.Add(producto);
                }
            }


            if (usuario.idUsuario != null && usuario.idUsuario != Guid.Empty)
            {
                DataTable dataTableProductosCargos = dataSet.Tables[11];
                Constantes.TIPO_CAMBIO_VIGENCIA_PRECIO_LIST = new List<MetaDataZAS>();

                foreach (DataRow row in dataTableProductosCargos.Rows)
                {
                    MetaDataZAS obj = new MetaDataZAS();
                    obj.codigo = Converter.GetString(row, "codigo");
                    obj.valor = Converter.GetString(row, "valor");
                    obj.descripcionCorta = Converter.GetString(row, "descripcion_corta");
                    Constantes.TIPO_CAMBIO_VIGENCIA_PRECIO_LIST.Add(obj);
                }
            }

            


            AlertaValidacionDAL alertaDal = new AlertaValidacionDAL();
            usuario.alertasList = alertaDal.getAlertasPorUsuario(usuario);


            return usuario;
        }

        public List<Usuario> getUsuariosMantenedor(Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_usuarios_mantenedor");
            InputParameterAdd.VarcharEmpty(objCommand, "textoBusqueda", usuario.nombre);
            InputParameterAdd.Int(objCommand, "estado", usuario.Estado);
            DataTable dataTable = Execute(objCommand);
            List<Usuario> lista = new List<Usuario>();

            foreach (DataRow row in dataTable.Rows)
            {
                Usuario obj = new Usuario();
                obj.idUsuario = Converter.GetGuid(row, "id_usuario");
                obj.email = Converter.GetString(row, "email");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.Estado = Converter.GetInt(row, "estado");
                lista.Add(obj);
            }

            return lista;
        }

        public Usuario getUsuarioMantenedor(Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_usuario_mantenedor");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable usuario = dataSet.Tables[0];
            DataTable permisos = dataSet.Tables[1];

            Usuario obj = new Usuario();

            foreach (DataRow row in usuario.Rows)
            {
                obj.idUsuario = Converter.GetGuid(row, "id_usuario");
                obj.email = Converter.GetString(row, "email");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.Estado = Converter.GetInt(row, "estado");
            }

            obj.permisoList = new List<Permiso>();
            foreach (DataRow row in permisos.Rows)
            {
                Permiso permiso = new Permiso();
                permiso.idPermiso = Converter.GetInt(row, "id_permiso");
                permiso.codigo = Converter.GetString(row, "codigo");
                permiso.descripcion_corta = Converter.GetString(row, "descripcion_corta");
                permiso.descripcion_larga = Converter.GetString(row, "descripcion_larga");
                permiso.categoriaPermiso = new CategoriaPermiso();
                permiso.categoriaPermiso.idCategoriaPermiso = Converter.GetInt(row, "id_categoria_permiso");
                permiso.categoriaPermiso.descripcion = Converter.GetString(row, "descripcion_categoria");
                permiso.byRol = Converter.GetInt(row, "es_rol") == 1;
                permiso.byUser = Converter.GetInt(row, "es_usuario") == 1;

                obj.permisoList.Add(permiso);
            }

            return obj;
        }


        public Usuario updatePermisos(Usuario obj)
        {
            var objCommand = GetSqlCommand("pu_usuario_permisos");
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.idUsuario);
            InputParameterAdd.Guid(objCommand, "idUsuarioModificacion", obj.IdUsuarioRegistro);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(int)));

            foreach (Permiso item in obj.permisoList)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item.idPermiso;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@permisos", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.IntegerList";


            ExecuteNonQuery(objCommand);

            return obj;
        }


        public List<Usuario> searchUsuariosVendedor(String textoBusqueda)
        {
            var objCommand = GetSqlCommand("ps_lista_usuarios");
            InputParameterAdd.Varchar(objCommand, "BusquedaUsuario", textoBusqueda);
            DataTable dataTable = Execute(objCommand);

            List<Usuario> usuarioList = new List<Usuario>();

            foreach (DataRow row in dataTable.Rows)
            {
                Usuario usuario = new Usuario();
                usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                usuario.email = Converter.GetString(row, "email");
                usuario.nombre = Converter.GetString(row, "nombre");
                usuarioList.Add(usuario);
            }

            return usuarioList;
        }

        public Usuario getUsuarioVendedor(Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_usuario_get_vendedor");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            DataTable dataTable = Execute(objCommand);

            Usuario obj = new Usuario();
            obj.sedeMP = new Ciudad();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idUsuario = Converter.GetGuid(row, "id_usuario");
                obj.cargo = Converter.GetString(row, "cargo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.email = Converter.GetString(row, "email");
                obj.contacto = Converter.GetString(row, "contacto");
                obj.sedeMP.idCiudad = Converter.GetGuid(row, "id_ciudad");
                obj.maximoPorcentajeDescuentoAprobacion = Converter.GetDecimal(row, "maximo_porcentaje_descuento_aprobacion");
            }

            return obj;
        }


        public Usuario getUsuarioMantenimiento(Guid? idUsuario)
        {
            var objCommand = GetSqlCommand("ps_detalle_usuario_edit");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            DataTable dataTable = Execute(objCommand);
            Usuario obj = new Usuario();
            obj.sedeMP = new Ciudad();
            foreach (DataRow row in dataTable.Rows)
            {
                obj.idUsuario = Converter.GetGuid(row, "id_usuario");
                obj.cargo = Converter.GetString(row, "cargo");
                obj.contacto = Converter.GetString(row, "contacto");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.Estado = Converter.GetInt(row, "estado");
                obj.email = Converter.GetString(row, "email");
                obj.password = Converter.GetString(row, "password");
                obj.esCliente = Converter.GetBool(row, "es_cliente");
                obj.sedeMP.idCiudad = Converter.GetGuid(row, "id_ciudad");
                obj.maximoPorcentajeDescuentoAprobacion = Converter.GetDecimal(row, "maximo_porcentaje_descuento_aprobacion");


            }

            return obj;
        }

        public Usuario insertUsuarioMantenedor(Usuario usuario)
        {
            var objCommand = GetSqlCommand("pi_usuario_mantenedor");
            InputParameterAdd.Guid(objCommand, "id_usuario_modificacion", usuario.idUsuarioModificacion);
            InputParameterAdd.Varchar(objCommand, "cargo", usuario.cargo);
            InputParameterAdd.Varchar(objCommand, "contacto", usuario.contacto);
            InputParameterAdd.Varchar(objCommand, "nombre", usuario.nombre);
            InputParameterAdd.Int(objCommand, "estado", usuario.Estado);
            InputParameterAdd.Bit(objCommand, "es_cliente", usuario.esCliente);
            InputParameterAdd.Varchar(objCommand, "email", usuario.email);
            InputParameterAdd.Varchar(objCommand, "pass", usuario.password);
            InputParameterAdd.Guid(objCommand, "id_ciudad", usuario.sedeMP.idCiudad);
            


            ExecuteNonQuery(objCommand);
            return usuario;
        }

        public Usuario updateUsuarioMantenedor(Usuario usuario)
        {
            var objCommand = GetSqlCommand("pu_usuario_mantenedor");
            InputParameterAdd.Guid(objCommand, "id_usuario", usuario.idUsuario);
            InputParameterAdd.Guid(objCommand, "id_usuario_modificacion", usuario.idUsuarioModificacion);
            InputParameterAdd.Varchar(objCommand, "cargo", usuario.cargo);
            InputParameterAdd.Varchar(objCommand, "contacto", usuario.contacto);
            InputParameterAdd.Varchar(objCommand, "nombre", usuario.nombre);
            InputParameterAdd.Int(objCommand, "estado", usuario.Estado);
            InputParameterAdd.Bit(objCommand, "es_cliente", usuario.esCliente);
            InputParameterAdd.Varchar(objCommand, "email", usuario.email);
            InputParameterAdd.Varchar(objCommand, "pass", usuario.password);
            InputParameterAdd.Guid(objCommand, "id_ciudad", usuario.sedeMP.idCiudad);
            InputParameterAdd.Decimal(objCommand, "max_por_des_apro", usuario.maximoPorcentajeDescuentoAprobacion);
            ExecuteNonQuery(objCommand);
            return usuario;
        }

        public bool confirmarPassword(string passActual, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_comparar_comtraseña");
            InputParameterAdd.Varchar(objCommand, "password_actual", passActual);
            InputParameterAdd.Guid(objCommand, "id_usuario", idUsuario);
            bool result = (bool)objCommand.ExecuteScalar();
            return result;
        }

        public void updateUsuarioCambioPassword(string passNuevo, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pu_cambiar_password");
            InputParameterAdd.Guid(objCommand, "id_usuario", idUsuario);
            InputParameterAdd.Varchar(objCommand, "pass_nuevo", passNuevo);
            ExecuteNonQuery(objCommand);            
        }

        public void updateUsuarioCambiarImagenFirma(Byte[] imagen, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pu_cambiar_firma_usuario");
            InputParameterAdd.Guid(objCommand, "id_usuario", idUsuario);
            InputParameterAdd.Binary(objCommand, "imagen", imagen);
            ExecuteNonQuery(objCommand);
        }
    }
}
