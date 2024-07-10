using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class PrecioEspecialCabecera : Auditoria
    {
        public PrecioEspecialCabecera() 
        {
            
        }

        public Guid idPrecioEspecialCabecera { get; set; }

        [Display(Name = "Cliente RUC:")]
        public ClienteSunat clienteSunat { get; set; }

        [Display(Name = "Grupo Cliente:")]
        public GrupoCliente grupoCliente { get; set; }

        [Display(Name = "Código:")]
        public String codigo { get; set; }

        [Display(Name = "Título:")]
        public String titulo { get; set; }

        [Display(Name = "Observaciones:")]
        public String observaciones { get; set; }

        [Display(Name = "Tipo Negociación:")]
        public String tipoNegociacion { get; set; }

        [Display(Name = "Fecha Inicio:")]
        public DateTime fechaInicio { get; set; }

        [Display(Name = "Fecha Fin:")]
        public DateTime fechaFin { get; set; }

        [Display(Name = "Código Registro Proveedor:")]
        public String codigoListaProveedor { get; set; }

        
        public string FechaInicioDesc
        {
            get { return fechaInicio.ToString("dd/MM/yyyy"); }
        }

        public string fechaFinDesc
        {
            get { return fechaFin.ToString("dd/MM/yyyy"); }
        }

        public string clienteOGrupoDesc
        {
            get {
                string datos = "";

                if (tipoNegociacion.Equals("RUC") && clienteSunat != null)
                {
                    datos = clienteSunat.razonSocial + " (" + clienteSunat.ruc + ")";
                }

                if (tipoNegociacion.Equals("GRUPO") && grupoCliente != null)
                {
                    datos = grupoCliente.codigoNombre;
                }

                return datos; 
            }
        }


        public List<PrecioEspecialDetalle> precios { get; set; }
    }
}