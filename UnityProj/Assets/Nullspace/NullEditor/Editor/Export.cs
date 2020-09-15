using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

public class Export
{
    [MenuItem("Assets/DeleteMissingAssets")]
    public static void DeleteMissingAssets()
    {
        Object obj = Selection.activeObject;
        if (obj is NodeGraph nodeGraph)
        {
            string assetpath = AssetDatabase.GetAssetPath(obj);
            Object[] nodes = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetpath);
            List<Object> validAssets = new List<Object>();
            bool delete = false;
            foreach (Object node in nodes)
            {
                if (node == null)
                {
                    delete = true;
                    break;
                }
            }
            if (delete)
            {
                DeleteNullAssets(nodeGraph);
            }
        }
    }

    private static void DeleteNullAssets(NodeGraph toDelete)
    {
        //Create a new instance of the object to delete
        ScriptableObject newInstance = ScriptableObject.CreateInstance(toDelete.GetType());
        //Copy the original content to the new instance
        EditorUtility.CopySerialized(toDelete, newInstance);
        newInstance.name = toDelete.name;

        string toDeletePath = AssetDatabase.GetAssetPath(toDelete);
        string clonePath = toDeletePath.Replace(".asset", "CLONE.asset");
        //Create the new asset on the project files
        AssetDatabase.CreateAsset(newInstance, clonePath);
        AssetDatabase.ImportAsset(clonePath);

        //Unhide sub-assets
        var subAssets = AssetDatabase.LoadAllAssetsAtPath(toDeletePath);
        HideFlags[] flags = new HideFlags[subAssets.Length];
        for (int i = 0; i < subAssets.Length; i++)
        {
            //Ignore the "corrupt" one
            if (subAssets[i] == null)
                continue;
            //Store the previous hide flag
            flags[i] = subAssets[i].hideFlags;
            subAssets[i].hideFlags = HideFlags.None;
            EditorUtility.SetDirty(subAssets[i]);
        }



        EditorUtility.SetDirty(toDelete);
        AssetDatabase.SaveAssets();
        //Reparent the subAssets to the new instance
        foreach (var subAsset in AssetDatabase.LoadAllAssetRepresentationsAtPath(toDeletePath))
        {
            //Ignore the "corrupt" one
            if (subAsset == null)
                continue;

            //We need to remove the parent before setting a new one
            AssetDatabase.RemoveObjectFromAsset(subAsset);
            AssetDatabase.AddObjectToAsset(subAsset, newInstance);
        }
        //Import both assets back to unity
        AssetDatabase.ImportAsset(toDeletePath);
        AssetDatabase.ImportAsset(clonePath);

        //Reset sub-asset flags
        for (int i = 0; i < subAssets.Length; i++)
        {
            //Ignore the "corrupt" one
            if (subAssets[i] == null)
                continue;
            subAssets[i].hideFlags = flags[i];
            EditorUtility.SetDirty(subAssets[i]);
        }

        EditorUtility.SetDirty(newInstance);
        AssetDatabase.SaveAssets();

        //Here's the magic. First, we need the system path of the assets
        string globalToDeletePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), toDeletePath);
        string globalClonePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), clonePath);

        //We need to delete the original file (the one with the missing script asset)
        //Rename the clone to the original file and finally
        //Delete the meta file from the clone since it no longer exists

        File.Delete(globalToDeletePath);
        File.Delete(globalClonePath + ".meta");
        File.Move(globalClonePath, globalToDeletePath);
        AssetDatabase.Refresh();
        Object mainObj = AssetDatabase.LoadMainAssetAtPath(toDeletePath);
        Node nodeAsset = mainObj as Node;
        // If the renamed asset is a node graph, but the v2 AssetDatabase has swapped a sub-asset node to be its
        // main asset, reset the node graph to be the main asset and rename the node asset back to its default
        // name.
        if (nodeAsset != null && AssetDatabase.IsMainAsset(nodeAsset))
        {
            AssetDatabase.SetMainObject(nodeAsset.graph, toDeletePath);
            AssetDatabase.ImportAsset(toDeletePath);
            if (nodeAsset != null)
            {
                nodeAsset.name = NodeEditorUtilities.NodeDefaultName(nodeAsset.GetType());
                EditorUtility.SetDirty(nodeAsset);
            }
        }

        AssetDatabase.Refresh();

    }
}
