using Spine.Unity;
using UnityEngine;

public static partial class Constant
{
    public static class SpineData
    {
        public const float Scale = 0.0015f;
        public const string ModelConst = "axie";
    }
}

public class SpineData : IModel
{
    public string Id { get; set; }
    public Texture2D CharTexture { get; set; }
    public string TxtModel { get; set; }
    public string TxtAtlas { get; set; }

    public SkeletonDataAsset SkeletonDataAsset { get; set; }
}