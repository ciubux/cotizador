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


        public Guid idGuiaRemision { get; set; }

        [Display(Name = "Pedido:")]
        public Pedido pedido { get; set; }

        [Display(Name = "SEDE MP:")]
        public Ciudad ciudadOrigen { get; set; }

      
        [Display(Name = "Punto Partida:")]
        public String direccionOrigen { get; set; }
        /*
        [Display(Name = "Ciudad Destino:")]
        public Ciudad ciudadDestino { get; set; }

        [Display(Name = "Dirección Entrega:")]
        public String direccionDestino { get; set; }*/


        [Display(Name = "Placa Vehículo:")]
        public String placaVehiculo { get; set; }

        [Display(Name = "Observaciones:")]
        public String observaciones { get; set; }

        [Display(Name = "Certificado Inscripción:")]
        public String certificadoInscripcion { get; set; }



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
            [Display(Name = "Devolución de Compra")]
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

        [Display(Name = "Motivo Traslado:")]
        public motivosTraslado motivoTraslado { get; set; }

        public List<DocumentoDetalle> documentoDetalle { get; set; }

        [Display(Name = "Transportista:")]
        public Transportista transportista { get; set; }

        public Boolean transportistaEsModificado { get; set; }

        public Boolean existeCambioTransportista { get; set; }
    }
}
