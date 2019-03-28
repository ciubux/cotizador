
using Model.ServiceReferencePSE;
using DataLayer;
using Model;
using System;
using System.IO;

namespace BusinessLayer
{
    public class FacturaBL
    {

        public void testFacturaElectronica()
        {
        //    this.callProcessOnline();


        }


        public void callProcessOnline()
        {
            IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();

            /*  var oUser = "ADMTAXTECH";
              var oPass = "T@xTech2018";*/

            var oUser = "admin@mp.eol.pe";
            var oPass = "7f2a87fb";

            CPE_CABECERA_BE cPE_CABECERA_BE = new CPE_CABECERA_BE();

            /*cPE_CABECERA_BE.ID = "98789764677MRTYU";
            cPE_CABECERA_BE.COD_GPO = "4"; //OK FIJO*/
            cPE_CABECERA_BE.ID = "876178c8fc875c949a0b7df5f716985d8338fa33e5a4017ebffb623ba8451d02";
            cPE_CABECERA_BE.COD_GPO = "14";

            cPE_CABECERA_BE.TIP_CPE = "01"; //OK Factura 
            cPE_CABECERA_BE.FEC_EMI = "2018-04-21"; //OK
            cPE_CABECERA_BE.HOR_EMI = "08:56:14";  //OK
            cPE_CABECERA_BE.SERIE = "F002";  //OK
            cPE_CABECERA_BE.CORRELATIVO = "00100013";  //OK
            cPE_CABECERA_BE.MONEDA = "PEN";
            cPE_CABECERA_BE.COD_TIP_OPE = "0101";
            cPE_CABECERA_BE.TIP_DOC_EMI = "6";
            cPE_CABECERA_BE.NRO_DOC_EMI = "20509411671";
            // cPE_CABECERA_BE.NOM_EMI = "<![CDATA[TAX TECHNOLOGY PRUEBA SAC]]>";
            cPE_CABECERA_BE.NOM_EMI = "MP INSTITUCIONAL SOCIEDAD ANONIMA CERRRADA";
            //  cPE_CABECERA_BE.NOM_COM_EMI = "<![CDATA[TAXTECH SAC]]>";
            cPE_CABECERA_BE.NOM_COM_EMI = "MP INSTITUCIONAL SOCIEDAD ANONIMA CERRRADA";
            cPE_CABECERA_BE.COD_LOC_EMI = "0000";
            cPE_CABECERA_BE.TIP_DOC_RCT = "6";
            cPE_CABECERA_BE.NRO_DOC_RCT = "20601890000";
            //cPE_CABECERA_BE.NOM_RCT = "<![CDATA[CLIENTE RECEPTOR S.A.C.]]>";
            cPE_CABECERA_BE.NOM_RCT = "CLIENTE RECEPTOR S.A.C.";
            // cPE_CABECERA_BE.DIR_DES_RCT = "<![CDATA[AV. COMANDANTE ESPINAR NRO. 435 DPTO. 803 CJRES PEDRO RUIZ GALLO]]>";

            cPE_CABECERA_BE.DIR_DES_RCT = "AV. COMANDANTE ESPINAR NRO. 435 DPTO. 803 CJRES PEDRO RUIZ GALLO";
            cPE_CABECERA_BE.UBI_RCT = "150101";
            cPE_CABECERA_BE.NUM_REG_MTC = "";
            cPE_CABECERA_BE.MNT_REF = "";
            cPE_CABECERA_BE.COD_MND_REF = "";
            cPE_CABECERA_BE.UBI_ENT = "";
            cPE_CABECERA_BE.DIR_DES_ENT = "";
            cPE_CABECERA_BE.DIR_URB_ENT = "";
            cPE_CABECERA_BE.DIR_PAI_ENT = "";
            cPE_CABECERA_BE.TIP_PAG = "001";
            cPE_CABECERA_BE.FRM_PAG = "001";
            cPE_CABECERA_BE.NRO_ORD_COM = "OC-235466";
            cPE_CABECERA_BE.COD_TIP_GRE = "";
            cPE_CABECERA_BE.NRO_GRE = "";
            cPE_CABECERA_BE.COD_OPC = "";
            cPE_CABECERA_BE.FMT_IMPR = "001";
            cPE_CABECERA_BE.IMPRESORA = "";
            cPE_CABECERA_BE.MNT_DCTO_GLB = "0.00";
            cPE_CABECERA_BE.FAC_DCTO_GLB = "0.00";
            cPE_CABECERA_BE.MNT_CARG_GLB = "0.00";
            cPE_CABECERA_BE.FAC_CARG_GLOB = "0.00";
            cPE_CABECERA_BE.TIP_CARG_GLOB = "";
            cPE_CABECERA_BE.MNT_TOT_PER = "0.00";
            cPE_CABECERA_BE.TIP_PER = "";
            cPE_CABECERA_BE.FAC_PER = "0.00";
            cPE_CABECERA_BE.MNT_TOT_IMP = "3.65";
            cPE_CABECERA_BE.MNT_TOT_GRV = "20.30";
            cPE_CABECERA_BE.MNT_TOT_INF = "0.00";
            cPE_CABECERA_BE.MNT_TOT_EXR = "0.00";
            cPE_CABECERA_BE.MNT_TOT_GRT = "0.00";
            cPE_CABECERA_BE.MNT_TOT_EXP = "0.00";
            cPE_CABECERA_BE.MNT_TOT_TRB_IGV = "3.65";
            cPE_CABECERA_BE.MNT_TOT_TRB_ISC = "0.00";
            cPE_CABECERA_BE.MNT_TOT_TRB_OTR = "0.00";
            cPE_CABECERA_BE.MNT_TOT_VAL_VTA = "0.00";
            cPE_CABECERA_BE.MNT_TOT_PRC_VTA = "0.00";
            cPE_CABECERA_BE.MNT_TOT_DCTO = "0.00";
            cPE_CABECERA_BE.MNT_TOT_OTR_CGO = "0.00";
            cPE_CABECERA_BE.MNT_TOT = "23.95";
            cPE_CABECERA_BE.MNT_TOT_ANTCP = "0.00";
            cPE_CABECERA_BE.TIP_CMB = "";
            cPE_CABECERA_BE.MNT_DCTO_GLB_NAC = "0.00";
            cPE_CABECERA_BE.MNT_CARG_GLB_NAC = "0.00";
            cPE_CABECERA_BE.MNT_TOT_PER_NAC = "0.00";
            cPE_CABECERA_BE.MNT_TOT_IMP_NAC = "3.65";
            cPE_CABECERA_BE.MNT_TOT_GRV_NAC = "20.30";
            cPE_CABECERA_BE.MNT_TOT_INF_NAC = "0.00";
            cPE_CABECERA_BE.MNT_TOT_EXR_NAC = "0.00";
            cPE_CABECERA_BE.MNT_TOT_GRT_NAC = "0.00";
            cPE_CABECERA_BE.MNT_TOT_EXP_NAC = "0.00";
            cPE_CABECERA_BE.MNT_TOT_TRB_IGV_NAC = "3.65";
            cPE_CABECERA_BE.MNT_TOT_TRB_ISC_NAC = "0.00";
            cPE_CABECERA_BE.MNT_TOT_TRB_OTR_NAC = "0.00";
            cPE_CABECERA_BE.MNT_TOT_VAL_VTA_NAC = "0.00";
            cPE_CABECERA_BE.MNT_TOT_PRC_VTA_NAC = "0.00";
            cPE_CABECERA_BE.MNT_TOT_DCTO_NAC = "0.00";
            cPE_CABECERA_BE.MNT_TOT_OTR_CGO_NAC = "0.00";
            cPE_CABECERA_BE.MNT_TOT_NAC = "23.95";
            cPE_CABECERA_BE.MNT_TOT_ANTCP_NAC = "0.00";
            cPE_CABECERA_BE.MNT_IMPTO_PER = "";
            cPE_CABECERA_BE.MNT_TOT_MAS_PER = "";
            cPE_CABECERA_BE.COD_TIP_DETRACCION = "";
            cPE_CABECERA_BE.MNT_TOT_DETRACCION = "";
            cPE_CABECERA_BE.FAC_DETRACCION = "";
            cPE_CABECERA_BE.COD_SOF_FACT = "COD001";
            cPE_CABECERA_BE.COD_TIP_NC = "";
            cPE_CABECERA_BE.COD_TIP_ND = "";
            cPE_CABECERA_BE.DES_MTVO_NC_ND = "";
            cPE_CABECERA_BE.CORREO_ENVIO = "moisesgomezab@gmail.com";
            cPE_CABECERA_BE.CORREO_COPIA = "";
            cPE_CABECERA_BE.CORREO_OCULTO = "";
            cPE_CABECERA_BE.PTO_VTA = "";
            cPE_CABECERA_BE.FLG_TIP_CAMBIO = "";
            cPE_CABECERA_BE.COD_PROCEDENCIA = "003";
            cPE_CABECERA_BE.ID_EXT_RZN = "1";
            cPE_CABECERA_BE.ETD_SNT = "101";

            CPE_DETALLE_BE[] cPE_DETALLE_BEList = new CPE_DETALLE_BE[1];
            CPE_DETALLE_BE cPE_DETALLE_BE = new CPE_DETALLE_BE();
            cPE_DETALLE_BE.LIN_ITM = "1";
            cPE_DETALLE_BE.COD_UND_ITM = "NIU";
            cPE_DETALLE_BE.CANT_UND_ITM = "1.00";
            cPE_DETALLE_BE.VAL_VTA_ITM = "15.00";

            cPE_DETALLE_BE.PRC_VTA_UND_ITM = "17.70";
            cPE_DETALLE_BE.VAL_UNIT_ITM = "15.00";
            cPE_DETALLE_BE.MNT_IGV_ITM = "2.70";
            cPE_DETALLE_BE.POR_IGV_ITM = "";

            cPE_DETALLE_BE.PRC_VTA_ITEM = "0.00";
            cPE_DETALLE_BE.VAL_VTA_BRT_ITEM = "0.00";
            cPE_DETALLE_BE.COD_TIP_AFECT_IGV_ITM = "10";
            cPE_DETALLE_BE.MNT_ISC_ITM = "";

            cPE_DETALLE_BE.POR_ISC_ITM = "";
            cPE_DETALLE_BE.COD_TIP_SIST_ISC = "";
            cPE_DETALLE_BE.PRECIO_SUGERIDO_ISC = "";
            cPE_DETALLE_BE.MNT_DCTO_ITM = "";

            cPE_DETALLE_BE.FAC_DCTO_ITM = "";
            cPE_DETALLE_BE.MNT_CARG_ITM = "";
            cPE_DETALLE_BE.FAC_CARG_ITM = "";
            cPE_DETALLE_BE.TIP_CARG_ITM = "";

            cPE_DETALLE_BE.MNT_TOT_PER_ITM = "";
            cPE_DETALLE_BE.FAC_PER_ITM = "";
            cPE_DETALLE_BE.TIP_PER_ITM = "";
            cPE_DETALLE_BE.TXT_DES_ITM = "Pruebas";

            cPE_DETALLE_BE.TXT_DES_ADIC_ITM = "";
            cPE_DETALLE_BE.COD_ITM = "03077";
            cPE_DETALLE_BE.COD_ITM_SUNAT = "SUNAT001";
            cPE_DETALLE_BE.DET_VAL_ADIC01 = "";

            cPE_DETALLE_BE.DET_VAL_ADIC02 = "";
            cPE_DETALLE_BE.DET_VAL_ADIC03 = "";
            cPE_DETALLE_BE.DET_VAL_ADIC04 = "";
            cPE_DETALLE_BE.DET_VAL_ADIC05 = "";

            cPE_DETALLE_BE.DET_VAL_ADIC06 = "";
            cPE_DETALLE_BE.DET_VAL_ADIC07 = "";
            cPE_DETALLE_BE.DET_VAL_ADIC08 = "";
            cPE_DETALLE_BE.DET_VAL_ADIC09 = "";

            cPE_DETALLE_BE.DET_VAL_ADIC10 = "";
            cPE_DETALLE_BE.DES_COMP = new CPE_DETALLE_COMPTA_BE[0];


            cPE_DETALLE_BEList[0] = cPE_DETALLE_BE;

            CPE_DAT_ADIC_BE[] cPE_DAT_ADIC_BE = new CPE_DAT_ADIC_BE[0];
            /*
            CPE_DAT_ADIC_BE cPE_DAT_ADIC_BE1 = new CPE_DAT_ADIC_BE();
            cPE_DAT_ADIC_BE1.COD_TIP_ADIC_SUNAT = "1000";
            cPE_DAT_ADIC_BE1.NUM_LIN_ADIC_SUNAT = "01";
            //cPE_DAT_ADIC_BE1.TXT_DESC_ADIC_SUNAT = "<![CDATA[VEINTITRES CON  95/100 NUEVOS SOLES]]>";
            cPE_DAT_ADIC_BE1.TXT_DESC_ADIC_SUNAT = "VEINTITRES CON  95/100 NUEVOS SOLES";
            cPE_DAT_ADIC_BE[0] = cPE_DAT_ADIC_BE1;


            CPE_DAT_ADIC_BE cPE_DAT_ADIC_BE2 = new CPE_DAT_ADIC_BE();
            cPE_DAT_ADIC_BE2.COD_TIP_ADIC_SUNAT = "0001";
            cPE_DAT_ADIC_BE2.NUM_LIN_ADIC_SUNAT = "02";
            //cPE_DAT_ADIC_BE2.TXT_DESC_ADIC_SUNAT = "<![CDATA[997923242]]>";
            cPE_DAT_ADIC_BE2.TXT_DESC_ADIC_SUNAT = "997923242";
            cPE_DAT_ADIC_BE[1] = cPE_DAT_ADIC_BE2;


            CPE_DAT_ADIC_BE cPE_DAT_ADIC_BE3 = new CPE_DAT_ADIC_BE();
            cPE_DAT_ADIC_BE3.COD_TIP_ADIC_SUNAT = "0002";
            cPE_DAT_ADIC_BE3.NUM_LIN_ADIC_SUNAT = "03";
            //cPE_DAT_ADIC_BE3.TXT_DESC_ADIC_SUNAT = "<![CDATA[http://www.taxech.pe]]>";
            cPE_DAT_ADIC_BE3.TXT_DESC_ADIC_SUNAT = "http://www.taxech.pe";
            cPE_DAT_ADIC_BE[2] = cPE_DAT_ADIC_BE3;

            CPE_DAT_ADIC_BE cPE_DAT_ADIC_BE4 = new CPE_DAT_ADIC_BE();
            cPE_DAT_ADIC_BE4.COD_TIP_ADIC_SUNAT = "0003";
            cPE_DAT_ADIC_BE4.NUM_LIN_ADIC_SUNAT = "04";
            //cPE_DAT_ADIC_BE4.TXT_DESC_ADIC_SUNAT = "<![CDATA[hola@taxtech.pe]]>";
            cPE_DAT_ADIC_BE4.TXT_DESC_ADIC_SUNAT = "hola@taxtech.pe";
            cPE_DAT_ADIC_BE[3] = cPE_DAT_ADIC_BE4;


            CPE_DAT_ADIC_BE cPE_DAT_ADIC_BE5 = new CPE_DAT_ADIC_BE();
            cPE_DAT_ADIC_BE5.COD_TIP_ADIC_SUNAT = "1001";
            cPE_DAT_ADIC_BE5.NUM_LIN_ADIC_SUNAT = "05";
            //cPE_DAT_ADIC_BE5.TXT_DESC_ADIC_SUNAT = "<![CDATA[022]]>";
            cPE_DAT_ADIC_BE5.TXT_DESC_ADIC_SUNAT = "022";
            cPE_DAT_ADIC_BE[4] = cPE_DAT_ADIC_BE5;

            CPE_DAT_ADIC_BE cPE_DAT_ADIC_BE6 = new CPE_DAT_ADIC_BE();
            cPE_DAT_ADIC_BE6.COD_TIP_ADIC_SUNAT = "1002";
            cPE_DAT_ADIC_BE6.NUM_LIN_ADIC_SUNAT = "06";
            //cPE_DAT_ADIC_BE6.TXT_DESC_ADIC_SUNAT = "<![CDATA[0666-000-120]]>";
            cPE_DAT_ADIC_BE6.TXT_DESC_ADIC_SUNAT = "0666-000-120";
            cPE_DAT_ADIC_BE[5] = cPE_DAT_ADIC_BE6;*/

            CPE_DOC_REF_BE[] cPE_DOC_REF_BE = new CPE_DOC_REF_BE[0];
            CPE_ANTICIPO_BE[] cPE_ANTICIPO_BE = new CPE_ANTICIPO_BE[0];
            CPE_FAC_GUIA_BE[] cPE_FAC_GUIA_BE = new CPE_FAC_GUIA_BE[0];
            CPE_DOC_ASOC_BE[] cPE_DOC_ASOC_BE = new CPE_DOC_ASOC_BE[0];
            GlobalEnumTipoOnline globalEnumTipoOnline = GlobalEnumTipoOnline.Normal;

            CPE_RESPUESTA_BE cPE_RESPUESTA_BE = client.callProcessOnline(oUser, oPass, cPE_CABECERA_BE, cPE_DETALLE_BEList, cPE_DAT_ADIC_BE, 
                cPE_DOC_REF_BE, cPE_ANTICIPO_BE, cPE_FAC_GUIA_BE, cPE_DOC_ASOC_BE, globalEnumTipoOnline);

            var CODIGO = cPE_RESPUESTA_BE.CODIGO;
            var COD_ESTD_SUNAT = cPE_RESPUESTA_BE.COD_ESTD_SUNAT;
            var DESCRIPCION = cPE_RESPUESTA_BE.DESCRIPCION;
            var DETALLE = cPE_RESPUESTA_BE.DETALLE;
            var NUM_CPE = cPE_RESPUESTA_BE.NUM_CPE;

            callStateCPE(cPE_CABECERA_BE.SERIE, cPE_CABECERA_BE.CORRELATIVO);
            callExtractCPE(cPE_CABECERA_BE.SERIE, cPE_CABECERA_BE.CORRELATIVO);
        }



        public void callStateCPE(String SERIE, String CORRELATIVO)
        {
            IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
            //102 103 codigos de estado aceptado SUNAT

            var oUser = "demo@mp.eol.pe";
            var oPass = "00144f91";

          //  var oUser = "ADMTAXTECH";
        //    var oPass = "T@xTech2018";
            var oNroIde = "20509411671";
            var oTipCpe = "01";
            var oSerCpe = SERIE;
            var oNroCpe = CORRELATIVO;

            RPTA_BE rpta_be = client.callStateCPE(oUser, oPass, oNroIde, oTipCpe, oSerCpe, oNroCpe);

            var codigo = rpta_be.CODIGO;
            var descripcion = rpta_be.DESCRIPCION;
            var detalle = rpta_be.DETALLE;
            var estado = rpta_be.ESTADO;

        }


        public void callExtractCPE(String SERIE, String CORRELATIVO)
        {
            IwsOnlineToCPEClient client = new IwsOnlineToCPEClient();
            //102 103 codigos de estado aceptado SUNAT

            var oUser = "demo@mp.eol.pe";
            var oPass = "00144f91";
            var oNroIde = "20509411671";
            var oTipCpe = "01";
            var oSerCpe = SERIE;
            var oNroCpe = CORRELATIVO;
            var oFlgXml = true;
            var oFlgPdf = true;
            var oFlgCdr = true;


            RPTA_DOC_TRIB_BE rpta_doc_trib_be = client.callExtractCPE(oUser, oPass, oNroIde, oTipCpe, oSerCpe, oNroCpe, oFlgXml, oFlgPdf, oFlgCdr);


            var cod_rpta = rpta_doc_trib_be.COD_RPTA;
            var descripcion = rpta_doc_trib_be.DESCRIPCION;
            var detalle = rpta_doc_trib_be.DETALLE;
            byte[] doc_trib_pdf = rpta_doc_trib_be.DOC_TRIB_PDF;
            var doc_trib_rpta = rpta_doc_trib_be.DOC_TRIB_RPTA;
            var doc_trib_xml = rpta_doc_trib_be.DOC_TRIB_XML;
            var num_ope = rpta_doc_trib_be.NUM_OPE;

            String pathrootsave = System.AppDomain.CurrentDomain.BaseDirectory + "\\pdf\\";
            File.WriteAllBytes(pathrootsave+"TAXTECH.pdf", doc_trib_pdf);

        }



        public void setFacturaStaging(FacturaStaging facturaStaging)
        {
            using (var facturaDAL = new FacturaDAL())
            {
                facturaDAL.setFacturaStaging(facturaStaging);
            }
        }

        public void truncateFacturaStaging()
        {
            using (var facturaDAL = new FacturaDAL())
            {
                facturaDAL.truncateFacturaStaging();
            }
        }

        public void mergeFacturaStaging()
        {
            using (var facturaDAL = new FacturaDAL())
            {
                facturaDAL.mergeFacturaStaging();
            }
        }
    }
}
