using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public static partial class Constant
{
    public static class HPBarView
    {
        public const float MaxBarDuration = 1.0f;
    }
}

public class HPBarView : MonoBehaviour,
    IView
{
    [SerializeField] private Image _remainingBar;

    public void UpdateView(int hp)
    {
        var remainingBarFillAmount = (float) hp / Constant.CharacterModel.MaxHP;
        ObservableTween.Tween(_remainingBar.fillAmount,
                remainingBarFillAmount,
                Mathf.Abs(remainingBarFillAmount - _remainingBar.fillAmount) * Constant.HPBarView.MaxBarDuration,
                ObservableTween.EaseType.Linear)
            .Subscribe(f => _remainingBar.fillAmount = f);
    }


    public void UpdatePos(Vector3 worldPos)
    {
        transform.position = worldPos;
    }

    public class Pool : MonoMemoryPool<HPBarView>
    {
    }
}