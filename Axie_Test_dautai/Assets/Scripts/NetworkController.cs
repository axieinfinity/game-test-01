using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController : MonoBehaviour {

    public Text processtext;
    public Text titleText;
    public GameObject PanelLoading;
    public GameObject btStartGame;
    string pathURL;
    string pathFolderLocal;
    int numLoadedFile = 0;

    AssetBundle[] bundles;
    void createListLoadedAsset()
    {
        bundles = (AssetBundle[])Resources.FindObjectsOfTypeAll(typeof(AssetBundle));
    }

    GameObject checkExistBundle(string name)
    {
        string pathName = name;
        for (int i = 0; i < bundles.Length; i++)
        {
            if (bundles[i].name.Equals(pathName))
            {
                return bundles[i].LoadAsset<GameObject>(name);
            }
        }
        return null;
    }

    void Start()
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;

        processtext.text = "Load File process: "+ numLoadedFile + "/2";

        pathFolderLocal = Application.dataPath + "/../spineResources/";
        Debug.Log("pathFolderLocal : "+ pathFolderLocal);

        createListLoadedAsset();

        string nameFile3999 = "axie3999";
        string fileLocal3999 = pathFolderLocal + nameFile3999;
        Debug.Log("path Local 3999: " + fileLocal3999);
        pathURL = "https://show-panorama.000webhostapp.com/testGame/axie3999"; //location of the file on the server
        CheckAndLoadFile(fileLocal3999, nameFile3999, pathURL);


        string nameFile5000 = "axie5000";
        string fileLocal5000 = pathFolderLocal + nameFile5000;
        Debug.Log("path Local 5000 : " + fileLocal5000);
        pathURL = "https://show-panorama.000webhostapp.com/testGame/axie5000"; //location of the file on the server
        CheckAndLoadFile(fileLocal5000, nameFile5000, pathURL);
    }

    void CheckAndLoadFile(string fileLocalPath, string nameFile, string pathURL)
    {
        
        GameObject gameObj = checkExistBundle(nameFile);
        if (gameObj != null)
        {
            Debug.Log("Load from cached Asset: " + nameFile);
            titleText.text = "Load from cached Asset ...";
            loadGameObject(gameObj, nameFile);
        }
        else if (!File.Exists(fileLocalPath))
        {
            
            Debug.Log("Load from internet: " + pathURL);
            titleText.text = "Download File From Internet...";
            StartCoroutine(LoadFile(pathURL, fileLocalPath, nameFile));
        }
        else
        {
            Debug.Log("Load from cached File Local: " + fileLocalPath);
            titleText.text = "Load from cached File ...";
            StartCoroutine(LoadFromLocal(fileLocalPath, nameFile));
        }
    }
    //IEnumerator allows yield so the information is not accessed
    //before it finished downloading
    IEnumerator LoadFile(string pathURL, string pathFileLocal, string nameFileLoad)
    {
        
        WWW objSERVER = new WWW(pathURL);

        // Wait for download to finish
        yield return objSERVER;

        // Save it to disk
        SaveDownloadedAsset(objSERVER, pathFileLocal, nameFileLoad);
        
        
    }

    public void SaveDownloadedAsset(WWW objSERVER, string pathFileLocal, string nameFileLoad)
    {
    
        // Create the directory if it doesn't already exist
        if (!Directory.Exists(pathFolderLocal))
        {
            Directory.CreateDirectory(pathFolderLocal);
        }
        // Initialize the byte string
        byte[] bytes = objSERVER.bytes;

        // Creates a new file, writes the specified byte array to the file, and then closes the file. 
        // If the target file already exists, it is overwritten.
        File.WriteAllBytes(pathFileLocal, bytes);
        StartCoroutine(LoadFromLocal(pathFileLocal, nameFileLoad));
    }

    IEnumerator LoadFromLocal(string pathFileLocal, string nameFileLoad)
    {
        AssetBundleCreateRequest abcrObject = AssetBundle.LoadFromFileAsync(pathFileLocal);
        yield return abcrObject;

        AssetBundleRequest abrObject = abcrObject.assetBundle.LoadAssetAsync(nameFileLoad);
        yield return abrObject;
        GameObject prefabObject = abrObject.asset as GameObject;

        loadGameObject(prefabObject, nameFileLoad);


    }

    void loadGameObject(GameObject prefabObject, string nameFileLoad)
    {
        // Instantiate the asset bundle
        GameObject instanceObject = Instantiate(prefabObject) as GameObject;

        if (nameFileLoad == "axie3999")
        {
            // Positioning
            instanceObject.transform.localPosition = new Vector3(-3.5f, -1.3f, 0.94f);
            // Resize it
            instanceObject.transform.localScale = new Vector3(-1f, 1f, 1f);
            BattleControler.instance.PlayersList.Add(instanceObject.GetComponent<PlayerController>());
            BattleControler.instance.myPlayerID = BattleControler.instance.PlayersList.Count - 1;
        }
        else
        {
            // Positioning
            instanceObject.transform.localPosition = new Vector3(3.5f, -1.3f, 0.94f);
            // Resize it
            instanceObject.transform.localScale = new Vector3(1f, 1f, 1f);
            BattleControler.instance.PlayersList.Add(instanceObject.GetComponent<PlayerController>());
        }
        numLoadedFile++;
        processtext.text = "Loading File process: " + numLoadedFile + "/2";

        if (numLoadedFile == 2)
        {
            btStartGame.SetActive(true);
        }
    }

    public void press_StartGame()
    {
        PanelLoading.SetActive(false);
        BattleControler.instance.setStartValue();
    }
}
