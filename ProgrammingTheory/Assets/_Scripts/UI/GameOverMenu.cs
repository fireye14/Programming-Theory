using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets._Scripts.GameManagement;
using Assets._Scripts.GameManagement.BaseTypes;
using UnityEngine;
using UnityEngine.UI;
using static Assets._Scripts.Helpers.EventArgs;

public class GameOverMenu : MonoBehaviour
{
    #region Fields

    [SerializeField] private Button _btnRestart;
    [SerializeField] private Button _btnMenu;
    [SerializeField] private Text _txtScore;
    [SerializeField] private Text _txtHighScoreNames;
    [SerializeField] private Text _txtHighScorePoints;

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prevState"></param>
    /// <param name="newState"></param>
    protected void OnGameStateChanged(IGameState prevState, IGameState newState)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnRestartClick()
    {
        UI.RestartGame();
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnMenuClick()
    {
        UI.ReturnToTitleScreen();
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateHighScoreList()
    {
        _txtHighScoreNames.text = string.Join("\n", DataManager.HighScores.Select(x => x.Name));
        _txtHighScorePoints.text = string.Join("\n", DataManager.HighScores.Select(x => x.Points));
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateScoreText()
    {
        _txtScore.text = $"SCORE: {GM.CurrentScore}";
    }

    #endregion
}
