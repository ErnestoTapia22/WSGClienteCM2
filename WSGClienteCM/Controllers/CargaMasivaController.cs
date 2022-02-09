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

namespace WSGClienteCM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CargaMasivaController : ControllerBase
    {
        private readonly ICargaMasivaService _cargaMasivaService;
        private readonly IHostingEnvironment _HostEnvironment;
        public CargaMasivaController(ICargaMasivaService cargaMasivaService, IHostingEnvironment HostEnvironment)
        {
            this._cargaMasivaService = cargaMasivaService;
            _HostEnvironment = HostEnvironment;
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

        [HttpGet("Busqueda/ObtenerTramaEnvioExitosa/{request}")]
        public async Task<IActionResult> ObtenerTramaEnvioExitosa(string request)
        {
            //  string P_SNOPROCESO  ="17z6I5Eo20220120113003";
            

            ResponseViewModel _objReturn = null;
            _objReturn = new ResponseViewModel();

            TramaRespuestaCargaMasivaResponse tramaExistosa;
            TramaRespuestaCargaMasivaResponse tramaError;
            TramaRespuestaCargaMasivaResponse correoUsuarios;

            List<Archivo> tramasList = new List<Archivo>();

            _objReturn = await _cargaMasivaService.SendEmails(request);





            if (_objReturn == null)
            {
                return NotFound();
            }

            _objReturn.P_NCODE = "0";
            _objReturn.P_SMESSAGE = "Se notificaron las tramas  con �xito";
            return Ok(_objReturn);
        }




    }
}
