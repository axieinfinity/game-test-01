using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIController : CustomSingleton<UIController>
{
    [SerializeField] GameObject btnPause, btnPlay;
    [SerializeField] Transform endGamePanel;
    [SerializeField] Text endGameText;

    public void OnPauseBtnClick()
    {
        GameController.instance.PauseGame();

        btnPause.SetActive(!btnPause.activeInHierarchy);
        btnPlay.SetActive(!btnPlay.activeInHierarchy);
    }
    public void OnResumeBtnClick()
    {
        GameController.instance.ResumeGame();

        btnPause.SetActive(!btnPause.activeInHierarchy);
        btnPlay.SetActive(!btnPlay.activeInHierarchy);
    }
    public void OnForwardBtnClick()
    {
        GameController.instance.SpeedUpGame();
    }

    public void ShowEndPanel(CharacterModel.CHARACTER_TYPE loser)
    {
        if (loser == CharacterModel.CHARACTER_TYPE.ATTACK)
        {
            endGameText.text = "attacker lose!!!";
        }
        else
        {
            endGameText.text = "defender lose!!!";
        }
        endGamePanel.DOMoveY(0, 1f).OnComplete(() =>
        {
            this.SetCallback(1, () =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Gameplay");
            });
        });
    }
}
