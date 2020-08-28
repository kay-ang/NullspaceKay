
using Nullspace;
using System.Collections.Generic;

namespace GameData
{
    public class ResourceConfig<T> : GameDataMap<int, T>, IResourceConfig where T : GameDataMap<int, T>, new()
    {
        public static readonly string FileUrl = "ResourceConfig#ResourceConfigs";
        public static readonly bool IsDelayInitialized = true;
        public static readonly List<string> KeyNameList = new List<string>() { "ID" };

        private int mID;
        private string mDirectory;
        private List<string> mNames;
        private bool mDelay;
        private StrategyType mStrategyType;
        private int mMaxSize;
        private int mMinSize;
        private int mLifeTime;
        private string mGoName;
        private bool mReset;
        private string mBehaviourName;
        private int mMask;
        private int mLevel;
        private bool mIsTimerOn;

        public ResourceConfig()
        {
            mNames = new List<string>();
        }

        public int ID { get { return mID; } private set { mID = value; } }
        public string Directory { get { return mDirectory; } private set { mDirectory = value; } }
        public List<string> Names { get { return mNames; } private set { mNames = value; } }
        public bool Delay { get { return mDelay; } private set { mDelay = value; } }
        public StrategyType StrategyType { get { return mStrategyType; } private set { mStrategyType = value; } }
        public int MaxSize { get { return mMaxSize; } private set { mMaxSize = value; } }
        public int MinSize { get { return mMinSize; } private set { mMinSize = value; } }
        public int LifeTime { get { return mLifeTime; } private set { mLifeTime = value; } }
        public string GoName { get { return mGoName; } private set { mGoName = value; } }
        public bool Reset { get { return mReset; } private set { mReset = value; } }
        public string BehaviourName { get { return mBehaviourName; } private set { mBehaviourName = value; } }
        public int Mask { get { return mMask; } private set { mMask = value; } }
        public int Level { get { return mLevel; } private set { mLevel = value; } }
        public bool IsTimerOn { get { return mIsTimerOn; } private set { mIsTimerOn = value; } }
    }
}
