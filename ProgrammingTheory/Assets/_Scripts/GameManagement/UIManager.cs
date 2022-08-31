
using System;
using System.Runtime.CompilerServices;
using Assets._Scripts.Characters;
using Assets._Scripts.GameManagement.BaseTypes;
using Assets._Scripts.Helpers;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static Assets._Scripts.Helpers.Enums;
using static Assets._Scripts.Helpers.EventArgs;

namespace Assets._Scripts.GameManagement
{
    public class UIManager : SystemManagerBase<UIManager, GameManager>
    {

        #region Fields

        [SerializeField] private GameObject TitleScreenContainer;
        [SerializeField] private GameObject HUDContainer;
        [SerializeField] private PauseMenu _pauseMenu;
        [SerializeField] private GameOverMenu _gameOverMenu;
        [SerializeField] private InputField _nameText;
        [SerializeField] private Text _errorText;
        [SerializeField] private SceneTransitionFader _sceneTransitionFader;
        [SerializeField] private Text _healthText;
        [SerializeField] private Text _scoreText;

        #endregion

        #region Events

        private void GM_LoadOperationCompleted(object sender, LoadOperationEventArgs e)
        {
            try
            {
                OnLoadOperationCompleted(e.OperationParams);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                throw;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GM_SceneTransitionFadeBegin(object sender, AsyncOperationEventArgs e)
        {
            try
            {
                OnSceneTransitionFadeBegin(e.AsyncOperation);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GM_GameStateChanged(object sender, GameStateChangedEventArgs e)
        {
            try
            {
                OnGameStateChanged(e.PrevState, e.NewState);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GM_ScoreChanged(object sender, ScoreChangedEventArgs e)
        {
            try
            {
                OnScoreChanged(e.NewScore);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                throw;
            }
        }

        #endregion


        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        protected override void AwakeSystemManager()
        {

            if(HUDContainer != null)
                HUDContainer.SetActive(false);

            if (_sceneTransitionFader == null)
                _sceneTransitionFader = GetComponentInChildren<SceneTransitionFader>();

            if (_sceneTransitionFader != null)
            {
                _sceneTransitionFader.gameObject.SetActive(true);
                _sceneTransitionFader.gameObject.SetActive(false);
            }

            if (_pauseMenu == null)
                _pauseMenu = GetComponentInChildren<PauseMenu>();

            if (_pauseMenu != null)
            {
                _pauseMenu.gameObject.SetActive(true);
                _pauseMenu.gameObject.SetActive(false);
            }

            if (_gameOverMenu == null)
                _gameOverMenu = GetComponentInChildren<GameOverMenu>();

            if (_gameOverMenu != null)
            {
                _gameOverMenu.gameObject.SetActive(true);
                _gameOverMenu.gameObject.SetActive(false);
            }

            GM.LoadOperationCompleted += GM_LoadOperationCompleted;
            //GM.SceneTransitionFadeBegin += GM_SceneTransitionFadeBegin;
            GM.GameStateChanged += GM_GameStateChanged;
            GM.ScoreChanged += GM_ScoreChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDestroySystemManager()
        {
            if (GM != null)
            {
                GM.LoadOperationCompleted -= GM_LoadOperationCompleted;
                //GM.SceneTransitionFadeBegin -= GM_SceneTransitionFadeBegin;
                GM.GameStateChanged -= GM_GameStateChanged;
                GM.ScoreChanged -= GM_ScoreChanged;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Start()
        {
            _nameText.Select();
            _nameText.ActivateInputField();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Update()
        {
            if (GM.CurrentSceneName == nameof(SceneName.TitleScreen))
            {
                if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    StartGame();
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    QuitGame();
                }
            }
            else if (GM.CurrentGameState == GameState.Running)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    // Escape pressed while game is running
                    PauseGame();
                }
            }
            else if (GM.CurrentGameState == GameState.Paused)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    // Escape pressed while game is paused
                    ResumeGame();
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void StartGame()
        {
            if (GM.IsLoadingScene)
                return;

            if (ValidateName())
            {
                GM.StartGame(_nameText.text.Trim());
            }
            else
            {
                // display error text 'Name must be filled in'
                _errorText.text = "Name must be filled in.";
                _errorText.gameObject.SetActive(true);
                _nameText.Select();
                _nameText.ActivateInputField();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual bool ValidateName() => _nameText.text.Trim() != string.Empty;

        /// <summary>
        /// 
        /// </summary>
        protected virtual void QuitGame()
        {
            GM.QuitGame();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void PauseGame()
        {
            _pauseMenu.gameObject.SetActive(true);
            GM.PauseGame();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void ResumeGame()
        {
            _pauseMenu.gameObject.SetActive(false);
            GM.ResumeGame();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void RestartGame()
        {
            _gameOverMenu.gameObject.SetActive(false);
            _pauseMenu.gameObject.SetActive(false);
            GM.RestartGame();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void ReturnToTitleScreen()
        {
            _gameOverMenu.gameObject.SetActive(false);
            _pauseMenu.gameObject.SetActive(false);
            GM.ReturnToTitleScreen();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        protected void OnLoadOperationCompleted(LoadOperationParams p)
        {
            if (p.OpType == LoadOperationType.Load)
            {
                // loading TitleScreen
                if (p.NewSceneName == nameof(SceneName.TitleScreen))
                {
                    if (TitleScreenContainer != null)
                        TitleScreenContainer.SetActive(true);
                }
                // loading new scene after TitleScreen
                else if (p.OldSceneName == nameof(SceneName.TitleScreen))
                {
                    if (TitleScreenContainer != null)
                        TitleScreenContainer.SetActive(false);
                }

                if(p.IsFadeTransition)
                    _sceneTransitionFader.FadeOut();
            }
            else
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ao"></param>
        protected void OnSceneTransitionFadeBegin(AsyncOperation ao)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prevState"></param>
        /// <param name="newState"></param>
        protected void OnGameStateChanged(IGameState prevState, IGameState newState)
        {
            if (newState == GameState.GameOver)
            {
                _gameOverMenu.gameObject.SetActive(true);
                _gameOverMenu.UpdateHighScoreList();
                _gameOverMenu.UpdateScoreText();
            }


            if (newState == GameState.Running)
            {
                HUDContainer.SetActive(true);
            }
            else if (prevState == GameState.Running)
            {
                HUDContainer.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newScore"></param>
        protected void OnScoreChanged(int newScore)
        {
            _scoreText.text = $"Score: {newScore}";
        }

        public virtual void UpdateHealth(int value)
        {
            _healthText.text = $"Health: {value}";
        }


        #endregion




        

    }
}
