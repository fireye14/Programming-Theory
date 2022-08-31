using System;
using System.Collections;
using System.Collections.Generic;
using Assets._Scripts.GameManagement;
using Assets._Scripts.GameManagement.BaseTypes;
using UnityEngine;
using UnityEngine.UI;
using static Assets._Scripts.Helpers.EventArgs;

public class PauseMenu : MonoBehaviour
{
    #region Fields

    [SerializeField] private Button _btnResume;
    [SerializeField] private Button _btnRestart;
    [SerializeField] private Button _btnMenu;

    #endregion

    #region Properties and Backing Fields

    protected GameManager GM => GameManager.Instance;

    protected UIManager UI => UIManager.Instance;

    #endregion

    #region Events

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

    #endregion

    #region Methods

    /// <summary>
    /// 
    /// </summary>
    protected void Awake()
    {
        if (GM != null)
        {
            GM.GameStateChanged += GM_GameStateChanged;
        }


        if(_btnResume != null)
            _btnResume.onClick.AddListener(OnResumeClick);

        if (_btnRestart != null)
            _btnRestart.onClick.AddListener(OnRestartClick);

        if (_btnMenu != null)
            _btnMenu.onClick.AddListener(OnMenuClick);

    }

    /// <summary>
    /// 
    /// </summary>
    protected void OnDestroy()
    {
        if (GM != null)
        {
            GM.GameStateChanged -= GM_GameStateChanged;
        }
    }

    protected void OnGameStateChanged(IGameState prevState, IGameState newState)
    {
        if (newState == GameState.Paused)
        {
            Time.timeScale = 0;
        }
        else if (prevState == GameState.Paused)
        {
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnResumeClick()
    {
        Time.timeScale = 1;
        UI.ResumeGame();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnRestartClick()
    {
        Time.timeScale = 1;
        UI.RestartGame();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnMenuClick()
    {
        Time.timeScale = 1;
        UI.ReturnToTitleScreen();
        gameObject.SetActive(false);
    }

    #endregion
}
