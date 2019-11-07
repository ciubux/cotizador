using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class GlobalDAL : DaoBase
    {
        public GlobalDAL(IDalSettings settings) : base(settings)
        {
        }

        public GlobalDAL() : this(new CotizadorSettings())
        {
        }


        public void JobDiario()
        {
            var objCommand = GetSqlCommand("pu_jobDiario");
            ExecuteNonQuery(objCommand);
        }
    }
}
