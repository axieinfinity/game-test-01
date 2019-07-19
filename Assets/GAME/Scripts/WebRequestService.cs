using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;

public interface IWebRequestService
{
    UniTask<string> GetTextAsync(UnityWebRequest req);
    Task<Texture2D> GetTextureAsync(UnityWebRequest req);
}

public class WebRequestService : IWebRequestService
{
    public async UniTask<string> GetTextAsync(UnityWebRequest req)
    {
        var op = await req.SendWebRequest();
        return op.downloadHandler.text;
    }

    public async Task<Texture2D> GetTextureAsync(UnityWebRequest req)
    {
        await req.SendWebRequest();
        return DownloadHandlerTexture.GetContent(req);
    }
}