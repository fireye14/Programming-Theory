
using System;
using Assets._Scripts.GameManagement.BaseTypes;
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
        [SerializeField] private PauseMenu _pauseMenu;
        [SerializeField] private InputField _nameText;
        [SerializeField] private Text _errorText;
        [SerializeField] private SceneTransitionFader _sceneTransitionFader;

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

        #endregion


        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        protected override void AwakeSystemManager()
        {
            if (_sceneTransitionFader == null)
                _sceneTransitionFader = GetComponentInChildren<SceneTransitionFader>();

            if (_sceneTransitionFader != null)
                _sceneTransitionFader.gameObject.SetActive(false);

            if (_pauseMenu == null)
                _pauseMenu = GetComponentInChildren<PauseMenu>();

            if(_pauseMenu != null)
                _pauseMenu.gameObject.SetActive(false);


            GM.LoadOperationCompleted += GM_LoadOperationCompleted;
            GM.SceneTransitionFadeBegin += GM_SceneTransitionFadeBegin;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDestroySystemManager()
        {
            if (GM != null)
            {
                GM.LoadOperationCompleted -= GM_LoadOperationCompleted;
                GM.SceneTransitionFadeBegin -= GM_SceneTransitionFadeBegin;
            }
        }

        #endregion

        #region Methods

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
                    HandleStartGame();
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
        protected virtual void HandleStartGame()
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
            _pauseMenu.gameObject.SetActive(false);
            GM.RestartGame();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void ReturnToTitleScreen()
        {
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
            _sceneTransitionFader.FadeIn(ao);
        }


        #endregion

    }
}
