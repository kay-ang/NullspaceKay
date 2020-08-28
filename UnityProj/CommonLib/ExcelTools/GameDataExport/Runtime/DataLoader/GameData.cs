
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;

namespace Nullspace
{

    public partial class GameData
    {
        protected const string KeyNameListName = "KeyNameList";
        protected const string FileUrlName = "FileUrl";
        private static Dictionary<Type, List<string>> mTypeKeyLists = new Dictionary<Type, List<string>>();
        protected static List<string> GetKeyList(Type type)
        {
            if (!mTypeKeyLists.ContainsKey(type))
            {
                List<string> keyNameList = type.GetField(KeyNameListName).GetValue(null) as List<string>;
                mTypeKeyLists.Add(type, keyNameList);
            }
            return mTypeKeyLists[type];
        }

        #region not used
        public class PropertyWrapper
        {
            public string Name;
            public Type PropertyType;
            public Delegate Action; // DynamicInvoke, most performance cost,should avoid
        }

        private static Dictionary<Type, List<PropertyWrapper>> mProp2Actions = new Dictionary<Type, List<PropertyWrapper>>();
        protected static Type[] TypeParams = new Type[2];
        protected static List<PropertyWrapper> GetWrappers(Type type)
        {
            if (!mProp2Actions.ContainsKey(type))
            {
                List<PropertyWrapper> wrappers = new List<PropertyWrapper>();
                mProp2Actions.Add(type, wrappers);
                PropertyInfo[] props = type.GetProperties();
                TypeParams[0] = type;
                foreach (PropertyInfo prop in props)
                {
                    MethodInfo setMethod = prop.GetSetMethod(true);
                    PropertyWrapper wrapper = new PropertyWrapper();
                    wrapper.Name = prop.Name;
                    wrapper.PropertyType = prop.PropertyType;
                    try
                    {
                        TypeParams[1] = prop.PropertyType;
                        var act = typeof(Action<,>).MakeGenericType(TypeParams);
                        wrapper.Action = Delegate.CreateDelegate(act, null, setMethod);
                        wrappers.Add(wrapper);
                    }
                    catch (Exception e)
                    {
                        DebugUtils.Log(InfoType.Error, e.Message + e.StackTrace);
                    }
                }
            }
            return mProp2Actions[type];
        }
        protected static void PropertySet<Target, Value>(Delegate d, Target target, Value v)
        {
            Action<Target, Value> act = d as Action<Target, Value>;
            act(target, v);
        }
        #endregion
    }

    public partial class GameData
    {
        protected bool mIsInitialized = false;
        protected SecurityElement mOriginData = null;
        protected void SetOriginData(SecurityElement originData)
        {
            mOriginData = originData;
        }

        public virtual void InitializeNoneKey()
        {
            if (!mIsInitialized)
            {
                if (mOriginData != null && mOriginData.Attributes != null)
                {
                    Convert();
                }
                mOriginData = null; // 释放
                mIsInitialized = true;
            }
        }

        // 默认使用反射处理
        // 子类可重写
        protected virtual void Convert()
        {
            PropertyInfo[] props = GetType().GetProperties();
            List<string> keyNameList = GetKeyList(GetType());
            foreach (PropertyInfo prop in props)
            {
                if (keyNameList != null && keyNameList.Contains(prop.Name))
                {
                    continue;
                }
                if (mOriginData.Attributes.ContainsKey(prop.Name))
                {
                    string value = (string)mOriginData.Attributes[prop.Name];
                    var v = DataUtils.ToObject(value, prop.PropertyType);
                    prop.SetValue(this, v, null);
                }
            }
        }

        protected string GetValue(string propName)
        {
            if (mOriginData.Attributes.ContainsKey(propName))
            {
                return (string)mOriginData.Attributes[propName];
            }
            return null;
        }
    }

    public partial class GameData<T> : GameData where T : GameData<T>, new()
    {
        // 默认反射，最多支持两个Key
        // 子类重写
        protected virtual int InitializeKeys(ref object key1, ref object key2)
        {
            return InitializeKeys((T)this, ref key1, ref key2);
        }
        
        private static int InitializeKeys(T instance, ref object key1, ref object key2)
        {
            if (instance.mOriginData.Attributes != null)
            {
                List<string> keyNameList = typeof(T).GetField(KeyNameListName).GetValue(null) as List<string>;
                if (keyNameList == null || keyNameList.Count == 0)
                {
                    return 0;
                }
                DebugUtils.Assert(keyNameList.Count <= 2, "So Many Key: " + keyNameList.Count);
                for (int i = 0; i < keyNameList.Count; ++i)
                {
                    string key = keyNameList[i];
                    DebugUtils.Assert(instance.mOriginData.Attributes.ContainsKey(key), "Not Contain Key: " + key);
                    string nodeV = instance.mOriginData.Attributes[key].ToString();
                    PropertyInfo info = typeof(T).GetProperty(key);
                    object obj = DataUtils.ToObject(nodeV, info.PropertyType);
                    DebugUtils.Assert(obj != null, "not right key: " + key);
                    info.SetValue(instance, obj, null);
                    if (i == 0)
                    {
                        key1 = obj;
                    }
                    else if (i == 1)
                    {
                        key2 = obj;
                    }
                }
                return keyNameList.Count;
            }
            return 0;
        }

        protected static void InitByFileUrl()
        {
            string fileUrl = typeof(T).GetField(FileUrlName).GetValue(null) as string;
            LogLoadedBegin(fileUrl);
            GameDataManager.InitByFile<T>(fileUrl);
        }
        protected static bool IsImmediateLoad()
        {
            return GameDataManager.ForceImmediate || !(bool)typeof(T).GetField("IsDelayInitialized").GetValue(null);
        }
        protected static void LogLoadedBegin(string fileUrl)
        {
            DebugUtils.Log(InfoType.Info, string.Format("***** GameDataTypeName: {0} FileUrl: {1} Begin ***** ", typeof(T).FullName, fileUrl));
        }
        protected static void LogLoadedEnd(string dataInfo)
        {
            DebugUtils.Log(InfoType.Info, string.Format("***** GameDataTypeName: {0}, IsImmediateLoad: {1}, Info: {2} End ***** ", typeof(T).FullName, IsImmediateLoad(), dataInfo));
        }
    }
}
