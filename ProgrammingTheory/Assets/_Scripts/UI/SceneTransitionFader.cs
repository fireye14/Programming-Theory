using System;
using System.Collections;
using System.Collections.Generic;
using Assets._Scripts.GameManagement;
using Assets._Scripts.Helpers;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionFader : MonoBehaviour
{
    #region Fields

    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeSpeed;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private Text _loadingText;

    private float _target;

    protected AsyncOperation LoadOperation;
    protected bool fadeIn;
    protected bool fadeOut;

    #endregion

    #region Properties and Backing Fields

    protected UIManager SM => UIManager.Instance;

    #endregion

    protected void Awake()
    {
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.alpha = 0;

        if (_fadeSpeed == 0f)
            _fadeSpeed = 1f;

        if (_progressBar == null)
            _progressBar = GetComponentInChildren<Slider>();

        gameObject.SetActive(false);
    }

    protected void Update()
    {
        if (fadeIn)
        {
            _canvasGroup.alpha += _fadeSpeed * Time.deltaTime;
            if (_canvasGroup.alpha >= 1f)
            {
                fadeIn = false;
                LoadOperation.allowSceneActivation = true;
            }
        }
        else if (fadeOut && _progressBar.value >= 1f)
        {
            _canvasGroup.alpha -= _fadeSpeed * Time.deltaTime;
            if (_canvasGroup.alpha <= 0f)
            {
                fadeOut = false;
                LoadOperation = null;
                gameObject.SetActive(false);
            }
        }

        if (LoadOperation != null)
        {
            _progressBar.value = Mathf.MoveTowards(_progressBar.value, LoadOperation.progress, Time.deltaTime);
            _loadingText.text = $"Loading: {Math.Ceiling(_progressBar.value * 100)}%";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void FadeIn(AsyncOperation ao)
    {
        LoadOperation = ao;
        LoadOperation.allowSceneActivation = false;
        fadeIn = true;
        _progressBar.value = 0;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void FadeOut()
    {
        fadeOut = true;
    }
}
