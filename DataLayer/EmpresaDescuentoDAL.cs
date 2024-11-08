using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;
using System.Data.SqlClient;

namespace DataLayer
{
    public class EmpresaDescuentoDAL : DaoBase
    {
        public EmpresaDescuentoDAL(IDalSettings settings) : base(settings)
        {
        }

        public EmpresaDescuentoDAL() : this(new CotizadorSettings())
        {
        }

        public List<EmpresaDescuento> Listar(int idEmpresa, Guid idUsuario, Guid idCiudad, int estado = -1)
        {
            var objCommand = GetSqlCommand("ps_descuentosProductoEmpresa");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Int(objCommand, "estado", estado);
            InputParameterAdd.Int(objCommand, "idEmpresa", idEmpresa);
            InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad);
            DataTable dataTable = Execute(objCommand);
            List<EmpresaDescuento> lista = new List<EmpresaDescuento>();

            foreach (DataRow row in dataTable.Rows)
            {
                EmpresaDescuento obj = new EmpresaDescuento();

                obj.idEmpresadescuento = Converter.GetGuid(row, "id_empresa_descuento_dase_producto");

                obj.tipoDescuento = Converter.GetString(row, "tipo_descuento");
                obj.descuento = Converter.GetDecimal(row, "descuento_base");
                obj.Estado = Converter.GetInt(row, "estado"); 

                obj.empresa = new Empresa();
                obj.empresa.idEmpresa = Converter.GetInt(row, "id_empresa");
                obj.empresa.nombre = Converter.GetString(row, "empresa_nombre");
                obj.empresa.codigo = Converter.GetString(row, "empresa_codigo");

                obj.ciudad = new Ciudad();
                obj.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                obj.ciudad.nombre = obj.ciudad.idCiudad.Equals(Guid.Empty) ? "Todos" : Converter.GetString(row, "ciudad_nombre");

                obj.producto = new Producto();
                obj.producto.idProducto = Converter.GetGuid(row, "id_producto");
                obj.producto.sku = Converter.GetString(row, "producto_sku");
                obj.producto.descripcion = Converter.GetString(row, "producto_descripcion");

                lista.Add(obj);
            }

            return lista;
        }

        public bool CargaMasiva(Guid idUsuario, int idEmpresa, List<EmpresaDescuento> lista)
        {
            var objCommand = GetSqlCommand("pi_empresaDescuentoProducto");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Int(objCommand, "idEmpresa", idEmpresa);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID_CIUDAD", typeof(Guid)));
            tvp.Columns.Add(new DataColumn("SKU", typeof(string)));
            tvp.Columns.Add(new DataColumn("TIPO_DESCUENTO", typeof(string)));
            tvp.Columns.Add(new DataColumn("DESCUENTO", typeof(decimal)));


            foreach (EmpresaDescuento obj in lista)
            {
                DataRow rowObj = tvp.NewRow();

                rowObj["ID_CIUDAD"] = obj.ciudad.idCiudad;
                rowObj["SKU"] = obj.producto.sku;
                rowObj["TIPO_DESCUENTO"] = obj.tipoDescuento;
                rowObj["DESCUENTO"] = obj.descuento;

                tvp.Rows.Add(rowObj);
            }
            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@items", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.EmpresaDescuentoProductoList";


            ExecuteNonQuery(objCommand);

            return true;
        }

    }
}

