
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text;
using System.Xml.Linq;

namespace Nullspace
{
    public partial class DataFormatConvertUtils
    {
        public static void ExportXlsx()
        {
            // ExportEmpty();
            ExportXlsxDir(MainEntry.Config.GetString("xlsx_dir", null), MainEntry.Config.GetBool("recursive", false));
        }
        public static void BuildDll()
        {
            if (MainEntry.Config.GetBool("export_cs", false))
            {
                string csDir = MainEntry.Config.GetString("cs_dir", null);
                DebugUtils.Log(InfoType.Warning, string.Format("export cs files in dir: {0}, please import into GameDataDefine c# project and Build GameDataDefine.dll", csDir));
                //string refDll = Path.GetFullPath(".");
                //csDir = Path.GetFullPath(csDir);
                //List<string> cmdList = new List<string>()
                //{
                //    string.Format("cd /d {0}", csDir),
                //    string.Format("set UnityEngine={0}/UnityEngine.CoreModule.dll", refDll),
                //    string.Format("set GameDataRuntime={0}/GameDataRuntime.dll", refDll),
                //    string.Format("set CommonUtils={0}/CommonUtils.dll", refDll),
                //    string.Format("call \"C:/Windows/Microsoft.NET/Framework/v3.5/csc.exe\" -target:library /r:%CommonUtils% /r:%UnityEngine% /r:%GameDataRuntime% /out:{0}/GameDataDefine.dll /recurse:*.cs", refDll),
                //    "exit"
                //};
                //string result = RunCmd(cmdList);
                //Console.WriteLine(result);
            }
        }
        /// <summary>
        /// 运行cmd命令
        /// 不显示命令窗口
        /// </summary>
        /// <param name="cmdExe">指定应用程序的完整路径</param>
        /// <param name="cmdStr">执行命令行参数</param>
        private static string RunCmd(List<string> cmdList)
        {
            string result = null;
            try
            {
                using (Process myPro = new Process())
                {
                    myPro.StartInfo.FileName = "cmd.exe";
                    myPro.StartInfo.UseShellExecute = false;
                    myPro.StartInfo.RedirectStandardInput = true;
                    myPro.StartInfo.RedirectStandardOutput = true;
                    myPro.StartInfo.RedirectStandardError = true;
                    myPro.StartInfo.CreateNoWindow = true;
                    myPro.Start();
                    myPro.StandardInput.AutoFlush = true;
                    for (int i = 0; i < cmdList.Count; ++i)
                    {
                        myPro.StandardInput.WriteLine(cmdList[i]);
                    }
                    result = myPro.StandardOutput.ReadToEnd();
                    myPro.WaitForExit();
                }
            }
            catch
            {

            }
            return result;
        }
        private static Properties ConvertXlsxToProperties(Xlsx excel)
        {
            return Properties.CreateFromXlsx(excel);
        }
        private static void ExportEmpty()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("namespace GameData");
            builder.AppendLine("{");
            string tab = "    ";
            builder.Append(tab).Append("public class EmptyGameData {}").AppendLine();
            builder.AppendLine("}");
            File.WriteAllText(string.Format("{0}/EmptyGameData.cs", MakeDir(MainEntry.Config.GetString("cs_dir", null))), builder.ToString());
        }
        private static bool ExportXlsxDir(string rootDir, bool recursive = true)
        {
            string[] files = Directory.GetFiles(rootDir, "*.xlsx");
            foreach (string file in files)
            {
                if (!ExportXlsxFile(file))
                {
                    return false;
                }
            }
            if (recursive)
            {
                string[] dirs = Directory.GetDirectories(rootDir);
                foreach (string dir in dirs)
                {
                    return ExportXlsxDir(dir, recursive);
                }
            }
            return true;
        }
        private static bool ExportXlsxFile(string filePath)
        {
            Xlsx xlsx = Xlsx.Create(filePath);
            if (xlsx != null)
            {
                xlsx.InitializeRecord();
                bool res = ExportCS(xlsx);
                res &= ExportXml(xlsx);
                return res;
            }
            else
            {
                MainEntry.Log(string.Format("ExportXlsxFile Wrong: {0}", filePath));
                return false;
            }
        }

        private static bool ExportCS(Xlsx xlsx)
        {
            Properties historyProp = MainEntry.XlsxHistory.GetNamespace(xlsx.FileName, false, false);
            if (historyProp == null)
            {
                historyProp = MainEntry.XlsxHistory.Append(xlsx.FileName, xlsx.FileName);
                historyProp.SetString("FilePath", xlsx.RecordInfo.FilePath);
            }
            string sheetHash = historyProp.GetString("SheetHash", null);
            if (sheetHash != xlsx.RecordInfo.SheetHash)
            {
                historyProp.SetString("SheetHash", xlsx.RecordInfo.SheetHash);
                MainEntry.Config.SetString("export_cs", "true");
                StringBuilder builder = new StringBuilder();
                xlsx.ExportCSharp(builder);
                File.WriteAllText(string.Format("{0}/{1}.cs", MakeDir(MainEntry.Config.GetString("cs_dir", null)), xlsx.FileName), builder.ToString());
            }
            return true;
        }
        private static bool ExportXml(Xlsx xlsx)
        {
            Properties historyProp = MainEntry.XlsxHistory.GetNamespace(xlsx.FileName, false, false);
            if (historyProp == null)
            {
                historyProp = MainEntry.XlsxHistory.Append(xlsx.FileName, xlsx.FileName);
                historyProp.SetString("FilePath", xlsx.RecordInfo.FilePath);
            }
            string fileHash = historyProp.GetString("FileHash", null);
            if (fileHash != xlsx.RecordInfo.FileHash)
            {
                historyProp.SetString("FileHash", xlsx.RecordInfo.FileHash);
                Properties prop = Properties.CreateFromXlsx(xlsx);
                SecurityElement clientRoot;
                SecurityElement serverRoot;
                Properties.ConvertXlsxPropertiesToXML(prop, out clientRoot, out serverRoot);
                XElement element = XElement.Parse(clientRoot.ToString());
                File.WriteAllText(string.Format("{0}/{1}_client.xml", MakeDir(MainEntry.Config.GetString("xml_dir", null)), xlsx.FileName), element.ToString());
                element = XElement.Parse(serverRoot.ToString());
                File.WriteAllText(string.Format("{0}/{1}_server.xml", MakeDir(MainEntry.Config.GetString("xml_dir", null)), xlsx.FileName), element.ToString());
            }
            return true;
        }

        private static string MakeDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir.Replace("\\", "/"));
            }
            return dir;
        }
    }
}
