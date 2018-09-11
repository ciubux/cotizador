using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

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

        public Usuario getUsuarioLogin(Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_usuario");
            InputParameterAdd.Varchar(objCommand, "email", usuario.email);
            InputParameterAdd.Varchar(objCommand, "password", usuario.password);
            DataSet dataSet = ExecuteDataSet(objCommand);

            DataTable dataTableUsuario = dataSet.Tables[0];   

            foreach (DataRow row in dataTableUsuario.Rows)
            {
                usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                usuario.cargo = Converter.GetString(row, "cargo");
                usuario.nombre = Converter.GetString(row, "nombre");
                usuario.contacto = Converter.GetString(row, "contacto");
                //Cotizaciones
                usuario.maximoPorcentajeDescuentoAprobacion = Converter.GetDecimal(row, "maximo_porcentaje_descuento_aprobacion");
                usuario.cotizacionSerializada = Converter.GetString(row, "cotizacion_serializada");
                usuario.apruebaCotizacionesLima = Converter.GetBool(row, "aprueba_cotizaciones_lima");
                usuario.apruebaCotizacionesProvincias = Converter.GetBool(row, "aprueba_cotizaciones_provincias");
                usuario.creaCotizaciones = Converter.GetBool(row, "crea_cotizaciones");
                usuario.creaCotizacionesProvincias = Converter.GetBool(row, "crea_cotizaciones_provincias");
                usuario.visualizaCotizaciones = Converter.GetBool(row, "visualiza_cotizaciones");

                //Pedidos
                usuario.tomaPedidos = Converter.GetBool(row, "toma_pedidos");
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

                //Guia
                usuario.creaGuias = Converter.GetBool(row, "crea_guias");
                usuario.administraGuiasLima = Converter.GetBool(row, "administra_guias_lima");
                usuario.administraGuiasProvincias = Converter.GetBool(row, "administra_guias_provincias");
                usuario.visualizaGuias = Converter.GetBool(row, "visualiza_guias_remision");

                //DOCUMENTOS VENTA
                usuario.creaDocumentosVenta = Converter.GetBool(row, "crea_documentos_venta");
                usuario.administraDocumentosVentaLima = Converter.GetBool(row, "administra_documentos_venta_lima");
                usuario.administraDocumentosVentaProvincias = Converter.GetBool(row, "administra_documentos_venta_provincias");
                usuario.visualizaDocumentosVenta = Converter.GetBool(row, "visualiza_documentos_venta");
                usuario.apruebaAnulaciones = Converter.GetBool(row, "aprueba_anulaciones");
                


                usuario.sedeMP = new Ciudad();
                usuario.sedeMP.idCiudad = Converter.GetGuid(row, "id_ciudad");

                usuario.modificaMaestroClientes = Converter.GetBool(row, "modifica_maestro_clientes");
                usuario.modificaMaestroProductos = Converter.GetBool(row, "modifica_maestro_productos");
                
               
                usuario.creaNotasCredito = Converter.GetBool(row, "crea_notas_credito");
                usuario.creaNotasDebito = Converter.GetBool(row, "crea_notas_debito");
                usuario.realizaRefacturacion = Converter.GetBool(row, "realiza_refacturacion");

                usuario.visualizaMargen = Converter.GetBool(row, "visualiza_margen");
                usuario.confirmaStock = Converter.GetBool(row, "confirma_stock");

                usuario.esCliente = Converter.GetBool(row, "es_cliente");

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
                        Constantes.OBSERVACION = valorParametro; break;
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

                    case "AMBIENTE_EOL":
                        { 
                            Constantes.AMBIENTE_EOL = valorParametro;
                          //  Constantes.AMBIENTE_EOL = "TEST";
                        }
                        break; //


                }

            }





            /*Si es usuario aprobador se recupera la lista de usuarios a los cuales puede aprobar cotizaciones*/
            if (usuario.apruebaCotizaciones)
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
            }
            return usuario;
        }
    }
}
