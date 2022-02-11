using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WSGClienteCM.Controllers
{
    public class WebHookController : Controller
    {
        [HttpPost("UpdateStatus")]
        public static void WriteErrorLog(string issue)
        {
            try
            {
           

            }
            catch (Exception e)
            {
               
            }

        }
    }
}
