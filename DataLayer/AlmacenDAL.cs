using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;

namespace DataLayer
{
    public class AlmacenDAL : DaoBase
    {
        public AlmacenDAL(IDalSettings settings) : base(settings)
        {
        }

        public AlmacenDAL() : this(new CotizadorSettings())
        {
        }

        public List<Almacen> getAlmacenesSedes(Guid idCiudad)
        {
            var objCommand = GetSqlCommand("ps_almacenesSede");
            InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad);
            DataTable dataTable = Execute(objCommand);
            List<Almacen> lista = new List<Almacen>();

            foreach (DataRow row in dataTable.Rows)
            {
                Almacen obj = new Almacen();

                obj.idAlmacen = Converter.GetGuid(row, "id_almacen");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.direccion = Converter.GetString(row, "direccion");
                obj.esPrincipal = Converter.GetBool(row, "es_principal");
                obj.Estado = 1;

                obj.ubigeo = new Ubigeo();
                obj.ubigeo.Id = Converter.GetString(row, "ubigeo");
                obj.codigoSucursalNextSoft = Converter.GetString(row, "codigo_sucursal_nextsoft");
                obj.codigoPuntoVentaNextSoft = Converter.GetString(row, "codigo_punto_venta_nextsoft");
                obj.codigoAlmacenNextSoft = Converter.GetString(row, "codigo_almacen_nextsoft");

                lista.Add(obj);
            }

            return lista;
        }

    }
}
