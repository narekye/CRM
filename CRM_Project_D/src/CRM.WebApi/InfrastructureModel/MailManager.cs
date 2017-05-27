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
    using System.Security.Cryptography;
    using System.IO;
    using System.Text;

    public class MailManager : IDisposable
    {
        //Decrypting start
        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";
        string Decrypt(string encryptedText)
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
        //Decrypting end

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
                string pwd = Decrypt(mailSettings.Smtp.Network.Password);
                string uid = Decrypt(mailSettings.Smtp.Network.UserName);

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