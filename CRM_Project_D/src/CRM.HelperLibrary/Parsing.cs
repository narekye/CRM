namespace CRM.HelperLibrary
{
    using System;
    using System.Collections.Generic;
    using Entities;
    public class Parsing
    {
        public List<Contact> GetContactsFromFile(byte[] bytes, string path)
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
                catch
                {

                }
            }
            return contactslist;
        }
    }
}
