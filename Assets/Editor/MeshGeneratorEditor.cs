using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BranchGenerator))]
public class MeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BranchGenerator _meshG = (BranchGenerator)target;
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
