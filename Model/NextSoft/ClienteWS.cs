﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Model.NextSoft
{
    public class ClienteWS : MasterWSClass
    {
        public async Task<object> crearCliente(object dataCliente)
        {
            string nombreServicio = "crearcliente";

            var sendData = new { 
                token = this.apiToken,
                cliente = dataCliente
            };

            object resultContent = await this.callService(sendData, nombreServicio);

            return resultContent;
        }

        
    }
}