using UniRx;

public class CharacterModel : IModel
{
    public string Id;
    public ReactiveProperty<int> HP;
    public int Damage;
    public bool IsPlayer;
    public string SpineDataId;

    public readonly IReadOnlyReactiveProperty<bool> IsDead;

    public CharacterModel()
    {
        HP = new ReactiveProperty<int>(Constant.CharacterModel.MaxHP);
        IsDead = HP.Select(hp => hp <= 0)
            .ToReactiveProperty();
    }
}


public static partial class Constant
{
    public static class CharacterModel
    {
        public const float ScaleXPlayer = 1.0f;
        public const float RotationYPlayer = 180;
        public const int MaxHP = 100;
        public const int MinDam = 50;
        public const int MaxDam = 100;
    }
}