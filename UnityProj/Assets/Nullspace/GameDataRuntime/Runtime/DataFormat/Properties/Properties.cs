
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Nullspace
{
    public partial class Properties
    {
        public static Properties CreateFromContent(string content, string urlString)
        {
            if (string.IsNullOrEmpty(content))
            {
                DebugUtils.Assert(false, "Attempting to create a Properties object from an empty URL!");
                return null;
            }
            // Calculate the file and full namespace path from the specified url.
            string fileString = null;
            List<string> namespacePath = new List<string>();
            if (urlString != null)
            {
                CalculateNamespacePath(ref urlString, ref fileString, namespacePath);
            }
            using (NullMemoryStream stream = NullMemoryStream.ReadTextFromString(content))
            {
                Properties properties = new Properties(stream);
                properties.ResolveInheritance(null);
                // Get the specified properties object.
                Properties p = GetPropertiesFromNamespacePath(properties, namespacePath);
                if (p == null)
                {
                    DebugUtils.Assert(false, string.Format("Failed to load properties from url {0}.", urlString));
                    return null;
                }
                if (p != properties)
                {
                    p = p.Clone();
                }
                if (fileString != null)
                {
                    p.SetDirectoryPath(Path.GetDirectoryName(fileString));
                }
                p.Rewind();
                return p;
            }
        }
        public static Properties Create(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                DebugUtils.Assert(false, "Attempting to create a Properties object from an empty URL!");
                return null;
            }
            // Calculate the file and full namespace path from the specified url.
            string urlString = url;
            string fileString = null;
            List<string> namespacePath = new List<string>();
            CalculateNamespacePath(ref urlString, ref fileString, namespacePath);
            if (File.Exists(fileString))
            {
                return CreateFromContent(File.ReadAllText(fileString), url);
            }
            else
            {
                Properties p = new Properties();
                p.mNamespace = fileString;
                p.mId = Path.GetFileNameWithoutExtension(fileString);
                p.SetDirectoryPath(fileString);
                return p;
            }
            
        }
        public static void CalculateNamespacePath(ref string urlString, ref string fileString, List<string> namespacePath)
        {
            int loc = urlString.IndexOf("#");
            if (loc != -1)
            {
                fileString = urlString.Substring(0, loc);
                string namespacePathString = urlString.Substring(loc + 1);
                while ((loc = namespacePathString.IndexOf("/")) != -1)
                {
                    namespacePath.Add(namespacePathString.Substring(0, loc));
                    namespacePathString = namespacePathString.Substring(loc + 1);
                }
                namespacePath.Add(namespacePathString);
            }
            else
            {
                fileString = urlString;
            }
        }
        public static Properties GetPropertiesFromNamespacePath(Properties properties, List<string> namespacePath)
        {
            if (namespacePath.Count > 0)
            {
                int size = namespacePath.Count;
                properties.Rewind();
                Properties iter = properties.GetNextNamespace();
                for (int i = 0; i < size;)
                {
                    while (true)
                    {
                        if (iter == null)
                        {
                            DebugUtils.Assert(false, "Failed to load properties object from url.");
                            return null;
                        }
                        if (namespacePath[i].Equals(iter.GetId())) // id, not namespace
                        {
                            if (i != size - 1)
                            {
                                properties = iter.GetNextNamespace();
                                iter.Rewind();
                                iter = properties;
                            }
                            else
                            {
                                properties = iter;
                            }
                            i++;
                            break;
                        }
                        iter = properties.GetNextNamespace();
                    }
                }
                properties.Rewind();
                return properties;
            }
            return properties;
        }
        public static bool IsVariable(string str, ref string outName)
        {
            int len = str.Length;
            if (len > 3 && str[0] == '$' && str[1] == '{' && str[len - 1] == '}')
            {
                outName = str.Substring(2, len - 3);
                return true;
            }
            return false;
        }
    }

    public partial class Properties
    {
        private Properties mParent;
        private string mNamespace;
        private string mId;
        private string mParentID;
        private List<Property> mProperties;
        private List<Properties> mNamespaces;
        private List<Property> mVariables;
        private string mDirPath;
        private bool mVisited;
        private int mPropertiesItr;
        private int mNamespacesItr;
        private class Property
        {
            public string Name;
            public string Value;
            public Property(string name, string value)
            {
                Name = name;
                Value = value;
            }

            public Property(Property other)
            {
                Name = other.Name;
                Value = other.Value;
            }
        }
        private Properties()
        {
            mVariables = null;
            mDirPath = null;
            mParent = null;
            mVisited = false;
            mProperties = new List<Property>();
            mNamespaces = new List<Properties>();
        }
        private Properties(NullMemoryStream stream) : this()
        {
            ReadProperties(stream);
            Rewind();
        }
        private Properties(Properties copy)
        {
            mNamespace = copy.mNamespace;
            mId = copy.mId;
            mParentID = copy.mParentID;
            mVisited = copy.mVisited;
            mParent = copy.mParent;
            mDirPath = null;
            mVariables = null;
            SetDirectoryPath(copy.mDirPath);
            mProperties = new List<Property>();
            mNamespaces = new List<Properties>();
            for (int it = 0; it < copy.mProperties.Count; ++it)
            {
                mProperties.Add(new Property(copy.mProperties[it]));
            }
            for (int it = 0; it < copy.mNamespaces.Count; ++it)
            {
                mNamespaces.Add(new Properties(copy.mNamespaces[it]));
            }
            Rewind();
        }
        private Properties(NullMemoryStream stream, string name, string id, string parentID, Properties parent) : this()
        {
            mNamespace = name;
            mVariables = null;
            mDirPath = null;
            mParent = parent;
            mVisited = false;
            if (id != null)
            {
                mId = id;
            }
            if (parentID != null)
            {
                mParentID = parentID;
            }
            ReadProperties(stream);
            Rewind();
        }

        public Properties Parent { get { return mParent; } }
        public string Namespace { get { return mNamespace; } }
        // Clones the Properties object.
        private Properties Clone()
        {
            Properties p = new Properties();

            p.mNamespace = mNamespace;
            p.mId = mId;
            p.mParentID = mParentID;
            p.mProperties = mProperties;
            p.mPropertiesItr = p.mProperties.Count;
            p.SetDirectoryPath(mDirPath);

            for (int i = 0, count = mNamespaces.Count; i < count; i++)
            {
                DebugUtils.Assert(mNamespaces[i] != null, "");
                Properties child = mNamespaces[i].Clone();
                p.mNamespaces.Add(child);
                child.mParent = p;
            }
            p.mNamespacesItr = p.mNamespaces.Count;

            return p;
        }

        public void WriteString(StringBuilder sb, int ident)
        {
            Rewind();
            string space = "";
            for (int i = 0; i < ident; ++i)
            {
                space += "    ";
            }
            sb.Append(space).AppendFormat("{0} {1}", mNamespace, mId).AppendLine();
            sb.Append(space).AppendLine("{");
            foreach (Property prop in mProperties)
            {
                sb.Append(space).Append("    ").AppendFormat("{0} = {1}", prop.Name, prop.Value).AppendLine();
            }
            if (mVariables != null)
            {
                foreach (Property variable in mVariables)
                {
                    sb.Append(space).Append("    ").AppendFormat("{0} = {1}", variable.Name, variable.Value).AppendLine();
                }
            }
            Properties p = null;
            while ((p = GetNextNamespace()) != null)
            {
                p.WriteString(sb, ident + 1);
                sb.AppendLine();
            }
            sb.Append(space).AppendLine("}");
        }
        public Properties Append(string namespaceName, string id)
        {
            Properties prop = new Properties();
            prop.mVisited = false;
            prop.mVariables = null;
            prop.mNamespace = namespaceName;
            prop.mParent = this;
            prop.mId = id;
            mNamespaces.Add(prop);
            return prop;
        }
        public string GetNextProperty()
        {
            if (mPropertiesItr == mProperties.Count)
            {
                // Restart from the beginning
                mPropertiesItr = 0;
            }
            else
            {
                // Move to the next property
                ++mPropertiesItr;
            }

            return mPropertiesItr == mProperties.Count ? null : mProperties[mPropertiesItr].Name;
        }
        public Properties GetNextNamespace()
        {
            if (mNamespacesItr == mNamespaces.Count)
            {
                // Restart from the beginning
                mNamespacesItr = 0;
            }
            else
            {
                ++mNamespacesItr;
            }
            if (mNamespacesItr != mNamespaces.Count)
            {
                Properties ns = mNamespaces[mNamespacesItr];
                return ns;
            }
            return null;
        }
        public void Rewind()
        {
            mPropertiesItr = mProperties.Count;
            mNamespacesItr = mNamespaces.Count;
        }
        public Properties GetNamespace(string id, bool searchNames, bool recurse)
        {
            DebugUtils.Assert(id != null, "");

            for (int i = 0; i < mNamespaces.Count; ++i)
            {
                Properties p = mNamespaces[i];
                if (id.Equals(searchNames ? p.mNamespace : p.mId))
                {
                    return p;
                }

                if (recurse)
                {
                    // Search recursively.
                    p = p.GetNamespace(id, searchNames, true);
                    if (p != null)
                    {
                        return p;
                    }
                }
            }
            return null;
        }
        public void GetNamespaces(string id, ref List<Properties> allList, bool searchNames, bool recurse)
        {
            DebugUtils.Assert(id != null, "");

            for (int i = 0; i < mNamespaces.Count; ++i)
            {
                Properties p = mNamespaces[i];
                if (id.Equals(searchNames ? p.mNamespace : p.mId))
                {
                    allList.Add(p);
                }

                if (recurse)
                {
                    // Search recursively.
                    p = p.GetNamespace(id, searchNames, true);
                    if (p != null)
                    {
                        allList.Add(p);
                    }
                }
            }
        }
        public string GetNamespace()
        {
            return mNamespace;
        }
        public string GetId()
        {
            return mId;
        }
        public bool Exists(string name)
        {
            if (name == null)
            {
                return false;
            }

            for (int itr = 0; itr < mProperties.Count; ++itr)
            {
                if (mProperties[itr].Name == name)
                {
                    return true;
                }
            }
            return false;
        }
        public string GetString(string name, string defaultValue)
        {
            string variable = null;
            string value = null;
            if (name != null)
            {
                // If 'name' is a variable, return the variable value
                if (IsVariable(name, ref variable))
                {
                    return GetVariable(variable, defaultValue);
                }
                for (int i = 0; i < mProperties.Count; ++i)
                {
                    if (mProperties[i].Name == name)
                    {
                        value = mProperties[i].Value;
                        break;
                    }
                }
            }
            else
            {
                // No name provided - get the value at the current iterator position
                if (mPropertiesItr != mProperties.Count)
                {
                    value = mProperties[mPropertiesItr].Value;
                }
            }

            if (value != null)
            {
                // If the value references a variable, return the variable value
                if (IsVariable(value, ref variable))
                {
                    return GetVariable(variable, defaultValue);
                }
                return value;
            }
            return defaultValue;
        }
        public bool SetString(string name, string value)
        {
            if (name != null)
            {
                for (int itr = 0; itr < mProperties.Count; ++itr)
                {
                    if (mProperties[itr].Name == name)
                    {
                        // Update the first property that matches this name
                        mProperties[itr].Value = value != null ? value : "";
                        return true;
                    }
                }
                // There is no property with this name, so add one
                mProperties.Add(new Property(name, value != null ? value : ""));
            }
            else
            {
                // If there's a current property, set its value
                if (mPropertiesItr == mProperties.Count)
                {
                    return false;
                }
                mProperties[mPropertiesItr].Value = value != null ? value : "";
            }

            return true;
        }
        public bool GetBool(string name, bool defaultValue) //  = false
        {
            string valueString = GetString(name, null);
            if (valueString != null)
            {
                return valueString == "true";
            }
            return defaultValue;
        }
        public int GetInt(string name)
        {
            string valueString = GetString(name, null);
            if (valueString != null)
            {
                int value;
                if (int.TryParse(valueString, out value))
                {
                    return value;
                }
                DebugUtils.Assert(false, string.Format("Error attempting to parse property {0} as an integer.", name));
            }

            return 0;
        }
        public float GetFloat(string name)
        {
            string valueString = GetString(name, null);
            if (valueString != null)
            {
                float value;
                if (float.TryParse(valueString, out value))
                {
                    return value;
                }
                DebugUtils.Assert(false, string.Format("Error attempting to parse property {0} as an integer.", name));
            }

            return 0;
        }
        public long GetLong(string name)
        {
            string valueString = GetString(name, null);
            if (valueString != null)
            {
                long value;
                if (long.TryParse(valueString, out value))
                {
                    return value;
                }
                DebugUtils.Assert(false, string.Format("Error attempting to parse property {0} as an integer.", name));
            }

            return 0;
        }
        public bool GetPath(string name, string path)
        {
            DebugUtils.Assert(name != null && path != null, "");

            string valueString = GetString(name, null);
            if (valueString != null)
            {
                bool found = false;
                if (File.Exists(valueString))
                {
                    path = valueString;
                    found = true;
                }
                else
                {
                    Properties prop = this;
                    while (prop != null)
                    {
                        // Search for the file path relative to the bundle file
                        string dirPath = prop.mDirPath;
                        if (!string.IsNullOrEmpty(dirPath))
                        {
                            string relativePath = dirPath;
                            relativePath += valueString;
                            if (File.Exists(relativePath))
                            {
                                path = relativePath;
                                found = true;
                                break;
                            }
                        }
                        prop = prop.mParent;
                    }
                }
                return found;
            }
            return false;
        }
        public string GetVariable(string name, string defaultValue)//  = null
        {
            if (name == null)
            {
                return defaultValue;
            }
            // Search for variable in this Properties object
            if (mVariables != null)
            {
                for (int i = 0, count = mVariables.Count; i < count; ++i)
                {
                    Property prop = mVariables[i];
                    if (prop.Name == name)
                    {
                        return prop.Value;
                    }
                }
            }
            // Search for variable in parent Properties
            return mParent != null ? mParent.GetVariable(name, defaultValue) : defaultValue;
        }
        public void SetVariable(string name, string value)
        {
            DebugUtils.Assert(name != null, "");

            Property prop = null;

            // Search for variable in this Properties object and parents
            Properties current = this;
            while (current != null)
            {
                if (current.mVariables != null)
                {
                    for (int i = 0, count = current.mVariables.Count; i < count; ++i)
                    {
                        Property p = current.mVariables[i];
                        if (p.Name == name)
                        {
                            prop = p;
                            break;
                        }
                    }
                }
                current = current.mParent;
            }
            if (prop != null)
            {
                // Found an existing property, set it
                prop.Value = value != null ? value : "";
            }
            else
            {
                // Add a new variable with this name
                if (mVariables == null)
                {
                    mVariables = new List<Property>();
                }
                mVariables.Add(new Property(name, value != null ? value : ""));
            }
        }
        /// <summary>
        /// '{' 和 '}' 一定要 单独一行
        /// </summary>
        /// <param name="stream"></param>
        private void ReadProperties(NullMemoryStream stream)
        {
            DebugUtils.Assert(stream != null, "");
            string namesp = null;
            string id = null;
            string parentID = null;
            bool comment = false;
            while (true)
            {
                // Stop when we have reached the end of the file.
                if (stream.Eof())  // 最后如果以  property 结束，需要加上最后一行的数据
                {
                    if (namesp != null)
                    {
                        mProperties.Add(new Property(namesp, id == null ? "" : id));
                    }
                    break;
                }

                // Read the next line.
                string line = stream.ReadLine();
                line = line.Trim();
                // Empty row
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                // Ignore comments
                if (comment)
                {
                    // Check for end of multi-line comment at either start or end of line
                    if (line.StartsWith("*/"))
                    {
                        comment = false;
                    }
                    else
                    {
                        if (line.EndsWith("*/"))
                        {
                            comment = false;
                        }
                    }
                }
                else
                {
                    if (line.StartsWith("/*"))
                    {
                        // Start of multi-line comment (must be at start of line)
                        comment = true;
                        if (line.EndsWith("*/")) // same line
                        {
                            comment = false;
                        }
                    }
                    else
                    {
                        int cc = line.IndexOf("//");// get data before first '//'
                        if (cc != -1)
                        {
                            line = line.Substring(0, cc);
                        }
                        if (!string.IsNullOrEmpty(line))
                        {
                            // If an '=' appears on this line, parse it as a name/value pair.
                            int rc = line.IndexOf("=");
                            if (rc != -1)
                            {
                                // First token should be the property name.

                                string[] strs = line.Split('=');
                                string name = strs[0];
                                // Remove white-space from name.
                                name = name.Trim();
                                // Scan for next token, the property's value.
                                string value = strs[1];
                                // Remove white-space from value.
                                value = value.Trim();
                                // Is this a variable assignment?
                                string variable = "";
                                if (IsVariable(name, ref variable))
                                {
                                    SetVariable(variable, value);
                                }
                                else
                                {
                                    // Normal name/value pair
                                    mProperties.Add(new Property(name, value));
                                }
                            }
                            else
                            {
                                // This line might begin or end a namespace,
                                // or it might be a key/value pair without '='.

                                // Check for '{' on same line.
                                rc = line.IndexOf("{");
                                if (rc != -1)
                                {
                                    if (line.Length > 1)
                                    {
                                        DebugUtils.Assert(false, "Error parsing this line should be only '{' : " + line);
                                        return;
                                    }
                                }

                                // Check for '}' on same line.
                                int rcc = line.IndexOf("}");
                                if (rcc != -1)
                                {
                                    if (line.Length > 1)
                                    {
                                        DebugUtils.Assert(false, "Error parsing this line should be only '}' : " + line);
                                        return;
                                    }
                                    // End of namespace.
                                    return;
                                }

                                if (rc != -1)
                                {
                                    if (string.IsNullOrEmpty(namesp))
                                    {
                                        DebugUtils.Assert(false, "ReadProperties Error parsing  namespace");
                                        return;
                                    }
                                    // New namespace without an ID.
                                    Properties space = new Properties(stream, namesp, id, parentID, this);
                                    mNamespaces.Add(space);
                                    // 重置
                                    namesp = null;
                                    id = null;
                                    parentID = null;
                                }
                                else
                                {
                                    if (namesp != null)
                                    {
                                        mProperties.Add(new Property(namesp, id == null ? "" : id));
                                    }
                                    // 记录上一行的信息
                                    namesp = null;
                                    id = null;
                                    parentID = null;

                                    string[] strs = line.Split(new char[] { ':' });
                                    string namevalue = strs[0].Trim();
                                    if (strs.Length == 2)
                                    {
                                        parentID = strs[1].Trim();
                                    }
                                    namesp = StringUtils.StrTok(namevalue, " ");
                                    if (namesp != null)
                                    {
                                        namesp = namesp.Trim();
                                    }
                                    if (string.IsNullOrEmpty(namesp))
                                    {
                                        namesp = null;
                                        DebugUtils.Assert(false, "Error parsing this line should be not null : " + line);
                                        return;
                                    }
                                    id = StringUtils.StrTok(null, "");
                                    if (id != null)
                                    {
                                        id = id.Trim();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        // Called after create(); copies info from parents into derived namespaces.
        private void ResolveInheritance(string id) // = null
        {
            // Namespaces can be defined like so:
            // "name id : parentID "
            // This method merges data from the parent namespace into the child.

            // Get a top-level namespace.
            Properties derived;
            if (id != null)
            {
                derived = GetNamespace(id, false, true);
            }
            else
            {
                derived = GetNextNamespace();
            }
            while (derived != null)
            {
                // If the namespace has a parent ID, find the parent.
                if (!string.IsNullOrEmpty(derived.mParentID))
                {
                    derived.mVisited = true;
                    Properties parent = GetNamespace(derived.mParentID, false, true);
                    if (parent != null)
                    {
                        DebugUtils.Assert(!parent.mVisited, "");
                        ResolveInheritance(parent.GetId());

                        // Copy the child.
                        Properties overrides = new Properties(derived);

                        // Delete the child's data.
                        for (int i = 0, count = derived.mNamespaces.Count; i < count; i++)
                        {
                            // derived.mNamespaces[i]
                        }

                        // Copy data from the parent into the child.
                        derived.mProperties = parent.mProperties;
                        derived.mNamespaces = new List<Properties>();
                        for (int itt = 0; itt < parent.mNamespaces.Count; ++itt)
                        {
                            DebugUtils.Assert(parent.mNamespaces[itt] != null, "");
                            derived.mNamespaces.Add(new Properties(parent.mNamespaces[itt]));
                        }
                        derived.Rewind();

                        // Take the original copy of the child and override the data copied from the parent.
                        derived.MergeWith(overrides);
                    }
                    derived.mVisited = false;
                }

                // Resolve inheritance within this namespace.
                derived.ResolveInheritance(null);

                // Get the next top-level namespace and check again.
                if (id != null)
                {
                    derived = GetNextNamespace();
                }
                else
                {
                    derived = null;
                }
            }
        }
        // Called by resolveInheritance().
        private void MergeWith(Properties overrides)
        {
            DebugUtils.Assert(overrides != null, "");

            // Overwrite or add each property found in child.
            overrides.Rewind();
            string name = overrides.GetNextProperty();
            while (name != null)
            {
                this.SetString(name, overrides.GetString(null, null));
                name = overrides.GetNextProperty();
            }
            mPropertiesItr = mProperties.Count;

            // Merge all common nested namespaces, add new ones.
            Properties overridesNamespace = overrides.GetNextNamespace();
            while (overridesNamespace != null)
            {
                bool merged = false;

                Rewind();
                Properties derivedNamespace = GetNextNamespace();
                while (derivedNamespace != null)
                {
                    if (derivedNamespace.GetNamespace() == overridesNamespace.GetNamespace() &&
                        derivedNamespace.GetId() == overridesNamespace.GetId())
                    {
                        derivedNamespace.MergeWith(overridesNamespace);
                        merged = true;
                    }

                    derivedNamespace = GetNextNamespace();
                }

                if (!merged)
                {
                    // Add this new namespace.
                    Properties newNamespace = new Properties(overridesNamespace);

                    mNamespaces.Add(newNamespace);
                    mNamespacesItr = this.mNamespaces.Count;
                }

                overridesNamespace = overrides.GetNextNamespace();
            }
        }
        private void SetDirectoryPath(string path)
        {
            mDirPath = path;
        }
    }
}
