using GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

public class AssetbundleTree
{
    private static HashSet<string> FilterExtensions = new HashSet<string>()
    {
        "meta",
        "cs",
        "unity",
        "xml"
    };

    public class FileItem
    {
        public DirItem Parent;
        public string FilePath; // 文件路径
        public int Levels;      // 被引用的次数
        public string AssetbundleName;
    }

    public class DirItem
    {
        public List<FileItem> Files;
        public List<DirItem> Dirs;
        public DirItem()
        {
            Files = new List<FileItem>();
            Dirs = new List<DirItem>();
        }
    }

    public static void GetDependences(string path, bool recursive, Dictionary<string, DirItem> dirItems, Dictionary<string, FileItem> fileItems)
    {
        string[] dependencies = AssetDatabase.GetDependencies(path, recursive);
        foreach (string dependence in dependencies)
        {
            string filePath = dependence.Replace("\\", "/");
            if (!fileItems.ContainsKey(filePath))
            {
                fileItems.Add(filePath, new FileItem() { FilePath = filePath, Levels = 0, Parent = null });
                string dirName = Path.GetDirectoryName(filePath).Replace("\\", "/");
                if (!dirItems.ContainsKey(dirName))
                {
                    dirItems.Add(dirName, new DirItem());
                }
                dirItems[dirName].Files.Add(fileItems[filePath]);
                fileItems[filePath].Parent = dirItems[dirName];
            }
            fileItems[filePath].Levels++;
        }
    }

    public static void AssetbundleDependencies()
    {
        Dictionary<string, DirItem> dirItems = new Dictionary<string, DirItem>();
        Dictionary< string, FileItem > fileItems = new Dictionary<string, FileItem>();
        foreach (BuildAssetbundleConfig data in BuildAssetbundleConfig.Data)
        {
            GetDependences(data.Path, true, dirItems, fileItems);
        }
    }

    public static void GetFiles(string dir, bool recursive)
    {
        string[] files = Directory.GetFiles(dir);
        foreach (string file in files)
        {
            string extension = Path.GetExtension(file);
            if (FilterExtensions.Contains(extension))
            {
                continue;
            }
        }
        if (recursive)
        {
            string[] dirs = Directory.GetDirectories(dir);
            foreach (string subDir in dirs)
            {
                GetFiles(subDir, recursive);
            }
        }
    }
}