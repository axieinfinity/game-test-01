using UnityEngine;
using System.Collections;
using System;

namespace Gameplay.Display {
    public class UnitDisplay : Instantiable<Unit>
    {
        public Spine.Unity.SkeletonDataAsset skeletonDataAttacker, skeletonDataDefender;

        [Space(32)]
        public Spine.Unity.SkeletonAnimation anim;

        [Space(32)]
        public GameObject markerAttacker;
        public GameObject markerDefender;

        [Space(32)]
        public GameObject hpBar;

        public override void OnCreate(Unit data)
        {
            anim.skeletonDataAsset = data.attacker ? skeletonDataAttacker : skeletonDataDefender;
            anim.Initialize(true);

            markerAttacker.SetActive(data.attacker == true);
            markerDefender.SetActive(data.attacker == false);

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
            if (data.maxHp == 0) return;
            Vector3 size = Vector3.one;
            size.x = data.hp * 1f / data.maxHp;
            hpBar.transform.localScale = size;
        }

        public IEnumerator CoMoveToTile(TileDisplay tile)
        {
            yield return new WaitForSeconds(1.6f);
            var pos_from = transform.position;
            var pos_to = tile.transform.position;
            for (var i = 0f; i < 1f; i += Time.deltaTime / 0.4f)
            {
                transform.position = Vector3.Lerp(pos_from, pos_to, i);
                yield return true;
            }
            transform.position = pos_to;
        }
    }
}