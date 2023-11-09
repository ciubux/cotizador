using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;

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
            var objCommand = GetSqlCommand("ps_ciudades");
            DataSet dataSet = ExecuteDataSet(objCommand);

            DataTable dataTableCiudad = dataSet.Tables[0];
            DataTable dataTableSerie = dataSet.Tables[1];

            List<Ciudad> ciudadList = new List<Ciudad>();

            foreach (DataRow row in dataTableCiudad.Rows)
            {
                Ciudad ciudad = new Ciudad
                {
                    idCiudad = Converter.GetGuid(row, "id_ciudad"),
                    nombre = Converter.GetString(row, "nombre"),
                    orden = Converter.GetInt(row, "orden"),
                    esProvincia = Converter.GetBool(row, "es_provincia"),
                    direccionPuntoPartida = Converter.GetString(row, "direccion_punto_partida"),
                    serieGuiaRemision = Converter.GetString(row, "serie_guia_remision"),
                    sede = Converter.GetString(row, "sede"),
                    codigoSede = Converter.GetString(row, "codigo_sede"),
                    correoCoordinador = Converter.GetString(row, "correo_coordinador")
                };
                ciudadList.Add(ciudad);
            }

            List<SerieDocumentoElectronico> serieDocumentoElectronicoList = new List<SerieDocumentoElectronico>();

            foreach (DataRow row in dataTableSerie.Rows)
            {
                SerieDocumentoElectronico serieDocumentoElectronico = new SerieDocumentoElectronico();
                serieDocumentoElectronico.sedeMP = new Ciudad();
                serieDocumentoElectronico.sedeMP.idCiudad = Converter.GetGuid(row, "id_sede_mp");
                serieDocumentoElectronico.serie = Converter.GetString(row, "serie");
                serieDocumentoElectronico.esPrincipal = Converter.GetBool(row, "es_principal");
                serieDocumentoElectronico.siguienteNumeroBoleta = Converter.GetInt(row, "siguiente_numero_boleta");
                serieDocumentoElectronico.siguienteNumeroFactura = Converter.GetInt(row, "siguiente_numero_factura");
                serieDocumentoElectronico.siguienteNumeroNotaCredito = Converter.GetInt(row, "siguiente_numero_nota_credito");
                serieDocumentoElectronico.siguienteNumeroNotaDebito = Converter.GetInt(row, "siguiente_numero_nota_debito");
                serieDocumentoElectronico.siguienteNumeroGuiaRemision = Converter.GetInt(row, "siguiente_numero_guia_remision");
                serieDocumentoElectronico.siguienteNumeroNotaIngreso = Converter.GetInt(row, "siguiente_numero_nota_ingreso");
                serieDocumentoElectronico.siguienteNumeroNotaCreditoBoleta = Converter.GetInt(row, "siguiente_numero_nota_credito_boleta");
                serieDocumentoElectronico.siguienteNumeroNotaDebitoBoleta = Converter.GetInt(row, "siguiente_numero_nota_debito_boleta");
                serieDocumentoElectronicoList.Add(serieDocumentoElectronico);
            }

            foreach (Ciudad ciudad in ciudadList)
            {
                ciudad.serieDocumentoElectronicoList = serieDocumentoElectronicoList.Where(s => s.sedeMP.idCiudad == ciudad.idCiudad).ToList();
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
