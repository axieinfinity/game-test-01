using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class TimeScaleEditor : EditorWindow
{
    [MenuItem("Tools/My Editor")]
    public static void Init()
    {
        TimeScaleEditor window = (TimeScaleEditor)GetWindow(typeof(TimeScaleEditor));
        window.Show();
    }

	static GUILayoutOption[] opts = new GUILayoutOption[] {};
    public void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        
		GUILayout.BeginVertical(opts);
		{
			float timeScale = EditorGUILayout.Slider("TimeScale", Time.timeScale, 0, 10);

			GUILayout.BeginHorizontal(opts);
			{
				if (GUILayout.Button("Min", opts)) {
					timeScale = 0;
				}

				if (GUILayout.Button("X1", opts)) {
					timeScale = 1;
				}

				if (GUILayout.Button("X2", opts)) {
					timeScale = 2;
				}

				if (GUILayout.Button("X4", opts)) {
					timeScale = 4;
				}

				if (GUILayout.Button("Max", opts)) {
					timeScale = 10;
				}
			}
			GUILayout.EndHorizontal();
			Time.timeScale = timeScale;
		}
		GUILayout.EndVertical();
    }

	[MenuItem("Tools/Change Files Name")]
	public static void ChangeFilesName () {
		string folder = EditorUtility.OpenFolderPanel("Change Files Name", Application.dataPath, string.Empty);
		var files = Directory.GetFiles(folder);
		foreach (var file in files) {
			var ext = Path.GetExtension(file);
			if (ext != ".meta") {
				var fileName = Path.GetFileNameWithoutExtension(file);

				fileName = fileName.Replace(' ', '_');
				fileName = fileName.ToLower();
				fileName = Path.Combine(folder, fileName + ext);

				File.Move(file, fileName);
				//Debug.LogError(file + " => " + fileName);
			}
		}

		AssetDatabase.Refresh();
	}

	public static void ForeachFiles(string root, Action<string> actionFile, Action<string> actionFolder)
	{
		string[] files = Directory.GetFiles(root);
		foreach (string file in files)
		{
			actionFile(file);
		}

		string[] folders = Directory.GetDirectories(root);
		foreach (string folder in folders)
		{
			actionFolder(folder);
			ForeachFiles(folder, actionFile, actionFolder);
		}
	}
}