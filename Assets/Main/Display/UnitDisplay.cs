using UnityEngine;
using System.Collections;
using System;

namespace Gameplay.Display {
    public class UnitDisplay : Instantiable<Unit>
    {

        public UnitPreset presetAttacker, presetDefender;

        [Space(32)]
        [SerializeField] private Spine.Unity.SkeletonAnimation anim;

        [Space(32)]
        [SerializeField] private GameObject markerAttacker;
        [SerializeField] private GameObject markerDefender;

        [Space(32)]
        [SerializeField] private HpBarDisplay hpBar;

        private bool facingRight { set
            {
                anim.transform.localScale = value ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
            } }

        public override void OnCreate(Unit data)
        {
            anim.skeletonDataAsset = data.attacker ? presetAttacker.skeleton : presetDefender.skeleton;
            anim.Initialize(true);

            markerAttacker.SetActive(data.attacker == true);
            markerDefender.SetActive(data.attacker == false);

            hpBar.SetInitHp(data.maxHp);
            UpdateHPBar();
        }

        private void LateUpdate()
        {
            var pos = transform.localPosition;
            pos.z = pos.y / 100;
            transform.localPosition = pos;
        }

        public void UpdateHPBar()
        {
            hpBar.UpdateHp(data.hp);
        }

        public IEnumerator CoPlayAttack(UnitDisplay target, DisplayAction attackDisplayAction, EventUnitAttackUnit e)
        {
            if (e.counterAttack) yield return new WaitForSeconds(1.3f);

            facingRight = target.transform.position.x > transform.position.x;

            anim.AnimationName = attackDisplayAction.actionSource;
            yield return new WaitForSeconds(attackDisplayAction.delaySourceToTarget);
            target.anim.AnimationName = attackDisplayAction.actionTarget;

            target.hpBar.UpdateHp(e.remainingHp);

            yield return new WaitForSeconds(1.2f - attackDisplayAction.delaySourceToTarget);
            anim.AnimationName = target.anim.AnimationName = "action/idle";
        }

        public IEnumerator CoMoveToTile(TileDisplay tile)
        {
            yield return new WaitForSeconds(1.5f + UnityEngine.Random.value * 0.1f);

            var pos_from = transform.position;
            var pos_to = tile.transform.position;
            facingRight = pos_to.x > pos_from.x;

            for (var i = 0f; i < 1f; i += Time.deltaTime / 0.5f)
            {
                transform.position = Vector3.Lerp(pos_from, pos_to, i);
                yield return true;
            }
            transform.position = pos_to;
        }

        public IEnumerator CoPlayDead()
        {
            yield return new WaitForSeconds(3);
            anim.AnimationName = "activity/sleep";

            for (var i = 0f; i < 1f; i += Time.deltaTime / 0.6f)
            {
                transform.localScale = Vector3.one * (1 - Mathf.Pow(i, 2));
                yield return true;
            }
            gameObject.SetActive(false);
        }

        public void PlayVictory()
        {
            anim.loop = true;
            anim.AnimationName = "action/victory-pose-back-flip";
        }
    }
}