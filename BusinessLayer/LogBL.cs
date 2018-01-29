
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class LogBL
    {
     
        public void insertLog(Log log)
        {
            using (var logDAL = new LogDAL())
            {
                logDAL.insertLog(log);
            }
        }
    }
}
