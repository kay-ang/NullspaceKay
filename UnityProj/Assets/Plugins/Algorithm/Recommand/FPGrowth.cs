
using System;
using System.Collections.Generic;

using UnityEngine;

namespace Nullspace
{
    public class FPItem
    {
        public FPItem(string desc, int id)
        {
            mName = desc;
            mId = id;
            mSupport = 1;
        }

        public void IncreaseSupport(int delta = 1)
        {
            mSupport += delta;
        }

        public FPTreeNode CreateNode()
        {
            return new FPTreeNode(mName, this);
        }
        public string mName;
        public int mId;
        public int mSupport;
    }

    public class FPTransition
    {
        public List<FPItem> mItems;
        public FPTransition()
        {
            mItems = new List<FPItem>();
        }
        public int Size()
        {
            return mItems.Count;
        }

        public void AddItem(FPItem item, bool duplicate)
        {
            if (duplicate)
            {
                mItems.Add(item);
            }
            else
            {
                if (!ContainItem(item))
                {
                    mItems.Add(item);
                }
            }   
        }

        public bool ContainItem(FPItem item)
        {
            int index = mItems.FindIndex(delegate(FPItem it) { return it.mName == item.mName; });
            return index >= 0;
        }
    }

    public class FPTransitions
    {
        public bool mDuplicate;
        public int mDepth;
        public Dictionary<string, FPItem> mItems;
        public List<FPTransition> mTransitions;
        private int _id = 0;
        public FPTransitions(bool duplicate = false)
        {
            mDuplicate = duplicate;
            mDepth = 1;
            mItems = new Dictionary<string, FPItem>();
            mTransitions = new List<FPTransition>();
        }

        public void AddTransition(FPTransition transition)
        {
            mTransitions.Add(transition);
        }
        public int Size()
        {
            return mTransitions.Count;
        }

        public int Depth
        {
            get
            {
                return mDepth;
            }
            set
            {
                mDepth = value;
            }
        }

        public FPTransition this[int index]
        {
            get
            {
                return mTransitions[index];
            }
        }

        public FPItem AddItem(string name)
        {
            if (!mItems.ContainsKey(name))
            {
                mItems.Add(name, new FPItem(name, _id++));
            }
            else
            {
                mItems[name].IncreaseSupport();
            }
            return mItems[name];
        }
    }

    public class FPTreeNode
    {
        public string mName;
        public FPTreeNode mParent;
        public int mCount;
        public List<FPTreeNode> mChildren;
        public FPTreeNode mSameNameNextNode;
        public FPItem mOwner;

        public FPTreeNode(string name, FPItem item)
        {
            mName = name;
            mOwner = item;
            mParent = null;
            mSameNameNextNode = null;
            mCount = 0;
            mChildren = new List<FPTreeNode>();
        }

        public FPTreeNode FindChild(string name)
        {
            return mChildren.Find(delegate(FPTreeNode node) { return node.mName == name; });
        }

        public void AddChild(FPTreeNode node)
        {
            mChildren.Add(node);
        }

        public override bool Equals(object obj)
        {
            return mOwner.mSupport == ((FPTreeNode)obj).mOwner.mSupport;
        }

        public override int GetHashCode()
        {
            return mName.GetHashCode();
        }

        public int ChildCount()
        {
            return mChildren.Count;
        }

        public FPTreeNode NextNode
        {
            get
            {
                return mSameNameNextNode;
            }
            set
            {
                mSameNameNextNode = value;
            }
        }
        public void Increasement(int count)
        {
            mCount += count;
        }
    }

    public class FPTree
    {
        public int mMinSupport;
	    public float mConfidenceFrequence;
	    public int mDepth;
        public int mMaxDepth;
        Dictionary<string, float> mProbs = new Dictionary<string,float>();
	    Dictionary<string, int> mExistCount = new Dictionary<string,int>();
        class FPNodeHeadTable
        {
            public List<FPTreeNode> mHeaderNode;
            public FPNodeHeadTable()
            {
                mHeaderNode = new List<FPTreeNode>();
            }

            public int Size()
            {
                return mHeaderNode.Count;
            }
            public void AddTableNode(FPTreeNode node)
            {
                mHeaderNode.Add(node);
            }
		    public FPTreeNode At(int index)
            {
                return mHeaderNode[index];
            }
            public void Sort()
            {
                mHeaderNode.Sort(new CompareItemSupport());
            }

            class CompareItemSupport : IComparer<FPTreeNode>
            {
                public int Compare(FPTreeNode x, FPTreeNode y)
                {
                    if (x.mOwner.mSupport > y.mOwner.mSupport)
                        return 1;
                    if (x.mOwner.mSupport < y.mOwner.mSupport)
                        return -1;
                    return 0;
                }
            }
        }

        public FPTree(int minsupp = 60, float confidence = 0.7f)
        {
            mMinSupport = minsupp;
            mConfidenceFrequence = confidence;
            mDepth = 1;
            mMaxDepth = 2;
        }

        public void SetMinSupport(int minsupport, int sampleNum)
        {
            mMinSupport = (int)(minsupport * 0.01f * sampleNum);
        }

        public void Growth(FPTransitions transitions, List<string> postPatterns)
        {
	        SetMinSupport(mMinSupport, transitions.Size());
	        FPNodeHeadTable table = BuildHeaderTable(transitions);
	        if (transitions.Depth > mMaxDepth)
		        return;
	        int size = table.Size();
	        if (size == 0)
		        return;
	        string post = "";
	        if (postPatterns != null)
	        {
		        int num = postPatterns.Count;
		        for (int n = num - 1; n >= 0; --n)
		        {
			        post += postPatterns[n] + " ";
		        }
		        post.Trim();
	        }
	        for (int i = 0; i < size; ++i)
	        {
		        FPTreeNode child = table.mHeaderNode[i];
		        FPTreeNode next = child.NextNode;
		        FPTransitions newTransitions = null;
		        List<string> samples = new List<string>(); 
		        while (next != null)
		        {
			        string str = "";
			        FPTreeNode parent = next.mParent;
			
			        while (parent != null)
			        {
				        str += parent.mName;
				        str += " ";
				        parent = parent.mParent;
			        }	
			        str.Trim();
			        if (str == "")
			        {
				        next = next.NextNode;
				        continue;
			        }

			        if (transitions.Depth <= mMaxDepth)
			        {
				        for (int j = 0; j < next.mCount; ++j)
				        {
					        samples.Add(str);
				        }
			        }
			        if (post == "")
			        {
                        Debug.Log(str);
			        }
			        else
			        {
                        Debug.Log(str + " : " + post);
			        }
			        next = next.NextNode;
		        }
		        if (transitions.Depth <= mMaxDepth)
		        {
			        newTransitions = FPGrowth.ParseFromArray(samples, transitions.mDuplicate);
			        List<string> newPostPattern = new List<string>();	
				    int num = postPatterns.Count;
				    for (int n = 0; n < num; ++n)
				    {
					    newPostPattern.Add(postPatterns[n]);
				    }
			        newPostPattern.Add(child.mName);
			        if (newTransitions != null)
			        {
                        newTransitions.Depth = transitions.Depth + 1;
				        Growth(newTransitions, newPostPattern);
			        }
		        }
	        }
        }

        public void GrowthProb(FPTransitions transitions)
        {
            SetMinSupport(mMinSupport, transitions.Size());
	        FPNodeHeadTable table = BuildHeaderTable(transitions);
	        FPTreeNode root = BuildFPTree(transitions, table);
	        if (transitions.mDepth > mMaxDepth)
		        return;
	        int total = transitions.Size();
            int size = table.Size();
	        if (size == 0)
		        return;
            GenerateProbOne(root, table, ref total);
            GenerateProbTwo(root, table, ref total);
            GenerateProbThree(root, table, ref total);   
        }

        public void Print()
        {
            foreach (var im in mProbs)
            {
                Debug.Log(im.Key + " " + im.Value);
            }
        }

        void GenerateProbOne(FPTreeNode root, FPNodeHeadTable table, ref int total)
        {
	        int size = table.Size();
	        for (int i = 0; i < size; ++i)
	        {
		        FPTreeNode child = table.At(i);
		        mProbs.Add(child.mName, child.mOwner.mSupport * 1.0f / total);
		        mExistCount.Add(child.mName, child.mOwner.mSupport);
	        }
        }

        void GenerateProbTwo(FPTreeNode root, FPNodeHeadTable table, ref int total)
        {
	        int size = table.Size();
	        if (size <= 2)
	        {
		        return;
	        }
	        FPTreeNode childI = null;
	        List<List<FPTreeNode>> calculate = new List<List<FPTreeNode>>();
	        for (int i = 0; i < size - 1; ++i)
	        {
		        childI = table.At(i);
		        FPTreeNode childJ = null;
		        for (int j = i + 1; j < size; ++j)
		        {
			        List<FPTreeNode> temp = new List<FPTreeNode>();
			        childJ = table.At(j);
			        temp.Add(childI);
			        temp.Add(childJ);
			        calculate.Add(temp);
		        }
	        }
	        size = calculate.Count;
	        for (int i = 0; i < size; ++i)
	        {
		        FPTreeNode nodeA = calculate[i][0];
		        FPTreeNode nodeB = calculate[i][1];
		        int shareCount = CalcuelateSharedCount(nodeA, nodeB);
		        if (shareCount == 0)
		        {
			        continue;
		        }
		        float AB = shareCount * 1.0f / mExistCount[nodeA.mName];
		        float BA = shareCount * 1.0f / mExistCount[nodeB.mName];
		        mProbs.Add(nodeA.mName + " " + nodeB.mName, AB);
		        mProbs.Add(nodeB.mName + " " + nodeA.mName, BA);
		        mExistCount.Add(nodeA.mName + " " + nodeB.mName, shareCount);
	        }
        }
        void GenerateProbThree(FPTreeNode root, FPNodeHeadTable table, ref int total)
        {
	        int size = table.Size();
	        if (size <= 3)
	        {
		        return;
	        }
	        FPTreeNode childI = null;
	        FPTreeNode childJ = null;
	        FPTreeNode childK = null;
	       List<List<FPTreeNode>> calculate = new List<List<FPTreeNode>>();
	        for (int i = 0; i < size - 2; ++i)
	        {
		        childI = table.At(i);	
		        for (int j = i + 1; j < size - 1; ++j)
		        {
			        childJ = table.At(j);
			        for (int k = j + 1; k < size; ++k)
			        {
				        childK = table.At(k);
				        List<FPTreeNode> temp = new List<FPTreeNode>();
				        temp.Add(childI);
				        temp.Add(childJ);
				        temp.Add(childK);
				        calculate.Add(temp);
			        }
		        }
	        }
	        size = calculate.Count;
	        for (int i = 0; i < size; ++i)
	        {
		        FPTreeNode nodeA = calculate[i][0];
		        FPTreeNode nodeB = calculate[i][1];
		        FPTreeNode nodeC = calculate[i][2];
		        int shareCount = CalcuelateSharedCount(nodeA, nodeB, nodeC);
		        if (shareCount == 0)
		        {
			        continue;
		        }
		        string ABC = nodeA.mName + " " + nodeB.mName;
                string ACB = nodeA.mName + " " + nodeC.mName;
                string BCA = nodeB.mName + " " + nodeC.mName;
                string BAC = nodeB.mName + " " + nodeA.mName;
                string CAB = nodeC.mName + " " + nodeA.mName;
                string CBA = nodeC.mName + " " + nodeB.mName;
		        float abc = shareCount * 1.0f / mExistCount[ABC];
		        float acb = shareCount * 1.0f / mExistCount[ACB];
		        float bca = shareCount * 1.0f / mExistCount[BCA];
		        float bac = shareCount * 1.0f / mExistCount[ABC];
		        float cab = shareCount * 1.0f / mExistCount[ACB];
		        float cba = shareCount * 1.0f / mExistCount[BCA];

		        mProbs.Add(ABC + " " + nodeC.mName, abc);
		        mProbs.Add(ACB + " " + nodeB.mName, acb);
		        mProbs.Add(BCA + " " + nodeA.mName, bca);
		        mProbs.Add(BAC + " " + nodeC.mName, bac);
		        mProbs.Add(CAB + " " + nodeB.mName, cab);
		        mProbs.Add(CBA + " " + nodeA.mName, cba);
		        mExistCount.Add(ABC + " " + nodeC.mName, shareCount);
	        }
        }

        int CalcuelateSharedCount(FPTreeNode nodeA, FPTreeNode nodeB, FPTreeNode nodeC)
        {
	        int count = 0;
	        FPTreeNode next = nodeC.NextNode;
	        FPTreeNode parent = null;
	        bool flagB = false;
	        bool flagA = false;
	        while (next != null)
	        {
		        parent = next.mParent;
		        while (parent != null)
		        {
			        if (!flagB && parent.mName == nodeB.mName)
			        {
				        flagB = true;
			        }
			        else if (flagB && !flagA && parent.mName == nodeA.mName)
			        {
				        flagA = true;
			        }
			        else if(flagB && flagA)
			        {
				        count += next.mCount;
				        break;
			        }
			        parent = parent.mParent;
		        }
		        flagB = false;
		        flagA = false;
                next = next.NextNode;
	        }
	        return count;
        }
        
        int CalcuelateSharedCount(FPTreeNode nodeA, FPTreeNode nodeB)
        {
	        int count = 0;
	        FPTreeNode next = nodeB.NextNode;
	        FPTreeNode parent = null;
	        while (next != null)
	        {
		        parent = next.mParent;
		        while (parent != null)
		        {
			        if (parent.mName == nodeA.mName)
			        {
				        count += next.mCount;
				        break;
			        }
			        parent = parent.mParent;
		        }
                next = next.NextNode;
	        }
	        return count;
        }

        private FPNodeHeadTable BuildHeaderTable(FPTransitions transitions)
        {
	        FPNodeHeadTable table = new FPNodeHeadTable();
            foreach (FPItem item in transitions.mItems.Values)
	        {
                if (item.mSupport > mMinSupport)
		        {
                    table.AddTableNode(item.CreateNode());
		        }
	        }
            table.Sort();
	        return table;
        }

        void SortByTable(FPTransition transition, FPNodeHeadTable nodeTable, ref Queue<FPItem> result)
        {
	        int size = nodeTable.Size();
	        for (int i = 0; i < size; ++i)
	        {
		        if (transition.ContainItem(nodeTable.At(i).mOwner))
		        {
			        result.Enqueue(nodeTable.At(i).mOwner);
		        }
	        }
        }

        private FPTreeNode BuildFPTree(FPTransitions transitions, FPNodeHeadTable table)
        {
	        FPTreeNode root = new FPTreeNode("", null);
	        int size = transitions.Size();
	        for (int i = 0; i < size; ++i)
	        {
		        Queue<FPItem> record = new Queue<FPItem>();
                SortByTable(transitions[i], table, ref record);
		        FPTreeNode subTreeRoot = root;
		        FPTreeNode tmpRoot = null;
                while (record.Count > 0 && ((tmpRoot = subTreeRoot.FindChild(record.Peek().mName)) != null))
		        {
                    tmpRoot.Increasement(1);
                    subTreeRoot = tmpRoot;
                    record.Dequeue();
		        }
                AddNodes(subTreeRoot, record, table);
	        }
	        return root;
        }

        void AddNodes(FPTreeNode ancestor, Queue<FPItem> record, FPNodeHeadTable nodeTable)
        {
	        int size = record.Count;
	        if (size > 0)
	        {
			    FPItem item = record.Dequeue();
			    FPTreeNode node = item.CreateNode();
			    node.mCount = 1;
			    node.mParent = ancestor;
			    ancestor.AddChild(node);
			    int count = nodeTable.Size();
			    FPTreeNode tempNode = null;
			    for (int j = 0; j < count; ++j)
			    {
				    tempNode = nodeTable.At(j);
				    if (tempNode.mName == node.mName)
				    {
                        node.NextNode = tempNode.NextNode;
                        tempNode.NextNode = node;
                        //while (tempNode.NextNode != null)
                        //{
                        //    tempNode = tempNode.NextNode;
                        //}
					    //tempNode.NextNode = node;
					    break;
				    }
			    }
			    AddNodes(node, record, nodeTable);
	        }

        }

    }

    // 关联分析
    public class FPGrowth
    {
        public static void GrowthProb(string fileName)
        {
	        FPTransitions transitions = LoadFromFile(fileName);
	        FPTree fpTree = new FPTree(3);
	        fpTree.mMaxDepth = 1;
	        fpTree.GrowthProb(transitions);
            fpTree.Print();
        }

        public static void Growth(string fileName)
        {
            FPTransitions transitions = LoadFromFile(fileName);
            FPTree fpTree = new FPTree(3);
            fpTree.mMaxDepth = 3;
            List<string> tmp = new List<string>();
            fpTree.Growth(transitions, tmp);
        }

        private static FPTransitions LoadFromFile(string fileName, bool duplicate = false)
        {
            List<string> tmp = new List<string>();
            tmp.Add("1 2 3 4 5");
            tmp.Add("1 3 5 10");
            tmp.Add("2 4 6 12");
            tmp.Add("1 4 7 13");
            tmp.Add("1 4 7 13");
            FPTransitions transitions = ParseFromArray(tmp, duplicate);
            return transitions;
        }


        public static FPTransitions ParseFromArray(List<string> sources, bool duplicate)
        {
            FPTransitions transitions = new FPTransitions(duplicate);
            int isize = sources.Count;
            for (int i = 0; i < isize; ++i)
            {
                FPTransition transition = ParseStringToTransition(transitions, sources[i]);
                transitions.AddTransition(transition);
            }
            return transitions;
        }
        private static FPTransition ParseStringToTransition(FPTransitions transitions, string source)
        {
            List<string> arrayOfTransition = new List<string>();
            string temp = source.Trim();
            string[] itels = temp.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            arrayOfTransition.AddRange(itels);
            FPTransition transition = ParseArrayToTransition(transitions, arrayOfTransition);
            return transition;
        }
        private static FPTransition ParseArrayToTransition(FPTransitions transitions, List<string> sourceArray)
        {
            FPTransition transition = new FPTransition();
            int size = sourceArray.Count;
            for (int i = 0; i < size; ++i)
            {
                FPItem item = ParseStringToItem(transitions, sourceArray[i]);
                transition.AddItem(item, transitions.mDuplicate);
            }
            return transition;
        }

        private static FPItem ParseStringToItem(FPTransitions transitions, string source)
        {
            return transitions.AddItem(source);
        }
    }
}
