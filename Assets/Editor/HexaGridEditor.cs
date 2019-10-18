using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexaGrid))]

public class HexaGridEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
		DrawDefaultInspector ();
		if (GUILayout.Button("Generate Units"))
		{
            HexaGrid item = (HexaGrid)target;

            if (item) item.GenerateUnits();
		}
	}
}