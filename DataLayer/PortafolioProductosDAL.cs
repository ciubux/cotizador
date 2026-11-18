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
    public class PortafolioProductosDAL : DaoBase
    {
        public PortafolioProductosDAL(IDalSettings settings) : base(settings)
        {
        }

        public PortafolioProductosDAL() : this(new CotizadorSettings())
        {
        }


        public PortafolioProductos getPortafolioProductos(Guid idPortafolio)
        {
            var objCommand = GetSqlCommand("ps_portafolio_productos");
            InputParameterAdd.Guid(objCommand, "idPortafolio", idPortafolio);

            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable headerTable = dataSet.Tables[0];
            DataTable productosTable = dataSet.Tables[1];
            DataTable usuariosTable = dataSet.Tables[2];
            DataTable rolesTable = dataSet.Tables[3];

            PortafolioProductos obj = new PortafolioProductos();

            foreach (DataRow row in headerTable.Rows)
            {
                obj.idPortafolioProductos = Converter.GetGuid(row, "id_portafolio_productos");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.Estado = Converter.GetInt(row, "estado");
                obj.comentarios = Converter.GetString(row, "comentarios");
            }

            foreach (DataRow row in productosTable.Rows)
            {
                PortafolioProductosItem item = new PortafolioProductosItem();
                item.producto.idProducto = Converter.GetGuid(row, "id_producto");
                item.producto.sku = Converter.GetString(row, "sku");
                item.Estado = 1;
                item.producto.descripcion = Converter.GetString(row, "descripcion");

                obj.items.Add(item);
            }

            foreach (DataRow row in usuariosTable.Rows)
            {
                Usuario usuario = new Usuario();
                usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                usuario.email = Converter.GetString(row, "email");
                usuario.nombre = Converter.GetString(row, "nombre");

                obj.usuarios.Add(usuario);
            }

            foreach (DataRow row in rolesTable.Rows)
            {
                Rol rol = new Rol();
                rol.idRol = Converter.GetInt(row, "id_rol");
                rol.codigo = Converter.GetString(row, "codigo");
                rol.nombre = Converter.GetString(row, "nombre");

                obj.roles.Add(rol);
            }

            return obj;
        }

        public List<PortafolioProductos> getPortafoliosProductos(string nombre, int estado)
        {
            var objCommand = GetSqlCommand("ps_portafolios_productos");
            InputParameterAdd.Varchar(objCommand, "nombre", nombre?.Trim() ?? string.Empty);
            InputParameterAdd.Int(objCommand, "estado", estado);

            DataTable dataTable = Execute(objCommand);
            List<PortafolioProductos> lista = new List<PortafolioProductos>();

            foreach (DataRow row in dataTable.Rows)
            {
                PortafolioProductos obj = new PortafolioProductos();
                obj.idPortafolioProductos = Converter.GetGuid(row, "id_portafolio_productos");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.Estado = Converter.GetInt(row, "estado");

                lista.Add(obj);
            }

            return lista;
        }

        public PortafolioProductos insertOrUpdatePortafolioProductos(PortafolioProductos obj, List<string> skus, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pi_portafolio_productos");

            if (obj.idPortafolioProductos != null && obj.idPortafolioProductos != Guid.Empty)
            {
                InputParameterAdd.Guid(objCommand, "idPortafolio", obj.idPortafolioProductos);
            }

            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            InputParameterAdd.Varchar(objCommand, "nombrePortafolio", obj.nombre?.Trim());
            InputParameterAdd.Varchar(objCommand, "comentarios", obj.comentarios?.Trim());

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(string)));

            if (skus != null)
            {
                foreach (string sku in skus)
                {
                    if (!string.IsNullOrWhiteSpace(sku))
                    {
                        DataRow rowObj = tvp.NewRow();
                        rowObj["ID"] = sku.Trim();
                        tvp.Rows.Add(rowObj);
                    }
                }
            }
            else if (obj.items != null)
            {
                foreach (PortafolioProductosItem item in obj.items)
                {
                    if (item.producto != null && !string.IsNullOrWhiteSpace(item.producto.sku))
                    {
                        DataRow rowObj = tvp.NewRow();
                        rowObj["ID"] = item.producto.sku.Trim();
                        tvp.Rows.Add(rowObj);
                    }
                }
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@skus", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.VarcharCList";

            OutputParameterAdd.UniqueIdentifier(objCommand, "idPortafolioNew");

            ExecuteNonQuery(objCommand);

            obj.idPortafolioProductos = (Guid)objCommand.Parameters["@idPortafolioNew"].Value;

            return obj;
        }
    }
}
