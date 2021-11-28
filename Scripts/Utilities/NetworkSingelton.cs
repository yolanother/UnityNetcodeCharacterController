using Unity.Netcode;
using UnityEngine;

namespace DoubTech.Networking
{
    public class NetworkSingelton<T> : NetworkBehaviour where T : Component
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (null == instance)
                {
                    var objs = FindObjectsOfType<T>();
                    if (objs.Length > 0) instance = objs[0];
                    if (objs.Length > 1)
                    {
                        Debug.LogError($"There is more than one {typeof(T).Name} in the scene.");
                    }

                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        instance = obj.AddComponent<T>();
                    }
                }

                return instance;
            }
        }
    }
}
