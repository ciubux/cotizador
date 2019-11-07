
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class GlobalBL
    {
        public void JobDiario()
        {
            using (var dal = new GlobalDAL())
            {
                dal.JobDiario();
            }
        }
    }
}
