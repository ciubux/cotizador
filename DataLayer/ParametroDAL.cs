using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;

namespace DataLayer
{
    public class ParametroDAL : DaoBase
    {
        public ParametroDAL(IDalSettings settings) : base(settings)
        {
        }

        public ParametroDAL() : this(new CotizadorSettings())
        {
        }
        
        public String getParametro(String codigo)
        {
            var objCommand = GetSqlCommand("ps_getParametro");
            InputParameterAdd.Varchar(objCommand, "codigo", codigo);
            DataTable dataTable = Execute(objCommand);

            string valor = "";
            foreach (DataRow row in dataTable.Rows)
            {
                valor = Converter.GetString(row, "valor");
            }
            return valor;
        }

        public void updateParametro(String codigo, String valor)
        {
            var objCommand = GetSqlCommand("pu_parametro");
            InputParameterAdd.Varchar(objCommand, "codigo", codigo);
            InputParameterAdd.Varchar(objCommand, "valor", valor);
            ExecuteNonQuery(objCommand);




        }

        public List<Parametro> getListaParametro(Parametro param)
        {
            var objCommand = GetSqlCommand("ps_lista_parametros");
            List<Parametro> lista = new List<Parametro>();
            InputParameterAdd.VarcharEmpty(objCommand, "codigo", param.codigo);
            
            DataTable dataTable = Execute(objCommand);
            foreach (DataRow row in dataTable.Rows)
            {
                Parametro obj = new Parametro();
                obj.idParametro = Converter.GetGuid(row, "id_parametro");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.descripcion = Converter.GetString(row, "descripcion");
                obj.valor = Converter.GetString(row, "valor");
                lista.Add(obj);
            }

            return lista;
        }
                
        public void modificarParametro(Parametro param,Usuario user)
        {
            var objCommand = GetSqlCommand("pu_parametros");           
            InputParameterAdd.Guid(objCommand, "id_parametro", param.idParametro);            
            InputParameterAdd.Varchar(objCommand, "valor", param.valor);
            InputParameterAdd.VarcharEmpty(objCommand, "descripcion", param.descripcion);
            InputParameterAdd.Guid(objCommand, "usuario_modificacion", user.idUsuario);
            ExecuteNonQuery(objCommand);
        }

        public int GetDataFacturacionEmpresaEOL(string eolId)
        {
            var objCommand = GetSqlCommand("ps_userEolById");
            InputParameterAdd.Varchar(objCommand, "eolId", eolId);


            OutputParameterAdd.Int(objCommand, "idEmpresa");
            OutputParameterAdd.Varchar(objCommand, "userEol", 500);
            OutputParameterAdd.Varchar(objCommand, "passwordEol", 500);

            ExecuteNonQuery(objCommand);

            int idEmpresa = (Int32)objCommand.Parameters["@idEmpresa"].Value;

            string userEol = (string)objCommand.Parameters["@userEol"].Value;
            string passwordEol = (string)objCommand.Parameters["@passwordEol"].Value;

            Constantes.USER_EOL_PROD = userEol;
            Constantes.PASSWORD_EOL_PROD = passwordEol;

            return idEmpresa;
        }
    }
}
