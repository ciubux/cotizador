using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class NotaIngreso : MovimientoAlmacen
    {
        public NotaIngreso()
        {
            this.motivoTraslado = motivosTraslado.Compra;
        }        


        [Display(Name = "Sede MP:")]
        public Ciudad ciudadOrigen { get; set; }

        [Display(Name = "Serie Nota Ingreso:")]
        public new String serieDocumento { get; set; }

        [Display(Name = "Número Nota Ingreso:")]
        public new long numeroDocumento { get; set; }

        [Display(Name = "Punto Llegada:")]
        public String direccionLlegada { get; set; }


        [Display(Name = "Atención Parcial:")]
        public Boolean atencionParcial { get; set; }

        public Boolean ultimaAtencionParcial { get; set; }
        


        [Display(Name = "Placa Vehículo:")]
        public String placaVehiculo { get; set; }

        [Display(Name = "Observaciones Nota Ingreso:")]
        public String observaciones { get; set; }


        [Display(Name = "Número Nota Ingreso:")]
        public String numeroDocumentoString { get { return this.numeroDocumento.ToString().PadLeft(7, '0'); } }


        [Display(Name = "Número Nota Ingreso:")]
        public String serieNumeroNotaIngreso { get { return  this.serieDocumento + "-" + this.numeroDocumentoString ; } }
             


        public List<DocumentoDetalle> documentoDetalle { get; set; }

        public DocumentoVenta documentoVenta { get; set; }

        [Display(Name = "Transportista:")]
        public Transportista transportista { get; set; }

        public Boolean transportistaEsModificado { get; set; }

        public Boolean existeCambioTransportista { get; set; }

        public NotaIngresoValidacion notaIngresoValidacion { get; set; }

        [Display(Name = "Serie:")]
        public SerieDocumentoElectronico serieDocumentoElectronico { get; set; }


        [Display(Name = "Motivo Traslado:")]
        public motivosTraslado motivoTraslado { get; set; }
        public enum motivosTraslado
        {
            [Display(Name = "Compra")]
            Compra = 'C',  /*PEDIDO DE COMPRA*/
            [Display(Name = "Comodato a Recibir")]
            ComodatoRecibido = 'M', /*PEDIDO DE COMPRA*/
            [Display(Name = "Transferencia Gratuita a Recibir")]
            TransferenciaGratuitaRecibida = 'G', /*PEDIDO DE COMPRA*/
            [Display(Name = "Préstamo a Recibir")]
            PrestamoRecibido = 'P', /*PEDIDO DE COMPRA*/
            [Display(Name = "Devolución de Compra")]
            DevolucionCompra = 'B', /*PEDIDO DE COMPRA*/
            [Display(Name = "Devolución de Préstamo Recibido")]
            DevolucionPrestamoRecibido = 'E', /*PEDIDO DE COMPRA*/
            [Display(Name = "Devolución de Comodato Recibido")]
            DevolucionComodatoRecibido = 'F', /*PEDIDO DE COMPRA*/
            [Display(Name = "Devolución de Transferencia Gratuita Recibida")]
            DevolucionTransferenciaGratuitaRecibida = 'H', /*PEDIDO DE COMPRA*/
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
