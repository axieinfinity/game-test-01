using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public static partial class Constant
{
    public static class ResourceLoader
    {
        public const string ModelPrefix = "Model";
        public const string AtlasPrefix = "Atlas";
    }
}

public class ResourceLoader
{
    [Inject] private IWebRequestService _webRequestService;
    [Inject] private ICacheService _cacheService;

    public async UniTask<SpineData[]> LoadCharacterData(string[] idList)
    {
        var spineDataArr = new SpineData[idList.Length];
        for (var i = 0;
            i < idList.Length;
            i++)
        {
            var id = idList[i];
            var modelId = $"{Constant.ResourceLoader.ModelPrefix}_{id}";
            var txtModel = _cacheService.GetCacheText(modelId);
            if (string.IsNullOrEmpty(txtModel))
            {
                txtModel = await _webRequestService.GetTextAsync(
                    UnityWebRequest.Get(UrlUtils.GetCharacterJsonUrl(id)));
                _cacheService.SetCacheText(modelId,
                    txtModel);
            }
            else
            {
                Debug.Log($"Load from cache ok {modelId}");
            }

            Debug.Log($"-->[ProgramEntry] received player character: {txtModel}");


            var atlasId = $"{Constant.ResourceLoader.AtlasPrefix}_{id}";
            var txtAtlas = _cacheService.GetCacheText(atlasId);
            if (string.IsNullOrEmpty(txtAtlas))
            {
                txtAtlas = await _webRequestService.GetTextAsync(
                    UnityWebRequest.Get(UrlUtils.GetCharacterAtlasUrl(id)));
                _cacheService.SetCacheText(atlasId,
                    txtAtlas);
            }
            else
            {
                Debug.Log($"Load from cache ok: {atlasId}");
            }

            Debug.Log($"-->[ProgramEntry] received player atlas: {txtAtlas}");


            var characterTextureUrl = UrlUtils.GetCharacterTextureUrl(id);
            Debug.Log($"-->[ProgramEntry] downloading character texture url: {characterTextureUrl}");
            var texture2D =
                await _webRequestService.GetTextureAsync(
                    UnityWebRequestTexture.GetTexture(characterTextureUrl));
            Debug.Log($"-->[ProgramEntry] character texture url: {texture2D.width} - {texture2D.height}");
            var data = new SpineData
            {
                Id = idList[i],
                TxtModel = txtModel,
                TxtAtlas = txtAtlas,
                CharTexture = texture2D,
                SkeletonDataAsset = null
            };
            spineDataArr[i] = data;
        }

        return spineDataArr;
    }
}