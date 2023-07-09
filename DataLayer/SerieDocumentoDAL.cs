﻿using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Data.SqlClient;

namespace DataLayer
{
    public class SerieDocumentoDAL : DaoBase
    {
        public SerieDocumentoDAL(IDalSettings settings) : base(settings)
        {
        }

        public SerieDocumentoDAL() : this(new CotizadorSettings())
        {
        }

        public List<SerieDocumentoElectronico> selectSeriesDocumento(Guid idSede)
        {
            var objCommand = GetSqlCommand("ps_seriesDocumentoPorSede");
            InputParameterAdd.Guid(objCommand, "idSedeMP", idSede); 

            DataTable dataTable = Execute(objCommand);

            List<SerieDocumentoElectronico> serieDocumentoElectronicoList = new List<SerieDocumentoElectronico>();

            foreach (DataRow row in dataTable.Rows)
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
            return serieDocumentoElectronicoList;
        }

        public SerieDocumentoElectronico selectSerieDocumento(String tipo, Guid idSede)
        {
            SqlCommand objCommand = null;

            switch (tipo)
            {
                case "DIFERIDA": objCommand = GetSqlCommand("ps_serieDocumentoDiferidaPorSede"); break;
                case "VENTA": objCommand = GetSqlCommand("ps_serieDocumentoElectronicoPorSede"); break;
                case "TRASLADOINTERNO": objCommand = GetSqlCommand("ps_serieTrasladoInternoPorSede"); break;
                case "TRASLADOSEDES": objCommand = GetSqlCommand("ps_serieTrasladoSedes"); break;
            }

            InputParameterAdd.Guid(objCommand, "idSedeMP", idSede);

            DataTable dataTable = Execute(objCommand);

            SerieDocumentoElectronico serieDocumentoElectronico = new SerieDocumentoElectronico();

            foreach (DataRow row in dataTable.Rows)
            {
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
            }
            return serieDocumentoElectronico;
        }
    }
}
