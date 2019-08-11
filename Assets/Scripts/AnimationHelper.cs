using UnityEngine;

public class AnimationHelper : MonoBehaviour
{
    public static AnimationHelper Instance;

    [SerializeField]
    private AnimationDetail[] _animDetails;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    public AnimationDetail GetAnimDetail(int id)
    {
        foreach (var item in _animDetails)
        {
            if (id == item.ID)
                return item;
        }
        return null;
    }

    public string GetAnimDetailName(int id)
    {
        foreach (var item in _animDetails)
        {
            if (id == item.ID)
                return item.Anim;
        }
        return string.Empty;
    }

    public AnimationDetail GetIdleAnimDetail()
    {
        return GetAnimDetail(0);
    }

}
