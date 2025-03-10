
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;
using Framework.DAL;
using System.Data.SqlClient;
using System.Data;

namespace BusinessLayer
{
    public class EmpresaBL
    {
        public Empresa GetEmpresa(int idEmpresa)
        {
            using (EmpresaDAL dal = new EmpresaDAL())
            {
                return dal.GetEmpresa(idEmpresa);
            }
        }

        public void ActualizarParametrosEmpresa(Empresa empresa)
        {
            using (EmpresaDAL dal = new EmpresaDAL())
            {
                dal.ActualizarParametrosEmpresa(empresa);
            }
        }
    }
}
