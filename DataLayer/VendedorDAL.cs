using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using Model;

namespace DataLayer
{
    public class VendedorDAL : DaoBase
    {

        public VendedorDAL(IDalSettings settings) : base(settings)
        {
        }
        public VendedorDAL() : this(new CotizadorSettings())
        {
        }


        public List<Vendedor> getVendedores(Vendedor vendedor)
        {
            var objCommand = GetSqlCommand("ps_vendedores");
            List<Vendedor> lista = new List<Vendedor>();

            InputParameterAdd.Int(objCommand, "estado", vendedor.estado);
            InputParameterAdd.VarcharEmpty(objCommand, "cod", vendedor.codigo);
            InputParameterAdd.VarcharEmpty(objCommand, "descripcion", vendedor.descripcion);
            InputParameterAdd.VarcharEmpty(objCommand, "email", vendedor.email);
            InputParameterAdd.Guid(objCommand, "ciudad", vendedor.ciudad.idCiudad);
            DataTable dataTable = Execute(objCommand);
            foreach (DataRow row in dataTable.Rows)
            {
                Vendedor obj = new Vendedor();
                obj.ciudad = new Ciudad();
                obj.idVendedor = Converter.GetInt(row, "id_vendedor");
                obj.descripcion = Converter.GetString(row, "descripcion");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.cargo = Converter.GetString(row, "cargo");
                obj.email = Converter.GetString(row, "email");
                obj.estado = Converter.GetInt(row, "estado");
                obj.ciudad.nombre = Converter.GetString(row, "nombre");
                lista.Add(obj);
            }
            return lista;
        }


        public Vendedor getVendedor(int idVendedor)
        {
            var objCommand = GetSqlCommand("ps_detalle_vendedores");
            InputParameterAdd.Int(objCommand, "idUsuarioVenta", idVendedor);
            DataTable dataTable = Execute(objCommand);
            Vendedor obj = new Vendedor();
            obj.supervisor = new Vendedor();
            obj.supervisor.usuario = new Usuario();
            obj.ciudad = new Ciudad();
            foreach (DataRow row in dataTable.Rows)
            {
                obj.idVendedor = Converter.GetInt(row, "id_vendedor");

                obj.idUsuarioVendedor = Converter.GetGuid(row, "id_usuario");
                obj.esAsistenteServicioCliente = Converter.GetBool(row, "es_asistente_servicio_cliente");
                obj.esSupervisorComercial = Converter.GetBool(row, "es_supervisor_comercial");
                obj.esResponsableComercial = Converter.GetBool(row, "es_responsable_comercial");
                obj.descripcion = Converter.GetString(row, "nombre");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.estado = Converter.GetInt(row, "estado");
                obj.cargo = Converter.GetString(row, "cargo");
                obj.contacto = Converter.GetString(row, "contacto");
                obj.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                obj.maxdesapro = Converter.GetDecimal(row, "maximo_porcentaje_descuento_aprobacion");

                obj.supervisor.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                obj.supervisor.usuario.email = Converter.GetString(row, "email");
                obj.supervisor.usuario.nombre = Converter.GetString(row, "nombre");

                obj.supervisor.idVendedor = Converter.GetInt(row, "id_supervisor_comercial");
                obj.supervisor.idUsuarioVendedor = Converter.GetGuid(row, "id_usuario_supervisor");
                obj.supervisor.descripcion = Converter.GetString(row, "nombre_supervisor");
                obj.supervisor.email = Converter.GetString(row, "email_supervisor");
                
            }

            return obj;
        }

        public Vendedor insertVendedor(Vendedor obj)
        {

            var objCommand = GetSqlCommand("pi_vendedor");
            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            //InputParameterAdd.Varchar(objCommand, "cargo", obj.cargo);
            //InputParameterAdd.Varchar(objCommand, "nombre", obj.descripcion);
            //InputParameterAdd.Varchar(objCommand, "contacto", obj.contacto);
            InputParameterAdd.Int(objCommand, "estado", obj.estado);
            InputParameterAdd.Guid(objCommand, "usuario_creacion", obj.usuario.idUsuario);
            InputParameterAdd.Decimal(objCommand, "maximo_descuento", obj.maxdesapro);
            InputParameterAdd.Guid(objCommand, "id_ciudad", obj.ciudad.idCiudad); 
            InputParameterAdd.Guid(objCommand, "id_usuario_vendedor", obj.supervisor.usuario.idUsuario);
            InputParameterAdd.Bit(objCommand, "es_supervisor_comercial", obj.esSupervisorComercial);
            InputParameterAdd.Bit(objCommand, "es_responsable_comercial", obj.esResponsableComercial);
            InputParameterAdd.Bit(objCommand, "es_asistente_servicio_cliente", obj.esAsistenteServicioCliente);
            InputParameterAdd.Int(objCommand, "id_supervisor_comercial", obj.supervisor.idVendedor);


            //OutputParameterAdd.UniqueIdentifier(objCommand, "newid");

            ExecuteNonQuery(objCommand);


            return obj;

        }


        public Vendedor updateVendedor(Vendedor obj)
        {
            var objCommand = GetSqlCommand("pu_vendedores");
            InputParameterAdd.Int(objCommand, "id_vendedor", obj.idVendedor);
            InputParameterAdd.Guid(objCommand, "usuario_modificacion", obj.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            //InputParameterAdd.Varchar(objCommand, "cargo", obj.cargo);
            //InputParameterAdd.Varchar(objCommand, "nombre", obj.descripcion);
            //InputParameterAdd.Varchar(objCommand, "contacto", obj.contacto);
            InputParameterAdd.Decimal(objCommand, "max_por_des_apro", obj.maxdesapro);
            InputParameterAdd.Int(objCommand, "estado", obj.estado);
            InputParameterAdd.Guid(objCommand, "id_ciudad", obj.ciudad.idCiudad);

            InputParameterAdd.Guid(objCommand, "id_usuario_vendedor", obj.supervisor.usuario.idUsuario);
            InputParameterAdd.Bit(objCommand, "es_supervisor_comercial", obj.esSupervisorComercial);
            InputParameterAdd.Bit(objCommand, "es_responsable_comercial", obj.esResponsableComercial);
            InputParameterAdd.Bit(objCommand, "es_asistente_servicio_cliente", obj.esAsistenteServicioCliente);
            InputParameterAdd.Int(objCommand, "id_supervisor_comercial", obj.supervisor.idVendedor);

            ExecuteNonQuery(objCommand);

            return obj;

        }

        public Vendedor getSupervisor(Guid idVendedor)
        {
            var objCommand = GetSqlCommand("ps_supervisor_get");
            InputParameterAdd.Guid(objCommand, "id_supervisor", idVendedor);
            DataTable dataTable = Execute(objCommand);
            Vendedor obj = new Vendedor();
            foreach (DataRow row in dataTable.Rows)
            {

                obj.idVendedor = Converter.GetInt(row, "id_vendedor");
                obj.idUsuarioVendedor = Converter.GetGuid(row, "id_usuario");
                obj.descripcion = Converter.GetString(row, "nombre");
                obj.email = Converter.GetString(row, "email");
            }

            return obj;
        }


        public List<Vendedor> searchVendedores(String textoBusqueda)
        {
            var objCommand = GetSqlCommand("ps_lista_vendedores_supervisores");
            InputParameterAdd.Varchar(objCommand, "BusquedaVendedor", textoBusqueda);
            DataTable dataTable = Execute(objCommand);

            List<Vendedor> usuarioList = new List<Vendedor>();

            foreach (DataRow row in dataTable.Rows)
            {
                Vendedor vendedor = new Vendedor();
                vendedor.supervisor = new Vendedor();
                vendedor.supervisor.idVendedor = Converter.GetInt(row, "id_vendedor");
                vendedor.supervisor.idUsuarioVendedor = Converter.GetGuid(row, "id_usuario");
                vendedor.supervisor.codigo = Converter.GetString(row, "codigo");
                vendedor.supervisor.descripcion = Converter.GetString(row, "nombre");
                vendedor.supervisor.email = Converter.GetString(row, "email");
                usuarioList.Add(vendedor);
            }

            return usuarioList;
        }

    }
}
