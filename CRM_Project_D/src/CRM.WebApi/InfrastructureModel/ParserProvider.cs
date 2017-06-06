namespace CRM.WebApi.InfrastructureModel
{
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Spreadsheet;
    using Entities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using OfficeOpenXml;
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
        private List<Contact> ReadFromExcel(byte[] bytes, string path)
        {
            var contactslist = new List<Contact>();
            try
            {
                File.WriteAllBytes(path, bytes);
                contactslist = ReadExcelFileXml(path);
                File.Delete(path);
            }
            catch
            {
                File.Delete(path);
                throw;
            }
            finally
            {
                File.Delete(path);
            }
            return contactslist;
        }

        static List<Contact> ReadExcelFileXml(string filename)
        {
            string[] strProperties = new string[5];
            List<Contact> list = new List<Contact>();
            Contact contact;
            int j = 0;
            using (SpreadsheetDocument myDoc = SpreadsheetDocument.Open(filename, true))
            {
                WorkbookPart workbookPart = myDoc.WorkbookPart;
                IEnumerable<Sheet> sheets = myDoc.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets?.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)myDoc.WorkbookPart.GetPartById(relationshipId);
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                int i = 1;
                string value;
                foreach (Row r in sheetData.Elements<Row>())
                {
                    if (i != 1)
                    {
                        foreach (Cell c in r.Elements<Cell>())
                        {
                            if (c == null) continue;
                            value = c.InnerText;
                            if (c.DataType != null)
                            {
                                var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                                if (stringTable != null)
                                {
                                    value = stringTable.SharedStringTable.
                                        ElementAt(int.Parse(value)).InnerText;
                                }
                            }
                            strProperties[j] = value;
                            j = j + 1;
                        }
                    }
                    j = 0;
                    i = i + 1;
                    if (strProperties.Any(p => p == null)) continue; // checks all nulls
                    contact = new Contact();
                    contact.FullName = strProperties[0];
                    contact.CompanyName = strProperties[1];
                    contact.Position = strProperties[2];
                    contact.Country = strProperties[3];
                    contact.Email = strProperties[4];
                    list.Add(contact);
                }
                return list;
            }
        }

        static byte[] CSVtoExcel(byte[] csvbytes, string path)
        {
            string csvFileName = path + "\\csvfile.csv";
            string excelFileName = path + "\\excelfile.xlsx";
            byte[] bytes;
            try
            {
                File.WriteAllBytes(csvFileName, csvbytes);
                string worksheetsName = "sheet1";
                bool firstRowIsHeader = true;

                var format = new ExcelTextFormat();
                format.Delimiter = ',';
                format.EOL = "\r";

                using (ExcelPackage package = new ExcelPackage(new FileInfo(excelFileName)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetsName);
                    worksheet.Cells["A1"].LoadFromText(new FileInfo(csvFileName), format, OfficeOpenXml.Table.TableStyles.Medium27, firstRowIsHeader);
                    package.Save();
                    bytes = File.ReadAllBytes(excelFileName);
                }
                File.Delete(csvFileName);
                File.Delete(excelFileName);
            }
            catch
            {
                bytes = null;
                File.Delete(csvFileName);
                File.Delete(excelFileName);
            }
            return bytes;
        }
        public List<Contact> GetContactsFromBytes(byte[] bytes, string path)
        {
            List<Contact> list = new List<Contact>();
            string p = GetMimeFromBytes(bytes);
            switch (p)
            {
                case "text/csv":
                case "text/plain":
                case "application/octet-stream":
                    bytes = CSVtoExcel(bytes, path);
                    list = ReadFromExcel(bytes, path + "\\file.xlsx");
                    break;
                case "application/vnd.ms-excel":
                case "application/x-zip-compressed":
                    list = ReadFromExcel(bytes, path + "\\file.xlsx");
                    break;
                default:
                    list = null;
                    break;
            }
            return list;
        }
    }
}