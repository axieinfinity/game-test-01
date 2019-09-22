using UnityEngine;
using System.Collections;

namespace Gameplay.Display {
    public class HpBarDisplay : MonoBehaviour
    {
        [SerializeField] private Transform hpBar, hpBarBack;
        private int hp, maxHp;

        Vector3 smoothDampHpBarBackScale;
        Vector3 smoothDampHpBarScale, targetHpBarScale;
        Vector3 smoothDampLocalScale;

        public void SetInitHp(int hp)
		{
            this.hp = this.maxHp = hp;
            hpBar.localScale = targetHpBarScale = Vector3.one;
		}

        public void UpdateHp(int hp)
        {
            this.hp = hp;
            targetHpBarScale = new Vector3(hp * 1f / maxHp, 1, 1);
            transform.localScale = new Vector3(1.2f, 1.6f, 1);
        }

        private void Update()
        {
            hpBar.localScale = Vector3.SmoothDamp(hpBar.localScale, targetHpBarScale, ref smoothDampHpBarScale, 0.3f);
            hpBarBack.localScale = Vector3.SmoothDamp(hpBarBack.localScale, targetHpBarScale, ref smoothDampHpBarBackScale, 0.5f);
            transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.one, ref smoothDampLocalScale, 0.15f);
        }
    }
}