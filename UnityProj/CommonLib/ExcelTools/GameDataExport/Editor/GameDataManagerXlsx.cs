
using System;

namespace Nullspace
{
    public partial class GameDataManager
    {
        public static void CheckData()
        {
            LoadGameDataTypes();
            Console.WriteLine("GameDataType Count: " + mGameDataTypes.Count);
        }
    }
}
