namespace CRM.WebApi.InfrastructureModel
{
    using System.Linq;
    using System;
    using System.Data.Entity;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Net;
    using System.Net.Configuration;
    using Entities;
    using System.Security.Cryptography;
    using System.IO;
    using System.Text;
    public class MailManager : IDisposable
    {
        private readonly CRMContext _database = new CRMContext();
        private static readonly string PasswordHash = "P@@Sw0rd";
        private static readonly string SaltKey = "S@LT&KEY";
        private static readonly string VIKey = "@1B2c3D4e5F6g7H8";
        private string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
        public async Task<bool> SendMail(Contact sendto, int templateid)
        {
            var template = await _database.Templates.FirstOrDefaultAsync(p => p.TemplateId == templateid);
            var getpath = template.TemplatePath;
            Configuration config = System.Web.HttpContext.Current != null ?
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~") :
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var mailSettings = config.GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;

            if (mailSettings == null) return false;
            try
            {
                int port = mailSettings.Smtp.Network.Port;
                string from = mailSettings.Smtp.From;
                string host = mailSettings.Smtp.Network.Host;
                string pwd = Decrypt(mailSettings.Smtp.Network.Password);
                string uid = Decrypt(mailSettings.Smtp.Network.UserName);
                var message = new MailMessage
                {
                    From = new MailAddress(@from)
                };
                string path = System.Web.HttpContext.Current?.Request.MapPath(getpath);
                if (ReferenceEquals(path, null)) throw new ArgumentNullException(nameof(path));
                var html = File.ReadAllText(path);
                var send = html.Replace("{yourname}", sendto.FullName);
                message.To.Add(new MailAddress(sendto.Email));
                message.CC.Add(new MailAddress(from));
                message.Subject = "BetConstruct CRM project team D";
                message.IsBodyHtml = true;
                message.Body = send;
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
            catch
            {
                throw;
            }
        }
        public async Task SendEmailToList(List<Contact> list, int t)
        {
            foreach (Contact contact in list)
                await SendMail(contact, t);
        }
        public async Task SendEmailToEmailList(int id, int template)
        {
            var emaillist = await _database.EmailLists.FirstOrDefaultAsync(p => p.EmailListID == id);
            await SendEmailToList(emaillist.Contacts.ToList(), template);
        }
        public async Task<List<Contact>> GetListOfEmailsByGuids(List<Guid> guids)
        {
            var list = new List<Contact>();
            foreach (Guid guid in guids)
            {
                try
                {
                    list.Add(await _database.Contacts.FirstOrDefaultAsync(p => p.GuID == guid));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return list;
        }
        public async Task<bool> SendConfirmationEmail(string whom = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(whom) || string.IsNullOrWhiteSpace(code)) return false;
            code = $"<a href={code}>Confirm your account</a>";

            Configuration config = System.Web.HttpContext.Current != null ?
               System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~") :
               ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            await Task.WhenAll();
            var mailSettings = config.GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;

            if (mailSettings == null) return false;
            try
            {
                int port = mailSettings.Smtp.Network.Port;
                string from = mailSettings.Smtp.From;
                string host = mailSettings.Smtp.Network.Host;
                string pwd = Decrypt(mailSettings.Smtp.Network.Password);
                string uid = Decrypt(mailSettings.Smtp.Network.UserName);
                var message = new MailMessage
                {
                    From = new MailAddress(@from)
                };
                string path = System.Web.HttpContext.Current?.Request.MapPath("~//Templates//simple.html");
                if (ReferenceEquals(path, null)) throw new ArgumentNullException(nameof(path));
                var html = File.ReadAllText(path);
                var send = html.Replace("{link}", code);
                message.To.Add(new MailAddress(whom));
                message.CC.Add(new MailAddress(from));
                message.Subject = "Account confirmation.";
                message.IsBodyHtml = true;
                message.Body = send;
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
            catch
            {
                throw;
            }

        }
        public void Dispose()
        {
            _database.Dispose();
        }
    }
}