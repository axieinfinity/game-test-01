using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace FuzzyFinder
{

    public class CreateShowHideAnim
	{

	    [MenuItem("Assets/Create/ShowHideAnim", priority = 5)]
		public static void FuncCreateShowHideAnim()
		{

			var type = typeof(Animator);

			string path = GetActiveFolderPath();// AssetDatabase.GetAssetPath(Selection.activeObject);
			string assetPath = path + $"/NewShowHide.controller";
			var duplicated_index = 1;
			while (System.IO.File.Exists(assetPath))
			{
				assetPath = path + $"/NewShowHide {duplicated_index}.controller";
				duplicated_index++;
			}

			var controller = new UnityEditor.Animations.AnimatorController();
			AssetDatabase.CreateAsset(controller, assetPath);
			Undo.RegisterCreatedObjectUndo(controller, "Generate new controller");

			controller.AddLayer("default");

			var clip1 = CreateAnimatorClip("Show", controller);
			var clip2 = CreateAnimatorClip("Hide", controller);

			AssetDatabase.AddObjectToAsset(clip1, controller);
			AssetDatabase.AddObjectToAsset(clip2, controller);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			Selection.activeObject = controller;
		}

		private static AnimationClip CreateAnimatorClip(string name, AnimatorController animator)
		{
			var state = animator.layers[0].stateMachine;
			var clip = new AnimationClip();
			clip.frameRate = 60;
			clip.name = name;

			var newState = state.AddState(name);
			newState.writeDefaultValues = false;
			newState.motion = clip;
			state.defaultState = state.defaultState ?? newState;

			return clip;
		}

		private static string GetActiveFolderPath()
		{
			Assembly editorAssembly = typeof(Editor).Assembly;
			System.Type projectBrowserType = editorAssembly.GetType("UnityEditor.ProjectBrowser");

			// This is the internal method, which performs the desired action.
			// Should only be called if the project window is in two column mode.
			MethodInfo showFolderContents = projectBrowserType.GetMethod(
				"GetActiveFolderPath", BindingFlags.Instance | BindingFlags.NonPublic);
			UnityEngine.Object[] projectBrowserInstances = Resources.FindObjectsOfTypeAll(projectBrowserType);

			if (projectBrowserInstances.Length > 0)
				for (int i = 0; i < projectBrowserInstances.Length; i++)
					return showFolderContents.Invoke(projectBrowserInstances[i], null) as string;
			else
				return AssetDatabase.GetAssetPath(Selection.activeObject);
			return "";
        }
	}
}