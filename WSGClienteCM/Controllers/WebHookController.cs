using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using WSGClienteCM.Models;
using WSGClienteCM.Services;

namespace WSGClienteCM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebHookController : Controller

    {
        private readonly ICargaMasivaService _cargaMasivaService;
        public WebHookController(ICargaMasivaService cargaMasivaService)
        {
            _cargaMasivaService = cargaMasivaService;
        }
        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdStatus(WebHookPayloadModel model)
        {
            ResponseViewModel response = new ResponseViewModel();
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt";
            StreamWriter sw = null;
            sw = new StreamWriter(filepath, true);
            try
            {

                if (model == null)
                {
                    response.P_NCODE = "2";
                    response.P_MESSAGE = "No se encontró el modelo";
                    return Ok(response);
                }
                else
                {
                    //DEV CY --INI
                    //response = await _cargaMasivaService.updateJiraState(model);
                    response = await _cargaMasivaService.updateJiraTicketState(model);
                    //DEV CY --FIN
                    sw.WriteLine(DateTime.Now.ToString() + ": " + JsonConvert.SerializeObject(response));

                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "2";
                response.P_MESSAGE = ex.Message;
                sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Message);
                return Ok(response);
            }
            finally
            {
                sw.Flush();
                sw.Close();
            }

        }
        [HttpPost("UpdateStatusSGC")]
        public async Task<IActionResult> UpdStatusSGC(WebHookPayloadModel model)
        {
            ResponseViewModel response = new ResponseViewModel();
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt";
            StreamWriter sw = null;
            sw = new StreamWriter(filepath, true);
            try
            {

                if (model == null)
                {
                    response.P_NCODE = "2";
                    response.P_MESSAGE = "No se encontró el modelo";
                    return Ok(response);
                }
                else
                {
                    response = await _cargaMasivaService.updateJiraStateSGC(model);
                    sw.WriteLine(DateTime.Now.ToString() + ": " + JsonConvert.SerializeObject(response));

                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "2";
                response.P_MESSAGE = ex.Message;
                sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Message);
                return Ok(response);
            }
            finally
            {
                sw.Flush();
                sw.Close();
            }

        }


        [HttpPost("UpdateStatus1")]
        public IActionResult UpdStatus(object model)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt";
                StreamWriter sw = null;
                if (model == null)
                {
                    return NotFound();
                }
                else
                {
                    sw = new StreamWriter(filepath, true);
                    sw.WriteLine(DateTime.Now.ToString() + ": " + JsonConvert.SerializeObject(model));
                    sw.Flush();
                    sw.Close();
                    return Ok(model);
                }

            }
            catch (Exception ex)
            {
                response.P_COD_ERR = "2";
                response.P_MESSAGE = ex.Message;
                return Ok(response);
            }

        }


    }
}
