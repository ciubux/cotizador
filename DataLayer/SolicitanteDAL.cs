using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class SolicitanteDAL : DaoBase
    {
        public SolicitanteDAL(IDalSettings settings) : base(settings)
        {
        }

        public SolicitanteDAL() : this(new CotizadorSettings())
        {
        }

        public List<Solicitante> getSolicitantes(Guid idCLiente)
        {
            var objCommand = GetSqlCommand("ps_solicitantes");
            InputParameterAdd.Guid(objCommand, "idCLiente", idCLiente); 

            DataTable dataTable = Execute(objCommand);
            List<Solicitante> lista = new List<Solicitante>();

            foreach (DataRow row in dataTable.Rows)
            {
                Solicitante obj = new Solicitante
                {
                    idSolicitante = Converter.GetGuid(row, "id_solicitante"),
                    nombre = Converter.GetString(row, "nombre"),
                    telefono = Converter.GetString(row, "telefono"),
                    correo = Converter.GetString(row, "correo")
                };
                lista.Add(obj);
            }
            return lista;
        }

        public List<Solicitante> getSolicitantesClienteSunat(int idClienteSunat)
        {
            var objCommand = GetSqlCommand("ps_solicitantes_cliente_sunat");
            InputParameterAdd.Int(objCommand, "idClienteSunat", idClienteSunat);

            DataTable dataTable = Execute(objCommand);
            List<Solicitante> lista = new List<Solicitante>();

            foreach (DataRow row in dataTable.Rows)
            {
                Solicitante obj = new Solicitante
                {
                    idSolicitante = Converter.GetGuid(row, "id_solicitante"),
                    nombre = Converter.GetString(row, "nombre"),
                    telefono = Converter.GetString(row, "telefono"),
                    correo = Converter.GetString(row, "correo")
                };
                lista.Add(obj);
            }
            return lista;
        }
        
    }
}
