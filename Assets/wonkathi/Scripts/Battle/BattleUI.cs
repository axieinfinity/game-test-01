using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

/// <summary>
/// Handle UI in game.
/// </summary>
public class BattleUI : MonoBehaviour
{
    [SerializeField] private Text txtFPS, txtRingCount,txtCharacterCount;
    [SerializeField] private Slider powerBarDefensor, powerBarAttacker;
    [SerializeField] private Text txtPowerDefensor, txtPowerAttacker;
    [SerializeField] private Button btnPause, btnSpeed, btnMenu,btnZoom;
    [SerializeField] private BattleController battleController;
    [SerializeField] private ResultView result;
    [SerializeField] private GameObject goLargestMap;

    public System.Action OnPauseAction, OnChangeSpeedAction, OnReplayAction,OnBackToMenuAction;
    public System.Action<bool> OnZoomAction;
    float powerBarAttackerVel, powerBarDefensorVel;
    bool isZoomIn = true;
    
    /// <summary>
    /// Reload the UI to default
    /// </summary>
    public void Reload()
    {
        var spriteName = battleController.IsPausedGame ? SpriteName.BtnPlay : SpriteName.BtnPause;
        var sprite = SpriteManager.Inst.GetSprite(spriteName);
        btnPause.GetComponent<Image>().sprite = sprite;

        spriteName = SpriteName.BtnSpeed1;
        if (battleController.GameSpeed == 2)
            spriteName = SpriteName.BtnSpeed2;
        else if (battleController.GameSpeed == 4)
            spriteName = SpriteName.BtnSpeed4;
        sprite = SpriteManager.Inst.GetSprite(spriteName);
        btnSpeed.GetComponent<Image>().sprite = sprite;
    }
    private void OnEnable()
    {
        result.gameObject.SetActive(false);
        goLargestMap.gameObject.SetActive(false);
        btnPause.onClick.AddListener(OnClickPause);
        btnSpeed.onClick.AddListener(OnClickSpeed);
        btnMenu.onClick.AddListener(OnBackToMenu);
        btnZoom.onClick.AddListener(OnClickZoom);
    }
    private void OnDisable()
    {
        btnPause.onClick.RemoveListener(OnClickPause);
        btnSpeed.onClick.RemoveListener(OnClickSpeed);
        btnMenu.onClick.RemoveListener(OnBackToMenu);
        btnZoom.onClick.RemoveListener(OnClickZoom);
    }

    public void ShowResult(EnCharacterType winner)
    {
        result.Show(winner, OnReplayAction, OnBackToMenuAction);
    }
    private void Update()
    {
        if (!battleController.IsReady)
            return;
        txtFPS.text = "FPS: " + (int)battleController.FPS;
        txtRingCount.text = "Ring quantity: " + battleController.RingCount;
        txtCharacterCount.text = "Character quantity: " + battleController.CharacterCount;
        btnSpeed.gameObject.SetActive(!battleController.IsShowingLargestMap);
        btnPause.gameObject.SetActive(!battleController.IsShowingLargestMap);
        goLargestMap.gameObject.SetActive(battleController.IsShowingLargestMap && !battleController.IsFinishedGenLargestMap);

        txtPowerAttacker.text = "HP: " + battleController.AttackterTotalHP + "/" + battleController.AttackterTotalMaxHP;
        txtPowerDefensor.text = "HP: " + battleController.DefensorTotalHP + "/" + battleController.DefensorTotalMaxHP;

        float attackerFill = battleController.AttackterTotalHP / (float)battleController.AttackterTotalMaxHP;
        powerBarAttacker.value = Mathf.SmoothDamp(powerBarAttacker.value, attackerFill, ref powerBarAttackerVel, 0.2f);
        float defensorFill = battleController.DefensorTotalHP / (float)battleController.DefensorTotalMaxHP;
        powerBarDefensor.value = Mathf.SmoothDamp(powerBarDefensor.value, defensorFill, ref powerBarDefensorVel, 0.2f);
    }

    void OnClickPause()
    {
        if (OnPauseAction != null)
            OnPauseAction.Invoke();
        var spriteName = battleController.IsPausedGame ? SpriteName.BtnPlay : SpriteName.BtnPause;
        var sprite = SpriteManager.Inst.GetSprite(spriteName);
        btnPause.GetComponent<Image>().sprite = sprite;
    }
    void OnClickSpeed()
    {
        if (OnChangeSpeedAction != null)
            OnChangeSpeedAction.Invoke();
        var spriteName = SpriteName.BtnSpeed1;
        if (battleController.GameSpeed == 2)
            spriteName = SpriteName.BtnSpeed2;
        else if (battleController.GameSpeed == 4)
            spriteName = SpriteName.BtnSpeed4;
        var sprite = SpriteManager.Inst.GetSprite(spriteName);
        btnSpeed.GetComponent<Image>().sprite = sprite;
    }
    void OnClickMenu()
    {
        if (OnBackToMenuAction != null)
            OnBackToMenuAction.Invoke();
    }
    void OnClickZoom()
    {
        isZoomIn = !isZoomIn;
        if (OnZoomAction != null)
            OnZoomAction.Invoke(isZoomIn);

        var spriteName = isZoomIn ? SpriteName.BtnZoomOut : SpriteName.BtnZoomIn;
        var sprite = SpriteManager.Inst.GetSprite(spriteName);
        btnZoom.GetComponent<Image>().sprite = sprite;
    }

    void OnReplay()
    {
        if (OnReplayAction != null)
            OnReplayAction.Invoke();
    }
    void OnBackToMenu()
    {
        if (OnBackToMenuAction != null)
            OnBackToMenuAction.Invoke();
    }
}
