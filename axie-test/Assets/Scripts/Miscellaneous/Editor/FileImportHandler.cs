using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


public class FileImportHandler : AssetPostprocessor
{
    private static string scriptFileName = "F";
    public static string ScriptFileName
    {
        get
        {
            scriptFileName = EditorPrefs.GetString("LastCreatedFile");
            return scriptFileName;
        }

        set
        {
            EditorPrefs.SetString("LastCreatedFile", value);
            scriptFileName = value;
        }
    }

    private static bool isWaitingCompiling;
    public static bool IsWaitingCompiling
    {
        get
        {
            isWaitingCompiling = EditorPrefs.GetBool("IsWaitingCompiling");
            return isWaitingCompiling;
        }

        set
        {
            EditorPrefs.SetBool("IsWaitingCompiling", value);
            isWaitingCompiling = value;
        }
    }

    private static string destinationPath = "F";
    public static string DestinationPath
    {
        get
        {
            destinationPath = EditorPrefs.GetString("DestinationPath");
            return destinationPath;
        }

        set
        {
            EditorPrefs.SetString("DestinationPath", value);
            destinationPath = value;
        }
    }


    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string oldFilePath in importedAssets)
        {
            //Debug.Log("<b>Old: </b>" + oldFilePath);

            string directory = Path.GetDirectoryName(oldFilePath);
            string extension = Path.GetExtension(oldFilePath).ToLower();
            //Debug.Log("dir: " + directory);
            if (extension.Equals(".cs") && directory.Equals("Assets"))
            {
                string filename = Path.GetFileName(oldFilePath);
                
                
                ScriptFileName = filename;
                Debug.Log("Not match folder: " + ScriptFileName);

                FileImportWindow.ShowWindow();

                break;
            }
        }



        //foreach (string assets in deletedAssets)
        //{
        //    Debug.Log("<b>Deleted</b>: " + assets);
        //}

        //foreach (string assets in movedAssets)
        //{
        //    Debug.Log("<b>Moved: </b>" + assets);
        //}

        //foreach (string assets in movedFromAssetPaths)
        //{
        //    Debug.Log("<b>MovedFrom</b> :" + assets);
        //}
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        if (IsWaitingCompiling == false)
        {
            return;
        }

        MoveScriptFile(DestinationPath);

        IsWaitingCompiling = false;
        DestinationPath = "W";
    }

    public static void MoveScriptFile(string destinationPath)
    {
        if (EditorApplication.isCompiling)
        {
            IsWaitingCompiling = true;
            DestinationPath = destinationPath;
            return;
        }

        string fileName = ScriptFileName;
        if (string.IsNullOrEmpty(fileName) == false && fileName.Length > 2)
        {
            string newPath = destinationPath;
            string oldFilePath = "Assets/" + fileName;
            AssetDatabase.MoveAsset(oldFilePath, newPath + fileName);
            ScriptFileName = "F";
            Debug.Log("Moved to " + newPath);
        }
    }
}
