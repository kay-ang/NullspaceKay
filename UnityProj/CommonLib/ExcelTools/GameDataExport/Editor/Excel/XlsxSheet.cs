
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Nullspace
{
    public class XlsxSheet : IExportCSharp
    {

        private static char KeySplitChar = ',';
        private const string SKIP_ROW = "(#SKIP)";
        private const int SHEET_DATA_MANAGER = 0;
        private const int VARIABLE_NAME_ROW = 1;
        private const int VARIABLE_TYPE_ROW = 2;
        private const int VARIABLE_SIDE_ROW = 3;
        private const int VARIABLE_DESCRIPTION_ROW = 4;
        private const int VALUE_START_ROW = 5;
        public static XlsxSheet Create(ExcelWorksheet tDS, Xlsx parent)
        {
            XlsxSheet sheet = new XlsxSheet(parent, tDS.Name);
            List<string> variableNames;
            List<DataTypeEnum> variableTypes;
            List<DataSideEnum> variableSides;
            string managerName = null;
            string keys = null;
            MainEntry.Log(string.Format("-------------------SheetName: {0} Begin", tDS.Name));
            List<XlsxRow> datas = CleanRowAndCols(tDS, sheet, out managerName, out keys, out variableNames, out variableTypes, out variableSides);
            if (datas != null)
            {
                sheet.Set(managerName, keys, variableNames, variableTypes, variableSides, datas);
                MainEntry.Log(string.Format("-------------------SheetName: {0} End", tDS.Name));
                return sheet;
            }
            MainEntry.Log(string.Format("*******************SheetName: {0} failed", tDS.Name));
            return null;
        }

        private static void ParseManagerType(string value, ref string managerName, ref string keys)
        {
            string leftValue = StringUtils.StrTok(value, ">"); 
            managerName = StringUtils.StrTok(leftValue, "<");
            if (managerName == null)
            {
                return;
            }
            managerName = managerName.Replace(" ", "").ToUpper();
            keys = StringUtils.StrTok(null, "");
            if (keys != null)
            {
                keys = keys.Replace(" ", "").Replace("，", ",");
            }
        }
        private static List<XlsxRow> CleanRowAndCols(ExcelWorksheet tDS, XlsxSheet sheet, out string managerName, out string keys, out List<string> variableNames, out List<DataTypeEnum> variableTypes, out List<DataSideEnum> variableSides)
        {
            variableNames = new List<string>();
            variableTypes = new List<DataTypeEnum>();
            variableSides = new List<DataSideEnum>();
            managerName = null;
            keys = null;
            ExcelRange range = tDS.Cells;
            object[,] values = (object[,])range.Value;
            int rows = values.GetLength(0);
            int cols = values.GetLength(1);
            HashSet<int> skipCols = new HashSet<int>();
            HashSet<int> skipRows = new HashSet<int>();
            if (rows > 0 && cols > 0)
            {
                // List,Group<ColName>,Map<ColName>,Map<ColName1, ColName2>
                string value = (string)values[SHEET_DATA_MANAGER, 0];
                if (value == null)
                {
                    MainEntry.Log(string.Format("****************SheetName: {0} Wrong At [0, 0] End", tDS.Name));
                    return null;
                }
                ParseManagerType(value, ref managerName, ref keys);
                if (managerName == null)
                {
                    MainEntry.Log(string.Format("****************SheetName: {0} Wrong ManagerName {1} End", tDS.Name, value));
                    return null;
                }
            }
            for (int i = 0; i < cols; ++i)
            {
                if (values[VARIABLE_SIDE_ROW, i] == null)
                {
                    MainEntry.Log(string.Format("skip null col: [{0}, {1}], maybe last col", VARIABLE_SIDE_ROW, i));
                    skipCols.Add(i);
                    continue;
                }
                string v = values[VARIABLE_SIDE_ROW, i].ToString().Trim().ToUpper();
                DataSideEnum side = EnumUtils.StringToEnum<DataSideEnum>(v);
                if (side == DataSideEnum.SKIP)
                {
                    MainEntry.Log(string.Format("skip flag col: [{0}, {1}]", VARIABLE_SIDE_ROW, i));
                    skipCols.Add(i);
                    continue;
                }
                if (values[VARIABLE_NAME_ROW, i] == null)
                {
                    // log
                    MainEntry.Log(string.Format("Not Define Variable Name: [{0}, {1}]", VARIABLE_NAME_ROW, i));
                    return null;
                }
                if (values[VARIABLE_TYPE_ROW, i] == null)
                {
                    // log
                    MainEntry.Log(string.Format("Not Define Variable Type: [{0}, {1}]", VARIABLE_TYPE_ROW, i));
                    return null;
                }
                variableSides.Add(side);
                v = values[VARIABLE_NAME_ROW, i].ToString().Trim();
                variableNames.Add(v);
                v = values[VARIABLE_TYPE_ROW, i].ToString().Trim();
                try
                {
                    DataTypeEnum dataType = EnumUtils.StringToEnum<DataTypeEnum>(v.ToUpper());
                    variableTypes.Add(dataType);
                }
                catch (Exception e)
                {
                    MainEntry.Log(string.Format("Not Define Variable Side: [{0}, {1}] = {2}, Error: {3}", VARIABLE_TYPE_ROW, i, v, e.Message));
                    return null;
                }
            }
            for (int i = VALUE_START_ROW; i < rows; ++i)
            {
                if (values[i, 0] != null)
                {
                    string v = values[i, 0].ToString().Trim();
                    if (v.StartsWith(SKIP_ROW))
                    {
                        MainEntry.Log(string.Format("skip flag row: [{0}, 0]", i));
                        skipRows.Add(i);
                    }
                }
                else
                {
                    MainEntry.Log(string.Format("skip null row: [{0}, 0], maybe last row", i));
                    skipRows.Add(i);
                }
            }
            List<XlsxRow> originalDatas = new List<XlsxRow>();
            int rowIndex = 0;
            for (int i = VALUE_START_ROW; i < rows; ++i)
            {
                if (skipRows.Contains(i))
                {
                    continue;
                }
                List<string> rowData = new List<string>();
                XlsxRow row = new XlsxRow(rowIndex++, rowData, sheet);
                originalDatas.Add(row);
                for (int j = 0; j < cols; ++j)
                {
                    if (skipCols.Contains(j))
                    {
                        continue;
                    }
                    rowData.Add(values[i, j] != null ? values[i, j].ToString() : null);
                }
            }
            return originalDatas;
        }
        public string SheetName { get; set; }
        private string Manager { get; set; }
        private bool Delay { get; set; }
        private string Keys { get; set; }
        private List<string> mVariableNames { get; set; }
        private List<DataTypeEnum> mVariableTypes { get; set; }
        private List<DataSideEnum> mVariableSides { get; set; }
        private List<XlsxRow> mDatas { get; set; }
        private List<int> mClientColumns { get; set; }
        private List<int> mServerColumns { get; set; }
        public Xlsx Parent { get; set; }
        public string RecordSheet { get; set; }
        private XlsxSheet(Xlsx parent, string sheetName)
        {
            Parent = parent;
            SheetName = sheetName;
        }
        private void Set(string managerName, string keys, List<string> variableNames, List<DataTypeEnum> variableTypes, List<DataSideEnum> variableSides, List<XlsxRow> datas)
        {
            mVariableNames = variableNames;
            mVariableTypes = variableTypes;
            mVariableSides = variableSides;
            mDatas = datas;
            ParseSheetAttribute(managerName, keys);
            SplitCS();
            GeneratorRecord();
        }

        private void GeneratorRecord()
        {
            string varNames = DataUtils.ToString(mVariableNames);
            string varTypes = DataUtils.ToString(mVariableTypes);
            string varSides = DataUtils.ToString(mVariableSides);
            Dictionary<string, string> record = new Dictionary<string, string>();
            record.Add("varNames", varNames);
            record.Add("varTypes", varTypes);
            record.Add("varSides", varSides);
            record.Add("Manager", Manager);
            record.Add("Delay", Delay.ToString());
            record.Add("Keys", Keys);
            RecordSheet = DataUtils.ToString(record);
        }

        private void SplitCS()
        {
            mClientColumns = new List<int>();
            mServerColumns = new List<int>();
            int count = mVariableSides.Count;
            for (int i = 0; i < count; ++i)
            {
                switch (mVariableSides[i])
                {
                    case DataSideEnum.CS:
                        mClientColumns.Add(i);
                        mServerColumns.Add(i);
                        break;
                    case DataSideEnum.C:
                        mClientColumns.Add(i);
                        break;
                    case DataSideEnum.S:
                        mServerColumns.Add(i);
                        break;
                    case DataSideEnum.SKIP:
                        throw new Exception("should never be happened here");
                }
            }
        }
        public string this[int row, int col]
        {
            get
            {
                DebugUtils.Assert(row < RowCount && col < ColCount, "");
                return mDatas[row][col];
            }
        }
        public XlsxRow this[int row]
        {
            get
            {
                return mDatas[row];
            }
        }
        public int RowCount { get { return mDatas.Count; } }
        public int ColCount { get { return RowCount > 0 ? mDatas[0].Count : 0; } }
        public bool GetCol(int i, ref string varName, ref DataTypeEnum varType)
        {
            varName = mVariableNames[i];
            varType = mVariableTypes[i];
            return true;
        }
        public List<int> GetColumns(DataSideEnum side)
        {
            switch (side)
            {
                case DataSideEnum.C:
                    return mClientColumns;
                case DataSideEnum.S:
                    return mServerColumns;
                default:
                    throw new Exception("wrong");
            }
        }
        public void ExportCSharp(StringBuilder builder)
        {
            ExportCSharp(builder, DataSideEnum.C);
        }
        private string GetKeyNames()
        {
            if (!string.IsNullOrEmpty(Keys))
            {
                List<string> keys = new List<string>(Keys.Split(KeySplitChar));
                string v = "new List<string>(){";
                foreach (string key in keys)
                {
                    v += string.Format(" \"{0}\",", key);
                }
                v = v.Substring(0, v.Length - 1);
                v += " }";
                return v;
            }
            return "null";
        }

        private string GetDataManager()
        {
            GameDataManagerType type = GameDataManagerType.LIST;
            if (Manager != null)
            {
                type = EnumUtils.StringToEnum<GameDataManagerType>(Manager.ToUpper());
            }
            switch (type)
            {
                case GameDataManagerType.LIST:
                    return "GameDataList";
                case GameDataManagerType.MAP:
                    return "GameDataMap";
                case GameDataManagerType.GROUP:
                    return "GameDataGroup";
            }
            return "GameDataList";
        }

        private void ParseSheetAttribute(string managerName, string keys)
        {
            Keys = keys;
            Delay = true;
            Manager = managerName;
        }

        private string MakeClassDefine()
        {
            GameDataManagerType type = GameDataManagerType.LIST;
            if (Manager != null)
            {
                type = EnumUtils.StringToEnum<GameDataManagerType>(Manager.ToUpper());
            }
            string baseName = GetDataManager();
            if (type == GameDataManagerType.LIST)
            {
                return string.Format("public class {0} : {1}<{2}>", SheetName, baseName, SheetName);
            }
            DebugUtils.Assert(!string.IsNullOrEmpty(Keys), "Wrong Keys");
            List<string> keys = new List<string>(Keys.Split(','));
            string dataTypeString = null;
            string readString = null;
            string writeString = null;
            string generic = string.Format("public class {0} : {1}<", SheetName, baseName);
            for (int j = 0; j < keys.Count; ++j)
            {
                string key = keys[j];
                int i = 0;
                for (i = 0; i < mVariableNames.Count; ++i)
                {
                    if (mVariableNames[i] == key)
                    {
                        break;
                    }
                }
                DebugUtils.Assert(i < mVariableNames.Count, string.Format("{0} KeyName Not Found", key));
                GetType(mVariableTypes[i], ref dataTypeString, ref readString, ref writeString);
                generic += dataTypeString + ", ";
            }
            generic += SheetName + ">";
            return generic;
        }

        public void ExportCSharp(StringBuilder builder, DataSideEnum side)
        {
            string tab = "    ";
            string doubleTab = tab + tab;
            builder.Append(tab).Append(MakeClassDefine());
            bool export_nullstream = MainEntry.Config.GetBool("export_nullstream", false);
            if (export_nullstream)
            {
                builder.Append(", INullStream");
            }
            builder.AppendLine();
            builder.Append(tab).AppendLine("{");

            builder.Append(doubleTab).Append(string.Format("public static readonly string FileUrl = \"{0}#{1}s\";", Parent.FileName, SheetName)).AppendLine();
            builder.Append(doubleTab).Append(string.Format("public static readonly bool IsDelayInitialized = {0};", Delay.ToString().ToLower())).AppendLine();
            builder.Append(doubleTab).Append(string.Format("public static readonly List<string> KeyNameList = {0};", GetKeyNames())).AppendLine();

            string dataTypeString = "";
            string readString = "";
            string writeString = "";

            List<int> cols = GetColumns(side);
            int count = cols.Count;
            List<KeyValuePair<string, string>> listNames = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < count; ++i)
            {
                DataTypeEnum dt = mVariableTypes[cols[i]];
                GetType(dt, ref dataTypeString, ref readString, ref writeString);
                builder.Append(doubleTab).Append(string.Format("public {0} {1} ", dataTypeString, mVariableNames[cols[i]])).Append("{ get; private set; }").AppendLine();

                // List 需要初始化
                if (dt > DataTypeEnum.LIST)
                {
                    KeyValuePair<string, string> pair = new KeyValuePair<string, string>(mVariableNames[cols[i]], dataTypeString);
                    listNames.Add(pair);
                }
            }
            builder.AppendLine();
            if (listNames.Count > 0)
            {
                builder.Append(doubleTab).Append(string.Format("public {0}()", SheetName)).AppendLine();
                builder.Append(doubleTab).Append("{").AppendLine();
                foreach (KeyValuePair<string, string> name in listNames)
                {
                    builder.Append(doubleTab).Append(tab).Append(string.Format("{0} = new {1}();", name.Key, name.Value)).AppendLine();
                }
                builder.Append(doubleTab).Append("}").AppendLine();
            }

            builder.AppendLine();
            builder.Append(doubleTab).AppendLine("protected override void Convert()");
            builder.Append(doubleTab).AppendLine("{");
            List<string> keys = null;
            if (Keys != null)
            {
                keys = new List<string>(Keys.Split(KeySplitChar));
                int cnt = keys.Count;
                for (int i = 0; i < cnt; ++i)
                {
                    keys[i] = keys[i].Trim();
                }
            }
            for (int i = 0; i < count; ++i)
            {
                string name = mVariableNames[cols[i]];
                if (keys != null && keys.Contains(name))
                {
                    continue;
                }

                DataTypeEnum dt = mVariableTypes[cols[i]];
                GetType(dt, ref dataTypeString, ref readString, ref writeString);
                string func = "ToObject";
                // List 需要初始化
                if (dt > DataTypeEnum.LIST)
                {
                    string l = StringUtils.StrTok(dataTypeString, "<");
                    dataTypeString = StringUtils.StrTok(null, ">");
                    func = "ToObjectList";
                }
                builder.Append(doubleTab).Append(tab).Append(string.Format("{0} = DataUtils.{1}<{2}>(GetValue(\"{0}\"));", name, func, dataTypeString)).AppendLine();
            }
            builder.Append(doubleTab).AppendLine("}");

            if (export_nullstream)
            {
                builder.Append(doubleTab).Append("public int SaveToStream(NullMemoryStream stream)").AppendLine();
                builder.Append(doubleTab).Append("{").AppendLine();
                builder.Append(doubleTab).Append(tab).Append("int size = 0;").AppendLine();
                for (int i = 0; i < count; ++i)
                {
                    GetType(mVariableTypes[cols[i]], ref dataTypeString, ref readString, ref writeString);
                    builder.Append(doubleTab).Append(tab).Append(string.Format("size += GameDataUtils.{0}(stream, {1})", writeString, mVariableNames[cols[i]])).AppendLine();
                }
                builder.Append(doubleTab).Append(tab).Append("return size;").AppendLine();
                builder.Append(doubleTab).Append("}").AppendLine();

                builder.AppendLine();
                builder.Append(doubleTab).Append("public bool LoadFromStream(NullMemoryStream stream)").AppendLine();
                builder.Append(doubleTab).Append("{").AppendLine();
                builder.Append(doubleTab).Append(tab).Append("bool res = true;").AppendLine();
                for (int i = 0; i < count; ++i)
                {
                    GetType(mVariableTypes[cols[i]], ref dataTypeString, ref readString, ref writeString);
                    builder.Append(doubleTab).Append(tab).Append(string.Format("res &= GameDataUtils.{0}(stream, out {1})", readString, mVariableNames[cols[i]])).AppendLine();
                }
                builder.Append(doubleTab).Append(tab).Append("return res;").AppendLine();
                builder.Append(doubleTab).Append("}").AppendLine();
            }

            builder.Append(tab).Append("}").AppendLine();
        }

        public static void GetType(DataTypeEnum dt, ref string dataTypeString, ref string readString, ref string writeString)
        {
            switch (dt)
            {
                case DataTypeEnum.BOOL:
                    dataTypeString = "bool";
                    readString = "ReadBool";
                    writeString = "WriteBool";
                    return;
                case DataTypeEnum.BOOLLIST:
                    dataTypeString = "List<bool>";
                    readString = "ReadList";
                    writeString = "WriteList";
                    return;
                case DataTypeEnum.BYTE:
                    dataTypeString = "byte";
                    readString = "ReadByte";
                    writeString = "WriteByte";
                    return;
                case DataTypeEnum.BYTELIST:
                    dataTypeString = "List<byte>";
                    readString = "ReadList";
                    writeString = "WriteList";
                    return;
                case DataTypeEnum.FLOAT:
                    dataTypeString = "float";
                    readString = "ReadFloat";
                    writeString = "WriteFloat";
                    return;
                case DataTypeEnum.FLOATLIST:
                    dataTypeString = "List<float>";
                    readString = "ReadList";
                    writeString = "WriteList";
                    return;
                case DataTypeEnum.INT:
                    dataTypeString = "int";
                    readString = "ReadInt";
                    writeString = "WriteInt";
                    return;
                case DataTypeEnum.INTLIST:
                    dataTypeString = "List<int>";
                    readString = "ReadList";
                    writeString = "WriteList";
                    return;
                case DataTypeEnum.LONG:
                    dataTypeString = "long";
                    readString = "ReadLong";
                    writeString = "WriteLong";
                    return;
                case DataTypeEnum.LONGLIST:
                    dataTypeString = "list<long>";
                    readString = "ReadList";
                    writeString = "WriteList";
                    return;
                case DataTypeEnum.SHORT:
                    dataTypeString = "short";
                    readString = "ReadShort";
                    writeString = "WriteShort";
                    return;
                case DataTypeEnum.SHORTLIST:
                    dataTypeString = "List<short>";
                    readString = "ReadList";
                    writeString = "WriteList";
                    return;
                case DataTypeEnum.UINT:
                    dataTypeString = "uint";
                    readString = "ReadUInt";
                    writeString = "WriteUInt";
                    return;
                case DataTypeEnum.UINTLIST:
                    dataTypeString = "List<uint>";
                    readString = "ReadList";
                    writeString = "WriteList";
                    return;
                case DataTypeEnum.ULONG:
                    dataTypeString = "ulong";
                    readString = "ReadULong";
                    writeString = "WriteULong";
                    return;
                case DataTypeEnum.ULONGLIST:
                    dataTypeString = "List<ulong>";
                    readString = "ReadList";
                    writeString = "WriteList";
                    return;
                case DataTypeEnum.USHORT:
                    dataTypeString = "ushort";
                    readString = "ReadUShort";
                    writeString = "WriteUShort";
                    return;
                case DataTypeEnum.USHORTLIST:
                    dataTypeString = "List<ushort>";
                    readString = "ReadList";
                    writeString = "WriteList";
                    return;
                case DataTypeEnum.WORD:
                    dataTypeString = "ushort";
                    readString = "ReadUShort";
                    writeString = "WriteUShort";
                    return;
                case DataTypeEnum.WORDLIST:
                    dataTypeString = "List<ushort>";
                    readString = "ReadList";
                    writeString = "WriteList";
                    return;
                case DataTypeEnum.STRING:
                    dataTypeString = "string";
                    readString = "ReadString";
                    writeString = "WriteString";
                    return;
                case DataTypeEnum.STRINGLIST:
                    dataTypeString = "List<string>";
                    readString = "ReadList";
                    writeString = "WriteList";
                    return;
                case DataTypeEnum.VECTOR2:
                    dataTypeString = "UnityEngine.Vector2";
                    readString = "ReadVector2";
                    writeString = "WriteVector2";
                    return;
                case DataTypeEnum.VECTOR2LIST:
                    dataTypeString = "List<UnityEngine.Vector2>";
                    readString = "ReadList";
                    writeString = "WriteList";
                    return;
                case DataTypeEnum.VECTOR3:
                    dataTypeString = "UnityEngine.Vector3";
                    readString = "ReadVector3";
                    writeString = "WriteVector3";
                    return;
                case DataTypeEnum.VECTOR3LIST:
                    dataTypeString = "List<UnityEngine.Vector3>";
                    readString = "ReadList";
                    writeString = "WriteList";
                    return;
                case DataTypeEnum.VECTOR4:
                    dataTypeString = "UnityEngine.Vector4";
                    readString = "ReadVector4";
                    writeString = "WriteVector4";
                    return;
                case DataTypeEnum.VECTOR4LIST:
                    dataTypeString = "List<UnityEngine.Vector4>";
                    readString = "ReadList";
                    writeString = "WriteList";
                    return;
                case DataTypeEnum.QUATERNION:
                    dataTypeString = "UnityEngine.Quaternion";
                    readString = "ReadQuaternion";
                    writeString = "WriteQuaternion";
                    return;
                case DataTypeEnum.QUATERNIONLIST:
                    dataTypeString = "List<UnityEngine.Quaternion>";
                    readString = "ReadList";
                    writeString = "WriteList";
                    return;
                case DataTypeEnum.COLOR:
                    dataTypeString = "UnityEngine.Color";
                    readString = "ReadColor";
                    writeString = "WriteColor";
                    return;
                case DataTypeEnum.COLORLIST:
                    dataTypeString = "List<UnityEngine.Color>";
                    readString = "ReadList";
                    writeString = "WriteList";
                    return;
                case DataTypeEnum.VECTOR3INT:
                    dataTypeString = "UnityEngine.Vector3Int";
                    readString = "ReadVector3Int";
                    writeString = "WriteVector3Int";
                    return;
                case DataTypeEnum.VECTOR3INTLIST:
                    dataTypeString = "List<UnityEngine.Vector3Int>";
                    readString = "ReadList";
                    writeString = "WriteList";
                    return;
            }
        }
    }
}
