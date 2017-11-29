using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SimpleBlitMaterial : MonoBehaviour
{
    public Material TransitionMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (TransitionMaterial != null)
            Graphics.Blit(src, dst, TransitionMaterial);
    }
}
