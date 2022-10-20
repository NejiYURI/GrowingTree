using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshGenerator))]
public class MeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MeshGenerator _meshG = (MeshGenerator)target;
        if (GUILayout.Button("Generate"))
        {
            _meshG.StartGenerate();
        }

        if (GUILayout.Button("Clear"))
        {
            _meshG.ClearFunc();
        }
    }
}
