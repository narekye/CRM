namespace CRM.WebApi.InfrastructureModel
{
    using System;
    using System.Collections.Generic;
    using Entities;
    using System.IO;
    using System.Linq;
    using LinqToExcel;
    using System.Runtime.InteropServices;

    public class ParserProvider
    {
        private static int MimeSampleSize = 256;

        private static string DefaultMimeType = "application/octet-stream";

        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private static extern uint FindMimeFromData(
            uint pBc,
            [MarshalAs(UnmanagedType.LPStr)] string pwzUrl,
            [MarshalAs(UnmanagedType.LPArray)] byte[] bytes,
            uint cbSize,
            [MarshalAs(UnmanagedType.LPStr)] string pwzMimeProposed,
            uint dwMimeFlags,
            out uint mimetype,
            uint dwReserverd
        );
        private string GetMimeFromBytes(byte[] data)
        {
            try
            {
                uint mimeType;
                FindMimeFromData(0, null, data, (uint)MimeSampleSize, null, 0, out mimeType, 0);

                var mimePointer = new IntPtr(mimeType);
                var mime = Marshal.PtrToStringUni(mimePointer);
                Marshal.FreeCoTaskMem(mimePointer);

                return mime ?? DefaultMimeType;
            }
            catch
            {
                return DefaultMimeType;
            }
        }
        private static bool Exist(List<string> list, string value, out int index)
        {
            bool t = false;
            index = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ToLower().Contains(value))
                {
                    t = true;
                    index = i;
                    break;
                }
            }
            return t;
        }
        private List<Contact> ReadFromExcel(byte[] bytes)
        {
            List<Contact> contactslist = new List<Contact>();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "file.xlsx";
            try
            {
                int index = -1, index1 = -1, index2 = -1;
                File.WriteAllBytes(path, bytes);
                ExcelQueryFactory excel = new ExcelQueryFactory(path);
                var sheets = excel.GetWorksheetNames();
                var contacts = (from c in excel.Worksheet<Row>(sheets.First())
                                select c).ToList();
                List<string> columns = new List<string>();
                var worksheetcolumns = excel.GetColumnNames(sheets.First()).ToList();
                if (worksheetcolumns.Count < 6)
                    return null;
                if (worksheetcolumns.Count < 6
                    || (!Exist(worksheetcolumns, "fullname", out index)
                    && !Exist(worksheetcolumns, "full name", out index))
                    || (!Exist(worksheetcolumns, "company", out index)
                    && !Exist(worksheetcolumns, "company name", out index)
                    && !Exist(worksheetcolumns, "companyname", out index))
                    || !Exist(worksheetcolumns, "position", out index)
                    || !Exist(worksheetcolumns, "country", out index)
                    || (!Exist(worksheetcolumns, "email", out index)
                    && !Exist(worksheetcolumns, "mail", out index))
                    || (!Exist(worksheetcolumns, "data inserted", out index)
                    && !Exist(worksheetcolumns, "datainserted", out index)))
                {
                    return null;
                }
                if (Exist(worksheetcolumns, "fullname", out index)
                    || Exist(worksheetcolumns, "full name", out index1))
                {
                    if (index1 == -1)
                        columns.Add(worksheetcolumns[index]);
                    else
                        columns.Add(worksheetcolumns[index1]);
                }
                if (Exist(worksheetcolumns, "company", out index)
                    || Exist(worksheetcolumns, "companyname", out index1)
                    || Exist(worksheetcolumns, "company name", out index2))
                {
                    if (index1 == -1 && index2 == -1)
                        columns.Add(worksheetcolumns[index]);
                    else if (index1 == -1)
                        columns.Add(worksheetcolumns[index2]);
                    else
                        columns.Add(worksheetcolumns[index1]);
                }
                if (Exist(worksheetcolumns, "position", out index))
                    columns.Add(worksheetcolumns[index]);
                if (Exist(worksheetcolumns, "country", out index))
                    columns.Add(worksheetcolumns[index]);
                if (Exist(worksheetcolumns, "email", out index)
                    || Exist(worksheetcolumns, "mail", out index1))
                {
                    if (index1 == -1)
                        columns.Add(worksheetcolumns[index]);
                    else
                        columns.Add(worksheetcolumns[index1]);
                }
                if (Exist(worksheetcolumns, "data inserted", out index)
                    || Exist(worksheetcolumns, "datainserted", out index1))
                {
                    if (index1 == -1)
                        columns.Add(worksheetcolumns[index]);
                    else
                        columns.Add(worksheetcolumns[index1]);
                }


                foreach (var m in contacts)
                {
                    Contact c = new Contact();
                    c.FullName = m[columns[0]];
                    c.CompanyName = m[columns[1]];
                    c.Country = m[columns[2]];
                    c.Position = m[columns[3]];
                    c.Email = m[columns[4]];
                    c.DateInserted = Convert.ToDateTime(m[columns[5]]);
                    c.GuID = Guid.NewGuid();
                    contactslist.Add(c);
                }
                File.Delete(path);
            }
            catch
            {
                File.Delete(path);
            }
            return contactslist;
        }

        private List<Contact> ReadFromCsv(byte[] bytes)
        {
            List<Contact> contactslist = new List<Contact>();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "file.csv";
            try
            {
                File.WriteAllBytes(path, bytes);
                string[] lines = File.ReadAllLines(path);
                string[] columns = lines[0].Split(',');
                Dictionary<string, int> d = new Dictionary<string, int>();
                d.Add("FullName", 0);
                d.Add("Company", 1);
                d.Add("Position", 2);
                d.Add("Country", 3);
                d.Add("Email", 4);
                d.Add("DataInserted", 5);

                for (int i = 1; i < lines.Length; i++)
                {
                    Contact contact = new Contact();
                    string[] values = lines[i].Split(',');
                    for (int j = 0; j < values.Length; j++)
                    {
                        switch (j)
                        {
                            case 0:
                                contact.FullName = values[d["FullName"]];
                                break;
                            case 1:
                                contact.CompanyName = values[d["Company"]];
                                break;
                            case 2:
                                contact.Position = values[d["Position"]];
                                break;
                            case 3:
                                contact.Country = values[d["Country"]];
                                break;
                            case 4:
                                contact.Email = values[d["Email"]];
                                break;
                            case 5:
                                contact.DateInserted = Convert.ToDateTime(values[d["DataInserted"]]);
                                break;
                        }
                    }
                    contact.GuID = new Guid();
                    contactslist.Add(contact);
                }
                File.Delete(path);
            }
            catch
            {
                File.Delete(path);
            }
            return contactslist;
        }

        public List<Contact> GetContactsFromBytes(byte[] bytes)
        {
            List<Contact> list = new List<Contact>();
            string p = GetMimeFromBytes(bytes);
            switch (p)
            {
                case "text/csv":
                case "text/plain":
                    list = ReadFromCsv(bytes);
                    break;
                case "application/vnd.ms-excel":
                case "application/x-zip-compressed":
                    list = ReadFromExcel(bytes);
                    break;
                default:
                    list = null;
                    break;
            }
            return list;
        }
    }
}