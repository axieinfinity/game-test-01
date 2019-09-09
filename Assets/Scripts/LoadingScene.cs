using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private Image _pregressBar;
    [SerializeField] private Text _pregressBarText;
    [SerializeField] private Text _changeSceneText;

    public string[] ResourceTypes; 
    public int[] ResourceIds;

    private string _savePath = "{0}/axie.{1}";
    private string _url = "https://storage.googleapis.com/assets.axieinfinity.com/axies/{0}/axie/axie.{1}";

    private int _totalResource = 0;
    private int _countResourceLoaded = 0;

    void Start()
    {
        _totalResource = ResourceIds.Length * ResourceTypes.Length;

        for (int i = 0; i < ResourceIds.Length; i++)
        {
            LoadResourceId(ResourceIds[i]);
        }

        StartCoroutine(LoadAsynScene(1));
    }

    private IEnumerator LoadAsynScene(int sceneIndex)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);

        op.allowSceneActivation = false;

        while (!op.isDone && _countResourceLoaded <= _totalResource)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f) * ((float)_countResourceLoaded/_totalResource);

            _pregressBar.fillAmount = progress;
            _pregressBarText.text = progress * 100 + "%";

            if (op.progress >= 0.9f && _countResourceLoaded == _totalResource)
            {
                _changeSceneText.gameObject.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Space))
                    op.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private void LoadResourceId(int id)
    {
        foreach (var type in ResourceTypes)
        {
            var path = Path.Combine(Application.persistentDataPath, string.Format(_savePath, id, type));
            var url = string.Format(_url, id, type);

            Debug.Log("path::" + path);
            Debug.Log("url::" + url);

            if (File.Exists(path))
            {
                // already downloaded
                _countResourceLoaded++;
            }
            else
            {
                StartCoroutine(Download(url, path));
            }
        }
    }

    IEnumerator Download(string url, string fileName)
    {
        var uwr = new UnityWebRequest(url);
        uwr.method = UnityWebRequest.kHttpVerbGET;
        var resultFile = Path.Combine(Application.persistentDataPath, fileName);
        var dh = new DownloadHandlerFile(resultFile);
        dh.removeFileOnAbort = true;
        uwr.downloadHandler = dh;
        yield return uwr.Send();
        if (uwr.isNetworkError || uwr.isHttpError)
            Debug.Log(uwr.error);
        else
        {
            _countResourceLoaded++;
            Debug.Log("Download saved to: " + resultFile);
        }
    }
}