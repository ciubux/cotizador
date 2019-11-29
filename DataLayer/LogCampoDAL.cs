using DataLayer.DALConverter;
using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using Model;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class LogCampoDAL : DaoBase
    {
        public LogCampoDAL(IDalSettings settings) : base(settings)
        {
        }

        public LogCampoDAL() : this(new CotizadorSettings())
        {
        }

        public List<LogCampo> getCampoLogPorTabla(String nombreTabla)
        {

            var objCommand = GetSqlCommand("ps_campos_log");
            InputParameterAdd.Varchar(objCommand, "nombreCatalogoTabla", nombreTabla);
            DataTable dataTable = Execute(objCommand);
            List<LogCampo> lista = new List<LogCampo>();

            foreach (DataRow row in dataTable.Rows)
            {
                LogCampo obj = new LogCampo();
                obj.idCampo = Converter.GetInt(row, "id_catalogo_campo");
                obj.idTabla = Converter.GetInt(row, "id_catalogo_Tabla");
                obj.nombre = Converter.GetString(row, "campo");
                obj.puedePersistir = Converter.GetInt(row, "puede_persistir") == 1 ? true : false;

                lista.Add(obj);
            }

            return lista;
        }


        public List<LogCampo> getTablaList()
        {
            var objCommand = GetSqlCommand("ps_catalogo_tabla");
            DataTable dataTable = Execute(objCommand);
            List<LogCampo> lista = new List<LogCampo>();

            foreach (DataRow row in dataTable.Rows)
            {
                LogCampo obj = new LogCampo();

                obj.tablaId = Converter.GetInt(row, "ID_CATALOGO_TABLA");
                obj.nombreTabla = Converter.GetString(row, "NOMBRE");
                obj.estadoTabla = Converter.GetInt(row, "ESTADO");
                lista.Add(obj);
            }
            return lista;
        }


        public List<LogCampo> getCatalogo(LogCampo logCampo)
        {

            var objCommand = GetSqlCommand("ps_detalle_catalogo_campo");
            InputParameterAdd.Int(objCommand, "ID_CATALOGO_TABLA", logCampo.tablaId);
            DataTable dataTable = Execute(objCommand);
            List<LogCampo> listcata = new List<LogCampo>();

            foreach (DataRow row in dataTable.Rows)
            {
                LogCampo obj = new LogCampo();
                obj.catalogoId = Converter.GetInt(row, "id_catalogo_campo");
                obj.estadoCatalogo = Converter.GetInt(row, "estado");
                obj.puede_persistir = Converter.GetInt(row, "puede_persistir");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.orden = Converter.GetInt(row, "orden");
                obj.esFuncional = Converter.GetInt(row, "es_funcional");
                listcata.Add(obj);

            }

            var objCommand2 = GetSqlCommand("ps_add_catalogo_campo");
            InputParameterAdd.Int(objCommand2, "id_tabla", logCampo.tablaId);
            dataTable = Execute(objCommand2);


            foreach (DataRow row in dataTable.Rows)
            {
                LogCampo obj = new LogCampo();

                obj.nombre = Converter.GetString(row, "COLUMN_NAME");
                listcata.Add(obj);
            }
            return listcata;
        }

        public LogCampo updateLogCampo(LogCampo obj)
        {
            var objCommand = GetSqlCommand("pu_catalogo_campo");
            InputParameterAdd.Int(objCommand, "ID_CATALOGO_CAMPO", obj.catalogoId);
            InputParameterAdd.Int(objCommand, "ESTADO", obj.estadoCatalogo);
            InputParameterAdd.Int(objCommand, "PUEDE_PERSISTIR", obj.puede_persistir);
            ExecuteNonQuery(objCommand);

            return obj;

        }

        public LogCampo insertLogCampo(LogCampo obj)
        {
            var objCommand = GetSqlCommand("pi_insert_add_log_campo");
            InputParameterAdd.Int(objCommand, "id_catalogo_tabla", obj.tablaId);
            InputParameterAdd.Int(objCommand, "ESTADO", obj.estadoCatalogo);
            InputParameterAdd.Int(objCommand, "PUEDE_PERSISTIR", obj.puede_persistir);
            InputParameterAdd.Varchar(objCommand, "name_log_campo", obj.nombre);


            ExecuteNonQuery(objCommand);

            return obj;

        }


    }
}

