using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class FamiliaDAL : DaoBase
    {
        public FamiliaDAL(IDalSettings settings) : base(settings)
        {
        }

        public FamiliaDAL() : this(new CotizadorSettings())
        {
        }

        public List<Familia> getFamilias(Guid idCategoria)
        {
            var objCommand = GetSqlCommand("ps_getfamilias");
            InputParameterAdd.Guid(objCommand, "idCategoria", idCategoria);
            DataTable dataTable = Execute(objCommand);
            List<Familia> lista = new List<Familia>();

            foreach (DataRow row in dataTable.Rows)
            {
                Familia obj = new Familia
                {
                    idFamilia = Converter.GetGuid(row, "id_familia"),
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
