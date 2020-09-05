using Nullspace;
using UnityEngine;
using Logger = Nullspace.Logger;

public class World : MonoBehaviour
{
    private static void InitializeLogger(Properties cfg)
    {
        string logConfig = cfg.GetString("log_config_file", null);
        if (logConfig != null)
        {
            Properties config = Properties.Create(logConfig);
            Logger.Instance.Initialize(config);
        }
    }

    private static void InitializeClient(Properties cfg)
    {
        string serverConfig = cfg.GetString("server_config_file", null);
        if (serverConfig != null)
        {
            Properties config = Properties.Create(serverConfig);
            NetworkClient.Instance.Initialize(config);
        }
    }

    void Awake()
    {
        DebugUtils.AddLogAction(DebugLog);
        Properties cfg = Properties.Create("config.txt");
        InitializeClient(cfg);
        InitializeLogger(cfg);
        TimerTaskQueue.Instance.Reset();
        FrameTimerTaskHeap.Instance.Reset();
        SequenceManager.Instance.Clear();
        IntEventDispatcher.Cleanup();
        NetworkEventHandler.Initialize();
        // 资源
        BundleManager.Instance.Initialize(Application.streamingAssetsPath + "/PC", "PC");
    }

    void DebugLog(InfoType type, string info)
    {
        Debug.Log(info);
    }

    // Update is called once per frame
    void Update()
    {
        // 协议
        NetworkEventHandler.Update();
        TimerTaskQueue.Instance.Tick();
        FrameTimerTaskHeap.Instance.Tick();
        SequenceManager.Instance.Tick();
    }

    void OnDestroy()
    {
        NetworkClient.Instance.Stop();
        NetworkEventHandler.Clear();
        SequenceManager.Instance.Clear();
        TimerTaskQueue.Instance.Reset();
        FrameTimerTaskHeap.Instance.Reset();
        IntEventDispatcher.Cleanup();
        Logger.Instance.Stop();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Load Ab"))
        {
            BundleManager.Instance.LoadBundleAsync("prefabs_test1.hd", null);
        }
        if (GUILayout.Button("Unload Ab"))
        {
            BundleManager.Instance.UnloadBundle("prefabs_test1.hd"); 
        }
    }
}
