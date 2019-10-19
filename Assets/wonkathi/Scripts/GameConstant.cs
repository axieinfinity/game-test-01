public static class AnimationAction
{
    public const string Idle = "action/idle";
    public const string MoveBack = "action/move-back";
    public const string MoveForward = "action/move-forward";
    public const string Prepair = "action/prepair";
    public const string Stun = "status/stun";
    public const string GotHit = "defense/hit-by-normal-attack";
    public const string Attack1 = "attack/melee/tail-multi-slap";
    public const string Attack2 = "attack/melee/toothless-bite";
    public const string Victory = "action/victory-pose-back-flip";
    public const string Sleep = "activity/sleep";
}

public static class SpriteName
{
    public const string BtnPlay = "btn_play";
    public const string BtnPause = "btn_pause";
    public const string BtnSpeed1 = "btn_x1";
    public const string BtnSpeed2 = "btn_x2";
    public const string BtnSpeed4 = "btn_x4";
    public const string BtnZoomIn = "btn_zoomin";
    public const string BtnZoomOut = "btn_zoomout";
}

public static class EffectName
{
    public const string AttackerDie = "GhostAttacker";
    public const string DefensorDie = "GhostDefensor";
}

public static class LayerName
{
    public const string Battle = "Battle";
}
