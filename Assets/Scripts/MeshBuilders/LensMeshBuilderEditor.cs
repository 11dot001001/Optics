using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(LensMeshBuilder))]
public class LensMeshBuilderEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("Build lens"))
		{
			LensMeshBuilder builder = (LensMeshBuilder)target;
			builder.UpdateMesh();
		}
	}
}
