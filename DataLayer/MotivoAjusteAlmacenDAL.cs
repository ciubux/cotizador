using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class MotivoAjusteAlmacenDAL : DaoBase
    {
        public MotivoAjusteAlmacenDAL(IDalSettings settings) : base(settings)
        {
        }
        public MotivoAjusteAlmacenDAL() : this(new CotizadorSettings())
        {
        }



        public List<MotivoAjusteAlmacen> getMotivos(MotivoAjusteAlmacen motivo)
        {
            var objCommand = GetSqlCommand("ps_motivos_ajuste_almacen");
            InputParameterAdd.Int(objCommand, "estado", motivo.Estado);
            DataTable dataTable = Execute(objCommand);
            List<MotivoAjusteAlmacen> lista = new List<MotivoAjusteAlmacen>();

            foreach (DataRow row in dataTable.Rows)
            {
                MotivoAjusteAlmacen obj = new MotivoAjusteAlmacen();
                obj.idMotivoAjusteAlmacen = Converter.GetInt(row, "id_motivo_ajuste_stock");
                obj.descripcion = Converter.GetString(row, "descripcion");
                obj.tipo = Converter.GetInt(row, "tipo");
                obj.Estado = Converter.GetInt(row, "estado");
                lista.Add(obj);
            }
            
            return lista;
        }
    }
}
