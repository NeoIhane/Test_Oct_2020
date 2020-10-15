using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class LiquidEffect : MonoBehaviour
{
    public Material advectionMat;
    public Material dyeMat;
    public Material jacobiMat;
    public Material forceMat;
    public Material divergenceMat;
    public Material gradientSubtractionMat;
    public Material boundaryMat;
    public Material vorticityMat;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!CheckAlreadyMaterials()) return;
        RenderTexture pass = RenderTexture.GetTemporary(source.width, source.height);
        Graphics.Blit(source, pass, advectionMat);
        boundaryMat.SetTexture("_x", pass);
        Graphics.Blit(pass, destination, boundaryMat);
        RenderTexture.ReleaseTemporary(pass);
    }
    bool CheckAlreadyMaterials()
    {
        if (advectionMat == null) return false;
        if (dyeMat == null) return false;
        if (jacobiMat == null) return false;
        if (forceMat == null) return false;
        if (divergenceMat == null) return false;
        if (gradientSubtractionMat == null) return false;
        if (boundaryMat == null) return false;
        if (vorticityMat == null) return false;
        return true;
    }
}
