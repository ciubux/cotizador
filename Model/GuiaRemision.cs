using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class GuiaRemision : MovimientoAlmacen
    {

        public GuiaRemision()
        {
            this.motivoTraslado = motivosTraslado.Venta;
        }       
            

        [Display(Name = "Sede MP:")]
        public Ciudad ciudadOrigen { get; set; }

        [Display(Name = "Serie  Guía Remisión:")]
        public new String serieDocumento { get; set; }

        [Display(Name = "Número Guía Remisión:")]
        public new long numeroDocumento { get; set; }

        [Display(Name = "Punto Partida:")]
        public String direccionOrigen { get; set; }


        [Display(Name = "Atención Parcial:")]
        public Boolean atencionParcial { get; set; }

        public Boolean ultimaAtencionParcial { get; set; }



        [Display(Name = "Placa Vehículo:")]
        public String placaVehiculo { get; set; }

        [Display(Name = "Observaciones Guía Remisión:")]
        public String observaciones { get; set; }

        [Display(Name = "Certificado Inscripción:")]
        public String certificadoInscripcion { get; set; }

        public String numeroDocumentoString { get { return this.numeroDocumento.ToString().PadLeft(7, '0'); } }


        [Display(Name = "Número Guía:")]
        public String serieNumeroGuia { get { return  this.serieDocumento + "-" + this.numeroDocumentoString ; } }
             


        public List<DocumentoDetalle> documentoDetalle { get; set; }

        public DocumentoVenta documentoVenta { get; set; }

        [Display(Name = "Transportista:")]
        public Transportista transportista { get; set; }

        public Boolean transportistaEsModificado { get; set; }

        public Boolean existeCambioTransportista { get; set; }

        public GuiaRemisionValidacion guiaRemisionValidacion { get; set; }

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


    }
}
