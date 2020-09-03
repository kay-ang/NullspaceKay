
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nullspace
{

    /// <summary>
    /// 基于迪杰斯特算法 查找最短路径
    /// </summary>
    public class Dijkstra<NodeType> // NodeType 必要时，重写 hashCode和equals
    {
        private Dictionary<NodeType, Dictionary<NodeType, float>> mAdjacencyLists;
        private float[] mCostMatrix;
        private NodeType[] mLeastNodeArray;
        private bool isValidPath;
        private NodeType mRootNode;

        public Dijkstra()
        {
            mAdjacencyLists = new Dictionary<NodeType, Dictionary<NodeType, float>>();
            isValidPath = false;
            mRootNode = default(NodeType);
        }

        public void AddNode(NodeType node)
        {
            if (!mAdjacencyLists.ContainsKey(node))
            {
                mAdjacencyLists.Add(node, new Dictionary<NodeType, float>());
            }
        }
        public void RemoveNode(NodeType node)
        {
            if (mAdjacencyLists.ContainsKey(node))
            {
                Queue<NodeType> nodeQueue = new Queue<NodeType>();
                nodeQueue.Enqueue(node);
                while (nodeQueue.Count > 0)
                {
                    NodeType d = nodeQueue.Dequeue();
                    mAdjacencyLists.Remove(d);
                    foreach (var v in mAdjacencyLists)
                    {
                        if (v.Value.ContainsKey(node))
                        {
                            v.Value.Remove(node);
                        }
                        if (v.Value.Count == 0) // 无连接的节点，也去除  默认需要去除
                        {
                            nodeQueue.Enqueue(v.Key);
                        }
                    }
                }
                ClearDijkstra();
            }
        }
        public void ClearDijkstra()
        {
            if (isValidPath)
            {
                isValidPath = false;
            }
        }
        public bool HasConnection(NodeType node1, NodeType node2)
        {
            if (node1.Equals(node2)) // 明白 gethashcode 和 equal
            {
                return false;
            }
            if (!mAdjacencyLists.ContainsKey(node1))
            {
                return false;
            }
            return mAdjacencyLists[node1].ContainsKey(node2);
        }
        public void AddConnection(NodeType node1, NodeType node2, float weight, bool doubleDirection)
        {
            if (!mAdjacencyLists.ContainsKey(node1))
            {
                AddNode(node1);
            }
            if (!mAdjacencyLists[node1].ContainsKey(node2))
            {
                mAdjacencyLists[node1].Add(node2, weight);
            }
            else
            {
                mAdjacencyLists[node1][node2] = weight;
            }

            if (!mAdjacencyLists.ContainsKey(node2))
            {
                AddNode(node2);
            }
            if (doubleDirection)
            {
                if (!mAdjacencyLists[node2].ContainsKey(node1))
                {
                    mAdjacencyLists[node2].Add(node1, weight);
                }
                else
                {
                    mAdjacencyLists[node2][node1] = weight;
                }
            }
        }
        public void AddConnection(NodeType node1, NodeType node2, float weight1, float weight2)
        {
            if (!mAdjacencyLists.ContainsKey(node1))
            {
                AddNode(node1);
            }
            if (!mAdjacencyLists[node1].ContainsKey(node2))
            {
                mAdjacencyLists[node1].Add(node2, weight1);
            }
            else
            {
                mAdjacencyLists[node1][node2] = weight1;
            }

            if (!mAdjacencyLists.ContainsKey(node2))
            {
                AddNode(node2);
            }
            if (!mAdjacencyLists[node2].ContainsKey(node1))
            {
                mAdjacencyLists[node2].Add(node1, weight2);
            }
            else
            {
                mAdjacencyLists[node2][node1] = weight2;
            }
        }
        public void RemoveConnection(NodeType node1, NodeType node2, bool doubleLink)
        {
            if (HasConnection(node1, node2)) // must exist HasConnection(node2, node1)
            {
                mAdjacencyLists[node1].Remove(node2);
                if (mAdjacencyLists[node1].Count == 0)
                {
                    RemoveNode(node1);
                }
            }

            if (doubleLink && HasConnection(node2, node1))
            {
                mAdjacencyLists[node2].Remove(node1);
                if (mAdjacencyLists[node2].Count == 0) // node2 may be deleted by RemoveNode(node1)
                {
                    RemoveNode(node2);
                }
            }
        }
        public void Clear()
        {
            foreach (var t in mAdjacencyLists)
            {
                t.Value.Clear();
            }
        }
        public bool GetShortestPath(List<NodeType> path, NodeType startNode, NodeType endNode, float infinite)
        {
            path.Clear();
            if (startNode.Equals(endNode))
            {
                path.Add(startNode);
                path.Add(endNode);
                return true;
            }
            if (mRootNode == null || isValidPath == false || !mRootNode.Equals(startNode))
            {
                ClearDijkstra();
                GenerateDisjktraMatrix(startNode, infinite);
            }

            if (mAdjacencyLists.Count - 2 == 0)
            {
                path.Add(startNode);
                path.Add(endNode);
                return true;
            }
            int col = GetIndexByKey(endNode);
            if (mAdjacencyLists.Count < 2 || col == -1)
            {
                return false;
            }
            int row = mAdjacencyLists.Count - 1; // 最后一行开始
            float currentWeight = mCostMatrix[row * mAdjacencyLists.Count + col];
            if (currentWeight == infinite)
            {
                return false;
            }
            Stack<NodeType> outputQueue = new Stack<NodeType>();
            NodeType vertex = endNode;
            outputQueue.Push(vertex);
            row--;
            while (true)
            {
                while (mCostMatrix[row * mAdjacencyLists.Count + col] == currentWeight)
                {
                    if (row == 0)
                    {
                        path.Add(startNode);
                        for (col = 0; outputQueue.Count != 0; col++)
                        {
                            path.Add(outputQueue.Pop());
                        }
                        return true;
                    }
                    --row;
                }
                vertex = mLeastNodeArray[row];
                outputQueue.Push(vertex);
                if (row == 0)
                {
                    break;
                }
                col = GetIndexByKey(vertex);
                currentWeight = mCostMatrix[row * mAdjacencyLists.Count + col];
            }
            path.Add(startNode);
            for (col = 0; outputQueue.Count != 0; col++)
            {
                path.Add(outputQueue.Pop());
            }
            return true;
        }
        public int GetIndexByKey(NodeType node)
        {
            return mAdjacencyLists.Keys.ToList().IndexOf(node);
        }
        public void GenerateDisjktraMatrix(NodeType startNode, float infinite)
        {
            if (mAdjacencyLists.Count == 0)
            {
                return;
            }
            mCostMatrix = new float[mAdjacencyLists.Count * mAdjacencyLists.Count];
            mLeastNodeArray = new NodeType[mAdjacencyLists.Count];
            //mCostMatrixIndices.Clear();
            //foreach (var key in mAdjacencyLists.Keys)
            //{
            //    mCostMatrixIndices.Add(key);
            //}
            //mCostMatrixIndices.Sort((NodeType node1, NodeType node2) => { return mCompareFunc(node1, node2); }); // 由小到大排序
            for (int i = 0; i < mAdjacencyLists.Count * mAdjacencyLists.Count; ++i)
            {
                mCostMatrix[i] = infinite;
            }
            int adjacentIndex = GetIndexByKey(startNode);
            if (adjacentIndex == -1)
            {
                throw new Exception("-1 null");
            }
            for (int r = 0; r < mAdjacencyLists.Count; ++r)
            {
                mCostMatrix[r * mAdjacencyLists.Count + adjacentIndex] = 0.0f;
            }
            mRootNode = startNode;
            int row = 0;
            NodeType currentNode = startNode;
            Dictionary<NodeType, float> adjacencyList;
            float edgeWeight, adjacentNodeWeight, currentNodeWeight = 0.0f;
            NodeType adjacentKey;
            Dictionary<NodeType, float> openSet = new Dictionary<NodeType, float>();
            while (row < mAdjacencyLists.Count - 1)
            {
                adjacencyList = mAdjacencyLists[currentNode];
                foreach (var v in adjacencyList)
                {
                    edgeWeight = v.Value;
                    adjacentKey = v.Key;
                    adjacentIndex = GetIndexByKey(adjacentKey);
                    adjacentNodeWeight = mCostMatrix[row * mAdjacencyLists.Count + adjacentIndex];
                    if (currentNodeWeight + edgeWeight < adjacentNodeWeight)
                    {
                        // Update the weight for the adjacent node
                        for (int r = row; r < mAdjacencyLists.Count; r++)
                        {
                            mCostMatrix[r * mAdjacencyLists.Count + adjacentIndex] = currentNodeWeight + edgeWeight;
                        }
                        if (!openSet.ContainsKey(adjacentKey))
                        {
                            openSet.Add(adjacentKey, currentNodeWeight + edgeWeight);
                        }
                        else
                        {
                            openSet[adjacentKey] = currentNodeWeight + edgeWeight;
                        }
                    }
                }
                PriorityQueue<NodeType, NodeType, float> minHeap = new PriorityQueue<NodeType, NodeType, float>();
                foreach (var open in openSet)
                {
                    minHeap.Enqueue(open.Key, open.Key, open.Value);
                }
                if (minHeap.Size == 0)
                {
                    isValidPath = true;
                    break;
                }
                currentNodeWeight = minHeap.PeekPriority();
                mLeastNodeArray[row] = minHeap.Dequeue();
                currentNode = mLeastNodeArray[row];
                openSet.Remove(currentNode);
                row++;
            }
            isValidPath = true;
        }
        public void PrintConnection()
        {
            foreach (var item1 in mAdjacencyLists)
            {
                NodeType node = item1.Key;
                foreach (var item2 in item1.Value)
                {
                    DebugUtils.Log(InfoType.Info, string.Format("PrintConnection", "{0} -> {1} : {2}", node.ToString(), item2.Key.ToString(), item2.Value));
                }
            }
        }
        public void PrintCostMatrix()
        {
            List<NodeType> keys = mAdjacencyLists.Keys.ToList();
            string temp = "";
            foreach (NodeType key in keys)
            {
                temp = temp + " " + key.ToString();
            }
            DebugUtils.Log(InfoType.Info, string.Format("PrintCostMatrix1 {0}", temp));

            for (int i = 0; i < mAdjacencyLists.Count; ++i)
            {
                temp = keys[i].ToString();
                for (int j = 0; j < mAdjacencyLists.Count; ++j)
                {
                    temp = string.Format("{0} {1}", temp, mCostMatrix[i * mAdjacencyLists.Count + j]);
                }
                DebugUtils.Log(InfoType.Info, string.Format("PrintCostMatrix2 {0}", temp));
            }
        }
    }
}
