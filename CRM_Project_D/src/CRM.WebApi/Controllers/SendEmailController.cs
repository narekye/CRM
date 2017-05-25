namespace CRM.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Web.Http;
    using Entities;
    using System.Linq;

    public class SendEmailController : ApiController
    {
        private readonly CRMContext _database = new CRMContext();
        private Template template = new Template();
        public IHttpActionResult PostSendTo(int templateid, [FromBody] Contact contact)
        {
            if (SendMail(contact.Email, templateid))
                return Ok(templateid);
            else
                return BadRequest();
        }
        public IHttpActionResult PostSendToList(int templateid, [FromBody] List<Contact> contacts)
        {
            foreach (var m in contacts)
                SendMail(m.Email, templateid);
            return Ok();
        }
        private bool SendMail(string sendto, int templateid)
        {
            bool t = true;
            try
            {
                template = _database.Templates.FirstOrDefault(p => p.TemplateId == templateid);
                if (!ReferenceEquals(template, null))
                {
                    t = true;
                    MailMessage mailMsg = new MailMessage();
                    mailMsg.To.Add(new MailAddress(sendto));
                    mailMsg.From = new MailAddress("forproj@ms.com");
                    mailMsg.Subject = "subject";
                    mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString("put template's text", null, MediaTypeNames.Text.Plain));
                    mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString("put template's text", null, MediaTypeNames.Text.Html));
                    SmtpClient smtpClient = new SmtpClient();
                    //System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("sahakyan_meri", "p42Zmx39");
                    //smtpClient.Credentials = credentials;
                    smtpClient.Send(mailMsg);
                }
            }
            catch
            {
                t = false;
            }
            return t;
        }
    }
}
