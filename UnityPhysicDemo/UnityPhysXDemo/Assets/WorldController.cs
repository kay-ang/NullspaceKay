using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public partial class WorldController
{
    public static Vector3 gImpulsion = Vector3.forward * 10;
    public static WorldController Instance = null;
    [HideInInspector]
    public Vector3 mStickPos;
    [HideInInspector]
    public Vector3 mBallPos;
    [HideInInspector]
    public Quaternion mStickRotation;
    [HideInInspector]
    public Quaternion mBallRotation;

    private void Awake()
    {
        ClearLogInfo();
        CreateBallAndStick();
        InitializeCamera();
        Instance = this;
    }

    private void CreateBallAndStick()
    {
        MakeBall();
        MakeStick();
        InitialzieInertiaRotation();
    }

    private void Update()
    {
        // 由于在碰撞Enter时，位置已经更新。这里实际上是拿到更新前的位置和朝向
        mStickPos = mStick.transform.position;
        mBallPos = mBall.transform.position;
        mStickRotation = mStick.transform.rotation;
        mBallRotation = mStick.transform.rotation;
        InputListen();
    }

    private void Reset()
    {
        StopAllCoroutines();
        CreateBallAndStick();
    }
    private void InitializeCamera()
    {
        Camera main = Camera.main;
        main.transform.position = new Vector3(1.65f, 1.26f, 0.34f);
        main.transform.rotation = Quaternion.Euler(35.514f, -143.198f, 5.042f);
        main.transform.localScale = Vector3.one;
    }

    private void InitialzieInertiaRotation()
    {
        mBallBody.inertiaTensorRotation = Quaternion.identity;
        mStickBody.inertiaTensorRotation = Quaternion.identity;
    }

}


public partial class WorldController : MonoBehaviour
{
    [HideInInspector]
    private int mClickCnt = 0;
    // 世界系 z轴
    
    
    private void InputListen()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            mClickCnt++;
            Time.timeScale = 0.1f;
            if ((mClickCnt & 1) == 1)
            {
                ApplyImpulsionToStick(gImpulsion);
                StartCoroutine(PrintApplyImpulsion());
            }
            else
            {
                ClearLogInfo();
                Reset();
            }
        }
    }
    
    IEnumerator PrintApplyImpulsion()
    {
        yield return new WaitForFixedUpdate();
        AppendInfo(string.Format("stick 速度 auto_{0} mann_{1}", mStickBody.velocity.ToStringNew(), mStickVelocity.ToStringNew()));
    }


}

public partial class WorldController
{
    public Material mWhiteMat;
    public Material mGreenMat;

    public Text mInfoText;
    private StringBuilder mInfoCache = new StringBuilder();

    private void ClearLogInfo()
    {
        mInfoCache.Length = 0;
        mInfoCache.Append("按'H'键:开始或重新开始").AppendLine();
    }
    public void AppendInfo(string info)
    {
        mInfoCache.Append(info).AppendLine();
        mInfoText.text = mInfoCache.ToString();
    }
}
