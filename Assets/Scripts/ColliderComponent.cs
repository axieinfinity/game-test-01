using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ColliderComponent : MonoBehaviour
{
    public Action<GameObject, Collider2D> OnTrigger;

    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log($"TriggerEnter2D {gameObject.name}__{collision.gameObject.name}");
        OnTrigger?.Invoke(gameObject, collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        //Debug.LogError($"StayyyyyEnter2D {gameObject.name}__{collision.gameObject.name}");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log($"Exit2D {gameObject.name}__{collision.gameObject.name}");
    }

    private void OnEnable()
    {
        _collider.enabled = true;
    }

    private void OnDisable()
    {
        _collider.enabled = false;
    }
}
