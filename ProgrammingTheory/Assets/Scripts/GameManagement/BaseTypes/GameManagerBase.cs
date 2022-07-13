using System;
using System.Collections.Generic;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Assets.Scripts.Helpers.Enums;
using static Assets.Scripts.Helpers.EventArgs;
using static Assets.Scripts.Helpers.Events;

namespace Assets.Scripts.GameManagement.BaseTypes
{
    /// <summary>
    /// Manages the game. Idea is that the GameManager should handle the complex operations that deal with changing scenes and game state.
    /// Most communication to the GameManager should be done through a SystemManager, except public GameManager events or accessors
    /// </summary>
    /// <typeparam name="G">GameManager type</typeparam>
    public abstract class GameManagerBase<G> : Singleton<G>, IGameManager
        where G : GameManagerBase<G>
    {
        #region Fields

        /// <summary>
        ///  Other globally-accessible SystemManagers the GameManager will instantiate and clean up
        /// </summary>
        [Tooltip("This array should consist only of prefab game objects that do not exist in the scene by default.")]
        [SerializeField]
        private GameObject[] SystemManagerPrefabs;

        #endregion


        #region Properties and Backing Fields

        /// <summary>
        /// Unfinished load operations, which get removed once complete
        /// </summary>
        protected Dictionary<AsyncOperation, string> LoadOperations { get; set; }

        /// <summary>
        /// SystemManger types that are managed by this GameManager.
        /// </summary>
        protected HashSet<ISystemManager> SystemManagers { get; set; }

        /// <summary>
        /// False when all loads are done
        /// </summary>
        public bool IsLoadingScene => LoadOperations.Count > 0;

        /// <summary>
        /// </summary>
        public string CurrentSceneName { get; protected set; }

        /// <summary>
        /// </summary>
        public IGameState CurrentGameState { get; protected set; }

        /// <summary>
        /// </summary>
        protected Stack<string> LoadedScenes;

        #endregion

        #region Events

        /// <summary>
        /// Invoked after a GameState has been updated
        /// </summary>
        public event GameStateChangedEventHandler GameStateChanged;

        /// <summary>
        /// Invoked after a scene load or unload operation has completed
        /// </summary>
        private event LoadOperationCompletedEventHandler LoadOperationCompleted;

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Called in the Awake method, after successful initialization
        /// </summary>
        protected abstract void AwakeGameManager();

        /// <summary>
        /// Listener of GameStateChanged event. Invoked after changing the game state.
        /// </summary>
        /// <param name="prevState">CurrentGameState before the change</param>
        /// <param name="newState">New CurrentGameState after the change</param>
        protected abstract void OnGameStateChanged(IGameState prevState, IGameState newState);

        /// <summary>
        /// Listener of LoadOperationCompleted event. Invoked after a scene load operation has completed
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="opType"></param>
        protected abstract void OnLoadOperationCompleted(string sceneName, LoadOperationType opType);

        #endregion


        #region Methods

        /// <summary>
        /// If initialized successfully, add this SystemManager type to its GameManager's list if necessary. Then call AwakeSystemManager().
        /// </summary>
        protected sealed override void Awake()
        {
            try
            {
                base.Awake();

                // Initialized successfully. 
                // Perform initializations then call the "real" Awake method defined by the derived class.


                CurrentGameState = GameStateBase.Pregame;
                SystemManagers = new HashSet<ISystemManager>();
                LoadOperations = new Dictionary<AsyncOperation, string>();

                InstantiateSystemManagerPrefabs();

                AwakeGameManager();

            }
            catch (Exceptions.DuplicateSingletonException e)
            {
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// </summary>
        protected virtual void OnEnable()
        {
            // TODO: figure out how to do this without extension method, during instantiation of system manager
            //ExtensionMethods.AddOrRemoveSystemManagerTypeEvent += OnAddOrRemoveSystemManagerType;
            //SystemManagerBase<G>.AddSystemManagerType += OnAddSystemManagerType;
        }

        /// <summary>
        /// </summary>
        protected virtual void OnDisable()
        {
            //ExtensionMethods.AddOrRemoveSystemManagerTypeEvent -= OnAddOrRemoveSystemManagerType;
        }

        /// <summary>
        /// Clean up other global system managers and their game objects
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var sm in SystemManagers)
            {
                if(sm is MonoBehaviour m)
                    Destroy(m.gameObject);
            }

            SystemManagers.Clear();
        }

        /// <summary>
        /// Instantiate other globally-accessible system managers. These Game Objects are added to the GameManager through the Inspector.
        /// </summary>
        private void InstantiateSystemManagerPrefabs()
        {
            foreach (var prefab in SystemManagerPrefabs)
            {
                // Do not instantiate unless the game object has a SystemManager component
                var sm = prefab.GetComponent<ISystemManager>();
                if (sm == null)
                {
                    Debug.LogWarning($"[{typeof(G).Name}] Attempt to instantiate a GameObject without a SystemManager component. " +
                                            $"GameObject '{prefab.name}' will not be created unless a SystemManager component is added.");
                    continue;
                }

                // Add SM to list before instantiating so SystemManager.Awake() can determine if it has been added
                SystemManagers.Add(sm);
                Instantiate(prefab);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="t">SystemManager type to check for</param>
        /// <returns>true if SystemManager is being managed by this GameManager</returns>
        public bool SystemManagerExists(ISystemManager sm)
        {
            return SystemManagers.Contains(sm);
        }

        internal void AddSystemManager(ISystemManager sm)
        {
            if (SystemManagerExists(sm))
                return;

            SystemManagers.Add(sm);
            OnSystemManagerAdded(sm);
        }

        internal void RemoveSystemManager(ISystemManager sm)
        {
            if (!SystemManagerExists(sm))
                return;

            SystemManagers.Remove(sm);
            OnSystemManagerRemoved(sm);
        }

        protected virtual void OnSystemManagerAdded(ISystemManager sm)
        {
        }

        protected virtual void OnSystemManagerRemoved(ISystemManager sm)
        {
        }

        /// <summary>
        /// Changes the current game state and invokes the GameStateChanged event
        /// </summary>
        /// <param name="newState">State to change the game to</param>
        protected void ChangeGameState(IGameState newState)
        {
            try
            {
                if (CurrentGameState == newState)
                    return;

                var prevState = CurrentGameState;
                CurrentGameState = newState;


                GameStateChanged?.Invoke(this, new GameStateChangedEventArgs(prevState, newState));
                OnGameStateChanged(prevState, newState);
                Debug.Log("Game State changed from " + prevState + " to " + CurrentGameState);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }


        /// <summary>
        /// Load a scene async and additive
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        protected AsyncOperation LoadScene(string sceneName)
        {
            // returns an AsyncOperation; additive so that it doesn't replace current scene
            var ao = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            if (ao == null)
            {
                Debug.LogError($"[{typeof(G).Name}] Unable to load scene '{sceneName}'");
                return null;
            }

            ao.completed += AOLoadCompleted;
            LoadOperations.Add(ao, sceneName);
            return ao;
        }

        /// <summary>
        /// Load a scene async and additive
        /// </summary>
        /// <param name="index">build index of the scene</param>
        /// <returns></returns>
        protected AsyncOperation LoadScene(int index)
        {
            var scene = SceneManager.GetSceneByBuildIndex(index);
            if (scene.IsValid())
                return LoadScene(scene.name);

            Debug.LogError($"[{typeof(G).Name}] Unable to load scene '{index}'");
            return null;
        }

        /// <summary>
        /// Unload a scene
        /// </summary>
        /// <param name="sceneName">Name of the scene to unload</param>
        protected AsyncOperation UnloadScene(string sceneName)
        {
            var ao = SceneManager.UnloadSceneAsync(sceneName);

            if (ao == null)
            {
                Debug.LogError($"[{typeof(G).Name}] Unable to unload scene {sceneName}");
                return null;
            }

            ao.completed += AOUnloadCompleted;
            LoadOperations.Add(ao, sceneName);
            return ao;
        }

        /// <summary>
        /// Unload a scene
        /// </summary>
        /// <param name="index">build index of the scene</param>
        /// <returns></returns>
        protected AsyncOperation UnloadScene(int index)
        {
            var scene = SceneManager.GetSceneByBuildIndex(index);
            if (scene.IsValid())
                return UnloadScene(scene.name);

            Debug.LogError($"[{typeof(G).Name}] Unable to unload scene '{index}'");
            return null;
        }


        /// <summary>
        /// Remove the load operation from the list and Invoke the LoadOperationCompleted event
        /// </summary>
        /// <param name="ao">The async operation that completed</param>
        /// <param name="opType">load operation type</param>
        private void AOCompleted(AsyncOperation ao, LoadOperationType opType)
        {
            try
            {
                if (!LoadOperations.ContainsKey(ao))
                    throw new Exceptions.LoadOperationNotContainedInListException($"[{typeof(G).Name}] Cannot call AOCompleted method with an AsyncOperation that hasn't been added to the LoadOperations dictionary.");

                LoadOperations.TryGetValue(ao, out var sceneName);
                LoadOperations.Remove(ao);
                LoadOperationCompleted?.Invoke(this, new LoadOperationCompletedEventArgs(sceneName, opType));
                OnLoadOperationCompleted(sceneName, opType);
            }
            catch (Exceptions.LoadOperationNotContainedInListException e)
            {
                Debug.LogError(e.Message);
            }
        }

        #endregion


        #region Events

        /// <summary>
        /// Call AOCompleted method with true to indicate a load operation
        /// </summary>
        /// <param name="ao">The async operation that has completed</param>
        private void AOLoadCompleted(AsyncOperation ao)
        {
            ao.completed -= AOLoadCompleted;
            AOCompleted(ao, LoadOperationType.Load);
        }

        /// <summary>
        /// Call AOCompleted method with false to indicate an unload operation
        /// </summary>
        /// <param name="ao">The async operation that has completed</param>
        private void AOUnloadCompleted(AsyncOperation ao)
        {
            ao.completed -= AOUnloadCompleted;
            AOCompleted(ao, LoadOperationType.Unload);
        }

        #endregion


    }
}
