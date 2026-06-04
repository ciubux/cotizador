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
    public class EmpresaProductoSeparadoDAL : DaoBase
    {
        public EmpresaProductoSeparadoDAL(IDalSettings settings) : base(settings)
        {
        }

        public EmpresaProductoSeparadoDAL() : this(new CotizadorSettings())
        {
        }

        public List<EmpresaProductoSeparado> ValidarProductos(Guid idUsuario, Guid idCiudad, List<Guid> idsProductos, int estado = -1)
        {
            var objCommand = GetSqlCommand("ps_validarProductosSeparadosEmpresa");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad);
            InputParameterAdd.Int(objCommand, "estado", estado);
            //InputParameterAdd.Int(objCommand, "idEmpresa", idEmpresa);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(Guid)));

            foreach (Guid item in idsProductos)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idsProductos", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";


            DataTable dataTable = Execute(objCommand);
            List<EmpresaProductoSeparado> lista = new List<EmpresaProductoSeparado>();

            foreach (DataRow row in dataTable.Rows)
            {
                EmpresaProductoSeparado obj = new EmpresaProductoSeparado();

                obj.idEmpresaProductoSeparado = Converter.GetGuid(row, "id_empresa_producto_separado");

                obj.Estado = Converter.GetInt(row, "estado");

                obj.unidadConteo = Converter.GetString(row, "unidad_conteo");
                obj.cantidadSeparada = Converter.GetInt(row, "cantidad_separada");

                obj.empresa = new Empresa();
                obj.empresa.idEmpresa = Converter.GetInt(row, "id_empresa");
                obj.empresa.nombre = Converter.GetString(row, "empresa_nombre");
                obj.empresa.codigo = Converter.GetString(row, "empresa_codigo");

                obj.producto = new Producto();
                obj.producto.idProducto = Converter.GetGuid(row, "id_producto");
                obj.producto.sku = Converter.GetString(row, "producto_sku");
                obj.producto.descripcion = Converter.GetString(row, "producto_descripcion");

                obj.producto.equivalenciaAlternativa = Converter.GetInt(row, "equivalencia");
                obj.producto.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");
                obj.producto.equivalenciaUnidadEstandarUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_estandar_unidad_conteo");
                obj.producto.equivalenciaUnidadAlternativaUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_alternativa_unidad_conteo");
                obj.producto.equivalenciaUnidadProveedorUnidadConteo = Converter.GetInt(row, "equivalencia_unidad_proveedor_unidad_conteo");

                lista.Add(obj);
            }

            return lista;
        }
    }
}

