using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Model.DocumentoVenta;

namespace Model
{
    public class NotaIngreso : MovimientoAlmacen
    {
        public NotaIngreso()
        {
            this.motivoTraslado = motivosTraslado.Compra;
            this.tipoDocumentoVentaReferencia = TiposDocumentoVentaReferencia.Ninguno;
        }        


        [Display(Name = "Sede MP:")]
        public Ciudad ciudadDestino { get; set; }

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
            [Display(Name = "Traslado Interno Recibido")]
            TrasladoInterno = 'I', /*PEDIDO DE ALMACEN*/
            [Display(Name = "Comodato a Recibir")]
            ComodatoRecibido = 'M', /*PEDIDO DE COMPRA*/
            [Display(Name = "Transferencia Gratuita Recibida")]
            TransferenciaGratuitaRecibida = 'G', /*PEDIDO DE COMPRA*/
            [Display(Name = "Préstamo a Recibir")]
            PrestamoRecibido = 'R', /*PEDIDO DE ALMACEN*/
            [Display(Name = "Devolución de Venta")]
            DevolucionVenta = 'D', /*PEDIDO DE VENTA*/ 
            [Display(Name = "Devolución de Préstamo Entregado")]
            DevolucionPrestamoEntregado = 'D',  /*PEDIDO DE ALMACEN*/
            [Display(Name = "Devolución de Comodato Entregado")]
            DevolucionComodatoEntregado = 'F', /*PEDIDO DE VENTA*/
            [Display(Name = "Devolución de Transferencia Gratuita Entregada")]
            DevolucionTransferenciaGratuitaEntregada = 'H', /*PEDIDO DE VENTA*/
            [Display(Name = "Extorno de Guía Remisión")]
            ExtornoGuíaRemision = 'X',  /*PEDIDO DE ALMACEN*/
        }


        public String motivoTrasladoString
        {
            get
            {
                return EnumHelper<motivosTraslado>.GetDisplayValue(this.motivoTraslado);
            }
        }

        public String estadoDescripcion
        {
            get
            {
                return this.estaAnulado ? "Nota Ingreso Anulada" : "Nota Ingreso Registrada";
            }

        }



        [Display(Name = "Serie Guía Remisión Referencia:")]
        public String serieGuiaReferencia { get; set; }
        [Display(Name = "Número Guía Remisión Referencia:")]
        public int numeroGuiaReferencia { get; set; }

        [Display(Name = "Tipo Documento Venta Referencia:")]
        public TiposDocumentoVentaReferencia tipoDocumentoVentaReferencia { get; set; }

        public String tipoDocumentoVentaReferenciaString
        {
            get
            {
                return EnumHelper<TiposDocumentoVentaReferencia>.GetDisplayValue(this.tipoDocumentoVentaReferencia);
            }
        }

        [Display(Name = "Serie Documento Venta Referencia:")]
        public String serieDocumentoVentaReferencia { get; set; }
        [Display(Name = "Número Documento Venta Referencia:")]
        public int numeroDocumentoVentaReferencia { get; set; }


        public enum TiposDocumentoVentaReferencia
        {
            [Display(Name = "Ninguno")]
            Ninguno = 0,
            [Display(Name = "Factura")]
            Factura = 1,
            [Display(Name = "Boleta de Venta")]
            BoletaVenta = 3,
            [Display(Name = "Nota de Crédito")]
            NotaCrédito = 7,
            [Display(Name = "Nota de Débito")]
            NotaDébito = 8
        };

    }
}
