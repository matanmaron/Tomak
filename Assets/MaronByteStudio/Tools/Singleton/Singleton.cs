/**
 * Singleton.cs
 * Author: Luke Holland (http://lukeholland.me/)
 * Modified by: MaronByteStudio
 */
using UnityEngine;

namespace MaronByteStudio
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _instanceLock = new object();
        private static bool _quitting = false;

        /// <summary>
        /// Override this property in your derived singleton class to prevent DontDestroyOnLoad.
        /// ex: protected override bool PersistBetweenScenes => true;
        /// </summary>
        protected virtual bool PersistBetweenScenes => false;

        public static T instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null && !_quitting)
                    {
                        _instance = GameObject.FindObjectOfType<T>();
                        if (_instance == null)
                        {
                            GameObject go = new GameObject(typeof(T).ToString());
                            _instance = go.AddComponent<T>();

                            // Handle persistence here
                            Singleton<T> singletonScript = _instance as Singleton<T>;
                            if (singletonScript != null && singletonScript.PersistBetweenScenes)
                            {
                                DontDestroyOnLoad(_instance.gameObject);
                            }
                        }
                    }

                    return _instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = gameObject.GetComponent<T>();

                if (PersistBetweenScenes)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (_instance.GetInstanceID() != GetInstanceID())
            {
                Destroy(gameObject);
                throw new System.Exception($"Instance of {GetType().FullName} already exists, removing {ToString()}");
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _quitting = true;
        }
    }
}
