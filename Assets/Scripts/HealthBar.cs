using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private const int BIG_DAMAGE = 15;
    private const int NORMAL_FONT_SIZE = 60;
    private const int BIG_FONT_SIZE = 80;

    [SerializeField] private float posYOffset = 4;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Transform targetObj;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private Vector2 moveStableOffset = new Vector2(0.5f, 0.5f);

    [SerializeField] private Image _mainImg;
    [SerializeField] private Image _subImg;
    [SerializeField] private Text _damageText;

    private RectTransform healthBarRect;
    private Camera camera;


    void Start()
    {
        camera = Camera.main;
        healthBarRect = healthBarImage.GetComponent<RectTransform>();
        Init();
        _damageText.text = string.Empty;
    }

    private void Init()
    {
        _mainImg.fillAmount = 1;
        _subImg.fillAmount = 1;
    }

    void Update()
    {
        Reposition();

        DoAnim();
    }

    private void Reposition()
    {
        var realPos = new Vector2(targetObj.position.x, targetObj.position.y + posYOffset);
        var viewportPos = camera.WorldToViewportPoint(realPos);
        var sizeCanvasX = canvas.sizeDelta.x;
        var sizeCanvasY = canvas.sizeDelta.y;

        var screenPos = new Vector2((viewportPos.x * sizeCanvasX - sizeCanvasX * 0.5f), (viewportPos.y * sizeCanvasY - sizeCanvasY * 0.5f));

        var moveOffset = screenPos - healthBarRect.anchoredPosition;
        var realMoveOffset = new Vector2(Mathf.Abs(moveOffset.x), Mathf.Abs(moveOffset.y));

        if (realMoveOffset.x > moveStableOffset.x)
            healthBarRect.anchoredPosition = Vector3.Lerp(healthBarRect.anchoredPosition, new Vector2(screenPos.x, healthBarRect.anchoredPosition.y), Time.deltaTime * 100);

        if (realMoveOffset.y > moveStableOffset.y)
            healthBarRect.anchoredPosition = Vector3.Lerp(healthBarRect.anchoredPosition, new Vector2(healthBarRect.anchoredPosition.x, screenPos.y), Time.deltaTime * 50);
    }

    private float _animTime = 0.5f;
    private float _delayTime = 0.1f;

    private float _hp;
    private float _current = 1f;
    private float _target;
    private float _amount;

    public void Init(int hp)
    {
        _hp = hp;
        _mainImg.fillAmount = 1;
    }

    public void Reset()
    {
        _current = 1;
        _mainImg.fillAmount = 1;
    }

    public void GetHit(float hpAmount)
    {
        SetHealthBarAnim(hpAmount);
        _doAnim = true;

        _damageText.text = "-" +hpAmount.ToString();

       var isBigDamage = hpAmount >= BIG_DAMAGE;
        _damageText.fontSize = isBigDamage ? BIG_FONT_SIZE : NORMAL_FONT_SIZE;
        _damageText.fontStyle = isBigDamage ? FontStyle.Bold:FontStyle.Normal;
    }

    private void SetHealthBarAnim(float amount)
    {
        _amount = amount;
        _target = _current - amount / _hp;

        if (_target < 0)
            _target = 0;
    }

    float _countAnimTime = 0;
    bool _doAnim;
    private void DoAnim()
    {
        if (_doAnim)
        {
            _countAnimTime += Time.deltaTime;
            var time = _countAnimTime / _animTime;

            float lerpValue = Mathf.Lerp(_current, _target, time);

            _mainImg.fillAmount = lerpValue;
            //_subImg.fillAmount = _mainImg.fillAmount + 0.05f;

            if (_countAnimTime >= _animTime)
            {
                _countAnimTime = 0;
                _doAnim = false;
                _current = _target;
                //_subImg.fillAmount = _mainImg.fillAmount;
                _damageText.text = string.Empty;
            }
        }
    }
}
