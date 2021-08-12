
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.Net.Http;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class TipoCambioBL
    {
        public TipoCambio getTipoCambio()
        {
            using (var tipoCambioDAL = new TipoCambioDAL())
            {
                return tipoCambioDAL.getTipoCambio();
            }
        }

        public void GetTipoCambioHoy()
        {
            
        }

        public async Task<TipoCambio> ObtenerTipoCambioSunat()
        {
            TipoCambio tc = new TipoCambio();
            string url = "https://www.sunat.gob.pe/a/txt/tipoCambio.txt";
            string mensajeRespuesta  = "";
            bool success = false;
            try { 
                using (HttpClient cliente = new HttpClient())
                {
                    using (HttpResponseMessage resultadoConsulta = await cliente.GetAsync(new Uri(url)))
                    {
                        if (resultadoConsulta.IsSuccessStatusCode)
                        {
                            string contenidoResultado = await resultadoConsulta.Content.ReadAsStringAsync();
                            if (contenidoResultado.Trim() == "")
                                mensajeRespuesta = "Se realizó correctamente la consulta a la URL de SUNAT pero no devolvió el valor en el contenido.";
                            else
                            {
                                string[] datosTC = contenidoResultado.Split('|');

                                if (datosTC.Length > 2) {
                                    string[] fechaPartes = datosTC[0].Split('/');
                                    tc.fecha = new DateTime(int.Parse(fechaPartes[2].Trim()), int.Parse(fechaPartes[2].Trim()), int.Parse(fechaPartes[2].Trim()));
                                    tc.valorSunatCompra = decimal.Parse(datosTC[1].Trim());
                                    tc.valorSunatVenta = decimal.Parse(datosTC[2].Trim());
                                    success = true;
                                }
                            }
                        }
                        else
                        {
                            mensajeRespuesta = string.Format("Ocurrió un inconveniente al consultar el tipo de cambio desde la URL de SUNAT.\r\nDetalle: {0}", mensajeRespuesta);
                        }
                    }
                }
            } catch (Exception ex)
            {

            }

            if (success)
            {
                //Registrar
            }

            return tc;
        }
    }
}
