using Spine.Unity;
using UnityEngine;

public class ResourceCreator
{
    public SkeletonDataAsset CreateSkeletonDataAsset(SpineData spineData)
    {
        var skeletonDataAsset = ScriptableObject.CreateInstance<SkeletonDataAsset>();
        skeletonDataAsset.Clear();

        var skeletonDataFile = new TextAsset(spineData.TxtModel);
        skeletonDataAsset.skeletonJSON = skeletonDataFile;

        var spineAtlasAsset = ScriptableObject.CreateInstance<SpineAtlasAsset>();
        spineAtlasAsset.atlasFile = new TextAsset(spineData.TxtAtlas);

        var mat = new Material(Shader.Find("Spine/Skeleton"))
        {
            mainTexture = spineData.CharTexture
        };
        mat.mainTexture.name = Constant.SpineData.ModelConst;
        Debug.Log("Texture name: " + spineData.CharTexture.name);
        mat.SetTexture("_MainText",
            spineData.CharTexture);
        spineAtlasAsset.materials = new[] {mat};

        skeletonDataAsset.atlasAssets = new[] {spineAtlasAsset};
        skeletonDataAsset.scale = Constant.SpineData.Scale;

//      if (initialize)
        skeletonDataAsset.GetSkeletonData(true);

        return skeletonDataAsset;
    }
}