using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using Model.UTILES;

namespace DataLayer
{
    public class PermisoDAL : DaoBase
    {
        public PermisoDAL(IDalSettings settings) : base(settings)
        {
        }

        public PermisoDAL() : this(new CotizadorSettings())
        {
        }




        public Permiso updatePermiso(Permiso obj)
        {
            var objCommand = GetSqlCommand("pu_permiso");
            InputParameterAdd.Int(objCommand, "idPermiso", obj.idPermiso);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "descripcionCorta", obj.descripcion_corta);
            InputParameterAdd.Varchar(objCommand, "descripcionLarga", obj.descripcion_larga);

            ExecuteNonQuery(objCommand);

            return obj;
        }


        public List<Permiso> selectPermisos()
        {
            var objCommand = GetSqlCommand("ps_permisos");
            DataTable dataTable = Execute(objCommand);
            List<Permiso> permisoList = new List<Permiso>();

            foreach (DataRow row in dataTable.Rows)
            {
                Permiso permiso = new Permiso();
                permiso.idPermiso = Converter.GetInt(row, "id_permiso");
                permiso.codigo = Converter.GetString(row, "codigo");
                permiso.descripcion_corta = Converter.GetString(row, "descripcion_corta");
                permiso.descripcion_larga = Converter.GetString(row, "descripcion_larga");
                permiso.categoriaPermiso = new CategoriaPermiso();
                permiso.categoriaPermiso.idCategoriaPermiso = Converter.GetInt(row, "id_categoria_permiso");
                permiso.categoriaPermiso.descripcion = Converter.GetString(row, "descripcion_categoria");
                permisoList.Add(permiso);
            }
            return permisoList;
        }

        public Permiso getPermiso(int idPermiso)
        {
            var objCommand = GetSqlCommand("ps_permiso");
            InputParameterAdd.Int(objCommand, "idPermiso", idPermiso);

            DataTable dataTable = Execute(objCommand);
            Permiso permiso = new Permiso();

            foreach (DataRow row in dataTable.Rows)
            {
                permiso.idPermiso = Converter.GetInt(row, "id_permiso");
                permiso.codigo = Converter.GetString(row, "codigo");
                permiso.descripcion_corta = Converter.GetString(row, "descripcion_corta");
                permiso.descripcion_larga = Converter.GetString(row, "descripcion_larga");

                permiso.categoriaPermiso = new CategoriaPermiso();
                permiso.categoriaPermiso.idCategoriaPermiso = Converter.GetInt(row, "id_categoria_permiso");
            }
            return permiso;
        }

        public ReporteMatriz ListaPermisosUsuarios(Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_permisos_usuarios_list");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable permisosDataTable = dataSet.Tables[0];
            DataTable usuariosDataTable = dataSet.Tables[1];
            DataTable valoresDataTable = dataSet.Tables[2];

            ReporteMatriz matriz = new ReporteMatriz();
            matriz.filas = new List<ReporteMatrizCabecera>();
            matriz.columnas = new List<ReporteMatrizCabecera>();
            matriz.datos = new List<ReporteMatrizDato>();
            matriz.agrupaColumnas = true;
            matriz.concatenaValores = true;
            matriz.concatenador = " ";
            matriz.tieneDescripcionColumnas = true;
            matriz.etiquetaColumnas = "Permisos";
            matriz.etiquetaFilas = "Usuario";

            foreach (DataRow row in permisosDataTable.Rows)
            {
                ReporteMatrizCabecera obj = new ReporteMatrizCabecera(); 
                obj.codigo = Converter.GetString(row, "id_permiso");
                obj.nombre = Converter.GetString(row, "descripcion_corta");
                obj.nombreAgrupador = Converter.GetString(row, "categoria");
                obj.descripcion = Converter.GetString(row, "descripcion_larga");

                matriz.columnas.Add(obj);
            }

            foreach (DataRow row in usuariosDataTable.Rows)
            {
                ReporteMatrizCabecera obj = new ReporteMatrizCabecera();
                obj.codigo = Converter.GetString(row, "id_usuario");
                obj.nombre = Converter.GetString(row, "nombre");

                matriz.filas.Add(obj);
            }

            foreach (DataRow row in valoresDataTable.Rows)
            {
                ReporteMatrizDato obj = new ReporteMatrizDato();
                obj.codigoColumna = Converter.GetString(row, "id_permiso");
                obj.codigoFila = Converter.GetString(row, "id_usuario");
                obj.valor = Converter.GetString(row, "valor");

                matriz.datos.Add(obj);
            }

            return matriz;
        }

        public ReporteMatriz ListaPermisosRoles(Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_permisos_roles_list");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable permisosDataTable = dataSet.Tables[0];
            DataTable rolesDataTable = dataSet.Tables[1];
            DataTable valoresDataTable = dataSet.Tables[2];

            ReporteMatriz matriz = new ReporteMatriz();
            matriz.filas = new List<ReporteMatrizCabecera>();
            matriz.columnas = new List<ReporteMatrizCabecera>();
            matriz.datos = new List<ReporteMatrizDato>();
            matriz.agrupaColumnas = true;
            matriz.tieneDescripcionColumnas = true;
            matriz.concatenaValores = false;
            matriz.etiquetaColumnas = "Permisos";
            matriz.etiquetaFilas = "Roles";

            foreach (DataRow row in permisosDataTable.Rows)
            {
                ReporteMatrizCabecera obj = new ReporteMatrizCabecera();
                obj.codigo = Converter.GetString(row, "id_permiso");
                obj.nombre = Converter.GetString(row, "descripcion_corta");
                obj.nombreAgrupador = Converter.GetString(row, "categoria");
                obj.descripcion = Converter.GetString(row, "descripcion_larga");

                matriz.columnas.Add(obj);
            }

            foreach (DataRow row in rolesDataTable.Rows)
            {
                ReporteMatrizCabecera obj = new ReporteMatrizCabecera();
                obj.codigo = Converter.GetString(row, "id_rol");
                obj.nombre = Converter.GetString(row, "nombre");

                matriz.filas.Add(obj);
            }

            foreach (DataRow row in valoresDataTable.Rows)
            {
                ReporteMatrizDato obj = new ReporteMatrizDato();
                obj.codigoColumna = Converter.GetString(row, "id_permiso");
                obj.codigoFila = Converter.GetString(row, "id_rol");
                obj.valor = "X";

                matriz.datos.Add(obj);
            }

            return matriz;
        }
    }
}

