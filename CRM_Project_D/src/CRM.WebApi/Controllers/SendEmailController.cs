using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CRM.WebApi.Controllers
{
    public class SendEmailController : ApiController
    {
        public IHttpActionResult PostSendTo([FromUri] string address)
        {
            SendMail(address);
            return Ok("sended");
        }
        private void SendMail(string sendto)
        {
            try
            {
                var client = new SendGridClient("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");
                var myMessage = new SendGridMessage();
                myMessage.AddTo(sendto);
                myMessage.From = new EmailAddress("mssend@ms.com");
                myMessage.Subject = "subject";
                myMessage.PlainTextContent = "body";
                var response = client.SendEmailAsync(myMessage);
            }
            catch
            {

            }
        }
    }
}
