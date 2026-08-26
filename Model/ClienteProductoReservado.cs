using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ClienteProductoReservado : Auditoria
    {
        public const string TIPO_PERMANENTE = "PERMANENTE";
        public const string TIPO_PUNTUAL = "PUNTUAL";
        public Guid idClienteProductoReservado { get; set; }

        public Empresa empresa { get; set; }
        
        [Display(Name = "Cantidad Original:")]
        public int? cantidadOriginal { get; set; }

        [Display(Name = "Cantidad Reserva:")]
        public int? cantidadReserva { get; set; }

        [Display(Name = "Cantidad Atendida:")]
        public int? cantidadAtendida { get; set; }

        [Display(Name = "Unidad Conteo:")]
        public string unidadConteo { get; set; }

        [Display(Name = "Tipo:")]
        public string tipo { get; set; }

        [Display(Name = "Fecha Última Recarga:")]
        public DateTime? fechaUltimaRecarga { get; set; }

        public Ciudad ciudad { get; set; }
        public Cliente cliente { get; set; }
        public Producto producto { get; set; }

        public ClienteProductoReservadoSolicitud solicitudRecargaActiva { get; set; }

        public List<ClienteProductoReservadoMovimiento> movimientos { get; set; }
        public int idPresentacionUnidad { get; set; }

        public bool tieneSolicitudRecargaActiva { get { return !this.solicitudRecargaActiva.idClienteProductoReservadoSolicitud.Equals(Guid.Empty); } }


        public int filtroTieneSolicitudRecargaActiva { get; set; }

        public ClienteProductoReservado()
        {
            this.empresa = new Empresa();
            this.ciudad = new Ciudad();
            this.cliente = new Cliente();
            this.producto = new Producto();
            this.solicitudRecargaActiva = new ClienteProductoReservadoSolicitud();
            this.movimientos = new List<ClienteProductoReservadoMovimiento>();
        }
    }
}
