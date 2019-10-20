using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class LobbyDialog : MonoBehaviour
{
    public UIManager uiManager;
    public SkeletonAnimation[] skeletonAnimations;

    private void OnEnable()
    {
        foreach (var skeletonAnimation in skeletonAnimations)
        {
            skeletonAnimation.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        foreach (var skeletonAnimation in skeletonAnimations)
        {
            skeletonAnimation.gameObject.SetActive(false);
        }
    }

    public void OnClickNewGame()
    {
        foreach (var skeletonAnimation in skeletonAnimations)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, AnimationName.attack, false);
            skeletonAnimation.AnimationState.SetAnimation(1, AnimationName.idle, true);
        }

        StartCoroutine(WaitStart());
    }

    IEnumerator WaitStart()
    {
        yield return new WaitForSeconds(1f);
        uiManager.OnClickNewGameFromLobby();
    }

}
