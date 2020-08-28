
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nullspace
{
    public class AStarDirection
    {
        public static sbyte[,] DiagonalDirection = new sbyte[8, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { 1, -1 }, { 1, 1 }, { -1, 1 }, { -1, -1 } };
        public static sbyte[,] NormalDirection = new sbyte[4, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };
    }

    public enum AStarFormula
    {
        Manhattan,
        MaxDXDY,
        DiagonalShortCut,
        Euclidean,
        EuclideanNoSQR,
        Custom
    }

    public struct PathNode
    {
        public int F;
        public int G;
        // parent
        public ushort PX;
        public ushort PY;
        public int Status;
    }

    public class PathNodeResult
    {
        public int F;
        public int G;
        public int H;
        public int X;
        public int Y;
        // parent
        public ushort PX;
        public ushort PY;
    }

    /// <summary>
    /// 基于优先队列(堆排序)
    /// 快速A* 
    /// 传统A*暂未处理
    /// </summary>
    public class AStarFast
    {
        private int mHEstimate = 2;
        private int mSearchLimit = 10000;
        private float mMultiple = 0.01f;
        private byte[,] mGrid;
        private ushort mGridX;
        private ushort mGridY;
        private ushort mGridXMod;
        private ushort mGridXLog2;
        private PathNode[] mGridNode = null;
        private PriorityQueue<ushort, ushort, float> mOpenTable;
        private List<PathNodeResult> mResultPath = new List<PathNodeResult>();
        private int mOpenStatusValue = 0;
        private int mCloseStatusValue = 1;

        public bool ReuseClose { get; set; }
        public bool UseDiagonal { get; set; }
        public bool UsePunish { get; set; }
        public bool UseTieBreaker { get; set; }
        public AStarFormula Formula { get; set; }

        public static ushort UpperPow2(ushort x)
        {
            ushort i = 1;
            while (i < x)
            {
                i <<= 1;
            }
            return i;
        }

        public static bool IsPower2(ushort x)
        {
            ushort y = UpperPow2(x);
            return x == y;
        }

        public static AStarFast CreateAStarFast(byte[,] grid)
        {
            ushort x = (ushort)grid.GetLength(0);
            ushort y = (ushort)grid.GetLength(1);
            ushort x1 = UpperPow2(x);
            ushort y1 = UpperPow2(y);
            if (x1 != x || y1 != y)
            {
                byte[,] newGrid = new byte[x1, y1];
                for (int i = 0; i < x1; ++i)
                {
                    for (int j = 0; j < y1; ++j)
                    {
                        newGrid[i, j] = 0; // zero is not reachable
                    }
                }
                for (int i = 0; i < x; ++i)
                {
                    for (int j = 0; j < y; ++j)
                    {
                        newGrid[i, j] = grid[i, j];
                    }
                }
                return new AStarFast(newGrid);
            }
            else
            {
                return new AStarFast(grid);
            }
            
        }

        public AStarFast(byte[,] grid)
        {
            Formula = AStarFormula.Manhattan;
            mGrid = grid;
            mGridX = (ushort)mGrid.GetLength(0);
            mGridY = (ushort)mGrid.GetLength(1);
            if (!IsPower2(mGridX) || !IsPower2(mGridY))
            {
                throw new Exception("must be power of 2, check size X, Y");
            }
            mGridXMod = (ushort)(mGridX - 1);
            mGridXLog2 = (ushort)Math.Log(mGridX, 2);
            mGridNode = new PathNode[mGridX * mGridY];
            mOpenTable = new PriorityQueue<ushort, ushort, float>();
            mMultiple = 1.0f / mGridX;
        }

        public List<Vector2Int> PathFind(Vector2Int start, Vector2Int end)
        {
            bool found = false;
            mOpenTable.Clear();
            mResultPath.Clear();
            mOpenStatusValue += 2;
            mCloseStatusValue += 2;
            int closeNodeCounter = 0;
            ushort location = (ushort)((start[1] << mGridXLog2) + start[0]);
            ushort endLocation = (ushort)((end[1] << mGridXLog2) + end[0]);
            mGridNode[location].G = 0;
            mGridNode[location].F = mHEstimate;
            mGridNode[location].PX = (ushort)start[0];
            mGridNode[location].PY = (ushort)start[1];
            mGridNode[location].Status = mOpenStatusValue;
            mOpenTable.Enqueue(location, location, mGridNode[location].F);
            ushort locationX;
            ushort locationY;
            ushort mHoriz = 0;

            sbyte[,] direction = UseDiagonal ? AStarDirection.DiagonalDirection : AStarDirection.NormalDirection;
            int directionCount = UseDiagonal ? 8 : 4;

            while (mOpenTable.Size > 0)
            {
                location = mOpenTable.Dequeue();
                if (mGridNode[location].Status == mCloseStatusValue)
                {
                    continue;
                }
                if (location == endLocation)
                {
                    mGridNode[location].Status = mCloseStatusValue;
                    found = true;
                    break;
                }
                if (closeNodeCounter > mSearchLimit)
                {
                    break;
                }
                locationX = (ushort)(location & mGridXMod);
                locationY = (ushort)(location >> mGridXLog2);
                if (UsePunish)
                {
                    mHoriz = (ushort)(locationX - mGridNode[location].PX);
                }
                int newG = 0;
                for (int i = 0; i < directionCount; i++)
                {
                    ushort newLocationX = (ushort)(locationX + direction[i, 0]);
                    ushort newLocationY = (ushort)(locationY + direction[i, 1]);
                    ushort newLocation = (ushort)((newLocationY << mGridXLog2) + newLocationX);
                    if (newLocationX >= mGridX || newLocationY >= mGridY)
                        continue;
                    if (mGridNode[newLocation].Status == mCloseStatusValue && !ReuseClose)
                        continue;
                    if (mGrid[newLocationX, newLocationY] == 0)
                        continue;
                    if (UseDiagonal && i > 3)
                    {
                        newG = mGridNode[location].G + (int)(mGrid[newLocationX, newLocationY] * 2.41);
                    }
                    else
                    {
                        newG = mGridNode[location].G + mGrid[newLocationX, newLocationY];
                    }
                    if (UsePunish)
                    {
                        if ((newLocationX - locationX) != 0)
                        {
                            if (mHoriz == 0)
                            {
                                newG += Math.Abs(newLocationX - end[0]) + Math.Abs(newLocationY - end[1]);
                            }
                        }
                        if ((newLocationY - locationY) != 0)
                        {
                            if (mHoriz != 0)
                            {
                                newG += Math.Abs(newLocationX - end[0]) + Math.Abs(newLocationY - end[1]);
                            }
                        }
                    }
                    if (mGridNode[newLocation].Status == mOpenStatusValue || mGridNode[newLocation].Status == mCloseStatusValue)
                    {
                        if (mGridNode[newLocation].G <= newG)
                        {
                            continue;
                        }
                    }
                    mGridNode[newLocation].PX = locationX;
                    mGridNode[newLocation].PY = locationY;
                    mGridNode[newLocation].G = newG;

                    int newH = 0;
                    switch (Formula)
                    {
                        case AStarFormula.Manhattan:
                            newH = mHEstimate * (Math.Abs(newLocationX - end[0]) + Math.Abs(newLocationY - end[1]));
                            break;
                        case AStarFormula.MaxDXDY:
                            newH = mHEstimate * (Math.Max(Math.Abs(newLocationX - end[0]), Math.Abs(newLocationY - end[1])));
                            break;
                        case AStarFormula.DiagonalShortCut:
                            int h_diagonal = Math.Min(Math.Abs(newLocationX - end[0]), Math.Abs(newLocationY - end[1]));
                            int h_straight = (Math.Abs(newLocationX - end[0]) + Math.Abs(newLocationY - end[1]));
                            newH = (mHEstimate * 2) * h_diagonal + mHEstimate * (h_straight - 2 * h_diagonal);
                            break;
                        case AStarFormula.Euclidean:
                            newH = (int)(mHEstimate * Math.Sqrt(Math.Pow((newLocationY - end[0]), 2) + Math.Pow((newLocationY - end[1]), 2)));
                            break;
                        case AStarFormula.EuclideanNoSQR:
                            newH = (int)(mHEstimate * (Math.Pow((newLocationX - end[0]), 2) + Math.Pow((newLocationY - end[1]), 2)));
                            break;
                        case AStarFormula.Custom:
                            Vector2Int dxy = new Vector2Int(Math.Abs(end[0] - newLocationX), Math.Abs(end[1] - newLocationY));
                            int Orthogonal = Math.Abs(dxy[0] - dxy[1]);
                            int Diagonal = Math.Abs(((dxy[0] + dxy[1]) - Orthogonal) / 2);
                            newH = mHEstimate * (Diagonal + Orthogonal + dxy[0] + dxy[1]);
                            break;
                    }
                    if (UseTieBreaker)
                    {
                        int dx1 = locationX - end[0];
                        int dy1 = locationY - end[1];
                        int dx2 = start[0] - end[0];
                        int dy2 = start[1] - end[1];
                        int cross = Math.Abs(dx1 * dy2 - dx2 * dy1);
                        newH = (int)(newH + cross * mMultiple);
                    }
                    mGridNode[newLocation].F = newG + newH;
                    mOpenTable.Enqueue(newLocation, newLocation, mGridNode[newLocation].F);
                    mGridNode[newLocation].Status = mOpenStatusValue;
                }
                closeNodeCounter++;
                mGridNode[location].Status = mCloseStatusValue; 
            }
            if (found)
            {
                mResultPath.Clear();
                PathNode tmp = mGridNode[(end[1] << mGridXLog2) + end[0]];
                PathNodeResult node = new PathNodeResult();
                node.F = tmp.F;
                node.G = tmp.G;
                node.H = 0;
                node.PX = tmp.PX;
                node.PY = tmp.PY;
                node.X = end[0];
                node.Y = end[1];
                while (node.X != node.PX || node.Y != node.PY)
                {
                    mResultPath.Add(node);
                    ushort posX = node.PX;
                    ushort posY = node.PY;
                    tmp = mGridNode[(posY << mGridXLog2) + posX];
                    node = new PathNodeResult();
                    node.F = tmp.F;
                    node.G = tmp.G;
                    node.H = 0;
                    node.PX = tmp.PX;
                    node.PY = tmp.PY;
                    node.X = posX;
                    node.Y = posY;
                }
                mResultPath.Add(node);
                mResultPath.Reverse(0, mResultPath.Count);
                List<Vector2Int> res = new List<Vector2Int>();
                foreach (PathNodeResult n in mResultPath)
                {
                    res.Add(new Vector2Int(n.X, n.Y));
                }
                return res;
            }
            return null;
        }
    }
}
