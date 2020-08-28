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
        Properties cfg = Properties.Create("config.txt");
        InitializeClient(cfg);
        InitializeLogger(cfg);
        TimerTaskQueue.Instance.Reset();
        FrameTimerTaskHeap.Instance.Reset();
        SequenceManager.Instance.Clear();
        IntEventDispatcher.Cleanup();
        NetworkEventHandler.Initialize();
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

}
