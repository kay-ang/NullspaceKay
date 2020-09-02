
/// referrence by http://blog.jobbole.com/83939/  
///
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nullspace
{
    public class MSTEdge
    {
        public MSTEdge(int begin, int end, int weight)
        {
            this.Begin = begin;
            this.End = end;
            this.Weight = weight;
        }

        public int Begin { get; private set; }
        public int End { get; private set; }
        public int Weight { get; private set; }

        public override string ToString()
        {
            return string.Format(
              "Begin[{0}], End[{1}], Weight[{2}]",
              Begin, End, Weight);
        }
    }
    public class MSTSubset
    {
        public int Parent { get; set; }
        public int Rank { get; set; }
    }
    // 最小生成树  Kruskal
    public class MSTKruskal
    {
        public static void TestMain()
        {
            MSTKruskal g = new MSTKruskal(9);
            g.AddEdge(0, 1, 4);
            g.AddEdge(0, 7, 8);
            g.AddEdge(1, 2, 8);
            g.AddEdge(1, 7, 11);
            g.AddEdge(2, 3, 7);
            g.AddEdge(2, 5, 4);
            g.AddEdge(8, 2, 2);
            g.AddEdge(3, 4, 9);
            g.AddEdge(3, 5, 14);
            g.AddEdge(5, 4, 10);
            g.AddEdge(6, 5, 2);
            g.AddEdge(8, 6, 6);
            g.AddEdge(7, 6, 1);
            g.AddEdge(7, 8, 7);

            DebugUtils.Log(InfoType.Info, string.Format("Graph Vertex Count : {0}", g.VertexCount));
            DebugUtils.Log(InfoType.Info, string.Format("Graph Edge Count : {0}", g.EdgeCount));
            DebugUtils.Log(InfoType.Info, string.Format("Is there cycle in graph: {0}", g.HasCycle()));

            MSTEdge[] mst = g.Kruskal();
            Console.WriteLine("MST Edges:");
            foreach (var edge in mst)
            {
                DebugUtils.Log(InfoType.Info, string.Format("\t{0}", edge));
            }
        }

        private Dictionary<int, List<MSTEdge>> mAdjacentEdges = new Dictionary<int, List<MSTEdge>>();

        public MSTKruskal(int vertexCount)
        {
            this.VertexCount = vertexCount;
        }

        public int VertexCount { get; private set; }

        public IEnumerable<int> Vertices { get { return mAdjacentEdges.Keys; } }

        public IEnumerable<MSTEdge> Edges
        {
            get { return mAdjacentEdges.Values.SelectMany(e => e); }
        }

        public int EdgeCount { get { return this.Edges.Count(); } }

        public void AddEdge(int begin, int end, int weight)
        {
            if (!mAdjacentEdges.ContainsKey(begin))
            {
                var edges = new List<MSTEdge>();
                mAdjacentEdges.Add(begin, edges);
            }
            mAdjacentEdges[begin].Add(new MSTEdge(begin, end, weight));
        }

        private int Find(MSTSubset[] subsets, int i)
        {
            // find root and make root as parent of i (path compression)
            if (subsets[i].Parent != i)
            {
                subsets[i].Parent = Find(subsets, subsets[i].Parent);
            }
            return subsets[i].Parent;
        }

        private void Union(MSTSubset[] subsets, int x, int y)
        {
            int xroot = Find(subsets, x);
            int yroot = Find(subsets, y);
            // Attach smaller rank tree under root of high rank tree
            // (Union by Rank)
            if (subsets[xroot].Rank < subsets[yroot].Rank)
            {
                subsets[xroot].Parent = yroot;
            }
            else if (subsets[xroot].Rank > subsets[yroot].Rank)
            {
                subsets[yroot].Parent = xroot;
            }
            // If ranks are same, then make one as root and increment
            // its rank by one
            else
            {
                subsets[yroot].Parent = xroot;
                subsets[xroot].Rank++;
            }
        }

        public bool HasCycle()
        {
            MSTSubset[] subsets = new MSTSubset[VertexCount];
            for (int i = 0; i < subsets.Length; i++)
            {
                subsets[i] = new MSTSubset();
                subsets[i].Parent = i;
                subsets[i].Rank = 0;
            }
            // Iterate through all edges of graph, find subset of both
            // vertices of every edge, if both subsets are same, 
            // then there is cycle in graph.
            foreach (var edge in this.Edges)
            {
                int x = Find(subsets, edge.Begin);
                int y = Find(subsets, edge.End);
                if (x == y)
                {
                    return true;
                }
                Union(subsets, x, y);
            }
            return false;
        }

        // 先判断有没有回路，有回路再处理
        public MSTEdge[] Kruskal()
        {
            // This will store the resultant MST
            MSTEdge[] mst = new MSTEdge[VertexCount - 1];
            // Step 1: Sort all the edges in non-decreasing order of their weight
            // If we are not allowed to change the given graph, we can create a copy of
            // array of edges
            var sortedEdges = this.Edges.OrderBy(t => t.Weight);
            var enumerator = sortedEdges.GetEnumerator();
            // Allocate memory for creating V ssubsets
            // Create V subsets with single elements
            MSTSubset[] subsets = new MSTSubset[VertexCount];
            for (int i = 0; i < subsets.Length; i++)
            {
                subsets[i] = new MSTSubset();
                subsets[i].Parent = i;
                subsets[i].Rank = 0;
            }
            // Number of edges to be taken is equal to V-1
            int e = 0;
            while (e < VertexCount - 1)
            {
                // Step 2: Pick the smallest edge. And increment the index
                // for next iteration
                MSTEdge nextEdge;
                if (enumerator.MoveNext())
                {
                    nextEdge = enumerator.Current;
                    int x = Find(subsets, nextEdge.Begin);
                    int y = Find(subsets, nextEdge.End);
                    // If including this edge does't cause cycle, include it
                    // in result and increment the index of result for next edge
                    if (x != y)
                    {
                        mst[e++] = nextEdge;
                        Union(subsets, x, y);
                    }
                    else
                    {
                        // Else discard the nextEdge
                    }
                }
            }
            return mst;
        }
    }
}
