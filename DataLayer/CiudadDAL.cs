using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class CiudadDAL : DaoBase
    {
        public CiudadDAL(IDalSettings settings) : base(settings)
        {
        }

        public CiudadDAL() : this(new CotizadorSettings())
        {
        }

        public List<Ciudad> getCiudades()
        {
            var objCommand = GetSqlCommand("ps_getCiudades");
            DataTable dataTable = Execute(objCommand);
            List<Ciudad> ciudadList = new List<Ciudad>();

            foreach (DataRow row in dataTable.Rows)
            {
                Ciudad ciudad = new Ciudad
                {
                    idCiudad = Converter.GetGuid(row, "id_ciudad"),
                    nombre = Converter.GetString(row, "nombre"),
                    orden = Converter.GetInt(row, "orden"),
                    direccionPuntoPartida = Converter.GetString(row, "direccion_punto_partida")
                };
                ciudadList.Add(ciudad);
            }
            return ciudadList;
        }

        public Ciudad getCiudad(Guid idCiudad)
        {
            var objCommand = GetSqlCommand("ps_getciudad");
            InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad);
            DataTable dataTable = Execute(objCommand);
            Ciudad obj = new Ciudad();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idCiudad = Converter.GetGuid(row, "id_ciudad");
                obj.orden = Converter.GetInt(row, "orden");
                obj.nombre = Converter.GetString(row, "nombre");
            }
            return obj;
        }
    }
}
