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
    public class LogCambioBL
    {

        public bool insertLogCambiosPogramados(List<LogCambio> cambios)
        {
            using (var dal = new LogCambioDAL())
            {
                foreach(LogCambio cambio in cambios)
                {
                    dal.insertLogProgramado(cambio);
                }

                return true;
            }
        }

        public bool insertLogCambios(List<LogCambio> cambios)
        {
            using (var dal = new LogCambioDAL())
            {
                foreach (LogCambio cambio in cambios)
                {
                    dal.insertLog(cambio);
                }

                return true;
            }
        }

        public bool insertLogCambiosPogramadosFull(List<LogCambio> cambios)
        {
            using (var dal = new LogCambioDAL())
            {
                foreach (LogCambio cambio in cambios)
                {
                    dal.insertLog(cambio);
                }

                return true;
            }
        }
        public List<LogCambio> getCambiosAplicar()
        {
            using (var dal = new LogCambioDAL())
            {
                return dal.getCambiosAplicar();
            }
        }
        
        public bool traspasarCambios(List<LogCambio> logs)
        {
            List<bool> repiteDato = new List<bool>();

            if (logs.Count <= 0)
            {
                return false;
            }
            
            LogCampoDAL logCampoDal = new LogCampoDAL();
            string tabla = logs.ElementAt<LogCambio>(0).tabla.nombre;
            List<LogCampo> campos = logCampoDal.getCampoLogPorTabla(Producto.NOMBRE_TABLA);
            List<LogCambio> aplicados = new List<LogCambio>();

            switch (tabla)
            {
                case Producto.NOMBRE_TABLA:
                    ProductoDAL prodDal = new ProductoDAL();
                    Guid idRegistro = Guid.Parse(logs.ElementAt<LogCambio>(0).idRegistro);
                    Producto prod = prodDal.GetProductoById(idRegistro);
                    if (prod.idProducto == null || prod.idProducto == Guid.Empty)
                    {   
                        string sku = "";
                        foreach (LogCambio cambio in logs)
                        {
                            if (cambio.campo.nombre.Equals("sku"))
                            {
                                sku = cambio.valor;
                            }
                        }

                        if (!sku.Equals(""))
                        {
                            prod.idProducto = prodDal.getProductoId(prod.sku);
                            prod = prodDal.GetProductoById(prod.idProducto);
                        }
                    }

                    aplicados = prod.aplicarCambios(logs);
                    prod.CargaMasiva = true;
                    prod.fechaInicioVigencia = logs.ElementAt<LogCambio>(0).fechaInicioVigencia;

                    prod.usuario = new Usuario();
                    prod.usuario.idUsuario = logs.ElementAt<LogCambio>(0).idUsuarioModificacion;
                    prod.IdUsuarioRegistro = prod.usuario.idUsuario;

                    if (aplicados.Count > 0)
                    {
                        if (prod.idProducto != null && prod.idProducto != Guid.Empty)
                        {
                            prodDal.updateProducto(prod);
                        }
                        else
                        {
                            prodDal.insertProducto(prod);
                            foreach (LogCambio cambio in logs)
                            {
                                cambio.idRegistro = prod.idProducto.ToString();
                            }
                        }
                    }
                    break;
            }

            if (aplicados.Count > 0)
            {
                using (var dal = new LogCambioDAL())
                {
                    dal.traspasarCambios(aplicados);
                }
            }

            return true;
        }

        public bool limpiarCambiosProgramados()
        {
            using (var dal = new LogCambioDAL())
            {
                dal.limpiarCambiosProgramados();

                return true;
            }
        }

        public bool aplicarLogCambios()
        {
            List<LogCambio> logs = this.getCambiosAplicar();

            List<LogCambio> registroLog = new List<LogCambio>();
            string idRegistro = "";
            string tabla = "";
            DateTime fiv = new DateTime();

            foreach (LogCambio log in logs)
            {
                if (!tabla.Equals(log.tabla.nombre) || !idRegistro.Equals(log.idRegistro) || fiv != log.fechaInicioVigencia)
                {
                    if (registroLog.Count > 0)
                    {
                        this.traspasarCambios(registroLog);
                    }
                    registroLog = new List<LogCambio>();
                    idRegistro = log.idRegistro;
                    tabla = log.tabla.nombre;
                    fiv = log.fechaInicioVigencia;
                }

                registroLog.Add(log);
            }

            if (registroLog.Count > 0)
            {
                this.traspasarCambios(registroLog);
            }

            return this.limpiarCambiosProgramados();
        }

        public bool UpdateLog(LogCambio cambio)
        {
            using (var dal = new LogCambioDAL())
            {
                dal.UpdatetLog(cambio);
              
                return true;
            }
        }
    }
}
