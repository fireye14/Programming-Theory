using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Assets._Scripts.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Assets._Scripts.Helpers.Enums;
using static Assets._Scripts.Helpers.EventArgs;
using static Assets._Scripts.Helpers.Events;

namespace Assets._Scripts.GameManagement.BaseTypes
{
    /// <summary>
    /// Manages the game. Idea is that the GameManager should handle the complex operations that deal with changing scenes and game state.
    /// Most communication to the GameManager should be done through a SystemManager, except public GameManager events or accessors
    /// </summary>
    /// <typeparam name="G">GameManager type</typeparam>
    [Serializable]
    public abstract class GameManagerBase<G> : Singleton<G>, IGameManager
        where G : GameManagerBase<G>
    {
        #region Fields

        /// <summary>
        /// Other globally-accessible SystemManagers the GameManager will instantiate and clean up
        /// </summary>
        [Tooltip("This array should consist only of prefab game objects that do not exist in the scene by default.")]
        [SerializeField]
        private GameObject[] SystemManagerPrefabs;

        #endregion

        #region Properties and Backing Fields

        /// <summary>
        /// Unfinished load operations, which get removed once complete
        /// </summary>
        protected Dictionary<AsyncOperation, LoadOperationParams> LoadOperations { get; set; }

        /// <summary>
        /// SystemManger objects that are managed by this GameManager.
        /// </summary>
        protected HashSet<ISystemManager<G>> SystemManagers { get; set; }

        /// <summary>
        /// False when all loads are done
        /// </summary>
        public bool IsLoadingScene => LoadOperations.Count > 0;

        /// <summary>
        /// </summary>
        private string _currentSceneName;
        public string CurrentSceneName
        {
            get
            {
                if (string.IsNullOrEmpty(_currentSceneName))
                    _currentSceneName = SceneManager.GetActiveScene().name;

                return _currentSceneName;
            }
            protected set => _currentSceneName = value;
        }

        /// <summary>
        /// </summary>
        private IGameState _currentGameState;
        public IGameState CurrentGameState
        {
            get => _currentGameState ??= GameStateBase.Default;
            protected set => _currentGameState = value;
        }

        /// <summary>
        /// Audio listener component of the main camera
        /// </summary>
        protected AudioListener CurrentAudioListener
        {
            get
            {
                if (Camera.main is { } cam && cam.gameObject != null && cam.GetComponent<AudioListener>() is { } listener)
                    return listener;

                return null;
            }
        }

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
        public event LoadOperationEventHandler LoadOperationCompleted;

        /// <summary>
        /// Invoked when loading a scene and fade is set to true
        /// </summary>
        public event AsyncOperationEventHandler SceneTransitionFadeBegin;

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Called in the Awake method, after successful initialization
        /// </summary>
        protected abstract void AwakeGameManager();

        /// <summary>
        /// Called in the OnDestroy method, after successful destroy
        /// </summary>
        protected abstract void OnDestroyGameManager();

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
        protected abstract void OnLoadOperationCompleted(LoadOperationParams p);

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

                SystemManagers = new HashSet<ISystemManager<G>>();
                LoadOperations = new Dictionary<AsyncOperation, LoadOperationParams>();

                InstantiateSystemManagerPrefabs();

                AwakeGameManager();
            }
            catch (Exceptions.DuplicateSingletonException e)
            {
                Debug.LogWarning(e.Message);
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
            try
            {
                foreach (var sm in SystemManagers)
                {
                    var obj = FindObjectOfType(sm.GetType());
                    if (obj is ISystemManager<G> && obj is MonoBehaviour m)
                        Destroy(m.gameObject);
                }

                SystemManagers.Clear();

                base.OnDestroy();
            }
            catch (Exceptions.DuplicateSingletonException e)
            {
                Debug.LogWarning(e.Message);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// Instantiate other globally-accessible system managers. These Game Objects are added to the GameManager through the Inspector.
        /// </summary>
        private void InstantiateSystemManagerPrefabs()
        {
            foreach (var prefab in SystemManagerPrefabs)
            {
                // Do not instantiate unless the game object has a SystemManager component
                var sm = prefab.GetComponent<ISystemManager<G>>();
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
        /// <param name="sm">SystemManager type to check for</param>
        /// <returns>true if SystemManager is being managed by this GameManager</returns>
        public bool SystemManagerExists(ISystemManager<G> sm)
        {
            return SystemManagers.Contains(sm);
        }

        /// <summary>
        /// Add system manager type
        /// </summary>
        /// <param name="sm">SystemManager type to add</param>
        internal void AddSystemManager(ISystemManager<G> sm)
        {
            if (SystemManagerExists(sm))
                return;

            SystemManagers.Add(sm);
            OnSystemManagerAdded(sm);
        }

        /// <summary>
        /// Remove system manager type
        /// </summary>
        /// <param name="sm">SystemManager type to remove</param>
        internal void RemoveSystemManager(ISystemManager<G> sm)
        {
            if (!SystemManagerExists(sm))
                return;

            SystemManagers.Remove(sm);
            OnSystemManagerRemoved(sm);
        }

        /// <summary>
        /// </summary>
        /// <param name="sm"></param>
        protected virtual void OnSystemManagerAdded(ISystemManager<G> sm)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sm"></param>
        protected virtual void OnSystemManagerRemoved(ISystemManager<G> sm)
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
                Debug.Log($"Game State changed from {prevState} to {CurrentGameState}.");
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
        protected AsyncOperation LoadScene(string sceneName, IGameState newGameState = null, bool fade = false)
        {
            // returns an AsyncOperation; additive so that it doesn't replace current scene
            var ao = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            if (ao == null)
            {
                Debug.LogError($"[{typeof(G).Name}] Unable to load scene '{sceneName}'");
                return null;
            }

            ao.allowSceneActivation = true;
            ao.completed += AOCompleted;

            LoadOperations.Add(ao, new LoadOperationParams(CurrentSceneName, sceneName, CurrentGameState, newGameState,
                LoadOperationType.Load, fade));

            if(fade)
                SceneTransitionFadeBegin?.Invoke(this, new AsyncOperationEventArgs(ao));

            return ao;
        }

        /// <summary>
        /// Load a scene async and additive
        /// </summary>
        /// <param name="index">build index of the scene</param>
        /// <returns></returns>
        protected AsyncOperation LoadScene(int index, IGameState newGameState = null, bool fade = false)
        {
            var scene = SceneManager.GetSceneByBuildIndex(index);
            if (scene.IsValid())
                return LoadScene(scene.name, newGameState, fade);

            Debug.LogError($"[{typeof(G).Name}] Scene '{index}' is invalid.");
            return null;
        }

        /// <summary>
        /// Load a scene async and additive
        /// </summary>
        /// <param name="scene">name of scene to load</param>
        /// <returns></returns>
        protected AsyncOperation LoadScene(SceneName scene, IGameState newGameState = null, bool fade = false)
        {
            return LoadScene(scene.ToString(), newGameState, fade);
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

            ao.completed += AOCompleted;
            LoadOperations.Add(ao,
                new LoadOperationParams(CurrentSceneName, null, CurrentGameState, null, LoadOperationType.Unload));
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
        /// Unload a scene 
        /// </summary>
        /// <param name="scene">name of scene to unload</param>
        /// <returns></returns>
        protected AsyncOperation UnloadScene(SceneName scene)
        {
            return UnloadScene(scene.ToString());
        }


        /// <summary>
        /// Remove the load operation from the list and Invoke the LoadOperationCompleted event
        /// </summary>
        /// <param name="ao">The async operation that completed</param>
        private void AOCompleted(AsyncOperation ao)
        {
            try
            {
                ao.completed -= AOCompleted;

                if (!LoadOperations.ContainsKey(ao))
                    throw new Exceptions.LoadOperationNotContainedInListException(
                        $"[{typeof(G).Name}] Cannot call AOCompleted method with an AsyncOperation that hasn't been added to the LoadOperations dictionary.");

                LoadOperations.TryGetValue(ao, out var p);
                LoadOperations.Remove(ao);
                if (p == null)
                    return;

                if (p.OpType == LoadOperationType.Load)
                {
                    if (CurrentAudioListener != null)
                        CurrentAudioListener.enabled = false;

                    // new scene was just loaded
                    UnloadScene(CurrentSceneName);

                    CurrentSceneName = p.NewSceneName;

                    if (p.NewGameState != null)
                        ChangeGameState(p.NewGameState);
                }
                else
                {

                }

                LoadOperationCompleted?.Invoke(this, new LoadOperationEventArgs(p));
                OnLoadOperationCompleted(p);
            }
            catch (Exceptions.LoadOperationNotContainedInListException e)
            {
                Debug.LogError(e.Message);
            }
        }

        #endregion


        #region Events


        #endregion


    }

    public class LoadOperationParams
    {
        public LoadOperationParams(string oldSceneName, string newSceneName, IGameState oldGameState, IGameState newGameState, LoadOperationType opType, bool isFadeTransition = false)
        {
            OldSceneName = oldSceneName; 
            NewSceneName = newSceneName;
            OldGameState = oldGameState;
            NewGameState = newGameState;
            OpType = opType;
            IsFadeTransition = isFadeTransition;
        }

        public string OldSceneName { get; }
        public string NewSceneName { get; }
        public IGameState OldGameState { get; }
        public IGameState NewGameState { get; }
        public LoadOperationType OpType { get; }
        public bool IsFadeTransition { get; }
    }
}
