
using UnityEngine;

namespace Nullspace
{
    public partial class NavTargetController
    {
        /// <summary>
        /// 到达边界处理
        /// </summary>
        public void OnReachBorder()
        {
            IsCheckReached = false;
            OnBorderRevert();
        }

        /// <summary>
        /// 到达边界后，反向飞行
        /// </summary>
        public void OnBorderRevert()
        {
            Vector3 refPos = Vector3.zero;
            Vector3 refDir = Vector3.zero;
            Vector3 nextPos = Vector3.zero;
            Reflect(ref BigBound, ref NormalBound, Pos, Direction, ref refPos, ref refDir, ref nextPos);
            Pos = refPos;
            Direction = refDir;
        }

        /// <summary>
        /// 到达目标物处理
        /// </summary>
        public void OnReachTarget()
        {
            IsCheckReached = false;
            // todo
        }

        /// <summary>
        /// 目标失效处理
        /// </summary>
        public void HandleInvalid()
        {
            IsCheckReached = false;
            // todo
        }

        /// <summary>
        ///  检查是否与其他物体碰撞
        /// </summary>
        public void CheckCollide()
        {
            if (!FreezeCollide && IsPlaying)
            {
                // 需要自定义 复杂的碰撞检测时，开启
                // todo
            }
        }
    }

    /// <summary>
    /// 目标对象处理。
    /// 这里冗余在一起，方便后面不重复 new 。而 会缓存
    /// </summary>
    public partial class NavTargetController
    {

        /// <summary>
        /// 设置 默认朝向交点 为 对象目标
        /// </summary>
        public void SetTarget()
        {
            TargetType = NavTargetType.NONE;
            IsCheckReached = false;
        }

        /// <summary>
        /// 设置固定位置的目标点
        /// </summary>
        /// <param name="targetPos">目标点</param>
        /// <param name="isBorder">是否是边界点</param>
        public void SetTarget(Vector3 targetPos)
        {
            TargetFollow = null;
            targetPos.y = 0;
            TargetPos = targetPos;
            IsCheckReached = true;
            TargetType = NavTargetType.POINT;
        }

        /// <summary>
        /// 设置移动物体为对象目标
        /// </summary>
        public void SetTarget(NavTargetController follow)
        {
            TargetFollow = follow;
            TargetType = NavTargetType.MOVABLE;
            IsCheckReached = true;
        }

        /// <summary>
        /// 获取目标位置
        /// </summary>
        /// <returns></returns>
        public Vector3 GetTargetPosition()
        {
            switch (TargetType)
            {
                case NavTargetType.POINT:
                    return TargetPos;
                case NavTargetType.MOVABLE:
                    Vector3 p = TargetFollow != null ? TargetFollow.transform.position : Vector3.zero;
                    p.y = 0;
                    return p;
            }
            return Pos;
        }

        /// <summary>
        /// 目标是否有效
        /// </summary>
        /// <returns></returns>
        public bool IsTargetValid()
        {
            switch (TargetType)
            {
                case NavTargetType.POINT:
                    return NormalBound.Contains(TargetPos);
                case NavTargetType.MOVABLE:
                    if (TargetFollow != null && TargetFollow.gameObject.activeSelf)
                    {
                        Vector3 pos = TargetFollow.transform.position;
                        pos.y = 0;
                        return NormalBound.Contains(pos);
                    }
                    return false;
            }
            return false;
        }

    }

    public partial class NavTargetController : MonoBehaviour
    {
        public static float Height = 0;
        /// <summary>
        /// 速度
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// 位置，实时变化
        /// </summary>
        public Vector3 Pos { get { Vector3 pos = transform.position; pos.y = 0; return pos; } set { value.y = Height; transform.position = value; } }

        /// <summary>
        /// 朝向，实时变化
        /// </summary>
        public Vector3 Direction { get { return transform.forward; } set { value.y = 0; transform.forward = value.normalized; } }

        /// <summary>
        /// 碰撞范围:平方
        /// </summary>
        public float SquareDistanceThreshold { get; set; }

        /// <summary>
        /// 控制：能否移动
        /// </summary>
        public bool FreezeMove { get; set; }

        /// <summary>
        /// 控制：能否改变朝向
        /// </summary>
        public bool FreezeRotate { get; set; }

        /// <summary>
        /// 控制：能否与其他物体碰撞
        /// </summary>
        public bool FreezeCollide { get; set; }

        /// <summary>
        /// 控制：是否检查达到目标点
        /// </summary>
        public bool IsCheckReached { get; set; }

        /// <summary>
        /// 碰撞检测是否在Lua层处理
        /// </summary>
        public bool IsLuaCollideCheck { get; set; }

        /// <summary>
        /// 朝目标移动
        /// </summary>
        private Vector3 TargetPos;  // 目标为点
        private NavTargetController TargetFollow;// 目标为移动物体

        /// <summary>
        /// 目标类别
        /// </summary>
        private NavTargetType TargetType;

        /// <summary>
        /// 是否激活状态
        /// </summary>
        private bool IsPlaying { get; set; }

        /// <summary>
        /// 飞行范围：略大一点
        /// </summary>
        private Bounds BigBound = new Bounds();

        /// <summary>
        /// 飞行范围：正常大小
        /// </summary>
        private Bounds NormalBound = new Bounds();

        /// <summary>
        /// 设置飞行边界
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        private void SetBoundWorld(Vector3 min, Vector3 max)
        {
            min.y = -1;
            max.y = 1;
            BigBound.min = min + new Vector3(-1, 0, -1);
            BigBound.max = max + new Vector3(1, 0, 1);
            NormalBound.min = min;
            NormalBound.max = max;

#if UNITY_EDITOR
            Vector3 mmin = min;
            Vector3 mmax = max;
            mmin.y = 0;
            mmax.y = 0;
            Vector3 mmin1 = new Vector3(mmax.x, 0, mmin.z);
            Vector3 mmax1 = new Vector3(mmin.x, 0, mmax.z);
            Debug.DrawLine(mmin, mmin1, Color.black, 1000000);
            Debug.DrawLine(mmin1, mmax, Color.black, 1000000);
            Debug.DrawLine(mmax, mmax1, Color.black, 1000000);
            Debug.DrawLine(mmax1, mmin, Color.black, 1000000);
#endif
        }

        /// <summary>
        /// 设置飞行边界
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <param name="isWorld">是否是世界坐标</param>
        public void SetBoundInfo(float minX, float minY, float maxX, float maxY, bool isWorld)
        {
            if (!isWorld)
            {
                Camera main = Camera.main;
                if (main != null)
                {
                    // 转换
                    Vector3 min = main.ScreenToWorldPoint(new Vector3(minX, minY, main.farClipPlane));
                    Vector3 max = main.ScreenToWorldPoint(new Vector3(maxX, maxY, main.farClipPlane));
                    SetBoundWorld(min, max);
                }
                else
                {
                    // 默认使用 屏幕的 宽高
                    Vector3 defaultV = new Vector3(Screen.width, 0, Screen.height) * 0.5f;
                    SetBoundWorld(-defaultV, defaultV);
                }
            }
            else
            {
                // 直接设置
                SetBoundWorld(new Vector3(minX, 0, minY), new Vector3(maxX, 0, maxY));
            }
        }

        /// <summary>
        /// 设置 飞行属性
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <param name="squreDistanceThreshold"></param>
        public void SetInfo(float speed, Vector3 pos, Vector3 dir, float squreDistanceThreshold = 400.0f)
        {
            Pos = pos;
            Speed = speed;
            Direction = dir;
            SquareDistanceThreshold = squreDistanceThreshold;
        }

        /// <summary>
        /// 控制开始执行
        /// </summary>
        /// <param name="forceUpdate"></param>
        public void Move(bool forceUpdate, bool freezeMove, bool freezeRotate, bool freezeCollide, bool checkReached, bool luaCollideCheck)
        {
            IsPlaying = true;
            FreezeMove = freezeMove;
            FreezeRotate = freezeRotate;
            FreezeCollide = freezeCollide;
            IsCheckReached = checkReached;
            IsLuaCollideCheck = luaCollideCheck;
            CleanData();
            if (forceUpdate)
            {
                Update();
            }
        }

        private void CleanData()
        {
            // 保证 初始位置在小范围内
            if (!NormalBound.Contains(Pos))
            {
                Pos = NormalBound.ClosestPoint(Pos);
            }

            if (TargetType == NavTargetType.NONE)
            {
                SetTarget();
            }

            // 保证 朝向为 朝着目标
            if (IsCheckReached)
            {
                RotateToTarget();
            }
        }

        /// <summary>
        /// 有效设置
        /// </summary>
        public void Stop()
        {
            IsPlaying = false;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (!IsPlaying)
            {
                return;
            }
            // 更新事件
            UpdateEvents();
            // 更新朝向
            UpdateDirection();
            // 更新位置
            UpdatePosition();
        }

        /// <summary>
        /// 更新事件
        /// </summary>
        public void UpdateEvents()
        {
            // 检测碰撞
            CheckCollide();
            // 检测目标是否有效
            // 或者 检测目标是否到达目标
            CheckTarget();
        }

        /// <summary>
        /// 检测 目标物体是否 有效
        /// </summary>
        public void CheckTarget()
        {
            if (IsPlaying && IsCheckReached)
            {
                if (!IsTargetValid())
                {
                    HandleInvalid();
                }
                else
                {
                    // 此时目标肯定在范围内
                    CheckReachTarget();
                }
            }
        }


        /// <summary>
        /// 检测 是否达到 目标
        /// </summary>
        public void CheckReachTarget()
        {
            if (IsPlaying && IsCheckReached)
            {
                Vector3 target = GetTargetPosition();
                Vector3 v = target - Pos;
                v.y = 0;
                if (v.sqrMagnitude < SquareDistanceThreshold)
                {
                    OnReachTarget();
                }
                else
                {
                    // 下次位置的更新会超出，则也认为达到目标
                    // 这一步执行，显然 目标位置在 范围内
                    if (!FreezeMove)
                    {
                        // 如果位置改变超过了范围
                        Vector3 pos = Pos + Speed * Time.deltaTime * Direction;
                        pos.y = 0;
                        if (!NormalBound.Contains(pos))
                        {
                            Pos = pos;
                            OnReachTarget();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更新坐标
        /// </summary>
        public void UpdatePosition()
        {
            if (!FreezeMove && IsPlaying)
            {
                Vector3 pos = Pos + Speed * Time.deltaTime * Direction;
                pos.y = 0;
                if (NormalBound.Contains(pos))
                {
                    Pos = pos;
                }
                else
                {
                    OnReachBorder();
                }
            }
        }

        /// <summary>
        /// 更新朝向
        /// </summary>
        /// <param name="force">如果目标被改变，需要强制朝向目标</param>
        public void UpdateDirection(bool force = false)
        {
            if ((force || !FreezeRotate) && IsPlaying)
            {
                // 更新朝向
                RotateToTarget();
            }
        }

        /// <summary>
        /// 旋转到 朝向目标
        /// </summary>
        public void RotateToPoint(Vector3 point)
        {
            // 更新朝向
            Vector3 dir = point - Pos;
            dir.y = 0;
            if (dir.sqrMagnitude > 0.0001f)
            {
                Direction = dir;
            }
        }

        /// <summary>
        /// 旋转到 朝向目标
        /// </summary>
        public void RotateToTarget()
        {
            if (IsPlaying && IsCheckReached)
            {
                RotateToPoint(GetTargetPosition());
            }
        }
    }

    public partial class NavTargetController
    {
        /// <summary>
        /// 缓存 射线实例，避免太多 new
        /// </summary>
        private static Ray cacheRay = new Ray();

        /// <summary>
        /// bigBound 参与 IntersectRay
        /// normalBound 参与 ClosestPoint
        /// bigBound 通过 IntersectRay 获得 的 pos， 与 normalBound 边界比较 确定 dir 的反射值
        /// </summary>
        /// <param name="bigBound"></param>
        /// <param name="normalBound"></param>
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        /// <param name="reflectPos"></param>
        /// <param name="reflectDir"></param>
        /// <param name="nextPos"></param>
        /// <returns></returns>
        public static bool Reflect(ref Bounds bigBound, ref Bounds normalBound, Vector3 pos, Vector3 dir, ref Vector3 reflectPos, ref Vector3 reflectDir, ref Vector3 nextPos)
        {
            // 如果是个 0 向量，直接改掉。
            if (dir.sqrMagnitude < 0.01f)
            {
                dir = Vector3.right;
            }
            // 如果在外面，直接拉到里面。必定存在 交点
            if (!normalBound.Contains(pos))
            {
                pos = normalBound.ClosestPoint(pos);
            }
            // 构造Ray
            cacheRay.origin = pos;
            // 这里因为 pos 为被指定到 bound 内部，这里 dir 取反向。 unity 这个接口的值 就是这样子的
            cacheRay.direction = -dir;
            // 必定相交，因为 pos 在 outerBound 内
            float distance;
            bool isIntersect = bigBound.IntersectRay(cacheRay, out distance);
            // 此时 distance 为负值，cacheRay.direction 为 -dir
            reflectPos = distance * cacheRay.direction + cacheRay.origin;
            // reflectPos 应在 outerBound 边界上，在 innerBound 外
            reflectDir = dir;
            // 反弹
            if (reflectPos.x >= normalBound.max.x || reflectPos.x <= normalBound.min.x)
            {
                reflectDir.x = -reflectDir.x;
            }
            if (reflectPos.z >= normalBound.max.z || reflectPos.z <= normalBound.min.z)
            {
                reflectDir.z = -reflectDir.z;
            }
            // 将 reflectPos 设置到 cacheBound 边界上
            // 略小于 normalBound，不希望在边界上
            reflectPos = normalBound.ClosestPoint(reflectPos);

#if UNITY_EDITOR
            // 构造Ray
            cacheRay.origin = reflectPos;
            // 这里因为 pos 为被指定到 bound 内部，这里 dir 取反向。 unity 这个接口的值 就是这样子的
            cacheRay.direction = -reflectDir;
            bigBound.IntersectRay(cacheRay, out distance);
            nextPos = reflectPos + cacheRay.direction * distance;
            nextPos = normalBound.ClosestPoint(nextPos);
            Debug.DrawLine(reflectPos, nextPos, Color.green, 1);
#endif
            return true;
        }

    }

}
