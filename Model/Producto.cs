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
            this.ProductoPresentacionList = new List<ProductoPresentacion>();
            this.CargaMasiva = false;
        }


        public Guid idProducto { get; set; }
        
        [Display(Name = "SKU MP:")]
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
        ///Codigo Sunat
        ///</summary>
        ///<return>
        ///Retorna el codigo Sunat 
        ///</return>
        [Display(Name = "Código SUNAT:")]
        public String codigoSunat { get; set; }

        ///<summary>
        ///Unidad de Sunat correspondiente a la unidad de venta estándar
        ///</summary>
        ///<return>
        ///Retorna unidad de Sunat correspondiente a la unidad de venta estándar
        ///</return>
        [Display(Name = "Unidad SUNAT:")]
        public String unidadEstandarInternacional { get; set; }

        public Decimal precioLista { get; set; }
        public Decimal precioListaAnterior { get; set; }

        [Display(Name = "Unidad Alternativa SUNAT:")]
        public String unidadAlternativaInternacional { get; set; }
        
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

        [Display(Name = "Unidad de Conteo:")]
        ///<summary>
        ///Unidad de conteo
        ///</summary>
        ///<return>
        ///Retorna la unidad de conteo 
        ///</return>
        public String unidadConteo { get; set; }

        [Display(Name = "Unidad Proveedor:")]
        public String unidadProveedor { get; set; }


        /// <summary>
        /// El campo equivalencia puede contener la equivalencia con respecto a la unidad de proveedor o alternativa
        /// En caso que la unidad se la estandar entonces la equivalencia debe ser 1
        /// </summary>
   //     public decimal equivalencia { get; set; }
        [Display(Name = "Unidad Proveedor Internacional:")]
        public String unidadProveedorInternacional { get; set; }


        // private int _equivalencia;
        [Display(Name = "Equivalencia Und. Alt.:")]
        public int equivalenciaAlternativa { get; set; }

        [Display(Name = "Equivalencia Und. Prov.:")]
        public int equivalenciaProveedor { get; set; }
        
        [Display(Name = "Equivalencia Und. Alt. Und. Conteo:")]
        ///<summary>
        ///Equivalencia unidad alternativa unidad conteo
        ///</summary>
        ///<return>
        ///Retorna la equivalencia unidad alternativa unidad conteo  
        ///</return>
        public int equivalenciaUnidadAlternativaUnidadConteo { get; set; }

        [Display(Name = "Equivalencia Und. Est. Und. Conteo:")]
        ///<summary>
        ///Equivalencia unidad estandar unidad conteo 
        ///</summary>
        ///<return>
        ///Retorna la equivalencia unidad estandar unidad conteo 
        ///</return>
        public int equivalenciaUnidadEstandarUnidadConteo { get; set; }


        [Display(Name = "Equivalencia Und. Prov. Und. Conteo:")]
        ///<summary>
        ///Equivalencia unidad proveedor unidad conteo 
        ///</summary>
        ///<return>
        ///Retorna la equivalencia unidad proveedor unidad conteo 
        ///</return>
        public int equivalenciaUnidadProveedorUnidadConteo { get; set; }


        [Display(Name = "Precio Lima Original:")]
        ///<summary>
        ///Precio original
        ///</summary>
        ///<return>
        ///Retorna el precio original del producto
        ///</return>
        public Decimal precioOriginal { get; set; }

        [Display(Name = "Precio Provincias Original:")]
        ///<summary>
        ///Precio provincias original
        ///</summary>
        ///<return>
        ///Retorna el precio provincias original del producto
        ///</return>
        public Decimal precioProvinciasOriginal { get; set; }

        [Display(Name = "Precio Lima:")]
        public Decimal precioSinIgv { get; set; }


        [Display(Name = "Precio Provincias:")]
        public Decimal precioProvinciaSinIgv { get; set; }

        /*     [Display(Name = "Precio Alternativo:")]
             public Decimal precioAlternativoSinIgv
             {
                 get
                 {
                     return Decimal.Parse(String.Format(Constantes.formatoDecimalesPrecioNeto, precioSinIgv / (equivalencia==0?1:equivalencia )));
                 }
             }*/

        public List<PrecioClienteProducto> precioListaList { get; set; }



        [Display(Name = "Costo Original:")]
        ///<summary>
        ///Costo original
        ///</summary>
        ///<return>
        ///Retorna el costo original del producto
        ///</return>
        public Decimal costoOriginal { get; set; }



        [Display(Name = "Costo:")]
        ///<summary>
        ///Costo sin IGV del producto en la unidad estándar
        ///</summary>
        ///<return>
        ///Retorna el costo del producto en la unidad estándar
        ///</return>
        public Decimal costoSinIgv { get; set; }

        ///<summary>
        ///Costo sin IGV del producto calculado en la unidad alternativa, no permite setear el valor.
        ///</summary>
        ///<return>
        ///Retorna el costo del producto calculado en la unidad alternativa en formato de dos decimales.
        ///</return>
     /*   public Decimal costoAlternativoSinIgv
        {
            get
            {
                return Decimal.Parse(String.Format(Constantes.formatoDosDecimales, costoSinIgv / (equivalencia == 0 ? 1 : equivalencia)));
            }
        }*/

        ///<summary>
        ///Costo del producto en la unidad estándar calculado con IGV o sin IGV dependiendo de la opción seleccionada, se utiliza en DocumentoDetalle
        ///</summary>
        ///<return>
        ///Retorna el costo del producto en la unidad estándar calculado con IGV o sin IGV dependiendo de la opción seleccionada
        ///</return>
        public Decimal costoLista { get; set; }

        public Decimal costoListaAnterior { get; set; }


        public override string ToString()
        {
            return this.sku.Trim() + " - " + this.descripcion;
        }

        public PrecioClienteProducto precioClienteProducto { get; set; }


        public int tipoProductoVista { get; set; }

        [Display(Name = "Moneda Proveedor:")]
        public string monedaProveedor { get; set; }

        [Display(Name = "Moneda MP:")]
        public string monedaMP { get; set; }

        [Display(Name = "Tipo Cambio:")]
        public Decimal tipoCambio { get; set; }
        
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
        
        public List<ProductoPresentacion> ProductoPresentacionList { get; set; }

        public ProductoPresentacion getProductoPresentacion(int idProductoPresentacion) {
            if (ProductoPresentacionList != null)
            {
                return ProductoPresentacionList.Where(p => p.IdProductoPresentacion == idProductoPresentacion).FirstOrDefault();
            }
            return null;
        }

        public int Stock { get; set; }

        public static List<CampoPersistir> obtenerCampos(List<LogCampo> campos, bool soloPersistentes = false) 
        {
            List<CampoPersistir> lista = new List<CampoPersistir>();

            Producto obj = new Producto();
            foreach (LogCampo campo in campos)
            {
                CampoPersistir cp = new CampoPersistir();
                cp.campo = campo;
                switch (campo.nombre)
                {
                    case "sku_proveedor": cp.nombre = Producto.nombreAtributo("skuProveedor"); break;
                    case "precio": cp.nombre = Producto.nombreAtributo("precioSinIgv"); break;
                    case "precio_provincia": cp.nombre = Producto.nombreAtributo("precioProvinciaSinIgv"); break;
                    case "costo": cp.nombre = Producto.nombreAtributo("costoSinIgv"); break;
                    case "sku": cp.nombre = Producto.nombreAtributo("sku"); break;
                    case "descripcion": cp.nombre = Producto.nombreAtributo("descripcion"); break;
                    case "familia": cp.nombre = Producto.nombreAtributo("familia"); break;
                    case "proveedor": cp.nombre = Producto.nombreAtributo("proveedor"); break;
                    case "unidad": cp.nombre = Producto.nombreAtributo("unidad"); break;
                    case "unidad_alternativa": cp.nombre = Producto.nombreAtributo("unidad_alternativa"); break;
                    case "equivalencia": cp.nombre = Producto.nombreAtributo("equivalenciaAlternativa"); break;
                    case "equivalencia_proveedor": cp.nombre = Producto.nombreAtributo("equivalenciaProveedor"); break;
                    case "moneda_compra": cp.nombre = Producto.nombreAtributo("monedaProveedor"); break;
                    case "moneda_venta": cp.nombre = Producto.nombreAtributo("monedaMP"); break;
                    case "unidad_proveedor": cp.nombre = Producto.nombreAtributo("unidadProveedor"); break;
                    case "unidad_estandar_internacional": cp.nombre = Producto.nombreAtributo("unidadEstandarInternacional"); break;
                    case "unidad_alternativa_internacional": cp.nombre = Producto.nombreAtributo("unidadAlternativaInternacional"); break;
                    case "inafecto": cp.nombre = Producto.nombreAtributo("inafecto"); break;
                    case "tipo": cp.nombre = Producto.nombreAtributo("tipoProducto"); break;

                    case "costo_original": cp.nombre = Producto.nombreAtributo("costoOriginal"); break;
                    case "precio_original": cp.nombre = Producto.nombreAtributo("precioOriginal"); break;
                    case "precio_provincia_original": cp.nombre = Producto.nombreAtributo("precioProvinciasOriginal"); break;
                    case "unidad_conteo": cp.nombre = Producto.nombreAtributo("unidadConteo"); break;
                    case "codigo_sunat": cp.nombre = Producto.nombreAtributo("codigoSunat"); break;
                    case "exonerado_igv": cp.nombre = Producto.nombreAtributo("exoneradoIgv"); break;
                    case "unidad_proveedor_internacional": cp.nombre = Producto.nombreAtributo("unidadProveedorInternacional"); break;
                    case "equivalencia_unidad_alternativa_unidad_conteo": cp.nombre = Producto.nombreAtributo("equivalenciaUnidadAlternativaUnidadConteo"); break;
                    case "equivalencia_unidad_estandar_unidad_conteo": cp.nombre = Producto.nombreAtributo("equivalenciaUnidadEstandarUnidadConteo"); break;
                    case "equivalencia_unidad_proveedor_unidad_conteo": cp.nombre = Producto.nombreAtributo("equivalenciaUnidadProveedorUnidadConteo"); break;
                    case "tipo_cambio": cp.nombre = Producto.nombreAtributo("tipoCambio"); break;
                    case "estado": cp.nombre = Producto.nombreAtributo("Estado"); break;

                    default: cp.nombre = "[NOT_FOUND]"; break;

                        /* TO DO: Evaluar si se usan
                        
                        case "equivalencia_unidad_conteo_estandar": cp.nombre = Producto.nombreAtributo("sku"); break;
                        case "equivalencia_unidad_conteo_alternativa": cp.nombre = Producto.nombreAtributo("sku"); break;
                        
                        */
                }

                if (campo.puedePersistir)
                {
                    cp.persiste = true;
                }

                if (!cp.nombre.Equals("[NOT_FOUND]")) { 
                    if (!soloPersistentes || (cp.persiste && soloPersistentes))
                    {
                        lista.Add(cp);
                    }
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


        public List<LogCambio> obtenerLogProgramado(List<CampoPersistir> campos, bool soloRegistro = false, bool soloPersiste = false)
        {
            List<LogCambio> lista = new List<LogCambio>();

            Producto obj = new Producto();
            foreach (CampoPersistir campo in campos)
            {
                LogCambio lc = null;

                switch (campo.campo.nombre)
                {
                    case "precio": lc = instanciarLogCambio(campo); lc.valor = this.precioSinIgv.ToString();  break;
                    case "costo": lc = instanciarLogCambio(campo); lc.valor = this.costoSinIgv.ToString(); break;
                    case "precio_provincia": lc = instanciarLogCambio(campo); lc.valor = this.precioProvinciaSinIgv.ToString(); break;
                    case "sku": lc = instanciarLogCambio(campo); lc.valor = this.sku; break;
                    case "sku_proveedor": lc = instanciarLogCambio(campo); lc.valor = this.skuProveedor; break;
                    case "descripcion": lc = instanciarLogCambio(campo); lc.valor = this.descripcion; break;
                    case "familia": lc = instanciarLogCambio(campo); lc.valor = this.familia; break;
                    case "proveedor": lc = instanciarLogCambio(campo); lc.valor = this.proveedor; break;
                    case "unidad": lc = instanciarLogCambio(campo); lc.valor = this.unidad; break;
                    case "unidad_alternativa": lc = instanciarLogCambio(campo); lc.valor = this.unidad_alternativa; break;
                        //Revisar
                    case "equivalencia": lc = instanciarLogCambio(campo); lc.valor = this.equivalenciaAlternativa.ToString(); break;
                    case "equivalencia_proveedor": lc = instanciarLogCambio(campo); lc.valor = this.equivalenciaProveedor.ToString(); break;
                    case "moneda_compra": lc = instanciarLogCambio(campo); lc.valor = this.monedaProveedor; break;
                    case "moneda_venta": lc = instanciarLogCambio(campo); lc.valor = this.monedaMP; break;
                    case "unidad_proveedor": lc = instanciarLogCambio(campo); lc.valor = this.unidadProveedor; break;
                    case "unidad_estandar_internacional": lc = instanciarLogCambio(campo); lc.valor = this.unidadEstandarInternacional; break;
                    case "unidad_alternativa_internacional": lc = instanciarLogCambio(campo); lc.valor = this.unidadAlternativaInternacional; break;
                    case "inafecto": lc = instanciarLogCambio(campo); lc.valor = this.inafecto.ToString(); break;
                    case "tipo": lc = instanciarLogCambio(campo); lc.valor = ((int) this.tipoProducto).ToString(); break;

                    case "costo_original": lc = instanciarLogCambio(campo); lc.valor = this.costoOriginal.ToString(); break;
                    case "precio_original": lc = instanciarLogCambio(campo); lc.valor = this.precioOriginal.ToString(); break;
                    case "precio_provincia_original": lc = instanciarLogCambio(campo); lc.valor = this.precioProvinciasOriginal.ToString(); break;
                    case "tipo_cambio": lc = instanciarLogCambio(campo); lc.valor = this.tipoCambio.ToString(); break;
                    case "equivalencia_unidad_alternativa_unidad_conteo": lc = instanciarLogCambio(campo); lc.valor = this.equivalenciaUnidadAlternativaUnidadConteo.ToString(); break;
                    case "equivalencia_unidad_estandar_unidad_conteo": lc = instanciarLogCambio(campo); lc.valor = this.equivalenciaUnidadEstandarUnidadConteo.ToString(); break;
                    case "equivalencia_unidad_proveedor_unidad_conteo": lc = instanciarLogCambio(campo); lc.valor = this.equivalenciaUnidadProveedorUnidadConteo.ToString(); break;
                    case "unidad_conteo": lc = instanciarLogCambio(campo); lc.valor = this.unidadConteo; break;
                    case "unidad_proveedor_internacional": lc = instanciarLogCambio(campo); lc.valor = this.unidadProveedorInternacional; break;
                    case "codigo_sunat": lc = instanciarLogCambio(campo); lc.valor = this.codigoSunat; break;
                    case "exonerado_igv": lc = instanciarLogCambio(campo); lc.valor = this.exoneradoIgv.ToString(); break;
                    case "estado": lc = instanciarLogCambio(campo); lc.valor = this.Estado.ToString(); break;
                }

                if (soloRegistro && !campo.registra)
                {
                    lc = null;
                }

                if (soloPersiste && !campo.persiste)
                {
                    lc = null;
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
                    case "costo_original":
                        if (this.costoOriginal == decimal.Parse(cambio.valor))
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.costoOriginal = decimal.Parse(cambio.valor);
                            lista.Add(cambio);
                        }
                        break;
                    case "precio_original":
                        if (this.precioOriginal == decimal.Parse(cambio.valor))
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.precioOriginal = decimal.Parse(cambio.valor);
                            lista.Add(cambio);
                        }
                        break;
                    case "precio_provincia_original":
                        if (this.precioProvinciasOriginal == decimal.Parse(cambio.valor))
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.precioProvinciasOriginal = decimal.Parse(cambio.valor);
                            lista.Add(cambio);
                        }
                        break;
                    case "tipo_cambio":
                        if (this.tipoCambio == decimal.Parse(cambio.valor))
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.tipoCambio = decimal.Parse(cambio.valor);
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
                    case "sku_proveedor":
                        if (this.skuProveedor == cambio.valor)
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.skuProveedor = cambio.valor;
                            lista.Add(cambio);
                        }
                        break;
                    case "descripcion":
                        if (this.descripcion == cambio.valor)
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.descripcion = cambio.valor;
                            lista.Add(cambio);
                        }
                        break;
                    case "familia":
                        if (this.familia == cambio.valor)
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.familia = cambio.valor;
                            lista.Add(cambio);
                        }
                        break;
                    case "proveedor":
                        if (this.proveedor == cambio.valor)
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.proveedor = cambio.valor;
                            lista.Add(cambio);
                        }
                        break;
                    case "unidad":
                        if (this.unidad == cambio.valor)
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.unidad = cambio.valor;
                            lista.Add(cambio);
                        }
                        break;
                    case "unidad_alternativa":
                        if (this.unidad_alternativa == cambio.valor)
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.unidad_alternativa = cambio.valor;
                            lista.Add(cambio);
                        }
                        break;
                    case "unidad_conteo":
                        if (this.unidadConteo == cambio.valor)
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.unidadConteo = cambio.valor;
                            lista.Add(cambio);
                        }
                        break;
                    case "unidad_proveedor_internacional":
                        if (this.unidadProveedorInternacional == cambio.valor)
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.unidadProveedorInternacional = cambio.valor;
                            lista.Add(cambio);
                        }
                        break;
                    case "codigo_sunat":
                        if (this.codigoSunat == cambio.valor)
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.codigoSunat = cambio.valor;
                            lista.Add(cambio);
                        }
                        break;
                    case "equivalencia":
                        if (this.equivalenciaAlternativa == int.Parse(cambio.valor))
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.equivalenciaAlternativa = int.Parse(cambio.valor);
                            lista.Add(cambio);
                        }
                        break;
                    case "equivalencia_proveedor":
                        if (this.equivalenciaProveedor == int.Parse(cambio.valor))
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.equivalenciaProveedor = int.Parse(cambio.valor);
                            lista.Add(cambio);
                        }
                        break;
                    case "equivalencia_unidad_alternativa_unidad_conteo":
                        if (this.equivalenciaUnidadAlternativaUnidadConteo == int.Parse(cambio.valor))
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.equivalenciaUnidadAlternativaUnidadConteo = int.Parse(cambio.valor);
                            lista.Add(cambio);
                        }
                        break;
                    case "equivalencia_unidad_estandar_unidad_conteo":
                        if (this.equivalenciaUnidadEstandarUnidadConteo == int.Parse(cambio.valor))
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.equivalenciaUnidadEstandarUnidadConteo = int.Parse(cambio.valor);
                            lista.Add(cambio);
                        }
                        break;
                    case "equivalencia_unidad_proveedor_unidad_conteo":
                        if (this.equivalenciaUnidadProveedorUnidadConteo == int.Parse(cambio.valor))
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.equivalenciaUnidadProveedorUnidadConteo = int.Parse(cambio.valor);
                            lista.Add(cambio);
                        }
                        break;
                    case "moneda_compra":
                        if (this.monedaProveedor == cambio.valor)
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.monedaProveedor = cambio.valor;
                            lista.Add(cambio);
                        }
                        break;
                    case "moneda_venta":
                        if (this.monedaMP == cambio.valor)
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.monedaMP = cambio.valor;
                            lista.Add(cambio);
                        }
                        break;
                    case "unidad_proveedor":
                        if (this.unidadProveedor == cambio.valor)
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.unidadProveedor = cambio.valor;
                            lista.Add(cambio);
                        }
                        break;
                    case "unidad_estandar_internacional":
                        if (this.unidadEstandarInternacional == cambio.valor)
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.unidadEstandarInternacional = cambio.valor;
                            lista.Add(cambio);
                        }
                        break;
                    case "unidad_alternativa_internacional":
                        if (this.unidadAlternativaInternacional == cambio.valor)
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.unidadAlternativaInternacional = cambio.valor;
                            lista.Add(cambio);
                        }
                        break;
                    case "inafecto":
                        if (this.inafecto == bool.Parse(cambio.valor))
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.inafecto = bool.Parse(cambio.valor);
                            lista.Add(cambio);
                        }
                        break;
                    case "tipo":
                        if (this.tipoProducto == (TipoProducto) int.Parse(cambio.valor))
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.tipoProducto = (TipoProducto) int.Parse(cambio.valor);
                            lista.Add(cambio);
                        }
                        break;
                    case "exonerado_igv":
                        if (this.exoneradoIgv == bool.Parse(cambio.valor))
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.exoneradoIgv = bool.Parse(cambio.valor);
                            lista.Add(cambio);
                        }
                        break;
                    case "estado":
                        if (this.Estado == int.Parse(cambio.valor))
                        {
                            if (cambio.persisteCambio)
                            {
                                cambio.repiteDato = true;
                                lista.Add(cambio);
                            }
                        }
                        else
                        {
                            this.Estado = int.Parse(cambio.valor);
                            lista.Add(cambio);
                        }
                        break;
                }
            }

            return lista;
        }
        private LogCambio instanciarLogCambio(CampoPersistir campo)
        {
            LogCambio cambio = new LogCambio();
            cambio.idRegistro = this.idProducto.ToString();
            cambio.estado = true;
            cambio.fechaInicioVigencia = this.fechaInicioVigencia;
            cambio.idCampo = campo.campo.idCampo;
            cambio.idTabla = campo.campo.idTabla;
            cambio.persisteCambio = campo.persiste;
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

        public static bool esCampoActualizableCargaMasiva(string campo)
        {
            switch (campo)
            {
                case "precio": return false; break;
                case "precio_provincia": return false; break;
                case "costo": return false; break;
                case "tipo_cambio": return false; break;
                
                default: return true; break;
            }

            return true;
        }

        public static bool esCampoCalculado(string campo)
        {
            switch (campo)
            {
                case "precio": return true; break;
                case "precio_provincia": return true; break;
                case "costo": return true; break;

                default: return false; break;
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