
using System;
using System.IO;
using System.Security;
using System.Text;
using System.Xml.Linq;

namespace Nullspace
{
    public class MainEntry
    {
        public static Properties Config;
        public static Properties XlsxHistory;
        public static void Main(string[] argvs)
        {
            Config = Properties.Create("config.txt");
            DebugUtils.SetLogAction(LogAction);
            string hashFile = "xlsx_info_record.txt";
            bool md5Hash = Config.GetBool("md5_hash", false);
            if (!md5Hash && File.Exists(hashFile))
            {
                File.Delete(hashFile);
            }
            XlsxHistory = Properties.Create("xlsx_info_record.txt#xlsx_info_record");
            DataFormatConvertUtils.ExportXlsx();
            DataFormatConvertUtils.BuildDll();
            StringBuilder sb = new StringBuilder();
            XlsxHistory.WriteString(sb, 0);
            File.WriteAllText(hashFile, sb.ToString());
            Console.ReadLine();
        }

        private static void LogAction(InfoType infoType, string info)
        {
            switch (infoType)
            {
                case InfoType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case InfoType.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case InfoType.Warning:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }
            Console.WriteLine(info);
        }

        public static void Log(string str)
        {
            Console.WriteLine(str);
        }

        private static void TestXmlAndProperties()
        {
            string filePath = "test_data.xml";
            Properties prop = DataFormatConvertUtils.ConvertXMLToPropertiesFromFile(filePath);
            StringBuilder sb = new StringBuilder();
            prop.WriteString(sb, 0);
            File.WriteAllText("xml_2_properties.txt", sb.ToString());

            SecurityElement root = Properties.ConvertPropertiesToXML(prop);
            // 格式化
            XElement element = XElement.Parse(root.ToString());
            File.WriteAllText("properties_2_xml.xml", element.ToString());
        }
        private static void TestXlsx()
        {
            string filePath = "test.xlsx";
            Xlsx xlsx = Xlsx.Create(filePath);
            StringBuilder sb = new StringBuilder();
            xlsx.ExportCSharp(sb);
            File.WriteAllText(string.Format("GameData/{0}.cs", xlsx.FileName), sb.ToString());

            Properties prop = Properties.CreateFromXlsx(xlsx);
            SecurityElement root = Properties.ConvertPropertiesToXML(prop);
            // 格式化
            XElement element = XElement.Parse(root.ToString());
            File.WriteAllText("xlsx_2_properties_2_xml.xml", element.ToString());
        }
    }
}
