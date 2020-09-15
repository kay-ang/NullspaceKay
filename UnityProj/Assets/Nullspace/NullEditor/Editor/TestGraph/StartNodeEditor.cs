using TestGraph;
using UnityEngine;
using XNodeEditor;


[CustomNodeEditor(typeof(StartNode))]
public class StartNodeEditor : NodeEditor
{
    protected override string[] GetDynamicExcudes()
    {
        return null;
    }

    public override void OnHeaderGUI()
    {
        GUILayout.Label(string.Format("{0}_{1}", target.name, ((StartNode)target).Start), NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
    }
}