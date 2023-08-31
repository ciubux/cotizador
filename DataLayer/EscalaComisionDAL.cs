using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Model;
using System.Security.Principal;

namespace DataLayer
{
    public class EscalaComisionDAL : DaoBase
    {
        public EscalaComisionDAL(IDalSettings settings) : base(settings)
        {
        }
        public EscalaComisionDAL() : this(new CotizadorSettings())
        {
        }


        public List<EscalaComision> getEscalasComisionValidas(Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_escalas_comision_validas");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            DataTable dataTable = Execute(objCommand);
            List<EscalaComision> lista = new List<EscalaComision>();

            foreach (DataRow row in dataTable.Rows)
            {
                EscalaComision obj = new EscalaComision();
                obj.idEscalaComision = Converter.GetInt(row, "id_escala_comision");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.margenDesde = Converter.GetDecimal(row, "margen_desde");
                obj.margenDesdeCondicion = Converter.GetString(row, "margen_desde_condicion");
                obj.margenHasta = Converter.GetDecimal(row, "margen_hasta");
                obj.margenHastaCondicion = Converter.GetString(row, "margen_hasta_condicion");

                lista.Add(obj);
            }

            return lista;
        }


    }
}

