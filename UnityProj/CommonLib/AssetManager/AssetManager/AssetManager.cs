using GameData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Concurrent;

namespace Nullspace
{
    public class AssetManager : Singleton<AssetManager>
    {
        public const string AppVersionInfoFile = "app_version_info";
        public const string AppVersionInfoTag = "app_version_infos";
        public const string AssetbundleInfoFile = "assetbundle_info";
        public const string AssetbundleInfosTag = "assetbundle_infos";
        
        private static WaitForEndOfFrame Wait = new WaitForEndOfFrame();
        // size 进度条处理
        public event Action<float> SizeProgress;
        // 数量 进度条 处理
        public event Action<int, int> CountProgress;
        // 结束 处理
        public event Action<string> Finished;
        // 下载请求
        private List<UnityWebRequest> mAssetbunlesWebRequest;
        // 文件大小
        private int mTotalSize;
        private int mDownloadSize;
        // 文件数量
        private int mTotalCount;
        private int mDownloadCount;

        // 更新版本数据
        private AppVersionUpdateData mVersionData;

        private void Awake()
        {
            mAssetbunlesWebRequest = new List<UnityWebRequest>();
            mTotalSize = 0;
            mDownloadSize = 0;
            mTotalCount = 0;
            mDownloadCount = 0;
            mVersionData = null;
        }

        public void StartDownload()
        {
            StartCoroutine(DownloadVersionUpdate());
        }

        private float SizePercent { get { return mTotalSize == 0 ? 1.0f : (mDownloadSize * 1.0f / mTotalSize); } }

        private void AddReq(UnityWebRequest req)
        {
            lock(this)
            {
                mAssetbunlesWebRequest.Add(req);
            }
        }

        private void RemoveReq(UnityWebRequest req)
        {
            lock (this)
            {
                mAssetbunlesWebRequest.Remove(req);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Finished = null;
            SizeProgress = null;
            CountProgress = null;
            mVersionData = null;
            StopAllCoroutines();
        }

        private IEnumerator DownloadVersionUpdate()
        {
            using (UnityWebRequest req = UnityWebRequest.Get(SystemConfig.GetFilePath("/", AppVersionInfoFile, false)))
            {
                UnityWebRequestAsyncOperation reqBin = req.SendWebRequest();
                yield return reqBin;
                if (req.isNetworkError || req.isHttpError)
                {
                    // 重新下载
                    DebugUtils.Log(InfoType.Error, string.Format("DiffAssets : {0}", SystemConfig.GetFilePath("/", AppVersionInfoFile, false)));
                }
                else
                {
                    // 保存到 临时目录
                    string text = req.downloadHandler.text;
                    File.WriteAllText(SystemConfig.LocalTempDir + "/" + AppVersionInfoFile, text);
                    GameDataManager.ChangeDir(SystemConfig.LocalTempDir);
                    List<AppVersionUpdateData> newDataInfos = AppVersionUpdateData.Select((item) => { return item.Start != null; });
                    GameDataManager.ClearFileData(AppVersionInfoFile);
                    GameDataManager.ChangeDir(SystemConfig.GameDataDir);
                    mVersionData = newDataInfos.Find((item)=> { return ContainVersion(item); });
                }
            }
            if (mVersionData != null)
            {
                yield return DownloadAssetBundles();
            }
        }

        private static bool ContainVersion(AppVersionUpdateData item)
        {
            Vector3Int version = SystemConfig.AppBaseVersion;
            if (item.Filters.Contains(version))
            {
                return false;
            }
            if (item.Start.x <= version.x && item.End.x >= version.x)
            {
                if (item.Start.y <= version.y && item.End.y >= version.y)
                {
                    if (item.Start.z <= version.z && item.End.z >= version.z)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private IEnumerator DownloadAssetBundles()
        {
            using (UnityWebRequest req = UnityWebRequest.Get(SystemConfig.GetFilePath("/", AssetbundleInfoFile, false)))
            {
                UnityWebRequestAsyncOperation reqBin = req.SendWebRequest();
                yield return reqBin;
                if (req.isNetworkError || req.isHttpError)
                {
                    // 重新下载
                    DebugUtils.Log(InfoType.Error, string.Format("DiffAssets : {0}", SystemConfig.GetFilePath("/", AssetbundleInfoFile, false)));
                }
                else
                {
                    if (req.downloadHandler.isDone)
                    {
                        GameDataManager.ChangeDir(SystemConfig.GameDataDir);
                        // 本地获取并解析, 全部拷贝一份
                        List<AssetbundleUpdateData> oldDataInfos = AssetbundleUpdateData.Select((item) => { return item.Name != null; });
                        // 清理不存在本地文件的实例
                        int notExistsCount = oldDataInfos.RemoveAll((item) => { return IsFileExist(item); });
                        DebugUtils.Log(InfoType.Warning, string.Format("notExistsCount {0}", notExistsCount));
                        // 再释放
                        GameDataManager.ClearFileData(AssetbundleInfoFile);
                        DebugUtils.Assert(AssetbundleUpdateData.IsOriginNull(), "wrong clear");
                        // 保存到 临时目录
                        string text = req.downloadHandler.text;
                        File.WriteAllText(SystemConfig.LocalTempDir + "/" + AssetbundleInfoFile, text);
                        // 切换到 临时目录
                        GameDataManager.ChangeDir(SystemConfig.LocalTempDir);
                        // 全部拷贝一份
                        List<AssetbundleUpdateData> newDataInfos = AssetbundleUpdateData.Select((item) => { return item.Name != null; });
                        // 释放
                        GameDataManager.ClearFileData(AssetbundleInfoFile);
                        DebugUtils.Assert(AssetbundleUpdateData.IsOriginNull(), "wrong clear");
                        GameDataManager.ChangeDir(SystemConfig.GameDataDir);
                        // 处理
                        List<AssetbundleUpdateData> addAssets = DiffAssetBundles(oldDataInfos, newDataInfos);
                        mDownloadSize = 0;
                        mTotalSize = 0;
                        mTotalCount = 0;
                        mDownloadCount = 0;
                        mAssetbunlesWebRequest.Clear();
                        mTotalCount = addAssets.Count;
                        addAssets.ForEach((item) => { mTotalSize += item.FileSize; });
                        foreach (AssetbundleUpdateData info in addAssets)
                        {
                            string url = SystemConfig.GetRemotePathInAssetbundle(info.Name);
                            UnityWebRequest abReq = UnityWebRequest.Get(url);
                            AddReq(abReq);
                            UnityWebRequestAsyncOperation reqAssetbundleReq = abReq.SendWebRequest();
                            reqAssetbundleReq.completed += (AsyncOperation operation) =>
                            {
                                if (abReq.isNetworkError || abReq.isHttpError)
                                {
                                    // 重新下载
                                    DebugUtils.Log(InfoType.Error, "wrong req: " + url);
                                }
                                else
                                {
                                    AddDownloadCount();
                                    byte[] data = abReq.downloadHandler.data;
                                    AddDownloadSize(data.Length);
                                    SaveLocalAssetBundle(info.Name, data);
                                    // 下载进度
                                    SizeProgress?.Invoke(SizePercent);
                                    CountProgress?.Invoke(mDownloadCount, mTotalCount);
                                }
                                RemoveReq(abReq);
                                abReq.Dispose();
                            };
                        }

                        // 开始进度
                        SizeProgress?.Invoke(SizePercent);
                        while (mAssetbunlesWebRequest.Count > 0)
                        {
                            yield return Wait;
                        }

                        Finished?.Invoke(mVersionData.InfoTips);
                    }
                }
            }
        }

        private void AddDownloadCount()
        {
            lock (this)
            {
                mDownloadCount += 1;
            }
        }

        private void AddDownloadSize(int size)
        {
            lock (this)
            {
                mDownloadSize += size;
            }
        }

        private bool IsFileExist(AssetbundleUpdateData info)
        {
            return File.Exists(SystemConfig.GetLocalPathInAssetBundle(info.Name));
        }

        private void RemoveLocalAssetBundle(AssetbundleUpdateData info)
        {
            string filePath = SystemConfig.GetLocalPathInAssetBundle(info.Name);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private void SaveLocalAssetBundle(string name, byte[] datas)
        {
            string filePath = SystemConfig.GetLocalPathInAssetBundle(name);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.WriteAllBytes(filePath, datas);
        }
        private List<AssetbundleUpdateData> DiffAssetBundles(List<AssetbundleUpdateData> oldData, List<AssetbundleUpdateData> newData)
        {
            List<string> shares = Share(newData, oldData);
            List<AssetbundleUpdateData> addAssets = Sub(newData, shares);
            List<AssetbundleUpdateData> removeAssets = Sub(oldData, shares);
            List<AssetbundleUpdateData> changedAssets = UpdatedItems(newData, oldData);
            addAssets.AddRange(changedAssets);
            foreach (AssetbundleUpdateData removeAsset in removeAssets)
            {
                RemoveLocalAssetBundle(removeAsset);
            }
            foreach (AssetbundleUpdateData changeAsset in changedAssets)
            {
                RemoveLocalAssetBundle(changeAsset);
            }
            return addAssets;
        }

        /// <summary>
        /// a and b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private List<string> Share(List<AssetbundleUpdateData> a, List<AssetbundleUpdateData> b)
        {
            List<string> shares = new List<string>();
            foreach (AssetbundleUpdateData infoDataA in a)
            {
                foreach (AssetbundleUpdateData infoDataB in b)
                {
                    if (infoDataA.Name == infoDataB.Name)
                    {
                        shares.Add(infoDataA.Name);
                        break;
                    }
                }
            }
            return shares;
        }

        /// <summary>
        /// a 有; b 没有
        /// a - b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>a - b</returns>
        private List<AssetbundleUpdateData> Sub(List<AssetbundleUpdateData> a, List<string> b)
        {
            List<AssetbundleUpdateData> externs = new List<AssetbundleUpdateData>();
            foreach (AssetbundleUpdateData infoDataA in a)
            {
                int idx = b.FindIndex((name) => { return infoDataA.Name == name; });
                if (idx == -1)
                {
                    externs.Add(infoDataA);
                }
            }
            return externs;
        }

        private List<AssetbundleUpdateData> UpdatedItems(List<AssetbundleUpdateData> newData, List<AssetbundleUpdateData> oldData)
        {
            List<AssetbundleUpdateData> updatedList = new List<AssetbundleUpdateData>();
            foreach (AssetbundleUpdateData infoDataA in newData)
            {
                foreach (AssetbundleUpdateData infoDataB in oldData)
                {
                    if (infoDataA.Name == infoDataB.Name)
                    {
                        if (infoDataA.Md5Hash != infoDataB.Md5Hash)
                        {
                            updatedList.Add(infoDataA);
                        }
                        break;
                    }
                }
            }
            return updatedList;
        }
    }
}
