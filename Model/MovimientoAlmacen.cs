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

        public Pedido pedido { get; set; }

        public Venta venta { get; set; }


        [Display(Name = "Motivo Traslado:")]
        public motivosTraslado motivoTraslado { get; set; }
        public enum motivosTraslado
        {
            [Display(Name = "Otros")]
            Otros = 'O',
            [Display(Name = "Abastecimiento de máq. expendedoras (ingreso/salida)")]
            AbastecimientoMaqExpendedoras = 'A',
            [Display(Name = "Compra")]
            Compra = 'C',
            [Display(Name = "Devolución de Compra")]
            DevolucionCompra = 'B',
            [Display(Name = "Devolución de Venta")]
            DevolucionVenta = 'D',
            [Display(Name = "Devolución de Préstamo")]
            DevolucionPrestamo = 'E',
            [Display(Name = "Comodato")]
            Comodato = 'M',
            [Display(Name = "Devolución de Comodato")]
            DevolucionComodato = 'F',
            [Display(Name = "Transferencia Gratuita")]
            TransferenciaGratuita = 'G',
            [Display(Name = "Devolución de Transferencia Gratuita")]
            DevolucionTransferenciaGratuita = 'H',
            [Display(Name = "Cambio de mercadería (entrega/retorno)")]
            CambioMercaderia = 'I',
            [Display(Name = "Consignación")]
            Consignacion = 'N',
            [Display(Name = "Préstamo")]
            Prestamo = 'P',
            [Display(Name = "Devolución de consignación")]
            DevoluciónConsignacion = 'Q',
            [Display(Name = "Traslado interno (misma emp.)")]
            TrasladoInterno = 'T',
            [Display(Name = "Recojo")]
            Recojo = 'T',
            [Display(Name = "Venta")]
            Venta = 'V'
        }

        public String motivoTrasladoString
        {
            get
            {
                return EnumHelper<motivosTraslado>.GetDisplayValue(this.motivoTraslado);
            }
        }

        public Boolean estaAnulado { get; set; }

        public Boolean estaFacturado { get; set; }

        public Boolean estaNoEntregado { get; set; }
        
        public String estadoDescripcion { get {

                return this.estaAnulado ? "Guía Anulada" : 
                    (this.estaFacturado ? "Guía Emitida y Facturada" :   "Guía Emitida");

            }

        }

        public String comentarioAnulado { get; set; }

        public long numero { get; set; }

        public enum tiposMovimiento {
            [Display(Name = "Ingreso")]
            Ingreso = 'I',
            [Display(Name = "Salida")]
            Salida = 'S',
        };

        public tiposMovimiento tipoMovimiento { get; set; }

        public Usuario usuario { get; set; }

        public SeguimientoMovimientoAlmacenSalida seguimientoMovimientoAlmacenSalida { get; set; }

        public SeguimientoMovimientoAlmacenEntrada seguimientoMovimientoAlmacenEntrada { get; set; }


        public enum estadosMovimiento {
            [Display(Name = "Anulado")]
            Anulado = 0,
            [Display(Name = "Activo")]
            Activo = 1,
        }

        public estadosMovimiento estadoMovimiento { get; set; }

        [Display(Name = "Fecha Emisión:")]
        public DateTime fechaEmision { get; set; }

        [Display(Name = "Fecha Emisión:")]
        public String fechaEmisionFormatoImpresion
        {
            get { return fechaEmision.ToString("dd-MMM-yy", CultureInfo.CreateSpecificCulture("es-ES")); }
        }
        
        [Display(Name = "Fecha Traslado:")]
        public DateTime fechaTraslado { get; set; }

        [Display(Name = "Fecha Traslado:")]
        public String fechaTrasladoFormatoImpresion {
            get { return fechaTraslado.ToString("dd-MMM-yy", CultureInfo.CreateSpecificCulture("es-ES")); }
        }

        
       

        [Display(Name = "Serie:")]
        public String serieDocumento { get; set; }

        [Display(Name = "Número:")]
        public int numeroDocumento { get; set; }

        
        [Display(Name = "Fecha Traslado Desde:")]
        public DateTime fechaTrasladoDesde { get; set; }

        [Display(Name = "Fecha Traslado Hasta:")]
        public DateTime fechaTrasladoHasta { get; set; }

        
    }
}
