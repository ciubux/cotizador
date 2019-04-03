﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Model
{
    public class Producto : Auditoria
    {
        public const string NOMBRE_TABLA = "PRODUCTO";

        public Producto()
        {
            this.tipoProducto = TipoProducto.Bien;
            this.CargaMasiva = false;
        }


        public Guid idProducto { get; set; }

        [Display(Name = "Código (SKU):")]
        public String sku { get; set; }
        [Display(Name = "SKU Prov.:")]
        public String skuProveedor { get; set; }
        [Display(Name = "Descripción:")]
        public String descripcion { get; set; }
        public Byte[] image { get; set; }     
        [Display(Name = "Proveedor:")]
        public String proveedor { get; set; }
        [Display(Name = "Familia:")]
        public String familia { get; set; }

        ///<summary>
        ///Identifica si el producto se encuentra exonerado de IGV
        ///</summary>
        ///<return>
        ///Retorna true si el producto se encuentra exonerado de IGV, false en caso contrario
        ///</return>
        [Display(Name = "Exonerado IGV:")]
        public bool exoneradoIgv { get; set; }

        ///<summary>
        ///Identifica si el producto se encuentra Inafecto de IGV
        ///</summary>
        ///<return>
        ///Retorna true si el producto se encuentra inafecto de IGV, false en caso contrario
        ///</return>
        [Display(Name = "Inafecto:")]
        public bool inafecto { get; set; }


        ///<summary>
        ///Unidad de Sunat correspondiente a la unidad de venta estándar
        ///</summary>
        ///<return>
        ///Retorna unidad de Sunat correspondiente a la unidad de venta estándar
        ///</return>
        [Display(Name = "Unidad SUNAT:")]
        public String unidadEstandarInternacional { get; set; }

        [Display(Name = "Unidad Alternativa SUNAT:")]
        public String unidadAlternativaInternacional { get; set; }





        public Decimal precioLista { get; set; }
        public Decimal precioListaAnterior { get; set; }        














        [Display(Name = "Unidad de Venta:")]
        ///<summary>
        ///Unidad estándar de venta
        ///</summary>
        ///<return>
        ///Retorna la unidad estándar de venta
        ///</return>
        public String unidad { get; set; }

        private String _unidad_alternativa;
        [Display(Name = "Unidad Alternativa:")]
        public String unidad_alternativa
        {
            //        get { return equivalencia == 1?String.Empty:_unidad_alternativa; }
            get { return _unidad_alternativa; }
            set { _unidad_alternativa = value; }
        }

        [Display(Name = "Unidad Proveedor:")]
        public String unidadProveedor { get; set; }
        // private int _equivalencia;
        [Display(Name = "Equivalencia Und. Alt.:")]
        public int equivalencia { get; set; }

        [Display(Name = "Equivalencia Und. Prov.:")]
        public int equivalenciaProveedor { get; set; }

       

        [Display(Name = "Precio Lima:")]
        public Decimal precioSinIgv { get;set;}


        [Display(Name = "Precio Provincias:")]
        public Decimal precioProvinciaSinIgv { get; set; }

        [Display(Name = "Precio Alternativo:")]
        public Decimal precioAlternativoSinIgv
        {
            get
            {
                return Decimal.Parse(String.Format(Constantes.formatoDecimalesPrecioNeto, precioSinIgv / (equivalencia==0?1:equivalencia )));
            }
        }

        public List<PrecioClienteProducto> precioListaList { get; set; }




        [Display(Name = "Costo:")]
        ///<summary>
        ///Costo sin IGV del producto en la unidad estándar
        ///</summary>
        ///<return>
        ///Retorna el costo del producto en la unidad estándar
        ///</return>
        public Decimal costoSinIgv { get;set; }

        ///<summary>
        ///Costo sin IGV del producto calculado en la unidad alternativa, no permite setear el valor.
        ///</summary>
        ///<return>
        ///Retorna el costo del producto calculado en la unidad alternativa en formato de dos decimales.
        ///</return>
        public Decimal costoAlternativoSinIgv
        {
            get
            {
                return Decimal.Parse(String.Format(Constantes.formatoDosDecimales, costoSinIgv / (equivalencia == 0 ? 1 : equivalencia)));
            }
        }

        ///<summary>
        ///Costo del producto en la unidad estándar calculado con IGV o sin IGV dependiendo de la opción seleccionada, se utiliza en DocumentoDetalle
        ///</summary>
        ///<return>
        ///Retorna el costo del producto en la unidad estándar calculado con IGV o sin IGV dependiendo de la opción seleccionada
        ///</return>
        public Decimal costoLista { get; set; }


        public override string ToString()
        {
            return this.sku.Trim() + " - " + this.descripcion;
        }

        public PrecioClienteProducto precioClienteProducto { get; set; }


        public int tipoProductoVista { get; set; }

        public string monedaProveedor { get; set; }

        public string monedaMP { get; set; }




        [Display(Name = "Tipo:")]
        public TipoProducto tipoProducto { get; set; }
        public enum TipoProducto
        {
            [Display(Name = "Producto")]
            Bien = 1,
            [Display(Name = "Equipo de Comodato")]
            Comodato = 2,
            [Display(Name = "Servicio")]
            Servicio = 3,
            [Display(Name = "Cargo")]
            Cargo = 4,
            [Display(Name = "Descuento")]
            Descuento = 5
        }

        public String tipoProductoToString
        {
            get
            {
                return EnumHelper<TipoProducto>.GetDisplayValue(this.tipoProducto);
            }
        }

        public static List<CampoPersistir> obtenerCampos(List<LogCampo> campos) 
        {
            List<CampoPersistir> lista = new List<CampoPersistir>();

            Producto obj = new Producto();
            foreach(LogCampo campo in campos) 
            {
               
                if (campo.puedePersistir)
                {
                    CampoPersistir cp = new CampoPersistir();
                    cp.campo = campo;
                    switch(campo.nombre)
                    {
                        case "precio": cp.nombre = Producto.nombreAtributo("precioSinIgv"); break;
                        case "precio_provincia": cp.nombre = Producto.nombreAtributo("precioProvinciaSinIgv"); break;
                        case "costo": cp.nombre = Producto.nombreAtributo("costoSinIgv"); break;
                    }

                    lista.Add(cp);
                }
            }

            return lista;
        }

        public static string nombreAtributo(string atributo)
        {
            MemberInfo property = typeof(Producto).GetProperty(atributo);
            var dd = property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
            string name = "";
            if (dd != null)
            {
                name = dd.Name;
            }

            return name.Replace(":", "");
        }


        public List<LogCambio> obtenerLogProgramado(List<LogCampo> campos, List<CampoPersistir> persistir)
        {
            List<LogCambio> lista = new List<LogCambio>();

            Producto obj = new Producto();
            foreach (LogCampo campo in campos)
            {
                LogCambio lc = null;

                switch (campo.nombre)
                {
                    case "precio": lc = instanciarLogCambio(campo, persistir); lc.valor = this.precioSinIgv.ToString();  break;
                    case "costo": lc = instanciarLogCambio(campo, persistir); lc.valor = this.costoSinIgv.ToString(); break;
                    case "precio_provincia": lc = instanciarLogCambio(campo, persistir); lc.valor = this.precioProvinciaSinIgv.ToString(); break;
                    case "sku": lc = instanciarLogCambio(campo, persistir); lc.valor = this.sku; break;
                }

                if (lc != null)
                {
                    lista.Add(lc);
                }
            }

            return lista;
        }

        public List<LogCambio> aplicarCambios(List<LogCambio> cambios)
        {
            List<LogCambio> lista = new List<LogCambio>();

            Producto obj = new Producto();
            foreach (LogCambio cambio in cambios)
            {

                switch (cambio.campo.nombre)
                {
                    case "precio":
                        if (this.precioSinIgv == decimal.Parse(cambio.valor))
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        } else
                        {
                            this.precioSinIgv = decimal.Parse(cambio.valor);
                            lista.Add(cambio);
                        }
                        break;
                    case "costo":
                        if (this.costoSinIgv == decimal.Parse(cambio.valor))
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.costoSinIgv = decimal.Parse(cambio.valor);
                            lista.Add(cambio);
                        }
                        break;
                    case "precio_provincia":
                        if (this.precioProvinciaSinIgv == decimal.Parse(cambio.valor))
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.precioProvinciaSinIgv = decimal.Parse(cambio.valor);
                            lista.Add(cambio);
                        }
                        break;
                    case "sku":
                        if (this.sku == cambio.valor)
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.sku = cambio.valor;
                            lista.Add(cambio);
                        }
                        break;
                }
            }

            return lista;
        }
        private LogCambio instanciarLogCambio(LogCampo campo, List<CampoPersistir> persistir)
        {
            LogCambio cambio = new LogCambio();
            cambio.idRegistro = this.idProducto.ToString();
            cambio.estado = true;
            cambio.fechaInicioVigencia = this.fechaInicioVigencia;
            cambio.idCampo = campo.idCampo;
            cambio.idTabla = campo.idTabla;
            cambio.persisteCambio = this.persisteCampo(campo, persistir);
            cambio.idUsuarioModificacion = this.usuario.idUsuario;

            return cambio;
        }
        
        private bool persisteCampo(LogCampo campo, List<CampoPersistir> persistir)
        {
            foreach (CampoPersistir cp in persistir)
            {
                if (cp.campo.nombre.Equals(campo.nombre))
                {
                    return true;
                }
            }
            return false;
        }
        /*     public Guid idFamilia { get; set; }
    public Guid idProveedor { get; set; }
    public Guid idUnidad { get; set; }*/
        /*  public String clase { get; set; }
public String marca { get; set; }*/
    }
}