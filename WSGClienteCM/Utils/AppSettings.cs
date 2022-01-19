using System;

namespace WSGClienteCM.Utils
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string ConnectionStringORA { get; set; }
        public string ConnectionStringTimeP { get; set; }
        public string ConnectionStringConciliacion { get; set; }
        public static String[] CampoObligatorio { get { return Util.ObtainConfig("CAMPO_OBLIGATORIO"); } }
        public static String[] TipoDatoNumerico { get { return Util.ObtainConfig("TIPO_DATO_NO_NUMERICO"); } }
        public static String[] EstructuraSinDatos { get { return Util.ObtainConfig("ESTRUCTURA_SIN_DATOS"); } }
        public static String[] OperacionNoEnviada { get { return Util.ObtainConfig("OPERACION_NO_ENVIADA"); } }
        public static String[] CampoObligatorioSub { get { return Util.ObtainConfig("CAMPO_OBLIGATORIO_SUB"); } }
        public static string AWSRegistrar { get { return Util.ObtainConfigAWS("SERVICIOAWS_REGISTRAR"); } }
        public static string AWSRegistrar2 { get { return Util.ObtainConfigAWS("SERVICIOAWS_REGISTRAR2"); } }
        public static string AWSKey { get { return Util.ObtainConfigAWS("SERVICIOAWS_KEY"); } }
        public static string AWSConsultar { get { return Util.ObtainConfigAWS("SERVICIOAWS_CONSULTA"); } }
        public static string AWSConsultar2 { get { return Util.ObtainConfigAWS("SERVICIOAWS_CONSULTA2"); } }
        public static string AWSAdjuntar { get { return Util.ObtainConfigAWS("SERVICIOAWS_ADJUNTAR"); } }
        public static string AWSGetAdjunto { get { return Util.ObtainConfigAWS("SERVICIOAWS_GETADJUNTO"); } }
        public static string GetTokenAwsSGS { get { return Util.ObtainConfigAWS("URL_GET_TOKEN_SGC"); } }
        public static string GetTokenAws360 { get { return Util.ObtainConfigAWS("URL_GET_TOKEN_360"); } }
        public static string GetUserName_SGC { get { return Util.ObtainConfigAWS("USERNAME_SGC"); } }
        public static string GetUserName_360 { get { return Util.ObtainConfigAWS("USERNAME_360"); } }
        public static string GetPassword_SGC { get { return Util.ObtainConfigAWS("PASSWORD_SGC"); } }
        public static string GetPassword_360 { get { return Util.ObtainConfigAWS("PASSWORD_360"); } }
        public static string GetScope { get { return Util.ObtainConfigAWS("SCOPE"); } }

    }
}