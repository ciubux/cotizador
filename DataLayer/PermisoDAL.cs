using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

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
    }
}
