using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Web.Http;

namespace CRM.WebApi.Controllers
{
    public class SendEmailController : ApiController
    {
        public IHttpActionResult PostSendTo([FromUri] string address)
        {
            SendMail(address);
            return Ok();
        }
        public IHttpActionResult PostSendToList(List<string> addresses)
        {
            foreach (var m in addresses)
                SendMail(m);
            return Ok();
        }
        private void SendMail(string sendto)
        {
            try
            {
                MailMessage mailMsg = new MailMessage();

                // To
                mailMsg.To.Add(new MailAddress(sendto));

                // From
                mailMsg.From = new MailAddress("forproj@ms.com");

                // Subject and multipart/alternative Body
                mailMsg.Subject = "subject";
                string text = "text body";
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));

                // Init SmtpClient and send
                SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("sahakyan_meri", "p42Zmx39");
                smtpClient.Credentials = credentials;

                smtpClient.Send(mailMsg);
            }
            catch
            {

            }

        }
    }
}
