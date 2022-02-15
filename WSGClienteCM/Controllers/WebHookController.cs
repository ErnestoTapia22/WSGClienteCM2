using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using WSGClienteCM.Models;

namespace WSGClienteCM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebHookController : Controller
    {
        [HttpPost("UpdateStatus")]
        public IActionResult UpdStatus(WebHookPayloadModel model)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt";
                StreamWriter sw = null;
                if (model == null)
                {
                    response.P_NCODE = "2";
                    response.P_MESSAGE = "No se encontró el modelo";
                    return Ok(response);
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
