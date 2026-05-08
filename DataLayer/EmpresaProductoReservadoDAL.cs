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
    public class EmpresaProductoReservadoDAL : DaoBase
    {
        public EmpresaProductoReservadoDAL(IDalSettings settings) : base(settings)
        {
        }

        public EmpresaProductoReservadoDAL() : this(new CotizadorSettings())
        {
        }

        public List<EmpresaProductoReservado> ValidarProductos(int idEmpresa, Guid idUsuario, List<Guid> idsProductos, int estado = -1)
        {
            var objCommand = GetSqlCommand("ps_validarProductosReservadosEmpresa");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Int(objCommand, "estado", estado);
            InputParameterAdd.Int(objCommand, "idEmpresa", idEmpresa);

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
            List<EmpresaProductoReservado> lista = new List<EmpresaProductoReservado>();

            foreach (DataRow row in dataTable.Rows)
            {
                EmpresaProductoReservado obj = new EmpresaProductoReservado();

                obj.idEmpresaProductoReservado = Converter.GetGuid(row, "id_empresa_producto_reservado");

                obj.Estado = Converter.GetInt(row, "estado");

                obj.unidadConteo = Converter.GetString(row, "unidad_conteo");
                obj.cantidadReservada = Converter.GetInt(row, "cantidad_reservada");
                obj.cantidadAtendida = Converter.GetInt(row, "cantidad_atendida");

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

