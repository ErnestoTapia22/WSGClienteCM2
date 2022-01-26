using System;

namespace WSGClienteCM.Models
{
    public class ClientDetailBindingModel
    {
        /* Atributos Validacion Masiva */
        public string NNOPROCESO_CAB { get; set; }
        public Int64 NNUMREG { get; set; }
        public string SFILENAME { get; set; }
        /* Fin */
        public string SORIGEN { get; set; }
        public string TIPO_CLIE { get; set; }

        public string CodAplicacion { get; set; }
        public string TipOper { get; set; }
        public string NUSERCODE { get; set; }
        public string NIDDOC_TYPE { get; set; }
        public string SIDDOC { get; set; }
        public string SFIRSTNAME { get; set; }
        public string SLASTNAME { get; set; }
        public string SLASTNAME2 { get; set; }
        public string SLEGALNAME { get; set; }
        public string SSEXCLIEN { get; set; }
        public string NINCAPACITY { get; set; }
        public string NINCAP_COD { get; set; }
        public string DBIRTHDAT { get; set; }
        public string DINCAPACITY { get; set; }
        public string NHEALTH_ORG { get; set; }
        public string DDEATHDAT { get; set; }
        public string DWEDD { get; set; }
        public string SACCOUNT_IN { get; set; }
        public string NINVOICING { get; set; }
        public string NSPECIALITY { get; set; }
        public string NCIVILSTA { get; set; }
        public string DAPROBDATE { get; set; }
        public string SBLOCKADE { get; set; }
        public string NCLASS { get; set; }
        public string DDRIVERDAT { get; set; }
        public string NHEIGHT { get; set; }
        public string NHOUSE_TYPE { get; set; }
        public string SLICENSE { get; set; }
        public string NNOTENUM { get; set; }
        public string NQ_CARS { get; set; }
        public string NQ_CHILD { get; set; }
        public string NRATE { get; set; }
        public string STAX_CODE { get; set; }
        public string SSMOKING { get; set; }
        public string SCUIT { get; set; }
        public string NTITLE { get; set; }
        public string NWEIGHT { get; set; }
        public string SAUTO_CHAR { get; set; }
        public string SCREDIT_CARD { get; set; }
        public string NECONOMIC_L { get; set; }
        public string NEMPL_QUA { get; set; }
        public string NIMAGENUM { get; set; }
        public string NAREA { get; set; }
        public string DDRIVEXPDAT { get; set; }
        public string NTYPDRIVER { get; set; }
        public string NDISABILITY { get; set; }
        public string NLIMITDRIV { get; set; }
        public string NAFP { get; set; }
        public string SBILL_IND { get; set; }
        public string NNATIONALITY { get; set; }
        public string SDIGIT { get; set; }
        public string DRETIREMENT { get; set; }
        public string DINDEPENDANT { get; set; }
        public string DDEPENDANT { get; set; }
        public string NMAILINGPREF { get; set; }
        public string NLANGUAGE { get; set; }
        public string SLEFTHANDED { get; set; }
        public string SBLOCKLAFT { get; set; }
        public string SISCLIENT_IND { get; set; }
        public string SISRENIEC_IND { get; set; }
        public string NCLIENT_SEG { get; set; }
        public string SPOLIZA_ELECT_IND { get; set; }
        public string TI_DOC_SUSTENT { get; set; }
        public string NU_DOC_SUSTENT { get; set; }
        public string COD_UBIG_DEP_NAC { get; set; }
        public string COD_UBIG_PROV_NAC { get; set; }
        public string COD_UBIG_DIST_NAC { get; set; }
        public string DEPARTAMENTO_NACIMIENTO { get; set; }
        public string PROVINCIA_NACIMIENTO { get; set; }
        public string DISTRITO_NACIMIENTO { get; set; }
        public string NOMBRE_PADRE { get; set; }
        public string NOMBRE_MADRE { get; set; }
        public string FECHA_INSC { get; set; }
        public string FECHA_EXPEDICION { get; set; }
        public string CONSTANCIA_VOTACION { get; set; }
        public string APELLIDO_CASADA { get; set; }
        public string SDIG_VERIFICACION { get; set; }
        public string SPROTEG_DATOS_IND { get; set; }
        public string SGRADO_INSTRUCCION { get; set; }
        public string COD_CUSPP { get; set; }
        public string COD_CIIU { get; set; }
        public string FOTO_RENIEC { get; set; }
        public string FIRMA_RENIEC { get; set; }
        public string SSISTEMA { get; set; }
        public string NROL { get; set; }
        public string SISSEACSA_IND { get; set; }
        public string SISCLIENT_GBD { get; set; }
        public string SISCLIENT_CRITICO { get; set; }
        public string SREGIST { get; set; }
        public string CONSUMESERV_IND { get; set; }
        public string SBAJAMAIL_IND { get; set; }
        public string SREGISTCIIU { get; set; }
        public string CHECKINFOEXT { get; set; }
        public string CHECKOPCMODF { get; set; }




        //Intermediarios y contrantes
        public string DFE_ANIVERSARIO { get; set; }

        public string SISCLIENT_CORREDOR { get; set; }
        public string SISCLIENT_CONTRATANTE { get; set; }


        public string NIND_CORREDOR { get; set; }
        public string SNAME_COMERCIAL { get; set; }
        public string NCANT_TRABAJADORES { get; set; }
        public string NNUMERO_USUARIO { get; set; }
        public string SCOD_SBS { get; set; }
        public string SIND_CONSENTI_CLAUS { get; set; }
        public string SPARTIDA_ELECTRONICA { get; set; }


        public string NID_SECTOR_EMPRE { get; set; }
        public string NID_AREA_RESPONSABLE { get; set; }
        public string DFECHA_RECEP_FACTU { get; set; }
        public string SIND_VENTA_COMERCIAL { get; set; }
        public string SLIDER_CONSORCIO { get; set; }

        //hcama@mg 03.06.2021 ini
        public string TIPO_BUSQUEDA { get; set; }
        public string VALOR_BUSQUEDA { get; set; }
        public Int64 NBRANCH { get; set; }
        public Int64 NPRODUCT { get; set; }
        public Int64 NPOLICY { get; set; }
        public Int64 NCERTIF { get; set; }
        //hcama@mg 03.06.2021 fin

        //nuevos campos
        public string SPROMOTIONS { get; set; }
        public string SDATACONSENT { get; set; }
        public string SCLIENTGOB { get; set; }
    }
}
