using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class WorldController
{
    [HideInInspector]
    public GameObject mStick;
    [HideInInspector]
    public Rigidbody mStickBody;
    [HideInInspector]
    public BoxCollider mStickCollider;
    [HideInInspector]
    public PhysicMaterial mStickMat;
    [HideInInspector]
    public StickOnCollision mStickCollision;
    [HideInInspector]
    public Vector3 mStickVelocity;
    private void DeleteStick()
    {
        if (mStick != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(mStick);
#else
            Destroy(mStick);
#endif
        }
    }

    public void MakeStick()
    {
        DeleteStick();
        mStick = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mStickCollision = mStick.AddComponent<StickOnCollision>();
        InitializeStickMaterial(mStick);
        InitializeStickTransform(mStick);
        InitializeStickCollider(mStick);
        InitializeStickRidgebody(mStick);
        CalculateStickInertia(mStickBody, mStickCollider);
    }

    private void InitializeStickMaterial(GameObject stick)
    {
        MeshRenderer render = stick.GetComponent<MeshRenderer>();
        render.sharedMaterial = mGreenMat;
    }
    
    private void InitializeStickTransform(GameObject stick)
    {
        stick.transform.position = new Vector3(0.4f, 0.5f, -5.0f);
        stick.transform.rotation = Quaternion.identity;
        stick.transform.localScale = new Vector3(0.1f, 0.1f, 6.0f);
    }

    private void InitializeStickCollider(GameObject stick)
    {
        BoxCollider stickCollider = stick.GetComponent<BoxCollider>();
        // 材质设置
        PhysicMaterial mat = new PhysicMaterial("stick");
        // 不受摩擦力影响
        mat.dynamicFriction = 0;
        mat.staticFriction = 0;
        // 反弹系数设置
        mat.bounceCombine = PhysicMaterialCombine.Average;
        mat.bounciness = 0.8f;
        mStickMat = mat;
        stickCollider.material = mStickMat;
        stickCollider.center = Vector3.zero;
        stickCollider.size = new Vector3(1, 1, 1);
        mStickCollider = stickCollider;
    }

    private void InitializeStickRidgebody(GameObject stick)
    {
        Rigidbody body = stick.AddComponent<Rigidbody>();
        body.angularDrag = 0;
        body.drag = 0;
        body.useGravity = false;
        body.mass = 1;
        body.angularVelocity = Vector3.zero;
        body.velocity = Vector3.zero;
        body.maxAngularVelocity = 100000;
        mStickBody = body;
    }

    private void CalculateStickInertia(Rigidbody rd, BoxCollider bc)
    {
        Vector3 size = bc.bounds.size;
        float mass = rd.mass;
        float a = (1.0f * mass) / 12;
        Vector3 squareSize = Vector3.Scale(size, size);
        Vector3 inertia = a * new Vector3(squareSize.y + squareSize.z, squareSize.x + squareSize.z, squareSize.y + squareSize.x);
        AppendInfo(string.Format("inertia stick mann_{0}, auto_{1}", inertia.ToStringNew(), rd.inertiaTensor.ToStringNew()));
    }

    private void ApplyImpulsionToStick(Vector3 impulsion)
    {
        if (mStickBody != null)
        {
            CalculateStickVelocity(impulsion);
            // 这里是世界坐标系下施加线冲量
            mStickBody.AddForce(impulsion, ForceMode.Impulse);
        }
    }

    private void CalculateStickVelocity(Vector3 impulsion)
    {
        mStickVelocity = impulsion * (1.0f / mStickBody.mass);
    }

}
