
using System.Collections.Generic;
using System.IO;
using XNode;

namespace Nullspace
{
    public class XUtils
    {
        public const string AssetDir = "Assets/Nullspace/NullEditor/Assets";
        public const string TestGraphName = "TestGraph";

        static XUtils()
        {
            if (!Directory.Exists(AssetDir))
            {
                Directory.CreateDirectory(AssetDir);
            }
        }
        
        public static T GetNode<T>(List<Node> nodes) where T : Node
        {
            return (T)nodes.Find((item) => { return typeof(T) == item.GetType(); });
        }

        public static List<Node> GetNodes<T>(List<Node> nodes) where T : Node
        {
            return nodes.FindAll((item) => { return typeof(T) == item.GetType(); });
        }
    }
}
