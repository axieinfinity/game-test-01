using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FileImportWindow : EditorWindow
{
    static string[] options = new string[]
    {
        "Assets/Scripts",
        "Assets/Scenes"
    };

    static bool fastUse;

    static int selected;
    

    private bool isPositionSet = false;

    public static int Selected
    {
        get
        {
            selected = EditorPrefs.GetInt("FileImportWindowSelected");
            return selected;
        }

        set
        {
            EditorPrefs.SetInt("FileImportWindowSelected", value);
            selected = value;
        }
    }

    [MenuItem("Tools/File Importer Window")]
    public static void ShowByMenu()
    {
        bool cache = fastUse;
        fastUse = false;
        ShowWindow();
        fastUse = cache;
    }

    public static void ShowWindow()
    {
        if (fastUse)
        {
            string destinationPath = options[Selected] + "/";
            FileImportHandler.MoveScriptFile(destinationPath);

            return;
        }

        EditorWindow window = EditorWindow.GetWindow(typeof(FileImportWindow));
        window.minSize = new Vector2(200, 30 + options.Length * 30);
        window.maxSize = new Vector2(800, 30 + options.Length * 30);

        
    }

    private void Update()
    {
        Repaint();
    }

    void OnGUI()
    {
        if (this.isPositionSet == false)
        {
            Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            Rect rect = new Rect();
            rect.position = new Vector2(mousePos.x - this.position.width * 0.5f, mousePos.y - this.position.height * 0.5f);
            rect.width = this.position.width;
            rect.height = this.position.height;

            this.position = rect;

            this.isPositionSet = true;
        }

        Selected = GUILayout.SelectionGrid(Selected, options, 1, EditorStyles.radioButton);

        GUILayout.Space(10);

        fastUse = GUILayout.Toggle(fastUse, "Quick move. Turn off by Tools->File Importer Window");

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Move"))
        {
            string destinationPath = options[Selected] + "/";
            FileImportHandler.MoveScriptFile(destinationPath);
            this.Close();
        }

        if (GUILayout.Button("Cancel"))
        {
            this.Close();
        }

        GUILayout.EndHorizontal();
    }

}
