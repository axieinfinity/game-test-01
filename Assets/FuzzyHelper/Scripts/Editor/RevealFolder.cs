using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FuzzyFinder
{
	public static class Folder
	{
        internal static void ShowProjectBrowserTab()
        {
            System.Type pt = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.ProjectBrowser");
            EditorWindow.GetWindow(pt).Show();
        }

        internal static void ShowFolderContents(string folderPath)
		{
            if (System.IO.Directory.Exists(folderPath) == false) return;
            var folder = AssetDatabase.LoadAssetAtPath<Object>(folderPath);
            var id = folder.GetInstanceID();
            ShowFolderContents(id);
        }

        internal static string GetActiveFolderPath()
        {
            Assembly editorAssembly = typeof(Editor).Assembly;
            System.Type projectBrowserType = editorAssembly.GetType("UnityEditor.ProjectBrowser");

            // This is the internal method, which performs the desired action.
            // Should only be called if the project window is in two column mode.
            MethodInfo showFolderContents = projectBrowserType.GetMethod(
                "GetActiveFolderPath", BindingFlags.Instance | BindingFlags.NonPublic);
            Object[] projectBrowserInstances = Resources.FindObjectsOfTypeAll(projectBrowserType);

            if (projectBrowserInstances.Length > 0)
                for (int i = 0; i < projectBrowserInstances.Length; i++)
                    return showFolderContents.Invoke(projectBrowserInstances[i], null) as string;
            else
                return AssetDatabase.GetAssetPath(Selection.activeObject);
            return "";
        }

        /// <summary>
	 /// Selects a folder in the project window and shows its content.
	 /// Opens a new project window, if none is open yet.
	 /// </summary>
	 /// <param name="folderInstanceID">The instance of the folder asset to open.</param>
		internal static void ShowFolderContents(int folderInstanceID)
		{
			// Find the internal ProjectBrowser class in the editor assembly.
			Assembly editorAssembly = typeof(Editor).Assembly;
			System.Type projectBrowserType = editorAssembly.GetType("UnityEditor.ProjectBrowser");

			// This is the internal method, which performs the desired action.
			// Should only be called if the project window is in two column mode.
			MethodInfo showFolderContents = projectBrowserType.GetMethod(
				"ShowFolderContents", BindingFlags.Instance | BindingFlags.NonPublic);

			// Find any open project browser windows.
			Object[] projectBrowserInstances = Resources.FindObjectsOfTypeAll(projectBrowserType);

			if (projectBrowserInstances.Length > 0)
			{
				for (int i = 0; i < projectBrowserInstances.Length; i++)
					ShowFolderContentsInternal(projectBrowserInstances[i], showFolderContents, folderInstanceID);
			}
			else
			{
				EditorWindow projectBrowser = OpenNewProjectBrowser(projectBrowserType);
				ShowFolderContentsInternal(projectBrowser, showFolderContents, folderInstanceID);
			}
		}

		private static void ShowFolderContentsInternal(Object projectBrowser, MethodInfo showFolderContents, int folderInstanceID)
		{
			// Sadly, there is no method to check for the view mode.
			// We can use the serialized object to find the private property.
			SerializedObject serializedObject = new SerializedObject(projectBrowser);
			bool inTwoColumnMode = serializedObject.FindProperty("m_ViewMode").enumValueIndex == 1;

			if (!inTwoColumnMode)
			{
				// If the browser is not in two column mode, we must set it to show the folder contents.
				MethodInfo setTwoColumns = projectBrowser.GetType().GetMethod(
					"SetTwoColumns", BindingFlags.Instance | BindingFlags.NonPublic);
				setTwoColumns.Invoke(projectBrowser, null);
			}

			bool revealAndFrameInFolderTree = true;
			showFolderContents.Invoke(projectBrowser, new object[] { folderInstanceID, revealAndFrameInFolderTree });
		}

		private static EditorWindow OpenNewProjectBrowser(System.Type projectBrowserType)
		{
			EditorWindow projectBrowser = EditorWindow.GetWindow(projectBrowserType);
			projectBrowser.Show();

			// Unity does some special initialization logic, which we must call,
			// before we can use the ShowFolderContents method (else we get a NullReferenceException).
			MethodInfo init = projectBrowserType.GetMethod("Init", BindingFlags.Instance | BindingFlags.Public);
			init.Invoke(projectBrowser, null);

			return projectBrowser;
		}
	}
}

