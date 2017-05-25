using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Linq;
using CRM.Entities;
using System.IO;

namespace CRM.WebApi.InfrastructureModel
{
    public class ApplicationManager
    {
        static Contact c;
        public static List<Contact> GetContactsFromFile(byte[] bytes)
        {
            List<Contact> contactsList = new List<Contact>();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\newfile.xlsx";
            try
            {
                File.WriteAllBytes(path, bytes);
                var excel = new ExcelQueryFactory(path);
                var contacts = from c in excel.Worksheet<Row>("Sheet1")
                               select c;

                foreach (var m in contacts)
                {
                    c = new Contact();
                    c.FullName = m["fullname"];
                    c.CompanyName = m["company"];
                    c.Country = m["country"];
                    c.Position = m["position"];
                    c.Email = m["email"];
                    c.DateInserted = Convert.ToDateTime(m["datainserted"]);
                    c.GuID = Guid.NewGuid();
                    contactsList.Add(c);
                }
            }
            catch
            {

            }
            finally
            {
                File.Delete(path);
            }
            if (contactsList == null) return null;
            return contactsList;
        }
    }
}