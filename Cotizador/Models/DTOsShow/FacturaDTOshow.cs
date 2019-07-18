using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotizador.Models.DTOshow
{
    public class FacturaDTOshow
    {


        public bool solicitadoAnulacion { get; set; }
        public Model.DocumentoVenta.EstadosDocumentoSunat estadoDocumentoSunat { get; set; }
        public Model.DocumentoVenta.TipoDocumento tipoDocumento { get; set; }
        public String tipoNotaCreditoString { get; set; }
        public String cPE_CABECERA_BE_DES_MTVO_NC_ND { get; set; }
      public String cPE_DOC_REF_BEList_FEC_DOC_REF { get; set; }
        public String cPE_DOC_REF_BEList_NUM_SERIE_CPE_REF { get; set; }
        public String cPE_DOC_REF_BEList_NUM_CORRE_CPE_REF { get; set; }

        public Guid idDocumentoVenta { get; set; }
        public String cPE_CABECERA_BE_FEC_EMI { get; set; }
        public String cPE_CABECERA_BE_HOR_EMI { get; set; }
        public String cPE_CABECERA_BE_SERIE { get; set; }
        public String cPE_CABECERA_BE_CORRELATIVO { get; set; }
        public String cPE_CABECERA_BE_NOM_RCT { get; set; }
        public String PE_CABECERA_BE_DIR_DES_RCT { get; set; }
        public String cPE_CABECERA_BE_NRO_ORD_COM { get; set; }
        public String cPE_CABECERA_BE_NRO_DOC_RCT { get; set; }
        public String cPE_CABECERA_BE_NRO_GRE { get; set; }
    //   public String cPE_DAT_ADIC_BEList_TXT_DESC_ADIC_SUNAT { get; set; }
        public List<Model.ServiceReferencePSE.CPE_DAT_ADIC_BE> cPE_DAT_ADIC_BEList { get; set; }
        public List<Model.ServiceReferencePSE.CPE_DOC_REF_BE> cPE_DOC_REF_BEList { get; set; }
        public List<Model.ServiceReferencePSE.CPE_DETALLE_BE> cPE_DETALLE_BEList { get; set; }
       
        public String cPE_CABECERA_BE_CORREO_ENVIO { get; set; }
        public String tipoPagoString { get; set; }        
    public String cPE_CABECERA_BE_FEC_VCTO { get; set; }
    public String cPE_CABECERA_BE_MNT_TOT_GRV { get; set; }
        public String cPE_CABECERA_BE_MNT_TOT_GRV_NAC { get; set; }
        public String CABECERA_BE_MNT_TOT_INF { get; set; }
        public String CABECERA_BE_MNT_TOT_EXR { get; set; }
        public String cPE_CABECERA_BE_MNT_TOT_VAL_VTA { get; set; }
        public String cPE_CABECERA_BE_MNT_TOT_TRB_IGV { get; set; }
        public String cPE_CABECERA_BE_MNT_TOT_PRC_VTA { get; set; }
        public String tipoNotaDebitoString { get; set; }
        public String cPE_CABECERA_BE_DIR_DES_RCT { get; set; }
        public String cPE_CABECERA_BE_MNT_TOT_INF { get; set; }
        public String cPE_CABECERA_BE_MNT_TOT_EXR { get; set; }
        public String cPE_CABECERA_BE_MNT_TOT_GRT { get; set; }
      
      

        //  public String cPE_DOC_REF_BEList0_FEC_DOC_REF { get; set; }
        //public String cPE_DOC_REF_BEList0_NUM_SERIE_CPE_REF { get; set; }
        //  public String cPE_DOC_REF_BEList0_NUM_CORRE_CPE_REF { get; set; }







    }
}