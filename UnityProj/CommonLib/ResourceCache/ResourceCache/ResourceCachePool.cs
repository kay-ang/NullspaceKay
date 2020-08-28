using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameData;

namespace Nullspace
{


    public static class  DeviceLevel
    {
        public const int High = 0;
        public const int Mid = 1;
        public const int Low = 2;
    }

    public partial class ResourceCachePool
    {
        private static List<ResourceCacheEntity> CACHE = new List<ResourceCacheEntity>();

        public void AddToStrategy(int instanceId)
        {
            Strategy.Increase(instanceId);
        }

        public void RemoveFromStrategy(int instanceId)
        {
            Strategy.Decrease(instanceId);
        }

        public bool CanAcquire()
        {
            return Strategy.CanAcquire();
        }

        public void Release(ResourceCacheEntity entity)
        {
            RemoveFromStrategy(entity.InstanceId);
            if (IsValid(entity))
            {
                entity.SetParent(Parent, false);
                if (entity.LevelIndex != LevelIndex || !CacheOn || GetLifeTime() == 0)
                {
                    entity.Destroy();
                }
                else
                {
                    entity.SetActive(false);
                    Container.AddFirst(entity);
                    entity.Behaviour.StartLifeTimer();
                }
            }
        }

        public void ChangeLevelIndex(int levelIndex)
        {
            if (HasResourceName())
            {
                int oldLevelIndex = LevelIndex;
                SetLevelIndex(levelIndex);
                if (oldLevelIndex != LevelIndex)
                {
                    LoadAsset();
                    foreach (var item in Container)
                    {
                        item.Destroy();
                    }
                    Container.Clear();
                }
            }
        }
        
        public ResourceCacheEntity Acquire()
        {
            if (!IsInitialized)
            {
                throw new Exception("AcquireArray Not Initialize");
            }
            ResourceCacheEntity entity = null;
            if (IsInitialized && !IsLevelIgnore())
            {
                if (CanAcquire())
                {
                    AcquireArray(1, CACHE);
                    if (CACHE.Count > 0)
                    {
                        entity = CACHE[0];
                        CACHE.Clear();
                    }
                }
            }
            return entity;
        }

        public void AcquireArray(int num, List<ResourceCacheEntity> res)
        {
            if (!IsInitialized)
            {
                throw new Exception("AcquireArray Not Initialize");
            }
            res.Clear();
            if (Container.Count < num)
            {
                CreateEntity(num - Container.Count);
            }
            int index = 0;
            while (index < num)
            {
                ResourceCacheEntity entity = Container.First();
                Container.RemoveFirst();
                if (IsValid(entity))
                {
                    ResetOrigin(entity);
                    if (entity.Behaviour == null)
                    {
                        Type behaviourType = Type.GetType(GetBehaviourName());
                        Debug.Assert(behaviourType != null, "" + GetBehaviourName());
                        Debug.Assert(behaviourType != null, "null behaviourType");
                        entity.Behaviour = (ResourceCacheBehaviour)entity.GameObject.AddComponent(behaviourType);
                        entity.Behaviour.InitializeBase(this, entity);
                    }
                    entity.Behaviour.StopLifeTimer();
                    res.Add(entity);
                }
                index++;
            }
            Debug.Assert(index == num, "wrong");
        }

        private bool IsValid(ResourceCacheEntity entity)
        {
            return entity != null && entity.GameObject != null;
        }

        private bool IsLevelIgnore()
        {
            return false;
        }
    }

    public partial class ResourceCachePool
    {
        private GameObject Asset { get; set; }
        private int LevelIndex { get; set; }
        private Transform Parent { get; set; }
        private bool IsInitialized { get; set; }
        private StrategyBase Strategy { get; set; }
        private bool CacheOn { get; set; }
        private IResourceConfig Config { get; set; }
        private LinkedList<ResourceCacheEntity> Container { get; set; }
        private Vector3 OriginPos { get; set; }
        private Vector3 OriginScale { get; set; }
        private Quaternion OriginRotation { get; set; }
        public ResourceCachePools OwnedPools { get; set; }

        public void Initialize<T>(ResourceConfig<T> config, Transform parent, int quality, ResourceCachePools ownedPools, bool cacheOn) where T : GameDataMap<int, T>, new()
        {
            Config = config;
            Parent = parent;
            OwnedPools = ownedPools;
            CacheOn = cacheOn;
            Container = new LinkedList<ResourceCacheEntity>();
            Asset = null;
            OriginPos = Vector3.zero;
            OriginScale = Vector3.one;
            OriginRotation = Quaternion.identity;
            SetLevelIndex(quality);
            SetStrategy();
            InitialDelay();
            IsInitialized = true;
        }

        private void SetStrategy()
        {
            StrategyParam param = new StrategyParam(GetMaxSize(), OwnedPools);
            Strategy = StrategyFactory.GetStrategy(GetStrategyType(), param);
        }

        private void InitialDelay()
        {
            if (!IsDelayLoad())
            {
                LoadAsset();
                CreateEntity(GetMinSize());
            }
        }

        public void LoadAsset()
        {
            if (IsAssetSet() && Asset == null)
            {
#if UNITY_EDITOR
                string path = GetAssetFilePath("Assets");
                Asset = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
#endif
                if (Asset != null)
                {
                    OriginPos = Asset.transform.position;
                    OriginScale = Asset.transform.localScale;
                    OriginRotation = Asset.transform.rotation;
                }
            }
        }

        public void CreateEntity(int size)
        {
            int num = 0;
            while (num < size)
            {
                ResourceCacheEntity entity = CreateObjectEntity();
                entity.ManagerId = GetManagerId();
                entity.LevelIndex = LevelIndex;
                entity.IsTimerOn = IsTimerOn();
                entity.GameObject = InstanceGameObject();
                entity.SetParent(Parent, false);
                Container.AddLast(entity);
                num = num + 1;
            }
        }

        public int Count { get { return Container.Count; } }

        private bool IsAssetSet()
        {
            return GetResourceDirectory() != null && GetResourceName() != null;
        }

        public GameObject InstanceGameObject()
        {
            if (Asset == null && IsDelayLoad())
            {
                LoadAsset();
            }
            GameObject go = null;
            if (Asset != null)
            {
                go = GameObject.Instantiate(Asset);
            }   
	        else
            {
                go = new GameObject();
            }
            go.name = GetGameObjectName();
            go.SetActive(false);
            return go;
        }

        public void Clear()
        {
            foreach (var item in Container)
            {
                item.Destroy();
            }
            Container.Clear();
            Config = null;
            Strategy.Clear();
            Strategy = null;
            IsInitialized = false;
        }

        public ResourceCacheEntity CreateObjectEntity()
        {
            return new ResourceCacheEntity();
        }

        private void ResetOrigin(ResourceCacheEntity entity)
        {
            entity.SetPos(OriginPos);
            entity.SetScale(OriginScale);
            entity.SetRotate(OriginRotation);
        }

        public void RemoveEntity(ResourceCacheEntity entity)
        {
            Container.Remove(entity);
            entity.Destroy();
        }

        public void SetLevelIndex(int levelIndex)
        {
            LevelIndex = levelIndex;
            if (Config.Names != null)
            {
                int len = Config.Names.Count;
                if (levelIndex >= len)
                {
                    LevelIndex = len - 1;
                }
            }
        }

        public string GetAssetFilePath(string baseDir)
        {
            string format = baseDir != null ? baseDir + "/{0}/{1}" : "{0}/{1}";
            format = format.Replace("//", "/");
            format = format.Replace("\\", "/");
            string name = GetResourceName();
            if (!name.EndsWith(".prefab"))
            {
                format += ".prefab";
            }
            return string.Format(format, GetResourceDirectory(), GetResourceName());
        }

        public string GetResourceDirectory()
        {
            return Config.Directory;
        }

        public string GetResourceName()
        {
            if (HasResourceName())
            {
                return Config.Names[LevelIndex];
            }
            return null;
        }

        private bool HasResourceName()
        {
            return Config.Names != null && Config.Names.Count > 0;
        }

        public string GetGameObjectName()
        {
            string name = Config.GoName;
            if (name == null)
            {
                name = "" + GetManagerId();
            }
            return name;
        }

        public StrategyType GetStrategyType()
        {
            return Config.StrategyType;
        }

        public bool IsDelayLoad()
        {
            return Config.Delay;
        }

        public int GetMaxSize()
        {
            return Config.MaxSize;
        }

        public int GetManagerId()
        {
            return Config.ID;
        }

        public int GetMinSize()
        {
            return Config.MinSize;
        }

        public int GetLifeTime()
        {
            return Config.LifeTime;
        }

        public string GetBehaviourName()
        {
            return Config.BehaviourName;
        }

        public int GetMask()
        {
            return Config.Mask;
        }

        public bool IsTimerOn()
        {
            return Config.IsTimerOn;
        }
    }
}
