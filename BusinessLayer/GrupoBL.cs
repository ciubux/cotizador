
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class GrupoClienteBL
    {
        public List<GrupoCliente> getGruposBusqueda(String textoBusqueda)
        {
            using (var grupoDAL = new GrupoClienteDAL())
            {
                return grupoDAL.getGruposBusqueda(textoBusqueda);
            }
        }
        public GrupoCliente getGrupo(Guid idGrupo)
        {
            using (var grupoDAL = new GrupoClienteDAL())
            {
                return grupoDAL.getGrupo(idGrupo);
            }
        }

        public List<GrupoCliente> getGruposCliente()
        {
            using (var grupoDAL = new GrupoClienteDAL())
            {
                return grupoDAL.getGruposCliente();
            }
        }
    }
}
