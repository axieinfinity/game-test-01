using System;
using UnityEngine;

public class ZoomButton : MonoBehaviour
{
    public enum BType {
        In,
        Out
    }
    [SerializeField] BType _type = BType.In;
    public BType Type {
        get {
            return _type;
        }
    }
    public static Action<ZoomButton, bool> onPress = delegate {};

    public void OnPress(bool isPressed) {
        onPress(this, isPressed);
    }
}
