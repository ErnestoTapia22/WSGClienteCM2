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

namespace WSGClienteCM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CargaMasivaController : ControllerBase
    {
        private readonly ICargaMasivaService _cargaMasivaService;
        public CargaMasivaController(ICargaMasivaService cargaMasivaService)
        {
            this._cargaMasivaService = cargaMasivaService;
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
            try {
                model = await this._cargaMasivaService.InsertData(request);
                if (model == null)
                {
                    return NotFound();
                }
                else {
                    return Ok(model);
                }

            }
            catch (Exception ex) {
                model.P_COD_ERR = "2";
                model.P_MESSAGE = ex.Message;
                return Ok(model);
            }
            
        }


    }
}
