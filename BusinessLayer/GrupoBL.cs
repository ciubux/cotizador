
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class GrupoBL
    {
        public List<Grupo> getGruposBusqueda(String textoBusqueda)
        {
            using (var grupoDAL = new GrupoDAL())
            {
                return grupoDAL.getGruposBusqueda(textoBusqueda);
            }
        }
        public Grupo getGrupo(Guid idGrupo)
        {
            using (var grupoDAL = new GrupoDAL())
            {
                return grupoDAL.getGrupo(idGrupo);
            }
        }
    }
}
