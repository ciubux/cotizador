using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Cotizacion
    {

        public enum OpcionesConsiderarCantidades {
            [Display(Name = "Solo Observaciones")]
            Observaciones = 0,
            [Display(Name = "Solo Cantidades")]
            Cantidades = 1,
            [Display(Name = "Cantidades y Observaciones")]
            Ambos =2};

        public Guid idCotizacion { get; set; }       
        
        public Int64 codigo { get; set; }
        public DateTime fecha { get; set; }
        public Boolean fechaEsModificada { get; set; }

        /* 0 indica días, 1 indica fecha */
        public int mostrarValidezOfertaEnDias { get; set; }
        //plazo oferta dias
        public int validezOfertaEnDias { get; set; }

        public DateTime fechaLimiteValidezOferta { get; set; }

        public DateTime? fechaFinVigenciaPrecios { get; set; }
        public DateTime? fechaInicioVigenciaPrecios { get; set; }

        public DateTime fechaModificacion { get; set; }

        public Usuario usuario { get; set; }

        public Boolean incluidoIgv { get; set; }
        public Decimal flete { get; set; }
        public String usuarioCreacion { get; set; }
      //  public Usuario usuario_aprobador { get; set; }
        public Decimal igv { get; set; }
        public Moneda moneda { get; set; }
        public Ciudad ciudad { get; set; }
        public Cliente cliente { get; set; }
        public Grupo grupo { get; set; }
        public OpcionesConsiderarCantidades considerarCantidades { get; set; }
        public Boolean mostrarCodigoProveedor { get; set; }
        public Decimal montoSubTotal { get; set; }
        public Decimal montoIGV { get; set; }
        public Decimal montoTotal { get; set; }
        public String observaciones { get; set; }
        public String contacto { get; set; }
        public List<CotizacionDetalle> cotizacionDetalleList { get; set; }
        public bool esRecotizacion { get; set; }
        /*0 pendiente, 1 aprobado, 2 rechazado*/
        public SeguimientoCotizacion seguimientoCotizacion { get; set; }

        public List<SeguimientoCotizacion> seguimientoCotizacionList { get; set; }

        public bool mostrarCosto { get; set; }

        public bool considerarDescontinuados { get; set; }

        public Decimal maximoPorcentajeDescuentoPermitido { get; set; }

        


        /*Campos utilizados para búsqueda*/
        public Usuario usuarioBusqueda { get; set; }
        public DateTime fechaDesde { get; set; }
        public DateTime fechaHasta { get; set; }

        public DateTime fechaPrecios { get; set; }
        

    }
}
