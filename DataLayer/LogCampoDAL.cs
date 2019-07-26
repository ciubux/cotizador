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
            var objCommand = GetSqlCommand("PS_CATALOGO_TABLA");
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


        public List<LogCampo> getCatalogoById(LogCampo Catalogo)
        {

            var objCommand = GetSqlCommand("PS_DETALLE_CATALOGO_CAMPO");
            InputParameterAdd.Int(objCommand, "ID_CATALOGO_TABLA", Catalogo.tablaId);
            DataTable dataTable = Execute(objCommand);
            List<LogCampo> listcata = new List<LogCampo>();

            foreach (DataRow row in dataTable.Rows)
            {
                LogCampo obj = new LogCampo();
                obj.catalogoId = Converter.GetInt(row, "id_catalogo_campo");
                obj.estadoCatalogo = Converter.GetInt(row, "estado");
                obj.puede_persistir = Converter.GetInt(row, "puede_persistir");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.id_catalogo_campo = Converter.GetInt(row, "id_catalogo_tabla");
                obj.tabla_referencia = Converter.GetString(row, "tabla_referencia");
                obj.orden = Converter.GetInt(row, "orden");
                obj.campo_referencia = Converter.GetString(row, "campo_referencia");
                listcata.Add(obj);

            }

            return listcata;
        }

        public LogCampo updateCatalogo(LogCampo obj)
        {
            var objCommand = GetSqlCommand("PU_CATALOGO_CAMPO");
            InputParameterAdd.Int(objCommand, "ID_CATALOGO_CAMPO", obj.catalogoId);
            InputParameterAdd.Int(objCommand, "ESTADO", obj.estadoCatalogo);
            InputParameterAdd.Int(objCommand, "PUEDE_PERSISTIR", obj.puede_persistir);
            ExecuteNonQuery(objCommand);

            return obj;

        }


    }
}

