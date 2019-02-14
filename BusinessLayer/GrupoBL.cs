
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
        public GrupoCliente getGrupo(int idGrupoCliente)
        {
            using (var grupoDAL = new GrupoClienteDAL())
            {
                return grupoDAL.getGrupo(idGrupoCliente);
            }
        }

        public List<GrupoCliente> getGruposCliente()
        {
            using (var grupoDAL = new GrupoClienteDAL())
            {
                return grupoDAL.getGruposCliente();
            }
        }

        public List<GrupoCliente> getGruposCliente(GrupoCliente grupoCliente)
        {
            using (var grupoDAL = new GrupoClienteDAL())
            {
                return grupoDAL.getGruposCliente(grupoCliente);
            }
        }


        public GrupoCliente insertGrupoCliente(GrupoCliente grupoCliente)
        {
            using (var grupoClienteDAL = new GrupoClienteDAL())
            { 
                return grupoClienteDAL.insertGrupoCliente(grupoCliente);
            }
        }

        public GrupoCliente updateGrupoCliente(GrupoCliente grupoCliente)
        {
            using (var grupoClienteDAL = new GrupoClienteDAL())
            {
                return grupoClienteDAL.updateGrupoCliente(grupoCliente);
            }
        }

    }
}
