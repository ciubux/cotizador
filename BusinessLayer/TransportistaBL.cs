
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using NPOI.SS.Formula.Functions;

namespace BusinessLayer
{
    public class TransportistaBL
    {
        public List<Transportista> getTransportistas(Guid idCiudad)
        {
            using (var dal = new TransportistaDAL())
            {
                return dal.getTransportistas(idCiudad);
            }
        }

        public Transportista transportistaDefectoEmrpesa(string codigoEmpresa) {
            Transportista transportista = new Transportista();

            if (codigoEmpresa.Equals(Constantes.EMPRESA_CODIGO_DISTRIPLUS))
            {
                transportista.idTransportista = Guid.Parse("1F23779D-1ADE-458E-B1C0-B4D2773541A7");
                transportista.ruc = "20611468548";
                transportista.descripcion = "Juan Hernàndez Olortegui";
                transportista.brevete = "Q03894685";
                transportista.direccion = "Av. los Precursores Nro. 435 Dpto. 205";
            }
            
            return transportista;
        }
    }
}
