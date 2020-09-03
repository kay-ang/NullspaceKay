
using System.IO;
using System.Security;

namespace Nullspace
{
    public partial class DataFormatConvertUtils
    {
        public static Properties ConvertXMLToPropertiesFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            string content = File.ReadAllText(filePath);
            return ConvertXMLToPropertiesFromContent(content, filePath);
        }

        public static Properties ConvertXMLToPropertiesFromContent(string content, string filePath)
        {
            if (string.IsNullOrEmpty(content))
            {
                DebugUtils.Assert(false, "Attempting to create a Properties object from an empty URL!");
                return null;
            }
            SecurityParser securityParser = new SecurityParser();
            securityParser.LoadXml(content);
            SecurityElement xml = securityParser.ToXml();
            return Properties.CreateFromXml(xml, filePath);
        }

        public static SecurityElement ConvertPropertiesToXMLFromContent(string content, string filePath)
        {
            Properties prop = Properties.CreateFromContent(content, filePath);
            return Properties.ConvertPropertiesToXML(prop);
        }

        public static SecurityElement ConvertPropertiesToXMLFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            Properties prop = Properties.Create(filePath);
            return Properties.ConvertPropertiesToXML(prop);
        }

    }
}
