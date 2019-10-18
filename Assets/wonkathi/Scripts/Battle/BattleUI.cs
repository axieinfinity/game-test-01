using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private Text txtFPS, txtRingCount,txtCharacterCount;
    [SerializeField] private Slider powerBarDefensor, powerBarAttacker;
    [SerializeField] private Text txtPowerDefensor, txtPowerAttacker;
    [SerializeField] private Button btnPause, btnSpeed, btnMenu;
    [SerializeField] private BattleController battleController;
    [SerializeField] private SpriteAtlas spriteAtlas;
    [SerializeField] private ResultView result;

    public System.Action OnPauseAction, OnChangeSpeedAction, OnReplayAction,OnBackToMenuAction;
    float powerBarAttackerVel, powerBarDefensorVel;
    public void Reload()
    {
        var spriteName = battleController.IsPausedGame ? SpriteName.BtnPlay : SpriteName.BtnPause;
        var sprite = spriteAtlas.GetSprite(spriteName);
        btnPause.GetComponent<Image>().sprite = sprite;

        spriteName = SpriteName.BtnSpeed1;
        if (battleController.GameSpeed == 2)
            spriteName = SpriteName.BtnSpeed2;
        else if (battleController.GameSpeed == 4)
            spriteName = SpriteName.BtnSpeed4;
        sprite = spriteAtlas.GetSprite(spriteName);
        btnSpeed.GetComponent<Image>().sprite = sprite;
    }
    private void OnEnable()
    {
        btnPause.onClick.AddListener(OnClickPause);
        btnSpeed.onClick.AddListener(OnClickSpeed);
        btnMenu.onClick.AddListener(OnBackToMenu);
    }
    private void OnDisable()
    {
        btnPause.onClick.RemoveListener(OnClickPause);
        btnSpeed.onClick.RemoveListener(OnClickSpeed);
        btnMenu.onClick.RemoveListener(OnBackToMenu);
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
        var sprite = spriteAtlas.GetSprite(spriteName);
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
        var sprite = spriteAtlas.GetSprite(spriteName);
        btnSpeed.GetComponent<Image>().sprite = sprite;
    }
    void OnClickMenu()
    {
        if (OnBackToMenuAction != null)
            OnBackToMenuAction.Invoke();
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
