using UnityEngine;

namespace Assets._Scripts.Helpers
{
    public abstract class Singleton<T> : MonoBehaviour
        where T : Singleton<T>
    {
        public static T Instance { get; private set; } 

        /// <summary>
        /// </summary>
        public static bool IsInitialized => Instance != null;

        /// <summary>
        /// If this is set to true, Dont Destroy On Load; true by default.
        /// This lets an individual class decide if the object persists through the entire game or only the current scene.
        /// </summary>
        public bool IsPersisted { get; protected set; } = true;

        /// <summary>
        /// </summary>
        protected virtual void Awake()
        {
            if (IsInitialized)
            {
                Destroy(gameObject);
                throw new Exceptions.DuplicateSingletonException("[" + typeof(T).Name + "] Attempt to instantiate a second instance of Singleton class.");
            }

            Instance = (T)this;

            if(IsPersisted)
                DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (Instance != this)
                throw new Exceptions.DuplicateSingletonException($"[{typeof(T).Name}] Destroying duplicate singleton.");

            Instance = null;
        }

    }
}
