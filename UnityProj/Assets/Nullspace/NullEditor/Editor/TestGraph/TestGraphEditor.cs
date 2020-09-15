
using Nullspace;
using System;
using XNodeEditor;

[CustomNodeGraphEditor(typeof(TestNodeGraph))]
class TestGraphEditor : NodeGraphEditor
{
    public override string GetNodeMenuName(Type type)
    {
        if (type.Namespace == "TestGraph")
        {
            return base.GetNodeMenuName(type);
        }
        else
        {
            return null;
        }
    }
}