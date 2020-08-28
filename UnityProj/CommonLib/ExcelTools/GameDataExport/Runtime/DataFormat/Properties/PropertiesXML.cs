
using System.Security;

namespace Nullspace
{
    public partial class Properties
    {
        public static SecurityElement ConvertPropertiesToXML(Properties prop)
        {
            SecurityElement xml = new SecurityElement(prop.mNamespace);
            prop.WriteProperties(xml);
            return xml;
        }

        public static Properties CreateFromXml(SecurityElement root, string urlString)
        {
            if (root == null)
            {
                return null;
            }
            Properties properties = new Properties(root);
            return properties;
        }

        private Properties(SecurityElement root) : this()
        {
            ReadProperties(root);
            Rewind();
        }

        private void ReadProperties(SecurityElement xml)
        {
            mNamespace = xml.Tag;
            mId = xml.Tag;
            int count = xml.Children.Count;
            ReadAttributes(xml);
            ReadTextValue(xml);
            for (int i = 0; i < count; ++i)
            {
                SecurityElement child = xml.Children[i] as SecurityElement;
                if (child.Children == null && child.Attributes == null)
                {
                    // property
                    ReadTextValue(child);
                }
                else
                {
                    // namespace
                    Properties childProp = new Properties(child);
                    mNamespaces.Add(childProp);
                }
            }
        }
        private void ReadTextValue(SecurityElement node)
        {
            if (!string.IsNullOrEmpty(node.Text))
            {
                string value = node.Text.Trim();
                mProperties.Add(new Property(node.Tag, value));
            }
        }

        private void ReadAttributes(SecurityElement node)
        {
            if (node.Attributes != null)
            {
                foreach (string key in node.Attributes.Keys)
                {
                    string value = node.Attributes[key].ToString();
                    mProperties.Add(new Property(key, value));
                }
            }
        }
        private void WriteProperties(SecurityElement xml)
        {
            WriteAttributes(xml);
            for (int i = 0; i < mNamespaces.Count; ++i)
            {
                Properties child = mNamespaces[i];
                SecurityElement xmlChild = new SecurityElement(child.mNamespace);
                child.WriteProperties(xmlChild);
                xml.AddChild(xmlChild);
            }
        }

        private void WriteAttributes(SecurityElement node)
        {
            if (mProperties != null)
            {
                foreach (Property prop in mProperties)
                {
                    node.AddAttribute(prop.Name, prop.Value);
                }
            }
        }
    }

}
