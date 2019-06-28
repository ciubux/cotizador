using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Data;

namespace DataLayer
{
    public class CatalogoDAL : DaoBase
    {
        public CatalogoDAL(IDalSettings settings) : base(settings)
        {
        }

        public CatalogoDAL() : this(new CotizadorSettings())
        {
        }


        public List<Catalogo> getCatalogoList()
        {
            var objCommand = GetSqlCommand("PS_CATALOGO_CAMPO");
            DataTable dataTable = Execute(objCommand);
            List<Catalogo> lista = new List<Catalogo>();

            foreach (DataRow row in dataTable.Rows)
            {
                Catalogo obj = new Catalogo();

                obj.catalogoId = Converter.GetInt(row, "ID_CATALOGO_CAMPO");
                obj.nombre = Converter.GetString(row, "NOMBRE");
                obj.estado = Converter.GetInt(row, "ESTADO");
                obj.puede_persistir = Converter.GetInt(row, "puede_persistir");
                lista.Add(obj);
            }
            return lista;
        }


        public Catalogo getCatalogo(Catalogo idCatalogo)
        {

            var objCommand = GetSqlCommand("PS_DETALLE_CATALOGO_CAMPO");
            InputParameterAdd.Int(objCommand, "ID_CATALOGO_CAMPO", idCatalogo.catalogoId);
            DataTable dataTable = Execute(objCommand);
            Catalogo obj = new Catalogo();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.catalogoId = Converter.GetInt(row, "id_catalogo_campo");
                obj.estado = Converter.GetInt(row, "estado");
                obj.puede_persistir = Converter.GetInt(row, "puede_persistir");

                obj.codigo = Converter.GetString(row, "codigo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.id_catalogo_campo = Converter.GetInt(row, "id_catalogo_tabla");
                obj.tabla_referencia = Converter.GetInt(row, "tabla_referencia");
                obj.orden = Converter.GetInt(row, "orden");
                obj.campo_referencia = Converter.GetInt(row, "campo_referencia");


            }

            return obj;
        }

        public Catalogo updateCatalogo(Catalogo obj)
        {
            var objCommand = GetSqlCommand("PU_CATALOGO_CAMPO");
            InputParameterAdd.Int(objCommand, "ID_CATALOGO_CAMPO", obj.catalogoId);
            InputParameterAdd.Int(objCommand, "ESTADO", obj.estado);
            InputParameterAdd.Int(objCommand, "PUEDE_PERSISTIR", obj.puede_persistir);


            ExecuteNonQuery(objCommand);

            return obj;

        }




    }  


}

