using DataLayer;
using Model;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class UbigeoBL
    {
        public static List<UbigeoDTO> ubigeoDTOList;

        /*   public List<UbigeoDTO> ObtenerDepartamentos()
           {

               if (UbigeoBl.ubigeoDTOList == null)
               {
                   try
                   {
                       using (var ubigeoDal = new UbigeoDal())
                       {
                           ubigeoDTOList = ubigeoDal.ObtenerUbigeos();
                       }
                   }
                   catch (Exception e)
                   {
                       String A = e.Message;
                   }
               }
               return ubigeoDTOList;

           }*/


        public Ubigeo getUbigeoPorDistritoProvincia(String distrito, String provincia)
        {
            using (var ubigeoDal = new UbigeoDAL())
            {
                return ubigeoDal.getUbigeoPorDistritoProvincia(distrito, provincia);
            }
        }

        public List<Ubigeo> GetDepartamentos()
        {
            using (var ubigeoDal = new UbigeoDAL())
            {
                return ubigeoDal.ObtenerDepartamentos();
            }
        }

        public List<Ubigeo> GetProvincias(string idDepartamento)
        {
            using (var ubigeoDal = new UbigeoDAL())
            {
                return ubigeoDal.ObtenerProvincias(idDepartamento);
            }
        }

        public List<Ubigeo> GetDistritos(string idDepartamento, string idProvincia)
        {
            using (var ubigeoDal = new UbigeoDAL())
            {
                return ubigeoDal.ObtenerDistritos(idDepartamento, idProvincia);
            }
        }
        /*
                public Ubigeo GetUbigeoById(string idUbigeo)
                {
                    using (var ubigeoDal = new UbigeoDAL())
                    {
                        return ubigeoDal.GetUbigeoById(idUbigeo);
                    }
                }*/
    }
}
