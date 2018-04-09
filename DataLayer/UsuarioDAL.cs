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
            var objCommand = GetSqlCommand("pu_cotizacion_serializada");
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "cotizacionSerializada", -1, cotizacionSerializada);
            ExecuteNonQuery(objCommand);
        }

        public void updatePedidoSerializado(Usuario usuario, String pedidoSerializado)
        {
          /*  var objCommand = GetSqlCommand("pu_pedidoSerializado");
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "pedidoSerializado", -1, pedidoSerializado);
            ExecuteNonQuery(objCommand);*/
        }

        public Usuario getUsuarioLogin(Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_getusuario_login");
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
                usuario.esAprobador = Converter.GetBool(row, "es_aprobador");
                usuario.maximoPorcentajeDescuentoAprobacion = Converter.GetDecimal(row, "maximo_porcentaje_descuento_aprobacion");
                usuario.cotizacionSerializada = Converter.GetString(row, "cotizacion_serializada");

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
                }

            }





            /*Si es usuario aprobador se recupera la lista de usuarios a los cuales puede aprobar cotizaciones*/
            if (usuario.esAprobador)
            {
                DataTable dataTableUsuarios = dataSet.Tables[2];
                List<Usuario> usuarioList = new List<Usuario>();
              
                foreach (DataRow row in dataTableUsuarios.Rows)
                {
                    Usuario usuarioTmp = new Usuario();
                    usuarioTmp.idUsuario = Converter.GetGuid(row, "id_usuario");
                    usuarioTmp.nombre = Converter.GetString(row, "nombre");
                    usuarioList.Add(usuarioTmp);
                }
                usuario.usuarioList = usuarioList;
            }





            return usuario;
        }
    }
}
