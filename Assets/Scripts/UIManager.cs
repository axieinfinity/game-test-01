using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public const float animationTime = 0.5f;

    public CanvasGroup lobbyDialog;
    public CanvasGroup gameDialog;
    public CanvasGroup pauseDialog;
    public MainGame mainGame;
    
    // lobby

    private void Start()
    {
        lobbyDialog.gameObject.SetActive(true);
        mainGame.gameObject.SetActive(false);
        pauseDialog.gameObject.SetActive(false);
        gameDialog.gameObject.SetActive(false);
    }

    public void OnClickNewGameFromLobby()
    {
        Hide(lobbyDialog, () =>
        {
            Show(gameDialog);
            mainGame.gameObject.SetActive(true);
            mainGame.NewGame();    
        });
    }

    // pause dialog

    public void OnClickPause()
    {
        mainGame.Pause();
        Show(pauseDialog);
    }

    public void OnClickResume()
    {
        Hide(pauseDialog);
        mainGame.Resume();
    }

    public void OnClickNewGameFromPause ()
    {
        Hide(pauseDialog);
        mainGame.NewGame();
    }

    public void OnGotoHome()
    {
        StartCoroutine(mainGame.WaitExit());
        Hide(pauseDialog);
        Hide(gameDialog);
        Show(lobbyDialog);
    }
    
    // ultils
    
    void Show (CanvasGroup dialog)
    {
        dialog.alpha = 0;
        dialog.blocksRaycasts = false;
        dialog.gameObject.SetActive(true);
        dialog.DOFade(1f, animationTime).OnComplete(() =>
        {
            dialog.blocksRaycasts = true;
        });
    }

    void Hide (CanvasGroup dialog, Action onComplete = null)
    {
        dialog.blocksRaycasts = false;
        dialog.DOFade(0f, animationTime).OnComplete(() =>
        {
            dialog.alpha = 0;
            dialog.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }
}
