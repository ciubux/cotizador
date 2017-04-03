using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class CategoriaDAL : DaoBase
    {
        public CategoriaDAL(IDalSettings settings) : base(settings)
        {
        }

        public CategoriaDAL() : this(new CotizadorSettings())
        {
        }

        public List<Categoria> getCategorias()
        {
            var objCommand = GetSqlCommand("ps_getcategorias");
            DataTable dataTable = Execute(objCommand);
            List<Categoria> lista = new List<Categoria>();

            foreach (DataRow row in dataTable.Rows)
            {
                Categoria obj = new Categoria
                {
                    idCategoria = Converter.GetGuid(row, "id_categoria"),
                    nombre = Converter.GetString(row, "nombre"),
                    codigo = Converter.GetString(row, "codigo")
                };
                lista.Add(obj);
            }
            return lista;
        }
    }
}
