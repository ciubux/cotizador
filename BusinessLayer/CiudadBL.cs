﻿
using DataLayer;
using System.Collections.Generic;
using System;
using Model;

namespace BusinessLayer
{
    public class CiudadBL
    {
        public List<Ciudad> getCiudades()
        {
            using (var ciudadDAL = new CiudadDAL())
            {
                return ciudadDAL.getCiudades();
            }
        }

        public Ciudad getCiudad(Guid idCiudad)
        {
            using (var ciudadDAL = new CiudadDAL())
            {
                return ciudadDAL.getCiudad(idCiudad);
            }
        }


        /*
                
        public List<TipoMuestra> GetTiposMuestraByIdExamen(Guid idExamen)
        {
            using (var tipoMuestraDal = new TipoMuestraDal())
            {
                return tipoMuestraDal.GetTiposMuestraByIdExamen(idExamen);
            }
        }

        public List<TipoMuestra> GetTiposMuestraByIdExamen(List<Guid> idExamenList)
        {
            using (var tipoMuestraDal = new TipoMuestraDal())
            {
                List<TipoMuestra> tipoMuestraList = new List<TipoMuestra>();
                foreach (Guid idExamen in idExamenList)
                {
                    List<TipoMuestra> tipoMuestraListTmp = tipoMuestraDal.GetTiposMuestraByIdExamen(idExamen);

                    foreach (TipoMuestra tipoMuestraTmp in tipoMuestraListTmp)
                    {
                        bool tipoMuestraExiste = false;
                        foreach (TipoMuestra tipoMuestra in tipoMuestraList)
                        {
                            if (tipoMuestraTmp.idTipoMuestra == tipoMuestra.idTipoMuestra)
                            {
                                tipoMuestraExiste = true;
                                break;
                            }
                        }
                        if (!tipoMuestraExiste)
                        {
                            tipoMuestraList.Add(tipoMuestraTmp);
                        }
                    }

                    
                }

                return tipoMuestraList;
            }
            
        }

        public TipoMuestra GetTiposMuestraById(int idTipoMuestra)
        {
            using (var tipoMuestraDal = new TipoMuestraDal())
            {
                return tipoMuestraDal.GetTiposMuestraById(idTipoMuestra);
            }
        }

        public TipoMuestra GetTipoMuestraById(int idTipoMuestra)
        {
            using (var tipoMuestraDal = new TipoMuestraDal())
            {
                return tipoMuestraDal.GetTipoMuestraById(idTipoMuestra);
            }
        }

        public void InsertTipoMuestra(TipoMuestra tipoM)
        {
            using (var tipoMuestraDal = new TipoMuestraDal())
            {
                tipoMuestraDal.InsertTipoMuestra(tipoM);
            }
        }

        public void UpdateTipoMuestra(TipoMuestra tipoM)
        {
            using (var tipoMuestraDal = new TipoMuestraDal())
            {
                tipoMuestraDal.UpdateTipoMuestra(tipoM);
            }
        }*/
    }
}
