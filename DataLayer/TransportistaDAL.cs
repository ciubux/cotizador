using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class TransportistaDAL : DaoBase
    {
        public TransportistaDAL(IDalSettings settings) : base(settings)
        {
        }

        public TransportistaDAL() : this(new CotizadorSettings())
        {
        }

        public List<Transportista> getTransportistas(Guid idCiudad)
        {
            var objCommand = GetSqlCommand("ps_getTransportistas");
            InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad); //puede ser null

            DataTable dataTable = Execute(objCommand);
            List<Transportista> lista = new List<Transportista>();

            foreach (DataRow row in dataTable.Rows)
            {
                Transportista obj = new Transportista
                {
                    idTransportista = Converter.GetGuid(row, "id_transportista"),
                    descripcion  = Converter.GetString(row, "descripcion"),
                    ruc = Converter.GetString(row, "ruc"),
                    direccion = Converter.GetString(row, "direccion"),
                    telefono = Converter.GetString(row, "telefono"),
                    observaciones = Converter.GetString(row, "observaciones"),
                    brevete = Converter.GetString(row, "brevete")
                };
                lista.Add(obj);
            }
            return lista;
        }
    }
}
