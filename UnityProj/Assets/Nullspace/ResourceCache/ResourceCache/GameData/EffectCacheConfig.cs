using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameData
{
    public class EffectCacheConfig : ResourceConfig<EffectCacheConfig>
    {
        public static readonly string FileUrl = "ResourceConfig#ResourceConfigs";
        public static readonly bool IsDelayInitialized = true;
        public static readonly List<string> KeyNameList = new List<string>() { "ID" };
    }
}
