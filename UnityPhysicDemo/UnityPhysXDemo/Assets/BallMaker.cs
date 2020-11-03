using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class WorldController
{
    [HideInInspector]
    public GameObject mBall;
    [HideInInspector]
    public Rigidbody mBallBody;
    [HideInInspector]
    public SphereCollider mBallCollider;
    [HideInInspector]
    public PhysicMaterial mBallMat;

    private void DeleteBall()
    {
        if (mBall != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(mBall);
#else
            Destroy(mBall);
#endif
        }
    }

    public void MakeBall()
    {
        DeleteBall();
        mBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        InitializeBallMaterial(mBall);
        InitializeBallTransform(mBall);
        InitializeBallCollider(mBall);
        InitializeBallRidgebody(mBall);
        CalculateBallInertia(mBallBody, mBallCollider);
    }

    private void InitializeBallMaterial(GameObject ball)
    {
        MeshRenderer render = ball.GetComponent<MeshRenderer>();
        render.sharedMaterial = mWhiteMat;
    }

    private void InitializeBallTransform(GameObject ball)
    {
        ball.transform.position = new Vector3(0, 0.5f, 0);

    }

    private void InitializeBallCollider(GameObject ball)
    {
        SphereCollider ballCollider = ball.GetComponent<SphereCollider>();
        // 材质设置
        PhysicMaterial mat = new PhysicMaterial("ball");
        // 不受摩擦力影响
        mat.dynamicFriction = 0;
        mat.staticFriction = 0;
        // 反弹系数设置
        mat.bounceCombine = PhysicMaterialCombine.Average;
        mat.bounciness = 0.8f;
        mBallMat = mat;
        ballCollider.material = mBallMat;
        ballCollider.center = Vector3.zero;
        ballCollider.radius = 0.5f;
        mBallCollider = ballCollider;
    }

    private void InitializeBallRidgebody(GameObject ball)
    {
        Rigidbody body = ball.AddComponent<Rigidbody>();
        body.angularDrag = 0;
        body.drag = 0;
        body.useGravity = false;
        body.mass = 1;
        body.angularVelocity = Vector3.zero;
        body.velocity = Vector3.zero;
        body.maxAngularVelocity = 100000;
        mBallBody = body;
    }

    private void CalculateBallInertia(Rigidbody rd, SphereCollider sc)
    {
        float v = 0.4f * rd.mass * sc.radius * sc.radius;
        Vector3 inertia = Vector3.one * v;
        AppendInfo(string.Format("inertia ball mann_{0}, auto_{1}", inertia.ToStringNew(), rd.inertiaTensor.ToStringNew()));
    }
}
