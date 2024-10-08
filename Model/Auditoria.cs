﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Auditoria
    {
        [Display(Name = "Estado:")]
        public int Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public Guid IdUsuarioRegistro { get; set; }
        public DateTime FechaEdicion { get; set; }
        public Guid IdUsuarioEdicion { get; set; }
        public Usuario UsuarioRegistro { get; set; }
        public Usuario usuario { get; set; }       
        public DateTime fechaInicioVigencia { get; set; }
        public bool CargaMasiva { get; set; }

        /* Filtro de búsqueda */
        public bool integraEmpresas { get; set; }

        public string EstadoDesc => Estado == 1 ? "Activo" : "Inactivo";

        public bool EstadoCheck
        {
            get { return Estado == 1; }
            set
            {
                Estado = value ? 1 : 0;
            }
        }

        public string FechaRegistroDesc
        {
            get { return FechaRegistro.ToString("dd/MM/yyyy HH:mm:ss"); }
        }

        public string FechaRegistroFormatoFecha
        {
            get { return FechaRegistro.ToString(Constantes.formatoFecha); }
        }

        public string FechaEdicionDesc
        {
            get { return FechaEdicion.ToString("dd/MM/yyyy HH:mm:ss"); }
        }

        public string FechaEdicionFormatoFecha
        {
            get { return FechaEdicion.ToString(Constantes.formatoFecha); }
        }

        public string FechaInicioVigenciaDesc
        {
            get { return fechaInicioVigencia.ToString("dd/MM/yyyy"); }
        }

        public bool hoyEsFechaRegistro
        {
            get { return FechaRegistro.ToString("yyyyMMdd").Equals(DateTime.Now.ToString("yyyyMMdd")); }
        }

        public List<ArchivoAdjunto> listArchivoAjunto { get; set; }

        [Display(Name = "Creado Desde:")]
        public DateTime? fechaRegistroDesde { get; set; }

        public string FechaRegistroDesdeFormatoFecha
        {
            get { return fechaRegistroDesde == null ? "" : fechaRegistroDesde.Value.ToString(Constantes.formatoFecha); }
        }

        

        [Display(Name = "Creado Hasta:")]
        public DateTime? fechaRegistroHasta { get; set; }

        public string FechaRegistroHastaFormatoFecha
        {
            get { return fechaRegistroHasta == null ? "" : fechaRegistroHasta.Value.ToString(Constantes.formatoFecha); }
        }
    }
}
