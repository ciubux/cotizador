using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Model;

namespace Cotizador.Email
{
    
    public class PedidoSinAtencion
    {
        public String urlVerPedido = "";
        private String mailPrincipal = @"<table border=""0"" cellspacing=""0"" cellpadding=""0"" align=""center"">
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
											<td style=""padding-left: 20px; padding-right: 10px;""><span style=""font-size: 19px; font-family: Arial; font-weight: bold; color: #555555; text-transform: uppercase;"">Datos del Pedido</span></td>
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
																<span style=""font-family: Arial; font-weight: bold;"">Número de Pedido: </span>
																<span style=""font-family: Arial; color: #777575;""><a href=""{{url_pedido}}"">{{nro_pedido}}</a></span>
															</td>
														</tr>
														<tr>
															<td colspan=""3"" align=""left"" style=""font-size: 14px;"">
																  <span style=""font-family: Arial; font-weight: bold;"">Código Cliente: </span>
																  <span style=""font-family: Arial; color: #777575;"">{{codigo_cliente}}</span>
															</td>
														</tr>
														<tr>
															<td colspan=""3"" align=""left"" style=""font-size: 14px;"">
																<span style=""font-family: Arial; font-weight: bold;"">Nombre Cliente: </span>
																<span style=""font-family: Arial; color: #777575;"">{{nombre_cliente}}</span>
															</td>
														</tr>
														<tr>
															<td>
																<table style=""width: 100%; padding: 30px 0px 30px 0px;"" border=""0"" cellspacing=""0"" cellpadding=""3"">
																	<tr>
																		<td colspan=""4"" style=""font-weight: bold; font-size: 16px; background-color: #cccccc; text-align: center; padding: 10px;"">
																			<span style=""font-family: Arial;"">Productos pendientes de atención</span>
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
																	{{detalle_pedido}}
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

        private String detallePedido = @"<tr>
											<td align=""left"" style=""font-size: 14px;"">
												<span style=""font-family: Arial; color: #777575;"">{{codigo_producto}}</span>
											</td>
											<td align=""center"" style=""font-size: 14px;"">
												<span style=""font-family: Arial; color: #777575;"">{{nombre_producto}}</span>
											</td>
											<td align=""right"" style=""font-size: 14px;"">
												<span style=""font-family: Arial; color: #777575;"">{{pendiente_producto}}</span>
											</td>
											<td align=""right"" style=""font-size: 14px; padding-bottom: 5px;"">
												<span style=""font-family: Arial; color: #777575;"">{{cantidad_producto}}</span>
											</td>
										</tr>";

        public String BuildTemplate(Pedido pedido)
        {
            
            String template = mailPrincipal;
            template = template.Replace("{{nro_pedido}}", pedido.numeroPedidoString);
            template = template.Replace("{{codigo_cliente}}", pedido.cliente.codigo);
            template = template.Replace("{{nombre_cliente}}", pedido.cliente.razonSocial);
            template = template.Replace("{{url_pedido}}", this.urlVerPedido);
            

            String detalles = "";
            
            foreach (PedidoDetalle det in pedido.pedidoDetalleList)
            {
                String detalle = detallePedido;
                detalle = detalle.Replace("{{codigo_producto}}", det.producto.sku);
                detalle = detalle.Replace("{{nombre_producto}}", det.producto.descripcion);
                detalle = detalle.Replace("{{pendiente_producto}}", det.cantidadPendienteAtencion.ToString());
                detalle = detalle.Replace("{{cantidad_producto}}", det.cantidad.ToString());

                detalles = detalles + detalle;
            }

            template = template.Replace("{{detalle_pedido}}", detalles);

            return template;
        }
    }
}