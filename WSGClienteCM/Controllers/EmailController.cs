using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WSGClienteCM.Helper;
using WSGClienteCM.Models;
using WSGClienteCM.Utils;

namespace WSGClienteCM.Controllers
{
    public class EmailController : Controller
    {
        private readonly IHostingEnvironment _HostEnvironment;
        private readonly AppSettings _appSettings;
        public EmailController( IHostingEnvironment HostEnvironment, IOptions<AppSettings> appSettings)
        {
          
            _HostEnvironment = HostEnvironment;
            this._appSettings = appSettings.Value;
        }
        [HttpPost("Busqueda/ObtenerTramaEnvioExitosa/")]
        public IActionResult SendEmail(RespuestaMail postobj)
        {
        
            ResponseViewModel _objReturn = new ResponseViewModel();

            string contentRootPath = _HostEnvironment.ContentRootPath;
            string path_CuerpoCorreo = Path.Combine(contentRootPath, @"Templates\CorreoTramaCargaMasiva01.html");
            string htmlCorreo = System.IO.File.ReadAllText(path_CuerpoCorreo);
            string addressFrom = _appSettings.EmailFrom;
            string pwdFrom = _appSettings.PassWordFrom;

            string addressTo;
            string subject = "Detalle  Carga  Masiva - Cliente  360";
           
            try
            {
                foreach (EmailViewModel email in postobj.correos.correoUsuarios)
                {
                    addressTo = email.P_SE_MAIL;
                   // objNotifyHelper.SendMail(addressFrom, pwdFrom, addressTo, subject, htmlCorreo, Convert.ToInt32(_appSettings.PortEmail), postobj.tramaslist);
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                    Attachment attachment = null;
                   
                        MailMessage mail = new MailMessage();

                        mail.From = new MailAddress(addressFrom, "Carga Masiva - Gestor de Clientes", Encoding.UTF8);
                        mail.To.Add(addressTo);
                        //mail.To.Add("ernesto.tapia@materiagris.pe");
                        mail.IsBodyHtml = true;
                        mail.Subject = subject;
                        mail.Body = htmlCorreo;


                        foreach (Archivo archivo in postobj.tramaslist)
                        {
                            MemoryStream stream1 = new MemoryStream(archivo.tramaEnviar64);
                            attachment = new Attachment(stream1, archivo.nombre, archivo.tipoMIME);
                            mail.Attachments.Add(attachment);
                        }

                        SmtpServer.Port = Convert.ToInt32(_appSettings.PortEmail);

                        SmtpServer.Credentials = new System.Net.NetworkCredential(addressFrom, pwdFrom);
                        SmtpServer.EnableSsl = true;
                        SmtpServer.Send(mail);
                        if (attachment != null)
                        {
                            attachment.Dispose();
                        }
                }

                if (_objReturn == null)
                {
                    return NotFound();
                }
            }
            catch (SmtpException smtpEx)
            {
                _objReturn.P_NCODE = "2";
                _objReturn.P_SMESSAGE = smtpEx.Message;
                return Ok(_objReturn);
            }
            catch (Exception ex)
            {
                _objReturn.P_NCODE = "2";
                _objReturn.P_SMESSAGE = ex.Message;
                return Ok(_objReturn);
            }

            _objReturn.P_NCODE = "0";
            _objReturn.P_SMESSAGE = "Se notificaron las tramas  con �xito";
            return Ok(_objReturn);
        }
    }
}
