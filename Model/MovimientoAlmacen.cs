using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class MovimientoAlmacen : Auditoria
    {
        public Guid idMovimientoAlmacen { get; set; }

        [Display(Name = "Venta:")]
        public Venta venta { get; set; }

        [Display(Name = "Pedido:")]
        public Pedido pedido { get; set; }


        public Boolean estaAnulado { get; set; }

        public Boolean estaFacturado { get; set; }

        public Boolean estaNoEntregado { get; set; }



        public String comentarioAnulado { get; set; }

        public long numero { get; set; }

        public enum tiposMovimiento {
            [Display(Name = "Entrada")]
            Entrada = 'E',
            [Display(Name = "Salida")]
            Salida = 'S',
        };

        public tiposMovimiento tipoMovimiento { get; set; }


        public SeguimientoMovimientoAlmacenSalida seguimientoMovimientoAlmacenSalida { get; set; }

        public SeguimientoMovimientoAlmacenEntrada seguimientoMovimientoAlmacenEntrada { get; set; }

        
        public int ajusteAprobado { get; set; }

        public String ajusteAprobadoDesc { get
            {
                String desc = "";
                switch (this.ajusteAprobado)
                {
                    case 0: desc = "Pendiente Aprobación"; break;
                    case 1: desc = "Aprobado"; break;
                }
                return desc;
            }
        }

        public String tipoMovimientoAjusteDesc
        {
            get
            {
                String desc = "";
                switch (this.tipoMovimiento)
                {
                    case tiposMovimiento.Entrada: desc = "Sobrante"; break;
                    case tiposMovimiento.Salida: desc = "Pérdida"; break;
                }
                return desc;
            }
        }

        public MotivoAjusteAlmacen motivoAjuste { get; set; }


        public enum estadosMovimiento {
            [Display(Name = "Anulado")]
            Anulado = 0,
            [Display(Name = "Activo")]
            Activo = 1,
        }

        public estadosMovimiento estadoMovimiento { get; set; }

        [Display(Name = "Fecha Emisión:")]
        public DateTime fechaEmision { get; set; }


        [Display(Name = "Fecha Emisión Desde:")]
        public DateTime fechaEmisionDesde { get; set; }

        [Display(Name = "Fecha Emisión Hasta:")]
        public DateTime fechaEmisionHasta { get; set; }



        [Display(Name = "Fecha Emisión:")]
        public String fechaEmisionFormatoImpresion
        {
            get { return fechaEmision.ToString("dd-MMM-yyyy", CultureInfo.CreateSpecificCulture("es-ES")); }
        }
        
        [Display(Name = "Fecha Traslado:")]
        public DateTime fechaTraslado { get; set; }

        [Display(Name = "Fecha Traslado:")]
        public String fechaTrasladoFormatoImpresion {
            get { return fechaTraslado.ToString("dd-MMM-yyyy", CultureInfo.CreateSpecificCulture("es-ES")); }
        }

        
       

        [Display(Name = "Serie:")]
        public String serieDocumento { get; set; }

        [Display(Name = "Número:")]
        public int numeroDocumento { get; set; }

        
        [Display(Name = "Fecha Inicio Traslado Desde:")]
        public DateTime fechaTrasladoDesde { get; set; }

        [Display(Name = "Fecha Inicio Traslado Hasta:")]
        public DateTime fechaTrasladoHasta { get; set; }

        public List<DocumentoDetalle> documentoDetalle { get; set; }

        [Display(Name = "Sustento de Extorno:")]
        public String sustentoExtorno { get; set; }

        

        public List<Transaccion> transaccionList { get; set; }

        [Display(Name = "Tipo Extorno:")]
        public TiposExtorno tipoExtorno { get; set; }

        public enum TiposExtorno
        {
            [Display(Name = "")]
            SinExtorno = 0,
            [Display(Name = "Extornada Totalmente (Anulación)")]
            ExtornadaTotalmentePorAnulacion = 1,
            [Display(Name = "Extornada Totalmente (Devolución)")]
            ExtornadaTotalmentePorDevolucion = 6,
            [Display(Name = "Extornada Parcialmente (Devolución)")]
            ExtornadaParcialmentePorDevolucion = 7
        }

        public String tipoExtornoToString
        {
            get
            {
                return EnumHelper<TiposExtorno>.GetDisplayValue(this.tipoExtorno);
            }
        }

        public List<MovimientoAlmacen> movimientoAlmacenExtornanteList = new List<MovimientoAlmacen>();

        /**
         * Utilizado para búsqueda
         */
        [Display(Name = "SKU:")]
        public String sku { get; set; }

    }
}
