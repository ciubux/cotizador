﻿using System;
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

        [Display(Name = "Venta:")]
        public Venta venta { get; set; }

        [Display(Name = "Pedido:")]
        public Pedido pedido { get; set; }

        [Display(Name = "Sede MP:")]
        public Ciudad ciudadOrigen { get; set; }

        [Display(Name = "Serie Nota Ingreso:")]
        public new String serieDocumento { get; set; }

        [Display(Name = "Número Nota Ingreso:")]
        public new long numeroDocumento { get; set; }

        [Display(Name = "Punto Llegada:")]
        public String direccionLlegada { get; set; }


    /*    [Display(Name = "Atención Parcial:")]
        public Boolean atencionParcial { get; set; }

        public Boolean ultimaAtencionParcial { get; set; }
        */


        [Display(Name = "Placa Vehículo:")]
        public String placaVehiculo { get; set; }

        [Display(Name = "Observaciones Nota Ingreso:")]
        public String observaciones { get; set; }

   /*     [Display(Name = "Certificado Inscripción:")]
        public String certificadoInscripcion { get; set; }*/

        public String numeroDocumentoString { get { return this.numeroDocumento.ToString().PadLeft(7, '0'); } }


        [Display(Name = "Número Nota Ingreso:")]
        public String serieNumeroGuia { get { return  this.serieDocumento + "-" + this.numeroDocumentoString ; } }
             


        public List<DocumentoDetalle> documentoDetalle { get; set; }

        public DocumentoVenta documentoVenta { get; set; }

        [Display(Name = "Transportista:")]
        public Transportista transportista { get; set; }

        public Boolean transportistaEsModificado { get; set; }

        public Boolean existeCambioTransportista { get; set; }

        public GuiaRemisionValidacion guiaRemisionValidacion { get; set; }

    }
}