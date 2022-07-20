using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Assets._Scripts.GameManagement.BaseTypes;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using static Assets._Scripts.Helpers.Enums;

namespace Assets._Scripts.GameManagement
{
    public class GameManager : GameManagerBase<GameManager>
    {

#if UNITY_EDITOR
        #region Unity Editor 

        /// <summary>
        /// 
        /// </summary>
        public static IGameState DefaultGameState { get; set; }

        #endregion
#endif


        #region Properties and Backing Fields

        /// <summary>
        /// Uses reflection to return array of all game states that have been added
        /// </summary>
        private static IGameState[] _gameStates;
        public static IGameState[] GameStates
        {
            get
            {
                return _gameStates ??= typeof(GameState)
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Select(f => (IGameState)f.GetValue(null)).ToArray();
            }
        }



        #endregion

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        protected override void AwakeGameManager()
        {
#if UNITY_EDITOR
            // Default game state is set from the dropdown list in GMInitializer, only used in Editor
            if (DefaultGameState != null)
                CurrentGameState = DefaultGameState;
#endif

            if(CurrentGameState == GameStateBase.Default && CurrentSceneName == nameof(SceneName.Boot))
                LoadScene(SceneName.TitleScreen, GameState.TitleScreen);
        }

        protected override void OnDestroyGameManager()
        {
        }

        protected override void OnGameStateChanged(IGameState prevState, IGameState newState)
        {
        }

        protected override void OnLoadOperationCompleted(LoadOperationParams p)
        {
            
        }

        #endregion


        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void StartGame(string playerName)
        {
            LoadScene(SceneName.Main, GameState.Running, false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
        }

        #endregion

    }
}
