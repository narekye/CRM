namespace CRM.WebApi.InfrastructureModel
{
    using System;
    using System.Collections.Generic;
    using Entities;
    using System.IO;
    using System.Linq;
    using LinqToExcel;
    public class ParserProvider
    {
        public static List<Contact> GetContactsFromFile(byte[] bytes, string path)
        {
            List<Contact> contactslist = new List<Contact>();
            System.IO.File.WriteAllBytes(path, bytes);
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(path);
            Microsoft.Office.Interop.Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Microsoft.Office.Interop.Excel.Range xlRange = xlWorksheet.UsedRange;

            for (int i = 1; i <= xlWorksheet.Rows.Count; i++)
            {
                Contact contact = new Contact();
                try
                {
                    if (xlRange.Cells[i, 1].Value2 != null && xlRange.Cells[i, 1] != null)
                    {
                        for (int j = 1; j <= xlWorksheet.Columns.Count; j++)
                        {
                            if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                            {
                                switch (j)
                                {
                                    case 1:
                                        contact.FullName = xlRange.Cells[i, j].Value2.ToString();
                                        break;
                                    case 2:
                                        contact.CompanyName = xlRange.Cells[i, j].Value2.ToString();
                                        break;
                                    case 3:
                                        contact.Position = xlRange.Cells[i, j].Value2.ToString();
                                        break;
                                    case 4:
                                        contact.Country = xlRange.Cells[i, j].Value2.ToString();
                                        break;
                                    case 5:
                                        contact.Email = xlRange.Cells[i, j].Value2.ToString();
                                        break;
                                    case 6:
                                        contact.DateInserted = (DateTime)xlRange.Cells[i, j].Value2.ToString();
                                        break;
                                }
                            }
                            else
                                break;
                        }
                        contact.GuID = Guid.NewGuid();
                        contactslist.Add(contact);
                    }
                    else
                        break;
                }
                catch (Exception ex)
                {
                    System.IO.File.Delete(path);
                    throw new Exception(ex.Message);
                }
                finally
                {
                    System.IO.File.Delete(path);
                }
            }
            return contactslist;
        }
        public static List<Contact> GetContactsFromFile(byte[] bytes)
        {
            var contact = new Contact();
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
                    contact = new Contact
                    {
                        FullName = m["fullname"],
                        CompanyName = m["company"],
                        Country = m["country"],
                        Position = m["position"],
                        Email = m["email"]
                    };
                    contactsList.Add(contact);
                }
            }
            catch (Exception ex)
            {
                File.Delete(path);
                throw new Exception(ex.Message);
            }
            finally
            {
                File.Delete(path);
            }
            return contactsList;
        }
    }
}