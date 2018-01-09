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
        
        public List<Producto> getProductosBusqueda(String textoBusqueda)
        {
            var objCommand = GetSqlCommand("ps_getproductos_search");
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

        public Producto getProducto(Guid idProducto, Boolean esProvincia)
        {
            var objCommand = GetSqlCommand("ps_getproducto");
            InputParameterAdd.Guid(objCommand, "idProducto", idProducto);
            DataTable dataTable = Execute(objCommand);
            Producto producto = new Producto();
            foreach (DataRow row in dataTable.Rows)
            {
                producto.idProducto = Converter.GetGuid(row, "id_producto");
                producto.descripcion = Converter.GetString(row, "descripcion");
                producto.sku = Converter.GetString(row, "sku");
                Byte[] img = new Byte[0];
                producto.image = (Byte[])(row["imagen"]);
                producto.precio = Converter.GetDecimal(row, "precio");
                producto.precio_provincia = Converter.GetDecimal(row, "precio_provincia");
                producto.valor = Converter.GetDecimal(row, "valor");
                producto.familia = Converter.GetString(row, "familia");
                producto.clase = Converter.GetString(row, "clase");
              //  producto.marca = Converter.GetString(row, "marca");
                producto.proveedor = Converter.GetString(row, "proveedor");
                producto.unidad = Converter.GetString(row, "unidad");
                producto.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                producto.equivalencia = Converter.GetDecimal(row, "equivalencia");
                //Si se ha seleccionado una provincia el precio a considerar es el precio de provincia
                if (esProvincia)
                {
                    producto.precio = producto.precio_provincia;
                }
            }
            return producto;
        }
    }
}
