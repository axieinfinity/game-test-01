using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHide : MonoBehaviour
{
    public float Delay = 1f;
    private void OnEnable()
    {
        if (Delay <= 0.2f)
            Delay = 0.2f;
        StartCoroutine("Hide");
    }

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(Delay);
        gameObject.SetActive(false);
    }
}
