using System;
using System.IO;
using UnityEngine;

namespace Nullspace
{
    public class SystemConfig
    {
        // 当前 app 基准 版本号
        public static readonly Vector3Int AppBaseVersion = new Vector3Int(1, 0, 0);
        // 当前 app 基准 版本号
        public static Vector3Int AppUpdateVersion = new Vector3Int(1, 0, 0);

        private static string FormatBaseVersion()
        {
            return string.Format("{0}.{1}.{2}", AppBaseVersion.x, AppBaseVersion.y, AppBaseVersion.z);
        }

        // 下载服务器 root 
        public static string RemoteHttp = "http://127.0.0.1:8080/";
        // 本地目录 root
        public static string LocalBaseDir = Application.persistentDataPath;
        // 远端下载服务器 root
        public static string RemoteBaseDir = string.Format("{0}/{1}", RemoteHttp, FormatBaseVersion());
        // 本地临时目录 
        public static string LocalTempDir = string.Format("{0}/Temp", LocalBaseDir);

        // ab包相对 LocalBaseDir 目录
        public static string LocalAssetbundleDir = string.Format("{0}/AssetBundles", LocalBaseDir);
        public static string RemoteAssetbundleDir = string.Format("{0}/AssetBundles", RemoteBaseDir);

        // 账号根目录
        public static string AccountDir = string.Format("{0}/Account", LocalBaseDir);

        // 日志目录
        public static string LoggerDir = string.Format("{0}/Logger", LocalBaseDir);

        // GameData 数据目录
        public static string GameDataDir = string.Format("{0}/GameData", LocalBaseDir);

        // 热更 数据目录
        public static string HotUpdateDir = string.Format("{0}/HotUpdate", LocalBaseDir);

        // 必须要在 Monobehaviour里 首次调用,不然  Application.persistentDataPath 报错
        static SystemConfig()
        {
            CreateDir(LocalTempDir);
            CreateDir(LocalAssetbundleDir);
            CreateDir(AccountDir);
            CreateDir(LoggerDir);
            CreateDir(GameDataDir);
            CreateDir(HotUpdateDir);
        }


        private static void CreateDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        
        public static void CreateAccountDir(string account)
        {
            CreateDir(string.Format("{0}/{1}", AccountDir, account));
        }

        // 考虑 relateDir = "/"
        // 末尾都不带 "/"
        // ("/", AssetbundleInfoFile, false)
        public static string GetFilePath(string relateDir, string fileName, bool isLocal)
        {
            DebugUtils.Assert(!string.IsNullOrEmpty(relateDir), "GetFilePath");
            string format = "{0}/{1}/{2}";
            if (relateDir == "/")
            {
                format = "{0}{1}{2}";
            }
            string baseDir = isLocal ? LocalAssetbundleDir : RemoteAssetbundleDir;
            return string.Format(format, baseDir, relateDir, fileName);
        }

        public static string GetLocalPathInAssetBundle(string name)
        {
            return string.Format("{0}/{1}", LocalAssetbundleDir, name);
        }

        public static string GetRemotePathInAssetbundle(string name)
        {
            return string.Format("{0}/{1}", RemoteAssetbundleDir, name);
        }
    }
}
