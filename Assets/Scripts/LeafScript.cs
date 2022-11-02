using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafScript : MonoBehaviour
{
    public MeshRenderer meshRenderer;

    public void MaterialSetup(Material _mat)
    {
        if (meshRenderer != null)
        {
            Material[] newMat = new Material[1];
            newMat[0] = _mat;
            meshRenderer.materials = newMat;
        }
    }

    public void ShowLeaf()
    {
        this.gameObject.transform.localScale = Vector3.zero;
        meshRenderer.enabled = true;
        this.gameObject.LeanScale(new Vector3(15,15,15),0.5f);
    }
}
