using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StickOnCollision : MonoBehaviour
{
    // 调用这里时，物体的Transform已经更改
    // 手动计算，需要获取更改前的数据，即上一帧的数据；这些数据保存在WorldController Update中
    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] cps = collision.contacts;
        if (cps.Length == 1)
        {
            WorldController.Instance.AppendInfo("************************************Unity计算结果**************************************************");
            WorldController.Instance.AppendInfo(string.Format("auto_collision Contacts={0},impulse={1}, 大小={2}, relativeVelocity={3}", cps.Length, collision.impulse.ToStringNew(), collision.impulse.magnitude, collision.relativeVelocity.ToStringNew()));
            WorldController.Instance.AppendInfo(string.Format("auto_速度 ball  v={0}, w={1}, 位置={2}", WorldController.Instance.mBallBody.velocity.ToStringNew(), WorldController.Instance.mBallBody.angularVelocity.ToStringNew(), WorldController.Instance.mBall.transform.position.ToStringNew()));
            WorldController.Instance.AppendInfo(string.Format("auto_速度 stick v={0}, w={1}", WorldController.Instance.mStickBody.velocity.ToStringNew(), WorldController.Instance.mStickBody.angularVelocity.ToStringNew()));
            ContactPoint cp = cps[0];
            WorldController.Instance.AppendInfo(string.Format("auto_pos={0},separation={1},normal={2}", cp.point.ToStringNew(), cp.separation, cp.normal.ToStringNew()));
            WorldController.Instance.AppendInfo("************************************模拟开始**************************************************");
            SelfCalculate(cp);
            WorldController.Instance.AppendInfo("************************************模拟结束**************************************************");
        }
        else
        {
            WorldController.Instance.AppendInfo("只测试1个接触点");
        }
    }

    private void SelfCalculate(ContactPoint cp)
    {
        Vector3 point = cp.point;
        Vector3 selfNormal = WorldController.Instance.mBallPos - point;
        selfNormal.Normalize();
        Vector3 ra = cp.point - WorldController.Instance.mBallPos;
        Vector3 rb = cp.point - WorldController.Instance.mStickPos;
        WorldController.Instance.AppendInfo(string.Format("(事先已知碰撞类型)直接拿unity的碰撞点计算，法线selfNormal={0}.与unity计算的方向相反。", selfNormal.ToStringNew()));
        WorldController.Instance.AppendInfo(string.Format("使用selfNormal参与后面计算。球是A，杆是B物体: ra={0}, rb={1}", ra.ToStringNew(), rb.ToStringNew()));
        WorldController.Instance.AppendInfo("通过对比inertia相同，可直接使用unity自带的");
        // 初始化时设置为 PhysicMaterialCombine.Average
        float epsion = (WorldController.Instance.mStickMat.bounciness + WorldController.Instance.mBallMat.bounciness) * 0.5f;
        WorldController.Instance.AppendInfo(string.Format("计算 epsion type={0}, value=({1}+{2})*0.5={3}", WorldController.Instance.mStickMat.bounceCombine, WorldController.Instance.mStickMat.bounciness, WorldController.Instance.mBallMat.bounciness, epsion));
        // 相对速度计算：已知球初始时速度和杆施加线冲量后的速度值
        Vector3 va = Vector3.zero;
        Vector3 wa = Vector3.zero;
        Vector3 vb = WorldController.Instance.mStickVelocity;
        Vector3 wb = Vector3.zero;
        Vector3 vrel = (va + Vector3.Cross(wa, ra)) - (vb + Vector3.Cross(wb, rb));
        WorldController.Instance.AppendInfo(string.Format("计算 vel relative={0}。对比unity计算结果", vrel.ToStringNew()));
        // 计算b
        float b = (1 + epsion) * Vector3.Dot(vrel, selfNormal);
        WorldController.Instance.AppendInfo(string.Format("计算 b={0}", b));
        // 计算有效质量
        Vector3 raxn = Vector3.Cross(ra, selfNormal);
        Vector3 rbxn = Vector3.Cross(rb, selfNormal);
        Quaternion qa = WorldController.Instance.mBallRotation * WorldController.Instance.mBallBody.inertiaTensorRotation;
        Quaternion qb = WorldController.Instance.mStickRotation * WorldController.Instance.mStickBody.inertiaTensorRotation;

        WorldController.Instance.AppendInfo(string.Format("stick={0}, ball={1}", WorldController.Instance.mStickBody.inertiaTensor.ToStringNew(), WorldController.Instance.mBallBody.inertiaTensor.ToStringNew()));

        Vector3 Iada = qa * Vector3.Scale(Quaternion.Inverse(qa) * raxn, WorldController.Instance.mBallBody.inertiaTensor.Invert());
        Vector3 Ibdb = qb * Vector3.Scale(Quaternion.Inverse(qb) * rbxn, WorldController.Instance.mStickBody.inertiaTensor.Invert());
        float meffective = 1.0f / WorldController.Instance.mBallBody.mass + Vector3.Dot(Iada, raxn) + 1.0f / WorldController.Instance.mStickBody.mass + Vector3.Dot(Ibdb, rbxn);
        WorldController.Instance.AppendInfo(string.Format("计算 meffective={0}", meffective));
        // 计算j
        float j = -b / meffective;
        Vector3 J = j * selfNormal;
        WorldController.Instance.AppendInfo(string.Format("计算 j={0}, 冲量J={1}", j, J.ToStringNew()));
        WorldController.Instance.AppendInfo("注意：实际物理引擎，j并不这么直接计算");
        // 更新速度
        Vector3 raJ = Vector3.Cross(ra, J);
        Vector3 rbJ = Vector3.Cross(rb, J);

        va = va + J * 1.0f / WorldController.Instance.mBallBody.mass;
        wa = wa + qa * Vector3.Scale(Quaternion.Inverse(qa) * raJ, WorldController.Instance.mBallBody.inertiaTensor.Invert());

        vb = vb - J * 1.0f / WorldController.Instance.mBallBody.mass;
        wb = wb - qb * Vector3.Scale(Quaternion.Inverse(qb) * rbJ, WorldController.Instance.mStickBody.inertiaTensor.Invert());

        // 球的位置计算
        Vector3 pos = WorldController.Instance.mBallPos + va * Time.fixedDeltaTime;
        WorldController.Instance.AppendInfo(string.Format("mann_速度 ball  v={0}, w={1}, 位置={2}", va.ToStringNew(), wa.ToStringNew(), pos.ToStringNew()));
        WorldController.Instance.AppendInfo(string.Format("mann_速度 stick v={0}, w={1}", vb.ToStringNew(), wb.ToStringNew()));
    }

    private void OnCollisionStay(Collision collision)
    {
        WorldController.Instance.AppendInfo("测试例子，不应该有Stay");
    }

    private void OnCollisionExit(Collision collision)
    {
        WorldController.Instance.AppendInfo("碰撞分离");
        Time.timeScale = 0;
    }
}