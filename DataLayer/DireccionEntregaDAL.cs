using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class DireccionEntregaDAL : DaoBase
    {
        public DireccionEntregaDAL(IDalSettings settings) : base(settings)
        {
        }

        public DireccionEntregaDAL() : this(new CotizadorSettings())
        {
        }

        public List<DireccionEntrega> getDireccionesEntrega(String ubigeo, Guid idCLiente)
        {
            var objCommand = GetSqlCommand("ps_DireccionesEntrega");

            InputParameterAdd.Varchar(objCommand, "ubigeo", ubigeo); 
            InputParameterAdd.Guid(objCommand, "idCLiente", idCLiente); 

            DataTable dataTable = Execute(objCommand);
            List<DireccionEntrega> lista = new List<DireccionEntrega>();

            foreach (DataRow row in dataTable.Rows)
            {
                DireccionEntrega obj = new DireccionEntrega
                {
                    idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega"),
                    descripcion = Converter.GetString(row, "descripcion"),
                    contacto = Converter.GetString(row, "contacto"),
                    telefono = Converter.GetString(row, "telefono"),
                    ubigeo = new Ubigeo {  Id = Converter.GetString(row, "ubigeo"),
                        Departamento = Converter.GetString(row, "departamento"),
                        Provincia = Converter.GetString(row, "provincia"),
                        Distrito = Converter.GetString(row, "distrito")
                    },
                    codigoCliente = Converter.GetString(row, "codigo_cliente"),
                    codigoMP = Converter.GetString(row, "codigo_mp"),
                    observaciones = Converter.GetString(row, "observaciones"),
                    nombre = Converter.GetString(row, "nombre")
                };
                lista.Add(obj);
            }
            return lista;
        }
    }
}
