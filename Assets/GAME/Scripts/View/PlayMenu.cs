using System;
using MenuSystemWithZenject;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayMenu : Menu<PlayMenu>,
    IPresenter
{
    [SerializeField] private Button _playButton;

    public IObservable<Unit> OnClickBtnPlayAsObservable()
    {
        return _playButton.onClick.AsObservable();
    }
}