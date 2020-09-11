using GameData;
using Nullspace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class AssetbundleTree
{

    private static Dictionary<string, DirItem> dirItems = new Dictionary<string, DirItem>();
    private static Dictionary<string, FileItem> fileItems = new Dictionary<string, FileItem>();
    private static HashSet<string> TravFilesCache = new HashSet<string>();

    private static HashSet<string> FilterExtensions = new HashSet<string>()
    {
        ".meta",
        ".cs",
        ".unity",
        ".xml",
        ".DS_Store",
        ".h",
        ".mm",
        ".cpp",
        ".m"
    };

    private class FileItem
    {
        public string FilePath; // 文件路径
        public int Levels;      // 被引用的次数
    }

    private class DirItem
    {
        public string DirPath;
        public List<FileItem> Files;
        public bool IsConfig;
        public bool Altas;
        public DirItem NearestConfigParent;
        public DirItem(string dirPath)
        {
            DirPath = dirPath;
            Files = new List<FileItem>();
            IsConfig = false;
            Altas = false;
            NearestConfigParent = null;
        }
    }

    public static void SetAssetABNames()
    {
        dirItems.Clear();
        fileItems.Clear();
        TravFilesCache.Clear();
        List<BuildAssetbundleConfig> configs = ReadConfig();
        GroupConfigDir(configs);
        GroupNearestConfig();
        PrintJustDependenceFiles();
        SetAssetbundleName();
    }

    private static void GetDependences(string path, bool recursive, Dictionary<string, DirItem> dirItems, Dictionary<string, FileItem> fileItems)
    {
        string[] dependencies = AssetDatabase.GetDependencies(path.Replace("\\", "/"), recursive);
        foreach (string dependence in dependencies)
        {
            string filePath = dependence.Replace("\\", "/");
            string fileKey = filePath.Replace("Assets/", "");
            if (!fileItems.ContainsKey(fileKey))
            {
                fileItems.Add(fileKey, new FileItem() { FilePath = filePath, Levels = 0 });
                string dirName = Path.GetDirectoryName(fileKey).Replace("\\", "/");
                if (!dirItems.ContainsKey(dirName))
                {
                    DirItem result = new DirItem(dirName);
                    dirItems.Add(dirName, result);
                }
                dirItems[dirName].Files.Add(fileItems[fileKey]);
            }
            fileItems[fileKey].Levels++;
        }
    }

    private class CompareConfig : IComparer<BuildAssetbundleConfig>
    {
        public int Compare(BuildAssetbundleConfig x, BuildAssetbundleConfig y)
        {
            return y.Path.CompareTo(x.Path);
        }
    }

    private static List<BuildAssetbundleConfig> ReadConfig()
    {
        GameDataManager.ChangeDir(".");
        List<BuildAssetbundleConfig> configs = BuildAssetbundleConfig.Select((item) => { return item.Path != null; });
        GameDataManager.ClearAllData();
        // 路径 从长到短 排序，先处理子目录
        configs.Sort(new CompareConfig());
        return configs;
    }

    private static void GroupConfigDir(List<BuildAssetbundleConfig> configs)
    {
        // 配置项 所有 依赖项
        foreach (BuildAssetbundleConfig data in configs)
        {
            List<string> files = new List<string>();
            // 目录标识
            string dirName = data.Path.Replace("\\", "/");
            if (!dirItems.ContainsKey(dirName))
            {
                dirItems.Add(dirName, new DirItem(dirName));
            }
            dirItems[dirName].IsConfig = true;
            dirItems[dirName].Altas = data.Altas;
            // 取出配置项下所有的文件(不包含过滤文件类型)
            GetFiles("Assets/" + data.Path, true, files);
            foreach (string file in files)
            {
                GetDependences(file, true, dirItems, fileItems);
            }
        }
    }

    private static void GroupNearestConfig()
    {
        // 非配置项 dir
        foreach (var pair in dirItems)
        {
            DirItem origin = pair.Value;
            DirItem item = origin;
            string dirName = item.DirPath;
            while (item != null)
            {
                if (item.IsConfig)
                {
                    origin.NearestConfigParent = item;
                    break;
                }
                item = null;
                dirName = Path.GetDirectoryName(dirName).Replace("\\", "/");
                if (dirItems.ContainsKey(dirName))
                {
                    item = dirItems[dirName];
                }
            }
        }
    }

    private static void PrintJustDependenceFiles()
    {
        // 打印无配置项，只是被依赖
        List<string> nonConfigs = new List<string>();
        foreach (DirItem item in dirItems.Values)
        {
            if (item.NearestConfigParent == null)
            {
                string line = string.Format("Dir: {0}", item.DirPath);
                foreach (FileItem fileItem in item.Files)
                {
                    nonConfigs.Add(string.Format("{0} : {1}", fileItem.FilePath, fileItem.Levels));
                }
            }
        }
    }

    private static void SetAssetbundleName()
    {
        foreach (DirItem item in dirItems.Values)
        {
            if (item.NearestConfigParent != null)
            {
                string abName = item.NearestConfigParent.DirPath.Replace("/", "_");
                foreach (FileItem fileItem in item.Files)
                {
                    string filePath = fileItem.FilePath;
                    AssetImporter importer = AssetImporter.GetAtPath(filePath); // "assets/**"
                    importer.assetBundleName = abName;
                    importer.assetBundleVariant = null;
                    if (item.NearestConfigParent.Altas)
                    {
                        TextureImporter texImporter = importer as TextureImporter;
                        texImporter.spritePackingTag = abName;
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
    }

    private static void GetFiles(string dir, bool recursive, List<string> filePath)
    {
        string[] files = Directory.GetFiles(dir);
        foreach (string file in files)
        {
            string extension = Path.GetExtension(file);
            if (FilterExtensions.Contains(extension))
            {
                continue;
            }
            if (TravFilesCache.Contains(file))
            {
                continue;
            }
            TravFilesCache.Add(file);
            filePath.Add(file);
        }
        if (recursive)
        {
            string[] dirs = Directory.GetDirectories(dir);
            foreach (string subDir in dirs)
            {
                GetFiles(subDir, recursive, filePath);
            }
        }
    }
}