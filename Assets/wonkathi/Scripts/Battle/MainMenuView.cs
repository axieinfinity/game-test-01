using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private Button btnDemo, btnLargestMap;
    public System.Action OnDemoAction, OnShowLargestMapAction;

    private void OnEnable()
    {
        btnDemo.onClick.AddListener(OnClickDemo);
        btnLargestMap.onClick.AddListener(OnClickLargestMap);
    }

    private void OnDisable()
    {
        btnDemo.onClick.RemoveListener(OnClickDemo);
        btnLargestMap.onClick.RemoveListener(OnClickLargestMap);
    }

    void OnClickDemo()
    {
        if (OnDemoAction != null)
            OnDemoAction.Invoke();
        OnClose();
    }
    void OnClickLargestMap()
    {
        if (OnShowLargestMapAction != null)
            OnShowLargestMapAction.Invoke();
        OnClose();
    }
    void OnClose()
    {
        gameObject.SetActive(false);
    }
}
