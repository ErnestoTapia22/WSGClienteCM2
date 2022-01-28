using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using WSGClienteCM.Models;

namespace WSGClienteCM.Helper
{
    public class NotifyHelper
    {
        public Archivo ComposeExcelErrores(string contentRootPath, List<ClientBindingModel> trama)
        {
            Archivo objArchivo = new Archivo();
            objArchivo.nombre = "ListadoClientesErrores.xls";
            objArchivo.tipoMIME= "application/vnd.ms-excel";
            //try
            //{
   
                string tdi = "<td style='border:1px solid black !important;text-align:center;vertical-align:middle;' >";
                string tdfi = "</td>";
                string htmlBodyTrama = string.Empty;

                foreach (ClientBindingModel rr in trama)
                {
                    htmlBodyTrama += "<tr>";
                    htmlBodyTrama += tdi + rr.P_SNOPROCESO + tdfi;
                    htmlBodyTrama += tdi + rr.P_NNUMREG + tdfi;
                    htmlBodyTrama += tdi + rr.P_SFILENAME + tdfi;
                    htmlBodyTrama += tdi + rr.P_SCOLUMNNAME + tdfi;
                    htmlBodyTrama += tdi + rr.P_SCOLUMNVALUE + tdfi;
                    htmlBodyTrama += tdi + rr.P_SERRORVALUE + tdfi;
                    htmlBodyTrama += tdi + rr.P_NUSERNAME + tdfi;
                    htmlBodyTrama += "</tr>";
                }           

                string path_trama;
                string htmlTrama;

                 path_trama = Path.Combine(contentRootPath, @"Templates\TramaErrores.html");                
                htmlTrama = System.IO.File.ReadAllText(path_trama);              
                htmlTrama = htmlTrama.Replace("[TramaError]", htmlBodyTrama);
                objArchivo.tramaEnviar64 = System.Text.Encoding.UTF8.GetBytes(htmlTrama);
             
            //}
            //catch (Exception ex)
            //{
              
            //}

            return objArchivo;
        }            
        public Archivo ComposeExcelExitoso(string contentRootPath, List<ClientBindingModel> trama)
        {
            Archivo objArchivo = new Archivo();
            objArchivo.nombre = "ListadoClientesExitoso.xls";
            objArchivo.tipoMIME= "application/vnd.ms-excel";
            //try
            //{
   
                string tdi = "<td style='border:1px solid black !important;text-align:center;vertical-align:middle;' >";
                string tdfi = "</td>";
                string htmlBodyTrama = string.Empty;

                foreach (ClientBindingModel rr in trama)
                {
                    htmlBodyTrama += "<tr>";
                    htmlBodyTrama += tdi + rr.P_SNOPROCESO + tdfi;
                    htmlBodyTrama += tdi + rr.P_NNUMREG + tdfi;
                    htmlBodyTrama += tdi + rr.P_SFILENAME + tdfi;  
                
                    htmlBodyTrama += tdi + rr.P_NIDDOC_TYPE + tdfi;                  
                    htmlBodyTrama += tdi + rr.P_SIDDOC + tdfi;                  
                    htmlBodyTrama += tdi + rr.P_SFIRSTNAME + tdfi;                  
                    htmlBodyTrama += tdi + rr.P_SLASTNAME + tdfi;                  
                    htmlBodyTrama += tdi + rr.P_SLASTNAME2 + tdfi;                  
                    htmlBodyTrama += tdi + rr.P_SLEGALNAME + tdfi;                  
                    htmlBodyTrama += tdi + rr.P_SSEXCLIEN + tdfi;                  
                    htmlBodyTrama += tdi + rr.P_NCIVILSTA + tdfi;                  
                    htmlBodyTrama += tdi + rr.P_NNATIONALITY + tdfi;                  
                    htmlBodyTrama += tdi + rr.P_DBIRTHDAT + tdfi;    
                
                    htmlBodyTrama += tdi + rr.EListAddresClient[0].P_ADDRESSTYPE + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListAddresClient[0].P_STI_DIRE + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListAddresClient[0].P_SNOM_DIRECCION + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListAddresClient[0].P_SNUM_DIRECCION + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListAddresClient[0].P_STI_BLOCKCHALET + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListAddresClient[0].P_SBLOCKCHALET + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListAddresClient[0].P_STI_INTERIOR + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListAddresClient[0].P_SNUM_INTERIOR + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListAddresClient[0].P_STI_CJHT + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListAddresClient[0].P_SNOM_CJHT + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListAddresClient[0].P_SETAPA + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListAddresClient[0].P_SMANZANA + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListAddresClient[0].P_SLOTE + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListAddresClient[0].P_SREFERENCIA + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListAddresClient[0].P_NMUNICIPALITY + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListAddresClient[0].P_NCOUNTRY + tdfi;                  
                         
                    htmlBodyTrama += tdi + rr.EListPhoneClient[0].P_NAREA_CODE + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListPhoneClient[0].P_NPHONE_TYPE + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListPhoneClient[0].P_SPHONE + tdfi;   
                
                    htmlBodyTrama += tdi + rr.EListEmailClient[0].P_SEMAILTYPE + tdfi;                  
                    htmlBodyTrama += tdi + rr.EListEmailClient[0].P_SE_MAIL + tdfi;   
                
                    htmlBodyTrama += tdi + rr.P_COD_CIIU + tdfi;                  
                    htmlBodyTrama += tdi + rr.P_COD_CUSPP + tdfi;                  
                  //  htmlBodyTrama += tdi + rr.P_SISCLIENT_IND + tdfi;                  
                    htmlBodyTrama += tdi + rr.P_SBAJAMAIL_IND + tdfi;                  
                    htmlBodyTrama += tdi + rr.P_SISCLIENT_GBD + tdfi;                  
                  //  htmlBodyTrama += tdi + rr.P_SPROMOTIONS + tdfi;                  
                  //  htmlBodyTrama += tdi + rr.P_SDATACONSENT + tdfi;                  
                  //  htmlBodyTrama += tdi + rr.P_SCLIENTGOB + tdfi;                  
                    htmlBodyTrama += "</tr>";
                }           

                string path_trama;
                string htmlTrama;

                path_trama = Path.Combine(contentRootPath, @"Templates\TramaExitosa.html");                
                htmlTrama = System.IO.File.ReadAllText(path_trama);              
                htmlTrama = htmlTrama.Replace("[TramaExitosa]", htmlBodyTrama);
                objArchivo.tramaEnviar64 = System.Text.Encoding.UTF8.GetBytes(htmlTrama);
             
            //}
            //catch (Exception ex)
            //{
              
            //}

            return objArchivo;
        }

        public void SendMail(string addressFrom,  string pwdFrom, string addressTo, string subject, string body, List<Archivo> tramasList = null)
        { 
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");    

            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(addressFrom);   
                mail.To.Add(addressTo);
                mail.IsBodyHtml = true;
                mail.Subject = subject;
                mail.Body = body;
                System.Net.Mail.Attachment attachment;

                foreach (Archivo archivo in tramasList)
                {
                    MemoryStream stream1 = new MemoryStream(archivo.tramaEnviar64);
                    attachment = new System.Net.Mail.Attachment(stream1, archivo.nombre, archivo.tipoMIME);
                    mail.Attachments.Add(attachment);
                }

                SmtpServer.Port = 587;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(addressFrom, pwdFrom);
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
            }         
        }

    }
}
