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
                //Pedidos
                usuario.tomaPedidos = Converter.GetBool(row, "toma_pedidos");
                usuario.apruebaPedidosLima = Converter.GetBool(row, "aprueba_pedidos_lima");
                usuario.apruebaPedidosProvincias = Converter.GetBool(row, "aprueba_pedidos_provincias");
                usuario.pedidoSerializado = Converter.GetString(row, "pedido_serializado");
                //Guia
                usuario.creaGuias = Converter.GetBool(row, "crea_guias");
                usuario.administraGuiasLima = Converter.GetBool(row, "administra_guias_lima");
                usuario.administraGuiasProvincias = Converter.GetBool(row, "administra_guias_provincias");
                //DOCUMENTOS VENTA
                usuario.creaDocumentosVenta = Converter.GetBool(row, "crea_documentos_venta");
                usuario.administraDocumentosVentaLima = Converter.GetBool(row, "administra_documentos_venta_lima");
                usuario.administraDocumentosVentaProvincias = Converter.GetBool(row, "administra_documentos_venta_provincias");

                usuario.sedeMP = new Ciudad();
                usuario.sedeMP.idCiudad = Converter.GetGuid(row, "id_ciudad");



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

            if (usuario.apruebaPedidos)
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


            return usuario;
        }
    }
}
