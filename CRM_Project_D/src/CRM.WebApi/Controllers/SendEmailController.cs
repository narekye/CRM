namespace CRM.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Web.Http;
    using Entities;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public class SendEmailController : ApiController
    {
        public async Task<IHttpActionResult> PostSendTo([FromUri] int templateid, [FromBody] Guid guid)
        {
            var email = await GetContactEmail(guid);
            if (ReferenceEquals(email, null)) return NotFound();
            if (await SendMail(email, templateid)) return Ok();
            return BadRequest();
        }
        [Route("api/sendemail/list")]
        public async Task<IHttpActionResult> PostSendToList([FromUri] int templateid, [FromBody] List<Guid> guids)
        {
            var list = new List<string>();
            foreach (Guid guid in guids)
                list.Add(await GetContactEmail(guid));
            SendEmailToList(list, templateid);
            return Ok();
        }
        private async Task<bool> SendMail(string sendto, int templateid)
        {
            bool t = true;
            try
            {
                using (var database = new CRMContext())
                {
                    Template template = await database.Templates.FirstOrDefaultAsync(p => p.TemplateId == templateid);
                    if (ReferenceEquals(template, null)) return false;
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
        // helper
        private async Task<string> GetContactEmail(Guid guid)
        {
            using (var database = new CRMContext())
            {
                var data = await database.Contacts.FirstOrDefaultAsync(p => p.GuID == guid);
                if (ReferenceEquals(data, null)) return null;
                return data.Email;
            }
        }

        private void SendEmailToList(List<string> list, int t)
        {
            list.ForEach(async i => await SendMail(i, t));
        }
    }
}
