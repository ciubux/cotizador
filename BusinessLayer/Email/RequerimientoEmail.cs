using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Model;

namespace BusinessLayer.Email
{
    
    public class RequerimientoEmail
    {
        public String urlVerRequerimiento = "";

        /*ACTUALIZACION DE REQUERIMIENTO*/

        private String mailUpdate = @"<table border=""0"" cellspacing=""0"" cellpadding=""0"" align=""center"">
        <tbody>
	        <tr>
		        <td>
			        <table style=""width: 100%; min-width: 600px; max-width: 600px; border: solid 1px #333;"" border=""0"" cellspacing=""0"" cellpadding=""0"">
				        <tbody>
					        <tr>
						        <td style=""background-color: #ffffff; height: 60px; padding: 5px 0px;"">
							        <table style=""width: 100%; height: 100%;"" border=""0"" cellspacing=""0"" cellpadding=""0"">
								        <tbody>
									        <tr>
										        <td style=""padding-left: 20px; padding-right: 10px;""><span style=""font-size: 19px; font-family: Arial; font-weight: bold; color: #555555; text-transform: uppercase;"">Datos del Requerimiento</span></td>
										        <td style=""text-align: right; padding: 0; padding-right: 12px;"" align=""right"">
											        <table style=""height: 100%;"" border=""0"" cellspacing=""0"" cellpadding=""0"" align=""right"">
												        <tbody>
													        <tr>
														        <td valign=""center""><img style=""max-height: 50px;"" src=""https://www.mpinstitucional.com/images/logo_ind.jpg"" align=""abscenter"" /></td>
													        </tr>
												        </tbody>
											        </table>
										        </td>
									        </tr>
								        </tbody>
							        </table>
						        </td>
					        </tr>
					        <tr style=""background-color: #f7f7f7;"">
						        <td style=""padding: 40px 25px 40px 25px; width: 100%;"">
							        <table style=""width: 100%; padding-bottom: 30px;"" border=""0"" cellspacing=""0"" cellpadding=""0"">
								        <tbody>
									        <tr>
										        <td>
											        <table style=""width: 100%;"" border=""0"" cellspacing=""0"" cellpadding=""0"">
												        <tbody>
													        <tr>
														        <td><span style=""font-size: 15px; font-family: Arial; color: #777777;"">&nbsp;</span></td>
													        </tr>
													        <tr>
														        <td colspan=""3"" align=""left"" style=""font-size: 14px;"">
															        <span style=""font-family: Arial; font-weight: bold;"">Número de Requerimiento: </span>
															        <span style=""font-family: Arial; color: #777575;""><a href=""{{url_requerimiento}}"">{{nro_requerimiento}}</a></span>
														        </td>
													        </tr>
													        <tr>
														        <td>
															        <table style=""width: 100%; padding: 30px 0px 30px 0px;"" border=""0"" cellspacing=""0"" cellpadding=""3"">
																        <tr>
																	        <td colspan=""4"" style=""font-weight: bold; font-size: 16px; background-color: #cccccc; text-align: center; padding: 10px;"">
																		        <span style=""font-family: Arial;"">Productos requeridos</span>
																	        </td>
																        </tr>
																        <tr>
																	        <th style=""font-weight: bold; font-size: 14px;"">
																		        <span style=""font-family: Arial;"">Código</span>
																	        </th>
																	        <th style=""font-weight: bold; font-size: 14px;"">
																		        <span style=""font-family: Arial;"">Descripción</span>
																	        </th>
																	        <th style=""font-weight: bold; font-size: 14px;"">
																		        <span style=""font-family: Arial;"">Cantidad <br/> Pendiente</span>
																	        </th>
																	        <th style=""font-weight: bold; font-size: 14px;"">
																		        <span style=""font-family: Arial;"">Cantidad <br/> Total</span>
																	        </th>
																        </tr>
																        {{detalle_requerimiento}}
															        </table>
														        </td>
													        </tr>
												        </tbody>
											        </table>
										        </td>
									        </tr>
								        </tbody>
							        </table>
						        </td>
					        </tr>
					        <tr style=""background-color: #07a1d5;"">
						        <td style=""padding: 0; color: #ffffff; font-family: Arial; font-weight: bold; font-size: 18px; padding-left: 20px; height: 50px; font-style: oblique;"">&nbsp;</td>
					        </tr>
				        </tbody>
			        </table>
		        </td>
	        </tr>
        </tbody>
        </table>";

        private String detalleMailUpdate = @"<tr>
											<td align=""left"" style=""font-size: 14px;"">
												<span style=""font-family: Arial; color: #777575;"">{{codigo_producto}}</span>
											</td>
											<td align=""left"" style=""font-size: 14px;"">
												<span style=""font-family: Arial; color: #777575;"">{{nombre_producto}}</span>
											</td>
											<td align=""right"" style=""font-size: 14px; padding-bottom: 5px;"">
												<span style=""font-family: Arial; color: #777575;"">{{cantidad_producto}}</span>
											</td>
										</tr>";


        /*APROBACION DE REQUERIMIENTOS*/

        private String mailAprobacion = @"<table border=""0"" cellspacing=""0"" cellpadding=""0"" align=""center"">
        <tbody>
	        <tr>
		        <td>
			        <table style=""width: 100%; min-width: 600px; max-width: 600px; border: solid 1px #333;"" border=""0"" cellspacing=""0"" cellpadding=""0"">
				        <tbody>
					        <tr>
						        <td style=""background-color: #ffffff; height: 60px; padding: 5px 0px;"">
							        <table style=""width: 100%; height: 100%;"" border=""0"" cellspacing=""0"" cellpadding=""0"">
								        <tbody>
									        <tr>
										        <td style=""padding-left: 20px; padding-right: 10px;""><span style=""font-size: 19px; font-family: Arial; font-weight: bold; color: #555555; text-transform: uppercase;"">Datos del Requerimiento</span></td>
										        <td style=""text-align: right; padding: 0; padding-right: 12px;"" align=""right"">
											        <table style=""height: 100%;"" border=""0"" cellspacing=""0"" cellpadding=""0"" align=""right"">
												        <tbody>
													        <tr>
														        <td valign=""center""><img style=""max-height: 50px;"" src=""https://www.mpinstitucional.com/images/logo_ind.jpg"" align=""abscenter"" /></td>
													        </tr>
												        </tbody>
											        </table>
										        </td>
									        </tr>
								        </tbody>
							        </table>
						        </td>
					        </tr>
					        <tr style=""background-color: #f7f7f7;"">
						        <td style=""padding: 40px 25px 40px 25px; width: 100%;"">
							        <table style=""width: 100%; padding-bottom: 30px;"" border=""0"" cellspacing=""0"" cellpadding=""0"">
								        <tbody>
									        <tr>
										        <td>
											        <table style=""width: 100%;"" border=""0"" cellspacing=""0"" cellpadding=""0"">
												        <tbody>
													        <tr>
														        <td><span style=""font-size: 15px; font-family: Arial; color: #777777;"">&nbsp;</span></td>
													        </tr>
													        <tr>
														        <td colspan=""3"" align=""left"" style=""font-size: 14px;"">
															        <span style=""font-family: Arial; font-weight: bold;"">Se aprobaron los siguientes requerimientos:</span>
														        </td>
													        </tr>
													        <tr>
														        <td>
															        <table style=""width: 100%; padding: 30px 0px 30px 0px;"" border=""0"" cellspacing=""0"" cellpadding=""3"">
																        <tr>
																	        <td  style=""font-weight: bold; font-size: 16px; background-color: #cccccc; text-align: center; padding: 10px;"">
																		        <span style=""font-family: Arial;"">Requerimientos</span>
																	        </td>
																        </tr>
																        <tr>
																	        <th style=""font-weight: bold; font-size: 14px;"">
																		        <span style=""font-family: Arial;"">Número Requerimiento</span>
																	        </th>
																	       
																        </tr>
																        {{detalle_requerimiento}}
															        </table>
														        </td>
													        </tr>
												        </tbody>
											        </table>
										        </td>
									        </tr>
								        </tbody>
							        </table>
						        </td>
					        </tr>
					        <tr style=""background-color: #07a1d5;"">
						        <td style=""padding: 0; color: #ffffff; font-family: Arial; font-weight: bold; font-size: 18px; padding-left: 20px; height: 50px; font-style: oblique;"">&nbsp;</td>
					        </tr>
				        </tbody>
			        </table>
		        </td>
	        </tr>
        </tbody>
        </table>";

        private String detalleMailAprobacion = @"<tr>
											<td align=""left"" style=""font-size: 14px;"">
												<span style=""font-family: Arial; color: #777575;"">{{codigo_producto}}</span>
											</td>
											<td align=""left"" style=""font-size: 14px;"">
												<span style=""font-family: Arial; color: #777575;"">{{nombre_producto}}</span>
											</td>
											<td align=""right"" style=""font-size: 14px; padding-bottom: 5px;"">
												<span style=""font-family: Arial; color: #777575;"">{{cantidad_producto}}</span>
											</td>
										</tr>";
        


        /*Construye correo de actualización de requerimiento*/
        public String BuildTemplate(Requerimiento requerimiento)
        {
            
            String template = mailUpdate;
            template = template.Replace("{{nro_requerimiento}}", requerimiento.numeroRequerimientoString);
            template = template.Replace("{{url_requerimiento}}", this.urlVerRequerimiento);
            

            String detalles = "";
            
            foreach (RequerimientoDetalle det in requerimiento.requerimientoDetalleList)
            {
                String detalle = detalleMailUpdate;
                detalle = detalle.Replace("{{codigo_producto}}", det.producto.sku);
                detalle = detalle.Replace("{{nombre_producto}}", det.producto.descripcion);
                detalle = detalle.Replace("{{cantidad_producto}}", det.cantidad.ToString());
                detalles = detalles + detalle;
            }

            template = template.Replace("{{detalle_requerimiento}}", detalles);

            return template;
        }

        /*Construye correo de aprobación de requerimientos*/
        public String BuildTemplate(List<Requerimiento> requerimientoList)
        {

            String template = mailAprobacion;
         /*   template = template.Replace("{{nro_requerimiento}}", requerimiento.numeroRequerimientoString);
            template = template.Replace("{{url_requerimiento}}", this.urlVerRequerimiento);*/


            String detalles = "";

            foreach (Requerimiento det in requerimientoList)
            {
                String detalle = detalleMailAprobacion;
                detalle = detalle.Replace("{{numero_requerimiento}}", det.numeroRequerimientoString);
                /*detalle = detalle.Replace("{{nombre_producto}}", det.producto.descripcion);
                detalle = detalle.Replace("{{cantidad_producto}}", det.cantidad.ToString());*/
                detalles = detalles + detalle;
            }

            template = template.Replace("{{detalle_requerimiento}}", detalles);

            return template;
        }
    }
}