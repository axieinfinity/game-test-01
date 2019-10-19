using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultView : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation attacker, defensor;
    [SerializeField] private Button btnBack, btnReplay;

    System.Action onReplayAction, onBackAction;

    public void Show(EnCharacterType winner, System.Action onReplayAction = null, System.Action onBackAction = null)
    {
        this.onReplayAction = onReplayAction;
        this.onBackAction = onBackAction;
        gameObject.SetActive(true);
        if(winner == EnCharacterType.Attacker)
        {
            attacker.gameObject.SetActive(true);
            defensor.gameObject.SetActive(false);
            attacker.AnimationState.SetAnimation(0, AnimationAction.Victory, false);
            attacker.AnimationState.AddAnimation(0, AnimationAction.Idle, true, 0);
        } else
        {
            attacker.gameObject.SetActive(false);
            defensor.gameObject.SetActive(true);
            defensor.AnimationState.SetAnimation(0, AnimationAction.Victory, false);
            defensor.AnimationState.AddAnimation(0, AnimationAction.Idle, true, 0);
        }
    }
    private void OnEnable()
    {
        btnReplay.onClick.AddListener(OnClickReplay);
        btnBack.onClick.AddListener(OnClickBack);
    }
    private void OnDisable()
    {
        btnReplay.onClick.RemoveListener(OnClickReplay);
        btnBack.onClick.RemoveListener(OnClickBack);
    }

    void OnClickReplay()
    {
        if (onReplayAction != null)
            onReplayAction.Invoke();
        Close();
    }

    void OnClickBack()
    {
        if (onBackAction != null)
            onBackAction.Invoke();
        Close();
    }
    void Close()
    {
        gameObject.SetActive(false);
    }
}
