using System;

namespace WSGClienteCM.Models
{
    public class DetailBindingModel
    {
        //Cabecera
        public string SCODAPLICACION { get; set; }
        public Int64 NUSERCODE { get; set; }
        public string SNROPROCESO_CAB { get; set; }

        public Decimal? NNROPROCESO_DET { get; set; }
        public string SIDDOC { get; set; }
        public string NIDDOC_TYPE { get; set; }
        //Client
        public string SFIRSTNAME  { get; set; }
        public string SLASTNAME  { get; set; }
        public string SLASTNAME2 { get; set; }
        public string SLEGALNAME { get; set; }
        public string SSEXCLIEN { get; set; }
        public string NCIVILSTA { get; set; }
        public string NNATIONALITY { get; set; }
        public string DBIRTHDAT { get; set; }
        public DateTime DEFFECDATE { get; set; }
        public DateTime DNULLDATE { get; set; }
        public DateTime DCOMPDATE { get; set; }

        //Address
        public string ADDRESSTYPE { get; set; }
       

       
        
        public string NCOUNTRY { get; set; }
        
        public string NMUNICIPALITY { get; set; }
       
       
        
        public string STI_DIRE { get; set; }
        public string SNOM_DIRECCION { get; set; }
        public string SNUM_DIRECCION { get; set; }
        public string STI_BLOCKCHALET { get; set; }
        public string SBLOCKCHALET { get; set; }
        public string STI_INTERIOR { get; set; }
        public string SNUM_INTERIOR { get; set; }
        public string STI_CJHT { get; set; }
        public string SNOM_CJHT { get; set; }
        public string SETAPA { get; set; }
        public string SMANZANA { get; set; }
        public string SLOTE { get; set; }
        public string SREFERENCIA { get; set; }
        

        //phone


        public string NAREA_CODE { get; set; }
        public string SPHONE { get; set; }
        public string NPHONE_TYPE { get; set; }


        //email

        public string SE_MAIL { get; set; }
        public string SEMAILTYPE { get; set; }
        //Indicadores
        public string COD_CIIU { get; set; }
        public string COD_CUSPP { get; set; }

        // error
        public string P_SMESSAGE { get; set; }
        public string P_COD_ERR { get; set; }
       

        // nuevos campos

        public string SPROTEG_DATOS_IND { get; set; }
        public string SBAJAMAIL_IND { get; set; }
        public string SISCLIENT_GBD { get; set; }


    }
}
