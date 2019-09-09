using System;
using MenuSystemWithZenject;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public static partial class Constant
{
    public static class GameOverMenu
    {
        public const string Win = "YOU WIN";
        public const string Lose = "YOU LOSE";
    }
}

public class GameOverMenu : Menu<GameOverMenu>,
    IPresenter
{
    [SerializeField] private TextMeshProUGUI _txtWin;
    [SerializeField] private Button _btnPlay;

    public IObservable<Unit> OnClickBtnPlayAsObservable()
    {
        return _btnPlay.onClick.AsObservable();
    }

    public void Open(bool isWin)
    {
        Open();
        _txtWin.text = isWin
            ? Constant.GameOverMenu.Win
            : Constant.GameOverMenu.Lose;
    }
}