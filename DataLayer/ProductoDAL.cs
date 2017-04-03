using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class ProductoDAL : DaoBase
    {
        public ProductoDAL(IDalSettings settings) : base(settings)
        {
        }

        public ProductoDAL() : this(new CotizadorSettings())
        {
        }
        
        public List<Producto> getProductosBusqueda(Guid idProveedor, Guid idFamilia, String textoBusqueda)
        {
            var objCommand = GetSqlCommand("ps_getproductos_search");
            InputParameterAdd.Guid(objCommand, "idFamilia", idFamilia);
            InputParameterAdd.Guid(objCommand, "idProveedor", idProveedor);
            InputParameterAdd.Varchar(objCommand, "textoBusqueda", textoBusqueda);
            DataTable dataTable = Execute(objCommand);
            List<Producto> lista = new List<Producto>();

            foreach (DataRow row in dataTable.Rows)
            {
                Producto obj = new Producto
                {
                    idProducto = Converter.GetGuid(row, "id_producto"),
                    descripcion = Converter.GetString(row, "descripcion"),
                    sku = Converter.GetString(row, "sku")
                };
                lista.Add(obj);
            }
            return lista;
        }

        public Producto getProducto(Guid idProducto)
        {
            var objCommand = GetSqlCommand("ps_getproducto");
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            DataTable dataTable = Execute(objCommand);
            Producto obj = new Producto();
            foreach (DataRow row in dataTable.Rows)
            {
                obj.idProducto = Converter.GetGuid(row, "id_producto");
                obj.descripcion = Converter.GetString(row, "descripcion");
                obj.sku = Converter.GetString(row, "sku");

                Unidad un = new Unidad();
                un.idUnidad = Converter.GetGuid(row, "id_unidad");
                un.descripcion = Converter.GetString(row, "desc_unidad");
                obj.unidad = un;
                
                Byte[] img = new Byte[0];
                obj.image = (Byte[])(row["imagen"]);
            }
            return obj;
        }
    }
}
