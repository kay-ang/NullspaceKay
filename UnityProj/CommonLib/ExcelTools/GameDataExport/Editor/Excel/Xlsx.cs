using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Text;

namespace Nullspace
{
    public class Xlsx : IEnumerable<XlsxSheet>, IExportCSharp
    {
        public static Xlsx Create(string filePath)
        {
            FileInfo newFile = new FileInfo(filePath);
            using (ExcelPackage pck = new ExcelPackage(newFile))
            {
                try
                {
                    string name = Path.GetFileNameWithoutExtension(filePath);
                    name = name.Substring(0, 1).ToUpper() + name.Substring(1);
                    ExcelWorkbook workBook = pck.Workbook;
                    if (workBook != null)
                    {
                        Xlsx excel = new Xlsx(filePath, name);
                        int cnt = workBook.Worksheets.Count;
                        var enumerator = workBook.Worksheets.GetEnumerator();
                        bool isSuccess = true;
                        while (enumerator.MoveNext())
                        {
                            ExcelWorksheet tDS = enumerator.Current;
                            XlsxSheet sheet = XlsxSheet.Create(tDS, excel);
                            if (sheet == null)
                            {
                                isSuccess = false;
                                MainEntry.Log(string.Format("Sheet:{0} failed", tDS.Name));
                                break;
                            }
                            excel.AddSeet(sheet);
                        }
                        if (isSuccess)
                        {
                            return excel;
                        }
                        MainEntry.Log(string.Format("ExcelWorkbook Failed, Not Parse Sheet Right: {0}", filePath));
                    }
                    else
                    {
                        MainEntry.Log(string.Format("ExcelWorkbook null, Not Parse Right: {0}", filePath));
                    }
                    return null;
                }
                catch (Exception e)
                {
                    MainEntry.Log(string.Format("ErrorInfo: {0}, StackTrace: {1}, filePath: {2}", e.Message, e.StackTrace, filePath));
                }
            }
            return null;
        }

        private List<XlsxSheet> mSheets;

        private Xlsx(string filePath, string fileName)
        {
            FileName = fileName;
            mSheets = new List<XlsxSheet>();
            RecordInfo = new XlsxExportRecord() { FilePath = filePath.Replace("\\", "/") };
        }
        private void AddSeet(XlsxSheet sheet)
        {
            mSheets.Add(sheet);
        }

        public void InitializeRecord()
        {
            Dictionary<string, string> records = new Dictionary<string, string>();
            foreach (XlsxSheet sheet in this)
            {
                records.Add(sheet.SheetName, sheet.RecordSheet);
            }
            RecordInfo.FileHash = MD5HashUtils.BuildFileMd5(RecordInfo.FilePath);
            RecordInfo.SheetInfo = DataUtils.ToString(records);
            RecordInfo.SheetHash = MD5HashUtils.Get(RecordInfo.SheetInfo);
        }

        public XlsxExportRecord RecordInfo { get; set; }
        public string FileName { get; set; }
        public void ExportCSharp(StringBuilder builder)
        {
            builder.AppendLine("/****************************************");
            builder.AppendLine("* The Class Is Generated Automatically By GameDataTool, ");
            builder.AppendLine("* Don't Modify It Manually.");
            builder.AppendFormat("* DateTime: {0}.", DateTimeUtils.GetDateTimeString()).AppendLine();
            builder.AppendLine("Later: Export Method InitializeFromXml(SecurityElement element), reduce reflection count");
            builder.AppendLine("****************************************/");
            builder.AppendLine();
            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.Text;");
            builder.AppendLine("using Nullspace;");
            builder.AppendLine("using UnityEngine;");
            builder.AppendLine("namespace GameData");
            builder.AppendLine("{");
            foreach (XlsxSheet sheet in this)
            {
                sheet.ExportCSharp(builder);
                builder.AppendLine();
            }
            builder.AppendLine("}");
        }
        public IEnumerator<XlsxSheet> GetEnumerator()
        {
            return mSheets.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return mSheets.GetEnumerator();
        }
    }
}
