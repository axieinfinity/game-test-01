using UnityEngine;

public class CustomSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance;
    protected bool useDontDestroyOnload;

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = (T)FindObjectOfType(typeof(T));
            if (useDontDestroyOnload)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}