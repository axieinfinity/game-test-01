using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FuzzyFinder
{

    public class ScriptableObjectCreator
    {
        static class Statics
        {
            internal static Vector2 SIZE;
            internal static int LABEL_SIZE = 160;
            internal static List<System.Type> types = new List<System.Type>();

            static Statics()
            {
                SIZE = new Vector2(720, 84);
                types.Clear();

                foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in asm.GetTypes())
                    {
                        if (type.IsAbstract == false &&
                            type.IsSubclassOf(typeof(ScriptableObject)) &&
                            type.IsSubclassOf(typeof(UnityEditor.Editor)) == false &&
                            type.FullName.StartsWith("UnityEngine") == false &&
                            type.FullName.StartsWith("UnityEditor") == false)
                            types.Add(type);
                    }
                }
            }
        }

        [UnityEditor.MenuItem("Assets/Create Scriptable Object... %>", priority = 10)]
        public static void CreateScriptableObject()
        {
            //Folder.ShowFolderContents("Assets/QuickFindFiles");

            var midScreen = EditorGUIUtility.ScreenToGUIPoint(new Vector2(Screen.width / 2, Screen.height / 2));
            var rect = new Rect(midScreen, Statics.SIZE);
            rect.position += Statics.SIZE / 4;
            rect.y -= 120;

            try {
                var instance = new Popup();
                instance.onSelected += FindResolve;
                PopupWindow.Show(rect, instance);
            }
            catch { }
        }

        static void FindResolve(string typeName)
        {
            var type = Statics.types.Find((t) => t.FullName == typeName);
            if (type == null) return;

            string path = Folder.GetActiveFolderPath();// AssetDatabase.GetAssetPath(Selection.activeObject);
            string assetPath = path + $"/New {type.Name}.asset";

            ScriptableObject so = ScriptableObject.CreateInstance(type);

            ProjectWindowUtil.CreateAsset(so, assetPath);
        }

        class Popup : PopupWindowContent
        {

            struct SearchMatch
            {
                public string fullName;
                public int score;
            }

            AutocompleteSearchField autocompleteSearchField;
            List<SearchMatch> matches;

            public System.Action onClosed;
            public System.Action<string> onSelected;

            public Popup()
            {
                autocompleteSearchField = new AutocompleteSearchField();
                autocompleteSearchField.onInputChanged = OnInputChanged;
                autocompleteSearchField.onConfirm = OnConfirm;
                matches = new List<SearchMatch>(Statics.types.Count);
            }

            public override void OnClose()
            {
                onClosed?.Invoke();
            }

            public override Vector2 GetWindowSize()
            {
                var s = Statics.SIZE;
                s.y += autocompleteSearchField.GetExtendedHeight();
                return s;
            }

            public override void OnGUI(Rect rect)
            {
                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
                {
                    this.editorWindow.Close();
                    return;
                }

                rect.width -= 12;
                rect.x += 6;
                rect.y += 4;
                GUILayout.BeginArea(rect);
                GUILayout.Label("Scriptable Object creator", EditorStyles.miniBoldLabel);
                GUILayout.Label(" ", EditorStyles.miniLabel);
                GUILayout.Label(" ", EditorStyles.miniLabel);
                autocompleteSearchField.OnGUI();
                GUILayout.EndArea();
            }

            void OnInputChanged(string searchString)
            {
                autocompleteSearchField.ClearResults();
                matches.Clear();
                if (!string.IsNullOrEmpty(searchString))
                {
                    int score = 0;
                    int count = 0;

                    foreach (var type in Statics.types)
                    {
                        if (FuzzyMatcher.FuzzyMatch(type.FullName, searchString, out score))
                        {
                            matches.Add(new SearchMatch()
                            {
                                fullName = type.FullName,
                                score = score
                            });
                            count++;
                            if (count > 90) break;
                        }
                    }

                    matches.Sort((m1, m2) => m2.score.CompareTo(m1.score));
                    for (var i = 0; i < Mathf.Min(matches.Count, autocompleteSearchField.maxResults); i++)
                        autocompleteSearchField.AddResult(matches[i].fullName);
                }
            }

            void OnConfirm(string result)
            {
                this.editorWindow.Close();
                this.onSelected?.Invoke(result);

            }
        }

    }
}