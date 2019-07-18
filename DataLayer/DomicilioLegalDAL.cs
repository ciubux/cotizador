using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class DomicilioLegalDAL : DaoBase
    {
        public DomicilioLegalDAL(IDalSettings settings) : base(settings)
        {
        }

        public DomicilioLegalDAL() : this(new CotizadorSettings())
        {
        }

        
        public List<DomicilioLegal> getDomiciliosLegalesPorCliente(Cliente cliente)
        {
            List<DomicilioLegal> domicilioLegalList = new List<DomicilioLegal>();
            var objCommand = GetSqlCommand("ps_domiciliosLegalesPorCliente");
            InputParameterAdd.Guid(objCommand, "idCliente", cliente.idCliente);
            DataTable dataTable = Execute(objCommand);

            foreach (DataRow row in dataTable.Rows)
            {
                DomicilioLegal domicilioLegal = new DomicilioLegal();
                domicilioLegal.idDomicilioLegal = Converter.GetInt(row, "id_domicilio_legal");
                domicilioLegal.codigo = Converter.GetString(row, "codigo");
                domicilioLegal.direccion = Converter.GetString(row, "direccion");
                domicilioLegal.tipoEstablecimiento = Converter.GetString(row, "tipo_establecimiento");
                domicilioLegal.esEstablecimientoAnexo = Converter.GetBool(row, "es_establecimiento_anexo");
                domicilioLegal.ubigeo = new Ubigeo
                {
                    Id = Converter.GetString(row, "ubigeo"),
                    Departamento = Converter.GetString(row, "departamento"),
                    Provincia = Converter.GetString(row, "provincia"),
                    Distrito = Converter.GetString(row, "distrito")
                };
                domicilioLegalList.Add(domicilioLegal);

            }

            return domicilioLegalList;
        }
        
    }
}
