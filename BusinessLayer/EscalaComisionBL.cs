
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class EscalaComisionBL
    {
       
        public List<EscalaComision> getEscalasComisionValidas(Guid idUsuario)
        {
            using (var dal = new EscalaComisionDAL())
            {
                return dal.getEscalasComisionValidas(idUsuario);
            }
        }
    }
}
