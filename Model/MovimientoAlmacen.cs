﻿using System;
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
        
        public String estadoDescripcion { get {

                return this.estaAnulado ? "Guía Anulada" : 
                    (this.estaFacturado ? "Guía Emitida y Facturada" :   "Guía Emitida");

            }

        }

        public String comentarioAnulado { get; set; }

        public long numero { get; set; }

        public enum tiposMovimiento {
            [Display(Name = "Entrada")]
            Entrada = 'E',
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
