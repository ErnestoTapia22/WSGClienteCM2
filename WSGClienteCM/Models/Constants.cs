namespace WSGClienteCM.Models
{
    public static class Constants
    {
        public const string MsgUnauthenticate = "{ \"message\":\"No se encuentra autenticado o su sesión a expirado.\" }";
        public const string MsgUnauthorize = "{ \"message\":\"No tiene autorización para acceder al recurso solicitado.\" }";
        public const string MsgLoginValid = "Usuario y/o contraseña no ingresado.";
        public const string MsgLoginFail = "El usuario o la contraseña es incorrecto.";
        public const string MsgLoginFailConfirmed = "Se le ha enviado un correo de verificación al email registrado, por favor valídelo para poder ingresar al sistema";
        public const string MsgRegisterFail = "El email ingresado ya está registrado en el sistema, intente con otro email o Inicie sesión con su email y password registrados dandole click al botón Iniciar Sesión";
        public const string MsgGetError = "Ha ocurrido un error obteniendo la información.";
        public const string MsgPostError = "Ha ocurrido un error al intentar registrar la información.";
        public const string MsgPutError = "Ha ocurrido un error al intentar actualizar la información.";
        public const string MsgDeleteError = "Ha ocurrido un error al eliminar.";
        public const string MsgDataNotFound = "No hay información para mostrar.";
        public const string MsgCodeCompany = "Error al guardar, ya existe una compañia con el mismo código.";
        public const string MsgTokenInvalidOrExpired = "Error al cambiar contraseña, el link es inválido o esta expirado";
    }
}
