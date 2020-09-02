
using UnityEngine;

namespace Nullspace
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T mInstance;
        private static object _mLock = new object();
        private static bool mApplicationIsQuitting = false;
        public static T Instance
        {
            get
            {
                lock (_mLock)
                {
                    if (mInstance == null)
                    {
                        mInstance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            return mInstance;
                        }

                        if (mInstance == null)
                        {
                            GameObject singleton = new GameObject();
                            mInstance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();

                            DontDestroyOnLoad(singleton);
                        }
                    }

                    return mInstance;
                }
            }
        }

        protected virtual void OnDestroy()
        {
            mApplicationIsQuitting = true;
        }

        public static bool IsDestroy()
        {
            return mApplicationIsQuitting;
        }


    }

}

