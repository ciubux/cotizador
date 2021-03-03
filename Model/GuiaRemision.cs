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
            this.tipoExtorno = TiposExtorno.SinExtorno;
            this.motivoExtornoNotaIngreso = MotivosExtornoNotaIngreso.DevolucionTotal;
            this.estadoFiltro = EstadoFiltro.Todos;
        }



        [Display(Name = "Sede MP:")]
        public Ciudad ciudadOrigen { get; set; }

        [Display(Name = "Serie  Guía Remisión:")]
        public new String serieDocumento { get; set; }

        [Display(Name = "Número Guía Remisión:")]
        public new long numeroDocumento { get; set; }

        [Display(Name = "Punto Partida:")]
        public String direccionOrigen { get; set; }

        public String direccionEntrega { get; set; }

        public Ubigeo ubigeoEntrega { get; set; }

        [Display(Name = "Atención Parcial:")]
        public Boolean atencionParcial { get; set; }

        public Boolean ultimaAtencionParcial { get; set; }

        public Boolean esGuiaDiferida { get {
                if (serieDocumento != null && serieDocumento.Substring(0,1).Equals("D"))
                {
                    return true;
                }

                return false;
            }
        }

        [Display(Name = "Placa Vehículo:")]
        public String placaVehiculo { get; set; }

        [Display(Name = "Observaciones Guía Remisión:")]
        public String observaciones { get; set; }

        [Display(Name = "Certificado Inscripción:")]
        public String certificadoInscripcion { get; set; }

        public String numeroDocumentoString { get { return this.numeroDocumento.ToString().PadLeft(7, '0'); } }


        [Display(Name = "Número Guía:")]
        public String serieNumeroGuia { get { return  "G"+this.serieDocumento + "-" + this.numeroDocumentoString ; } }
             

        public DocumentoVenta documentoVenta { get; set; }

        public string cpeNro { get; set; }
        [Display(Name = "Transportista:")]
        public Transportista transportista { get; set; }

        public Boolean transportistaEsModificado { get; set; }

        public Boolean existeCambioTransportista { get; set; }

        public GuiaRemisionValidacion guiaRemisionValidacion { get; set; }

        [Display(Name = "Motivo Traslado:")]
        public motivosTrasladoBusqueda motivoTrasladoBusqueda { get; set; }
        public enum motivosTrasladoBusqueda
        {
            [Display(Name = "Todos")]
            Todos = '0', /*PEDIDOS DE VENTA*/
            [Display(Name = "Venta")]
            Venta = 'V', /*PEDIDOS DE VENTA*/
            [Display(Name = "Traslado Interno a Entregar")]
            TrasladoInterno = 'T',  /*PEDIDO DE ALMACEN*/
            [Display(Name = "Comodato a Entregar")]
            ComodatoEntregado = 'M', /*PEDIDO DE VENTA*/
            [Display(Name = "Transferencia Gratuita a Entregar")]
            TransferenciaGratuitaEntregada = 'G', /*PEDIDO DE VENTA*/
            [Display(Name = "Préstamo a Entregar")]
            PrestamoEntregado = 'P', /*PEDIDO DE VENTA*/


            [Display(Name = "Extorno de Compra")]
            DevolucionCompra = 'B', /*PEDIDO DE COMPRA*/
            [Display(Name = "Extorno de Préstamo Recibido")]
            DevolucionPrestamoRecibido = 'E', /*PEDIDO DE ALMACEN*/
            [Display(Name = "Extorno de Comodato Recibido")]
            DevolucionComodatoRecibido = 'F', /*PEDIDO DE COMPRA*/
            [Display(Name = "Extorno de Transferencia Gratuita Recibida")]
            DevolucionTransferenciaGratuitaRecibida = 'H', /*PEDIDO DE COMPRA*/

        }

            [Display(Name = "Motivo Traslado:")]
        public motivosTraslado motivoTraslado { get; set; }
        public enum motivosTraslado
        {
            [Display(Name = "Venta")]
            Venta = 'V', /*PEDIDOS DE VENTA*/
            [Display(Name = "Traslado Interno")]
            TrasladoInterno = 'T',  /*PEDIDO DE ALMACEN*/
            [Display(Name = "Comodato a Entregar")]
            ComodatoEntregado = 'M', /*PEDIDO DE VENTA*/
            [Display(Name = "Transferencia Gratuita a Entregar")]
            TransferenciaGratuitaEntregada = 'G', /*PEDIDO DE VENTA*/
            [Display(Name = "Préstamo a Entregar")]
            PrestamoEntregado = 'P', /*PEDIDO DE VENTA*/


            [Display(Name = "Extorno de Compra")]
            DevolucionCompra = 'B', /*PEDIDO DE COMPRA*/
            [Display(Name = "Extorno de Préstamo Recibido")]
            DevolucionPrestamoRecibido = 'E', /*PEDIDO DE ALMACEN*/
            [Display(Name = "Extorno de Comodato Recibido")]
            DevolucionComodatoRecibido = 'F', /*PEDIDO DE COMPRA*/
            [Display(Name = "Extorno de Transferencia Gratuita Recibida")]
            DevolucionTransferenciaGratuitaRecibida = 'H', /*PEDIDO DE COMPRA*/


            /*




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

    */







        }


        [Display(Name = "Estado:")]
        public EstadoFiltro estadoFiltro { get; set; }

        public enum EstadoFiltro
        {
            [Display(Name = "Todos")]
            Todos = 0,
            [Display(Name = "Emitida")]
            Emitida = 1,
            [Display(Name = "Emitida y Facturada")]
            EmitidaFacturada = 2,
            [Display(Name = "Extornada")]
            Extornada = 3,
            [Display(Name = "Anulada")]
            Anulada = 4

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
                return this.estaAnulado ? "Anulada" :
                    (this.estaFacturado ? "Emitida y Facturada" : "Emitida");
            }
        }


        /*Atributos para extorno de notas de ingreso*/


        [Display(Name = "Motivo Extorno Nota Ingreso:")]
        public MotivosExtornoNotaIngreso motivoExtornoNotaIngreso { get; set; }
        public enum MotivosExtornoNotaIngreso
        {
            [Display(Name = "DEVOLUCIÓN TOTAL")]
            DevolucionTotal = 6,
            [Display(Name = "DEVOLUCIÓN POR ITEM")]
            DevolucionItem = 7
        };

        public String motivoExtornoNotaIngresoToString
        {
            get
            {
                return EnumHelper<MotivosExtornoNotaIngreso>.GetDisplayValue(this.motivoExtornoNotaIngreso);
            }
        }


        /*INGRESO*/

        public NotaIngreso notaIngresoAExtornar { get; set; }

        public Boolean ingresado { get; set; }

    }
}
