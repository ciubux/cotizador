
using DataLayer;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BusinessLayer
{
    public class ConsolidadoAtencionBL
    {
        public ConsolidadoAtencion getConsolidadoAtencion(Guid idConsolidado)
        {
            using (ConsolidadoAtencionDAL dal = new ConsolidadoAtencionDAL())
            {
                return dal.getConsolidadoAtencion(idConsolidado);
            }
        }

        public List<ConsolidadoAtencion> getConsolidadosAtencion(ConsolidadoAtencion obj)
        {
            using (ConsolidadoAtencionDAL dal = new ConsolidadoAtencionDAL())
            {
                return dal.getConsolidadosAtencion(obj);
            }
        }

        public ConsolidadoAtencion insertConsolidadoAtencion(ConsolidadoAtencion obj)
        {
            using (ConsolidadoAtencionDAL dal = new ConsolidadoAtencionDAL())
            {
                return dal.insertConsolidadoAtencion(obj);
            }
        }

        public ConsolidadoAtencion updateConsolidadoAtencion(ConsolidadoAtencion obj)
        {
            using (ConsolidadoAtencionDAL dal = new ConsolidadoAtencionDAL())
            {
                return dal.updateConsolidadoAtencion(obj);
            }
        }

        public List<ConsolidadoAtencion> getConsolidadosAtencionPedidos(List<Guid> idsConsolidados)
        {
            using (ConsolidadoAtencionDAL dal = new ConsolidadoAtencionDAL())
            {
                return dal.getConsolidadosAtencionPedidos(idsConsolidados);
            }
        }

        public List<ConsolidadoAtencion> getConsolidadoAtencionsReparto(List<Guid> idsConsolidados)
        {
            List<ConsolidadoAtencion> lista = new List<ConsolidadoAtencion>();

            using (ConsolidadoAtencionDAL dal = new ConsolidadoAtencionDAL())
            {
                lista = dal.getConsolidadoAtencionsReparto(idsConsolidados);
                
                foreach (ConsolidadoAtencion obj in lista)
                {
                    resumenDetalleGuias(obj);
                }
            }
            return lista;
        }

        public ConsolidadoAtencion resumenDetalleGuias(ConsolidadoAtencion obj)
        {
            obj.resumenDetalleGuias = new List<DocumentoDetalle>();

            foreach (GuiaRemision guia in obj.guias)
            {
                foreach (DocumentoDetalle det in guia.documentoDetalle)
                {
                    if (det.producto != null && !det.producto.idProducto.Equals(Guid.Empty))
                    {
                        DocumentoDetalle item = obj.resumenDetalleGuias.Where(d => d.producto != null && d.producto.idProducto.Equals(det.producto.idProducto)).FirstOrDefault();
                        
                        if (item == null)
                        {
                            //Clonar datos para modificar a discrecion
                            item = JsonConvert.DeserializeObject<DocumentoDetalle>(JsonConvert.SerializeObject(det));
                            obj.resumenDetalleGuias.Add(item);
                        } else
                        {
                            //Si son las mismas unidades se suman las cantidades, sino se convierte a la unidad menor y se suman las cantidades.
                            if (item.ProductoPresentacion.IdProductoPresentacion == det.ProductoPresentacion.IdProductoPresentacion)
                            {
                                item.cantidad += det.cantidad;
                            } else
                            {
                                if (item.ProductoPresentacion.Equivalencia > det.ProductoPresentacion.Equivalencia)
                                {
                                    //Convertir cantidad de "det" a cantidad de "item"
                                    int equivalencia = 1;

                                    if (det.ProductoPresentacion.IdProductoPresentacion == 2)
                                    {
                                        equivalencia = equivalencia * det.producto.equivalenciaProveedor;
                                    }

                                    if (item.ProductoPresentacion.IdProductoPresentacion == 1)
                                    {
                                        equivalencia = equivalencia * det.producto.equivalenciaAlternativa;
                                    }

                                    item.cantidad += det.cantidad * equivalencia;
                                } else
                                {
                                    //Convertir cantidad de "item" a cantidad de "det"
                                    int equivalencia = 1;

                                    if (item.ProductoPresentacion.IdProductoPresentacion == 2)
                                    {
                                        equivalencia = equivalencia * det.producto.equivalenciaProveedor;
                                    }

                                    if (det.ProductoPresentacion.IdProductoPresentacion == 1)
                                    {
                                        equivalencia = equivalencia * det.producto.equivalenciaAlternativa;
                                    }

                                    item.cantidad = item.cantidad * equivalencia;
                                    item.cantidad += det.cantidad;

                                    item.ProductoPresentacion.IdProductoPresentacion = det.ProductoPresentacion.IdProductoPresentacion;
                                    item.ProductoPresentacion.Equivalencia = det.ProductoPresentacion.Equivalencia;
                                }
                            }
                        }

                    }
                }
            }

            return obj;
        }
    }
}
