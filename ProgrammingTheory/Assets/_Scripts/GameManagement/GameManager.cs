using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Assets._Scripts.GameManagement.BaseTypes;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using static Assets._Scripts.Helpers.Enums;
using static Assets._Scripts.Helpers.EventArgs;
using static Assets._Scripts.Helpers.Events;

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

        /// <summary>
        /// </summary>
        public string PlayerName { get; protected set; }

        /// <summary>
        /// </summary>
        public int CurrentScore { get; protected set; }

        #endregion

        #region Events

        /// <summary>
        /// Invoked when score changed
        /// </summary>
        public event ScoreChangedEventHandler ScoreChanged;

        #endregion

        #region Overrides

        /// <summary>
        /// </summary>
        protected override void AwakeGameManager()
        {
#if UNITY_EDITOR
            // Default game state is set from the dropdown list in GMInitializer, only used in Editor
            if (DefaultGameState != null)
            {
                ChangeGameState(DefaultGameState);
            }
#endif

            if(CurrentGameState == GameStateBase.Default && CurrentSceneName == nameof(SceneName.Boot))
                LoadScene(SceneName.TitleScreen, GameState.TitleScreen);

            // Load save data
            DataManager.LoadData();
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
            PlayerName = playerName;
            LoadScene(SceneName.Main, GameState.Running, true);
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

        /// <summary>
        /// 
        /// </summary>
        public void PauseGame()
        {
            ChangeGameState(GameState.Paused);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResumeGame()
        {
            ChangeGameState(GameState.Running);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RestartGame()
        {
            LoadScene(SceneName.Main, GameState.Running, true);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReturnToTitleScreen()
        {
            LoadScene(SceneName.TitleScreen, GameState.TitleScreen, true);
        }

        /// <summary>
        /// 
        /// </summary>
        public void GameOver()
        {
            DataManager.AddHighScore(PlayerName, CurrentScore);
            DataManager.SaveData();
            ChangeGameState(GameState.GameOver);
            ResetScore();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="score">Score to add</param>
        public void AddScore(int score)
        {
            CurrentScore += score;
            ScoreChanged?.Invoke(this, new ScoreChangedEventArgs(CurrentScore));
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetScore()
        {
            CurrentScore = 0;
            ScoreChanged?.Invoke(this, new ScoreChangedEventArgs(CurrentScore));
        }

        #endregion

    }
}
