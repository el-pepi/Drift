using UnityEngine;

namespace VascoGames.Common
{
    public abstract class Singleton<S> : MonoBehaviour where S : Singleton<S>
    {
        private static S instance = null;
        protected static bool dontDestroyOnLoad = true;
        private static object instanceLock = new object();
        public static S Instance { get { return GetInstance(); } }
        public static S UnSafeInstance { get { return instance; } }
        private bool initialised;

        bool willDestroy = false;

        protected bool WillBeDestroyed{
        
          get{ return willDestroy; }
        }

        public static S GetInstance()
        {
            if (instance != null)
                return instance;

            lock (instanceLock)
            {
                S[] components = GameObject.FindObjectsOfType<S>();

                if (components.Length > 0)
                {
                    instance = components[0];

                    for (int iter = 1; iter < components.Length; iter++)
                        Destroy(components[iter].gameObject);
                }

                if (instance == null)
                {
                    string name = typeof(S).Name;
                    GameObject prefab = (GameObject)Resources.Load("Singleton/" + name, typeof(GameObject));

                    if (prefab != null)
                    {
                        //Debug.Log("--------------Creating prefab singeton " + name + "---------------");
#if UNITY_4_6 || UNITY_4_7
                        instance = (Instantiate(prefab) as S).GetComponent<S>();
#else
                        instance = (S)Instantiate(prefab).GetComponent<S>();
#endif
                    }
                    else
                    {
                        //Debug.Log("--------------Creating singeton " + name + "---------------");
                        GameObject newSingleton = new GameObject(name);
                        instance = newSingleton.AddComponent<S>();
                    }

                    if (instance == null)
                    {
                        if (!instance.initialised)
                        {
                            instance.initialised = true;
                            instance.Ini();
                        }
                    }
                }
            }

            return instance;
        }

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                willDestroy = true;
                Destroy(this.gameObject);
                return;
            }

            if (instance == null)
            {
                instance = this as S;

                if (!initialised)
                {
                    initialised = true;
                    Ini();
                }
            }

            if (dontDestroyOnLoad)
                DontDestroyOnLoad(instance.transform.root.gameObject);
        }

        protected virtual void Ini(){ }
    }
}

