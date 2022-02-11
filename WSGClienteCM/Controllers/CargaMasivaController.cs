using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WSGClienteCM.Models;
using WSGClienteCM.Services;
using WSGClienteCM.Utils;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Net.Mail;
using WSGClienteCM.Helper;
using System.Text;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace WSGClienteCM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CargaMasivaController : ControllerBase
    {
        private readonly ICargaMasivaService _cargaMasivaService;
        private readonly IHostingEnvironment _HostEnvironment;
        private readonly AppSettings _appSettings;
        public CargaMasivaController(ICargaMasivaService cargaMasivaService, IHostingEnvironment HostEnvironment, IOptions<AppSettings> appSettings)
        {
            this._cargaMasivaService = cargaMasivaService;
            _HostEnvironment = HostEnvironment;
            this._appSettings = appSettings.Value;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "ticket1", "ticket2" };
        }

        //[HttpGet("Search/GetCanalTicket")]
        //public IActionResult GetCanalTicket()
        //{
        //    var _objReturn = this._ticketService.GetCanales();
        //    if (_objReturn == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(_objReturn);
        //}

        [HttpPost("Job/InitProcess")]
        public async Task<IActionResult> InitProcess()
        {

            var _objReturn = await this._cargaMasivaService.InitProcess();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpPost("Data/Insert")]
        public async Task<IActionResult> Insert(List<ClientBindingModel> request)
        {
            ResponseViewModel model = new ResponseViewModel();
            try
            {
                model = await this._cargaMasivaService.InsertData(request);
                if (model == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(model);
                }

            }
            catch (Exception ex)
            {
                model.P_COD_ERR = "2";
                model.P_MESSAGE = ex.Message;
                return Ok(model);
            }

        }

        [HttpPost("test/Webhook")]
        public static void WriteErrorLog(string issue)
        {


            StreamWriter sw = null;


            try
            {
                sw = new StreamWriter(@"D:\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + issue);
                sw.Flush();
                sw.Close();

            }
            catch (Exception e)
            {
                WriteErrorLog(e, "writeerror mess");
            }

        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public static void WriteErrorLog(Exception ex, String datos)
        {
            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(@"D:\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim() + "; " + ex.Message.ToString().Trim());
                sw.WriteLine("en " + datos);
                sw.Flush();
                sw.Close();
            }
            catch (Exception e)
            {
                var es = ex;
            }
        }

    
        [ApiExplorerSettings(IgnoreApi = true)]
        public void SendMail(string addressFrom, string pwdFrom, string addressTo, string subject, string body, int port, List<Archivo> tramasList = null)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            Attachment attachment = null;
            try
            {
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(addressFrom, "Carga Masiva - Gestor de Clientes", Encoding.UTF8);
                // mail.To.Add(addressTo);
                mail.To.Add("ernesto.tapia@materiagris.pe");
                mail.IsBodyHtml = true;
                mail.Subject = subject;
                mail.Body = body;


                foreach (Archivo archivo in tramasList)
                {
                    MemoryStream stream1 = new MemoryStream(archivo.tramaEnviar64);
                    attachment = new Attachment(stream1, archivo.nombre, archivo.tipoMIME);
                    mail.Attachments.Add(attachment);
                }

                SmtpServer.Port = port;

                SmtpServer.Credentials = new System.Net.NetworkCredential(addressFrom, pwdFrom);
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
                attachment.Dispose();

            }
            catch (SmtpException smtpEx)
            {
                attachment.Dispose();
                throw smtpEx;
            }


        }

        [HttpGet("SendEmail")]
        public async Task<IActionResult> SendEmail(string nroproceso)
        {
            //  string P_SNOPROCESO  ="17z6I5Eo20220120113003";

            RespuestaMail respuestam = new RespuestaMail();

            ResponseViewModel _objReturn = null;
            _objReturn = new ResponseViewModel();

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
             
               
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    HttpResponseMessage response = await client.GetAsync("http://localhost:34809/api/CargaMasiva/Busqueda/ObtenerTramaEnvioExitosa/"+ nroproceso);//http://10.10.1.58/WSGClienteQACMS/api/CargaMasiva/Busqueda/ObtenerTramaEnvioExitosa/
                    string result = await response.Content.ReadAsStringAsync();
                    _objReturn = JsonConvert.DeserializeObject<ResponseViewModel>(result);
                    

                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }

            if (_objReturn == null)
            {
                return NotFound();
            }

          
            return Ok(_objReturn);
        }
        [HttpPost("ValidarEmail")]
        public IActionResult PruebaMail(ClientBindingModel request)
        {
            ResponseViewModel response = new ResponseViewModel();
            List<ListViewErrores> listErrores = new List<ListViewErrores>();
            try
            {
                EnviarEmail2();
                return Ok(response);

            }

            catch (Exception ex)
            {
                response.P_SMESSAGE = ex.Message;
                response.P_NCODE = "1";
                return Ok(response);
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        private  void EnviarEmail2()
        {
            

            //string mstr_Firma = "";

            //string mstr_Body_Excel = ConfigurationManager.AppSettings["wstr_Body_Excel"];

            ResponseViewModel responseCorreo = new ResponseViewModel();


            System.Net.Mail.Attachment archivo = null;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                //Especificamos el correo desde el que se enviará el Email y el nombre de la persona que lo envía
                mail.From = new MailAddress("clientes@protectasecurity.pe", "Prueba Email", Encoding.UTF8);
                mail.Subject = "Prueba Envio Email";

                mail.To.Add("ernesto.tapia@materiagris.pe");


                //archivo.Dispose(); 

                mail.Body = "This is for testing SMTP mail from GMAIL";
              
                SmtpServer.Port = 587; //Puerto que utiliza Gmail para sus servicios
                                        //Especificamos las credenciales con las que enviaremos el mail
                SmtpServer.Credentials = new System.Net.NetworkCredential("clientes@protectasecurity.pe", "Protecta%%");
                SmtpServer.EnableSsl = true;
                //SmtpServer.UseDefaultCredentials = true;
                SmtpServer.Send(mail);




                //return "Correcto";
            }
            catch (Exception ex)
            {

                //log.Info(string.Format("Estado Email: {0}", "No Enviado"));
                //  log.Info(string.Format("Error: {0}", ex.Message));
                throw ex;
            }
        }



    }
}
