
namespace Nullspace
{
    /// <summary>
    /// 确定坐标和方向的计算方式
    /// </summary>
    public enum NavPathMoveType
    {
        Fixed,             // 不移动，固定
        CurvePosCurveDir,      // 线段长度确定u值，曲线插值计算坐标和方向
        LinePosLineDir,       // 线段长度确定u值，线性插值计算坐标，使用线段方向
        LinePosCurveDir,  // 线段长度确定u值，线性插值计算坐标，曲线插值计算方向
        LinePosLineAngle   // 线段长度确定u值，线性插值计算坐标，利用角速度线性插值计算方向
    }
}
