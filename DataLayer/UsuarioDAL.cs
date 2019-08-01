﻿using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Data.SqlClient;

namespace DataLayer
{
    public class UsuarioDAL : DaoBase
    {
        public UsuarioDAL(IDalSettings settings) : base(settings)
        {
        }

        public UsuarioDAL() : this(new CotizadorSettings())
        {
        }

        public List<Usuario> searchUsuarios(String textoBusqueda)
        {
            var objCommand = GetSqlCommand("ps_usuarios_search");
            InputParameterAdd.Varchar(objCommand, "textoBusqueda", textoBusqueda);
            DataTable dataTable = Execute(objCommand);

            List<Usuario> usuarioList = new List<Usuario>();

            foreach (DataRow row in dataTable.Rows)
            {
                Usuario usuario = new Usuario();
                usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                usuario.email = Converter.GetString(row, "email");
                usuario.nombre = Converter.GetString(row, "nombre");
                usuarioList.Add(usuario);
            }

            return usuarioList;
        }

        public void updateCotizacionSerializada(Usuario usuario,String cotizacionSerializada)
        {
            var objCommand = GetSqlCommand("pu_cotizacionSerializada");
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "cotizacionSerializada", -1, cotizacionSerializada);
            ExecuteNonQuery(objCommand);
        }

        public void updatePedidoSerializado(Usuario usuario, String pedidoSerializado)
        {
            var objCommand = GetSqlCommand("pu_pedidoSerializado");
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "pedidoSerializado", -1, pedidoSerializado);
            ExecuteNonQuery(objCommand);
        }


        public List<Usuario> selectUsuarios()
        {
            var objCommand = GetSqlCommand("ps_usuarios");
            DataTable dataTable = Execute(objCommand);
            List<Usuario> usuarioList = new List<Usuario>();

            foreach (DataRow row in dataTable.Rows)
            {
                Usuario usuario = new Usuario();
                usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                usuario.email = Converter.GetString(row, "email");
                usuario.nombre = Converter.GetString(row, "nombre");
                usuarioList.Add(usuario);
            }

            return usuarioList;
        }

        public List<Usuario> selectUsuariosPorPermiso(Permiso permiso)
        {
            var objCommand = GetSqlCommand("ps_usuariosPorPermiso");
            InputParameterAdd.Int(objCommand, "idPermiso", permiso.idPermiso);
            DataTable dataTable = Execute(objCommand);
            List<Usuario> usuarioList = new List<Usuario>();

            foreach (DataRow row in dataTable.Rows)
            {
                Usuario usuario = new Usuario();
                usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                usuario.email = Converter.GetString(row, "email");
                usuario.nombre = Converter.GetString(row, "nombre");
                usuarioList.Add(usuario);
            }

            return usuarioList;
        }

        public void updatePermiso(List<Usuario> usuarioLit, Permiso permiso, Usuario usuario)
        {
            var objCommand = GetSqlCommand("pu_usuarioPermiso");
            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("idUsuarioList", typeof(Guid)));

            // populate DataTable from your List here
            foreach (var id in usuarioLit)
                tvp.Rows.Add(id.idUsuario);            

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idUsuarioList", tvp);
            // these next lines are important to map the C# DataTable object to the correct SQL User Defined Type
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";

            InputParameterAdd.Int(objCommand, "idPermiso", permiso.idPermiso);
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            ExecuteNonQuery(objCommand);      
        }

        public Usuario getUsuario(Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_usuario_get");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            DataTable dataTable = Execute(objCommand);

            Usuario obj = new Usuario();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idUsuario = Converter.GetGuid(row, "id_usuario");
                obj.cargo = Converter.GetString(row, "cargo");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.email = Converter.GetString(row, "email");
                obj.contacto = Converter.GetString(row, "contacto");
            }

            return obj;
        }
        

        public Usuario getUsuarioLogin(Usuario usuario, String ruc)
        {
            var objCommand = GetSqlCommand("CLIENTE.ps_usuario");
            InputParameterAdd.Varchar(objCommand, "ruc", ruc);
            InputParameterAdd.Varchar(objCommand, "login", usuario.email);
            InputParameterAdd.Varchar(objCommand, "password", usuario.password);
            DataSet dataSet = ExecuteDataSet(objCommand);

            DataTable dataTableUsuario = dataSet.Tables[0];

            foreach (DataRow row in dataTableUsuario.Rows)
            {
                usuario.idUsuario = Converter.GetGuid(row, "id_cliente_usuario");
                usuario.cargo = Converter.GetString(row, "cargo");
                usuario.nombre = Converter.GetString(row, "nombre");
                usuario.contacto = Converter.GetString(row, "telefono");

                usuario.sedeMP = new Ciudad();
                usuario.sedeMP.idCiudad = Converter.GetGuid(row, "id_ciudad");
            }


            //Permisos de usuario
            if (usuario.idUsuario != null && usuario.idUsuario != Guid.Empty)
            {
                DataTable dataTablePermisos = dataSet.Tables[1];
                usuario.permisoList = new List<Permiso>();

                foreach (DataRow row in dataTablePermisos.Rows)
                {
                    Permiso permiso = new Permiso();
                    permiso.idPermiso = Converter.GetInt(row, "id_Permiso");
                    permiso.codigo = Converter.GetString(row, "codigo");
                    permiso.descripcion_corta = Converter.GetString(row, "descripcion_corta");
                    permiso.descripcion_larga = Converter.GetString(row, "descripcion_larga");
                    usuario.permisoList.Add(permiso);
                }
            }
           

            if (usuario.idUsuario != null && usuario.idUsuario != Guid.Empty)
            {
                DataTable dataTableClientes = dataSet.Tables[1];
                List<Cliente> clienteList = new List<Cliente>();

                foreach (DataRow row in dataTableClientes.Rows)
                {
                    Cliente cliente = new Cliente();
                    cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                    cliente.razonSocial = Converter.GetString(row, "razon_social");
                    cliente.codigo = Converter.GetString(row, "codigo");
                    cliente.ruc = Converter.GetString(row, "ruc");
                    cliente.nombreComercial = Converter.GetString(row, "nombre_comercial");
                    clienteList.Add(cliente);
                }
                usuario.clienteList = clienteList;
            }


            DataTable dataTableParametros = dataSet.Tables[3];
            foreach (DataRow row in dataTableParametros.Rows)
            {
                String codigoParametro = Converter.GetString(row, "codigo");
                String valorParametro = Converter.GetString(row, "valor");

                switch (codigoParametro)
                {
                    case "IGV":
                        Constantes.IGV = Decimal.Parse(valorParametro); break;
                    case "SIMBOLO_SOL":
                        Constantes.SIMBOLO_SOL = valorParametro; break;
                    case "DEBUG":
                        Constantes.DEBUG = int.Parse(valorParametro); break;
                    case "DIAS_MAX_BUSQUEDA_PRECIOS":
                        Constantes.DIAS_MAX_BUSQUEDA_PRECIOS = int.Parse(valorParametro); break;
                    case "OBSERVACION":
                        Constantes.OBSERVACION = valorParametro; break;
                    case "MILISEGUNDOS_AUTOGUARDADO":
                        Constantes.MILISEGUNDOS_AUTOGUARDADO = int.Parse(valorParametro); break;
                    case "DIAS_MAX_VIGENCIA_PRECIOS_COTIZACION":
                        Constantes.DIAS_MAX_VIGENCIA_PRECIOS_COTIZACION = int.Parse(valorParametro); break;
                    case "DIAS_MAX_VIGENCIA_PRECIOS_PEDIDO":
                        Constantes.DIAS_MAX_VIGENCIA_PRECIOS_PEDIDO = int.Parse(valorParametro); break;
                    case "VARIACION_PRECIO_ITEM_PEDIDO":
                        Constantes.VARIACION_PRECIO_ITEM_PEDIDO = Decimal.Parse(valorParametro); break;

                    case "RUC_MP":
                        Constantes.RUC_MP = valorParametro; break;
                    case "RAZON_SOCIAL_MP":
                        Constantes.RAZON_SOCIAL_MP = valorParametro; break;
                    case "DIRECCION_MP":
                        Constantes.DIRECCION_MP = valorParametro; break;
                    case "TELEFONO_MP":
                        Constantes.TELEFONO_MP = valorParametro; break;
                    case "WEB_MP":
                        Constantes.WEB_MP = valorParametro; break;
                }

            }
            return usuario;
        }

        public List<Usuario> getUsuariosMantenedor(Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_usuarios_mantenedor");
            InputParameterAdd.VarcharEmpty(objCommand, "textoBusqueda", usuario.nombre);
            InputParameterAdd.Int(objCommand, "estado", usuario.Estado);
            DataTable dataTable = Execute(objCommand);
            List<Usuario> lista = new List<Usuario>();

            foreach (DataRow row in dataTable.Rows)
            {
                Usuario obj = new Usuario();
                obj.idUsuario = Converter.GetGuid(row, "id_usuario");
                obj.email = Converter.GetString(row, "email");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.Estado = Converter.GetInt(row, "estado");
                lista.Add(obj);
            }

            return lista;
        }

        public Usuario getUsuarioMantenedor(Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_usuario_mantenedor");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable usuario = dataSet.Tables[0];
            DataTable permisos = dataSet.Tables[1];

            Usuario obj = new Usuario();

            foreach (DataRow row in usuario.Rows)
            {
                obj.idUsuario = Converter.GetGuid(row, "id_usuario");
                obj.email = Converter.GetString(row, "email");
                obj.nombre = Converter.GetString(row, "nombre");
                obj.Estado = Converter.GetInt(row, "estado");
            }

            obj.permisoList = new List<Permiso>();
            foreach (DataRow row in permisos.Rows)
            {
                Permiso permiso = new Permiso();
                permiso.idPermiso = Converter.GetInt(row, "id_permiso");
                permiso.codigo = Converter.GetString(row, "codigo");
                permiso.descripcion_corta = Converter.GetString(row, "descripcion_corta");
                permiso.descripcion_larga = Converter.GetString(row, "descripcion_larga");
                permiso.categoriaPermiso = new CategoriaPermiso();
                permiso.categoriaPermiso.idCategoriaPermiso = Converter.GetInt(row, "id_categoria_permiso");
                permiso.categoriaPermiso.descripcion = Converter.GetString(row, "descripcion_categoria");
                permiso.byRol = Converter.GetInt(row, "es_rol") == 1;
                permiso.byUser = Converter.GetInt(row, "es_usuario") == 1;

                obj.permisoList.Add(permiso);
            }

            return obj;
        }


        public Usuario updatePermisos(Usuario obj)
        {
            var objCommand = GetSqlCommand("pu_usuario_permisos");
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.idUsuario);
            InputParameterAdd.Guid(objCommand, "idUsuarioModificacion", obj.IdUsuarioRegistro);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(int)));

            foreach (Permiso item in obj.permisoList)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = item.idPermiso;
                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@permisos", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.IntegerList";


            ExecuteNonQuery(objCommand);

            return obj;
        }
    }
}
