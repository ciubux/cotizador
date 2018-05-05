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
                    esProvincia = Converter.GetBool(row, "es_provincia"),
                    direccionPuntoPartida = Converter.GetString(row, "direccion_punto_partida"),
                    serieGuiaRemision = Converter.GetString(row, "serie_guia_remision"),
                    sede = Converter.GetString(row, "sede")
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
            Ciudad ciudad = new Ciudad();

            foreach (DataRow row in dataTable.Rows)
            {
                ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                ciudad.orden = Converter.GetInt(row, "orden");
                ciudad.nombre = Converter.GetString(row, "nombre");
                ciudad.esProvincia = Converter.GetBool(row, "es_provincia");
                ciudad.direccionPuntoPartida = Converter.GetString(row, "direccion_punto_partida");
                ciudad.serieGuiaRemision = Converter.GetString(row, "serie_guia_remision");
                ciudad.siguienteNumeroGuiaRemision = Converter.GetInt(row, "siguiente_numero_guia_remision");
                ciudad.sede = Converter.GetString(row, "sede");
            }
            return ciudad;
        }
    }
}
