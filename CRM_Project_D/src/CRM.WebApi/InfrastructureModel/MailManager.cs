namespace CRM.WebApi.InfrastructureModel
{
    using System;
    using System.Data.Entity;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Entities;

    public class MailManager : IDisposable
    {
        private static readonly CRMContext Database = new CRMContext();
        public async Task<bool> SendMail(string sendto, int templateid)
        {
            bool t = true;
            try
            {
                Template template = await Database.Templates.FirstOrDefaultAsync(p => p.TemplateId == templateid);
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
            catch
            {
                t = false;
            }
            return t;
        }
        public void SendEmailToList(List<Contact> list, int t)
        {
            list.ForEach(async i => await SendMail(i.Email, t));
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}