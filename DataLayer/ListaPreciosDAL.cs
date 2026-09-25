using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Model;
using Model.UTILES;

namespace DataLayer
{
    public class ListaPreciosDAL : DaoBase
    {
        public ListaPreciosDAL(IDalSettings settings) : base(settings)
        {
        }

        public ListaPreciosDAL() : this(new CotizadorSettings())
        {
        }

        public ListaPrecios GetListaPrecios(Guid idListaPrecios, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_lista_precios");
            InputParameterAdd.Guid(objCommand, "idListaPrecios", idListaPrecios);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            DataSet dataSet = ExecuteDataSet(objCommand); 
            DataTable headerTable = dataSet.Tables[0]; 
            DataTable itemsTable = dataSet.Tables[1]; 

            ListaPrecios obj = new ListaPrecios();

            foreach (DataRow row in headerTable.Rows)
            {
                obj.idListaPrecios = Converter.GetGuid(row, "id_lista_precios"); 
                obj.empresa.idEmpresa = Converter.GetInt(row, "id_empresa"); 
                obj.nombre = Converter.GetString(row, "nombre");
                obj.comentarios = Converter.GetString(row, "comentarios");
                obj.Estado = Converter.GetInt(row, "estado"); 
            }

            foreach (DataRow row in itemsTable.Rows)
            {
                ListaPreciosItem item = new ListaPreciosItem();
                item.idListaPreciosItem = Converter.GetGuid(row, "id_lista_precios_item");
                item.idListaPrecios = Converter.GetGuid(row, "id_lista_precios");
                item.unidad = Converter.GetString(row, "unidad"); 
                item.idProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion");
                item.precio = Converter.GetDecimal(row, "precio");
                item.Estado = Converter.GetInt(row, "estado");

                item.producto.idProducto = Converter.GetGuid(row, "id_producto");
                item.producto.sku = Converter.GetString(row, "sku");
                item.producto.descripcion = Converter.GetString(row, "descripcion");
                item.producto.unidad = Converter.GetString(row, "unidad_mp");
                item.producto.unidadProveedor = Converter.GetString(row, "unidad_proveedor");
                item.producto.unidad_alternativa = Converter.GetString(row, "unidad_alternativa");
                item.producto.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia");
                item.producto.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");
                item.producto.precioSinIgv = Converter.GetDecimal(row, "precio_lima");
                item.producto.precioProvinciaSinIgv = Converter.GetDecimal(row, "precio_provincia");

                obj.items.Add(item);
            }

            return obj;
        }

        public List<ListaPrecios> GetListasPrecios(Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_listas_precios");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario); 

            DataTable dataTable = Execute(objCommand); 
            List<ListaPrecios> lista = new List<ListaPrecios>(); 

            foreach (DataRow row in dataTable.Rows)
            {
                ListaPrecios obj = new ListaPrecios();
                obj.idListaPrecios = Converter.GetGuid(row, "id_lista_precios"); 
                obj.empresa.idEmpresa = Converter.GetInt(row, "id_empresa");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.comentarios = Converter.GetString(row, "comentarios");
                obj.Estado = Converter.GetInt(row, "estado"); 
                
                lista.Add(obj);
            }

            return lista;
        }

        public ListaPrecios GuardarListaPrecios(ListaPrecios obj)
        {
            var objCommand = GetSqlCommand("pi_lista_precios");

            if (!obj.idListaPrecios.Equals(Guid.Empty))
            {
                InputParameterAdd.Guid(objCommand, "idListaPrecios", obj.idListaPrecios);
            }
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro); 
            InputParameterAdd.Varchar(objCommand, "nombre", obj.nombre?.Trim());
            InputParameterAdd.Varchar(objCommand, "comentarios", obj.comentarios?.Trim());

            DataTable tvp = new DataTable(); 
            tvp.Columns.Add(new DataColumn("ID_PRODUCTO", typeof(Guid)));
            tvp.Columns.Add(new DataColumn("SKU", typeof(string))); 
            tvp.Columns.Add(new DataColumn("PRECIO", typeof(decimal))); 
            tvp.Columns.Add(new DataColumn("ID_PRODUCTO_PRESENTACION", typeof(int))); 

            foreach (ListaPreciosItem item in obj.items)
            {
                DataRow rowObj = tvp.NewRow(); 
                rowObj["ID_PRODUCTO"] = item.producto.idProducto; 
                rowObj["SKU"] = item.producto.sku;
                rowObj["PRECIO"] = item.precio;
                rowObj["ID_PRODUCTO_PRESENTACION"] = item.idProductoPresentacion;

                tvp.Rows.Add(rowObj); 
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@precios", tvp); 
            tvparam.SqlDbType = SqlDbType.Structured; 
            tvparam.TypeName = "dbo.ListaPreciosItemList"; 

            OutputParameterAdd.UniqueIdentifier(objCommand, "idListaPreciosNew"); 

            ExecuteNonQuery(objCommand); 

            obj.idListaPrecios = (Guid)objCommand.Parameters["@idListaPreciosNew"].Value; 

            return obj;
        }

        public void EliminarListaPrecios(Guid idListaPrecios, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pu_lista_precios_eliminar");
            InputParameterAdd.Guid(objCommand, "idListaPrecios", idListaPrecios);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            ExecuteNonQuery(objCommand); 
        }
    }
}
