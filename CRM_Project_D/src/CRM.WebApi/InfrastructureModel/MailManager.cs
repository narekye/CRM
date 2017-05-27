namespace CRM.WebApi.InfrastructureModel
{
    using System;
    using System.Data.Entity;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Net;
    using System.Net.Configuration;
    using Entities;
    public class MailManager : IDisposable
    {
        private readonly CRMContext _database = new CRMContext();

        public bool SendMail(string sendto, int templateid)
        {
            // TODO: get the template and put to mail body.
            Configuration config;
            if (System.Web.HttpContext.Current != null)
                config =
                    System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            else
                config =
                    ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var mailSettings = config.GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;

            if (mailSettings == null) return false;
            try
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
                message.Subject = "CRM Project Group-D";
                message.IsBodyHtml = true;
                message.Body = "Hello!";
                var client = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    Credentials = new NetworkCredential(uid, pwd),
                    EnableSsl = true
                };

                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void SendEmailToList(List<string> list, int t)
        {
            list.ForEach(i => SendMail(i, t));
        }
        public async Task<List<string>> GetListOfEmails(List<Guid> guids)
        {
            var list = new List<string>();
            foreach (Guid guid in guids)
            {
                try
                {
                    list.Add((await _database.Contacts.FirstOrDefaultAsync(p => p.GuID == guid)).Email);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return list;
        }
        public void Dispose()
        {
            _database.Dispose();
        }
    }
}