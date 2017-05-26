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
    using System.Configuration;
    using System.Net.Configuration;
    using System.Net;

    public class SendEmailController : ApiController
    {
        public async Task<IHttpActionResult> PostSendTo([FromUri] int templateid, [FromBody] Guid guid)
        {
            var email = await GetContactEmail(guid);
            if (ReferenceEquals(email, null)) return NotFound();
            await SendMail(email, templateid);
            return Ok();
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
        [Route("/api/sendplz")]
        public async Task<IHttpActionResult> GetSending()
        {
            await SendMail("merisahakyan1@gmail.com", 1);
            return Ok();
        }
        private async Task SendMail(string sendto, int templateid)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var mailSettings = config.GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;

            if (mailSettings != null)
            {
                int port = mailSettings.Smtp.Network.Port;
                string from = mailSettings.Smtp.From;
                string host = mailSettings.Smtp.Network.Host;
                string pwd = mailSettings.Smtp.Network.Password;
                string uid = mailSettings.Smtp.Network.UserName;

                var message = new MailMessage
                {
                    From = new MailAddress(@from)
                };
                message.To.Add(new MailAddress(sendto));
                message.CC.Add(new MailAddress(from));
                message.Subject = "subject";
                message.IsBodyHtml = true;
                message.Body = "Hello!";

                var client = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    Credentials = new NetworkCredential(uid, pwd),
                    EnableSsl = true
                };

                try
                {
                    client.Send(message);
                }
                catch (Exception ex)
                {
                }
            }
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
