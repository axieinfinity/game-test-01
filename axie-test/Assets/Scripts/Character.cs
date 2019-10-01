using UnityEngine;
using Spine.Unity;
using Random = UnityEngine.Random;
using DG.Tweening;

public class Character : MonoBehaviour, IHitable
{
    public enum CHARACTER_STATE
    {
        ACTIVE, INACTIVE,
    }


    public CHARACTER_STATE state;
    public static bool canClick = true;
    public CharacterModel model;
    public Vector2 gridPosition;

    [SerializeField] CharacterHPBar hpBar;
    [SerializeField, SpineAnimation] string idle, move, attack, onclick, behit;
    [SerializeField] SkeletonAnimation skeletonAnimation;
    float currentHP;
    float touchTime;
    float delay = 0.2f;


    private void Start()
    {
        currentHP = model.startHP;
    }

    private void OnMouseDown()
    {
        touchTime = Time.time;
    }

    private void OnMouseDrag()
    {

    }

    void OnMouseUp()
    {
        if (Time.time - touchTime < delay)
        {
            if (canClick && GameController.instance.state == GameController.GAME_STATE.PLAY && state == CHARACTER_STATE.ACTIVE)
            {
                canClick = false;
                BehaveOnUserInput();
            }
        }
    }

    public virtual void BehaveOnUserInput()
    {
        this.Log("virtual fucntion");
    }

    public virtual void Attack(Character enemy)
    {
        SetAnimAttack();

        this.SetCallback(0.5f, () =>
        {
            Camera.main.DOShakePosition(0.25f, 0.5f, 10);
            var randomPoint = Random.Range(0, 2);
            enemy.GetComponent<IHitable>().GetHit(randomPoint);
        });
    }

    public void SetAnimIdle()
    {
        if (true)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, idle, true);
        }
    }

    public void SetAnimAttack()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, attack, false).Complete += delegate
        {
            SetAnimIdle();
            canClick = true;
        };
    }

    public void SetAnimMove()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, move, true);
    }

    public void SetAnimOnClick()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, onclick, false).Complete += delegate
        {
            SetAnimIdle();
        };
    }

    public virtual void GetHit(int attackerRandomPoint)
    {
        if (state == CHARACTER_STATE.ACTIVE)
        {
            var myRandomPoint = Random.Range(0, 2);
            var tmp = (3 + attackerRandomPoint - myRandomPoint) % 3;
            var dmg = 0f;
            switch (tmp)
            {
                case 0:
                    dmg = 4;
                    break;
                case 1:
                    dmg = 5;
                    break;
                case 2:
                    dmg = 3;
                    break;
            }
            SetAnimBeHit();
            UpdateHP(dmg);
        }
    }

    private void SetAnimBeHit()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, behit, false).Complete += delegate
        {
            SetAnimIdle();
        };
    }

    void UpdateHP(float dmg)
    {
        currentHP -= dmg;
        hpBar.PlayAnimSmoothly(currentHP / model.startHP).onComplete += () =>
        {
            if (currentHP <= 0)
            {
                Die();
            }
        };
    }

    private void Die()
    {
        state = CHARACTER_STATE.INACTIVE;
        GridController.instance.RemoveCharacterFromCell(gridPosition);
        GameController.instance.RemoveCharacter(this, model.type);
        PowerBar.instance.UpdateValue(model.type, model.startHP);
        gameObject.SetActive(false);
    }
}
