using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Model;

namespace BusinessLayer.Email
{
    
    public class PedidoTecnica
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
															<td valign=""center""></td>
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
															<td colspan=""3"" align=""left"" style=""font-size: 14px; padding-bottom: 10px;"">
																<span style=""font-family: Arial; font-weight: bold;"">Número de Pedido: </span>
																<span style=""font-family: Arial; color: #777575;""><a href=""{{url_pedido}}"">{{nro_pedido}}</a></span>
															</td>
														</tr>
                                                        <tr>
															<td colspan=""3"" align=""left"" style=""font-size: 14px; padding-bottom: 10px;"">
																<span style=""font-family: Arial; font-weight: bold;"">Estado: </span>
																<span style=""font-family: Arial; color: #777575;"">{{estado_pedido}}</span>
															</td>
														</tr>
														<tr>
															<td colspan=""3"" align=""left"" style=""font-size: 14px; padding-bottom: 10px;"">
																  <span style=""font-family: Arial; font-weight: bold;"">Código Cliente: </span>
																  <span style=""font-family: Arial; color: #777575;"">{{codigo_cliente}}</span>
															</td>
														</tr>
														<tr>
															<td colspan=""3"" align=""left"" style=""font-size: 14px; padding-bottom: 10px;"">
																<span style=""font-family: Arial; font-weight: bold;"">{{tipo_documento}}: </span>
																<span style=""font-family: Arial; color: #777575;"">{{nro_documento}}</span>
															</td>
														</tr>
														<tr>
															<td colspan=""3"" align=""left"" style=""font-size: 14px; padding-bottom: 10px;"">
																<span style=""font-family: Arial; font-weight: bold;"">Nombre Cliente: </span>
																<span style=""font-family: Arial; color: #777575;"">{{nombre_cliente}}</span>
															</td>
														</tr>
														<tr>
															<td colspan=""3"" align=""left"" style=""font-size: 14px;"">
																<span style=""font-family: Arial; font-weight: bold;"">Dirección Fiscal: </span>
																<span style=""font-family: Arial; color: #777575;"">{{direccion_fiscal}}</span>
															</td>
														</tr>
														<tr>
															<td>
																<table style=""width: 100%; padding: 30px 0px 30px 0px;"" border=""0"" cellspacing=""0"" cellpadding=""3"">
																	<tr>
																		<td colspan=""6"" style=""font-weight: bold; font-size: 16px; background-color: #cccccc; text-align: center; padding: 10px;"">
																			<span style=""font-family: Arial;"">Productos</span>
																		</td>
																	</tr>
																	<tr>
																	   <th style=""font-weight: bold; font-size: 14px;"">
																			<span style=""font-family: Arial;"">Código Proveedor</span>
																		</th>
																		<th style=""font-weight: bold; font-size: 14px;"">
																			<span style=""font-family: Arial;"">Descripción</span>
																		</th>
																		<th style=""font-weight: bold; font-size: 14px;"">
																			<span style=""font-family: Arial;"">Unidad</span>
																		</th>
																		<th style=""font-weight: bold; font-size: 14px;"">
																			<span style=""font-family: Arial;"">Precio Unitario</span>
																		</th>
																		<th style=""font-weight: bold; font-size: 14px;"">
																			<span style=""font-family: Arial;"">Cantidad</span>
																		</th>
																		<th style=""font-weight: bold; font-size: 14px;"">
																			<span style=""font-family: Arial;"">Total<br/> Item</span>
																		</th>
																	</tr>
																	{{detalle_pedido}}
																</table>
															</td>
														</tr>
														<tr>
														</tr>
														<tr>
															<td colspan=""3"" align=""left"" style=""font-size: 14px; padding-bottom: 10px;"">
																<span style=""font-family: Arial; font-weight: bold;"">Dirección Entrega: </span>
																<span style=""font-family: Arial; color: #777575;"">{{direccion_entrega}}</span>
															</td>
														</tr>
                                                        <tr>
															<td colspan=""3"" align=""left"" style=""font-size: 14px; padding-bottom: 10px;"">
																<span style=""font-family: Arial; font-weight: bold;"">Contacto Entrega: </span>
																<span style=""font-family: Arial; color: #777575;"">{{contacto_entrega}}</span>
															</td>
														</tr>
														<tr>
															<td colspan=""3"" align=""left"" style=""font-size: 14px; padding-bottom: 10px;"">
																  <span style=""font-family: Arial; font-weight: bold;"">Observaciones Pedido: </span>
																  <span style=""font-family: Arial; color: #777575;"">{{observaciones_pedido}}</span>
															</td>
														</tr>
														<tr>
															<td colspan=""3"" align=""left"" style=""font-size: 14px; padding-bottom: 10px;"">
																<span style=""font-family: Arial; font-weight: bold;"">Observaciones Guía: </span>
																<span style=""font-family: Arial; color: #777575;"">{{observaciones_guia}}</span>
															</td>
														</tr>
														<tr>
															<td colspan=""3"" align=""left"" style=""font-size: 14px;"">
																<span style=""font-family: Arial; font-weight: bold;"">Observaciones Factura: </span>
																<span style=""font-family: Arial; color: #777575;"">{{observaciones_factura}}</span>
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
											<td align=""left"" style=""font-size: 14px; padding-bottom: 5px;"">
												<span style=""font-family: Arial; color: #777575;"">{{codigo_producto}}</span>
											</td>
											<td align=""left"" style=""font-size: 14px; padding-bottom: 5px;"">
												<span style=""font-family: Arial; color: #777575;"">{{nombre_producto}}</span>
											</td>
											<td align=""center"" style=""font-size: 14px; padding-bottom: 5px;"">
												<span style=""font-family: Arial; color: #777575;"">{{unidad_producto}}</span>
											</td>
											<td align=""right"" style=""font-size: 14px; padding-bottom: 5px;"">
												<span style=""font-family: Arial; color: #777575;"">{{precio_unitario}}</span>
											</td>
											<td align=""center"" style=""font-size: 14px; padding-bottom: 5px;"">
												<span style=""font-family: Arial; color: #777575;"">{{cantidad_producto}}</span>
											</td>
											<td align=""right"" style=""font-size: 14px;"">
												<span style=""font-family: Arial; color: #777575;"">{{total_item}}</span>
											</td>
										</tr>";

        public String BuildTemplate(Pedido pedido)
        {
            
            String template = mailPrincipal;
            template = template.Replace("{{nro_pedido}}", pedido.numeroPedidoString);
            template = template.Replace("{{estado_pedido}}", pedido.seguimientoPedido.estadoString);
            
            template = template.Replace("{{codigo_cliente}}", pedido.cliente.codigo);
			template = template.Replace("{{tipo_documento}}", pedido.cliente.tipoDocumentoIdentidadToString);
			template = template.Replace("{{nro_documento}}", pedido.cliente.ruc);
			template = template.Replace("{{nombre_cliente}}", pedido.cliente.razonSocial);
            template = template.Replace("{{url_pedido}}", this.urlVerPedido);
			template = template.Replace("{{direccion_fiscal}}", pedido.cliente.direccionDomicilioLegalSunat);

			template = template.Replace("{{direccion_entrega}}", pedido.direccionEntrega.descripcion);
			template = template.Replace("{{contacto_entrega}}", pedido.direccionEntrega.contacto + " (Telf.: " + pedido.direccionEntrega.telefono + ")");
			template = template.Replace("{{observaciones_pedido}}", pedido.observaciones);
			template = template.Replace("{{observaciones_guia}}", pedido.observacionesGuiaRemision);
			template = template.Replace("{{observaciones_factura}}", pedido.observacionesFactura);
			String detalles = "";
            
            foreach (PedidoDetalle det in pedido.pedidoDetalleList)
            {
                String detalle = detallePedido;
                detalle = detalle.Replace("{{codigo_producto}}", det.producto.skuProveedor);
                detalle = detalle.Replace("{{nombre_producto}}", det.producto.descripcion);
				detalle = detalle.Replace("{{unidad_producto}}", det.unidad);
				detalle = detalle.Replace("{{precio_unitario}}", det.precioUnitario.ToString());
				detalle = detalle.Replace("{{cantidad_producto}}", det.cantidad.ToString());
				detalle = detalle.Replace("{{total_item}}", det.subTotal.ToString());

				detalles = detalles + detalle;
            }

            template = template.Replace("{{detalle_pedido}}", detalles);

            return template;
        }
    }
}