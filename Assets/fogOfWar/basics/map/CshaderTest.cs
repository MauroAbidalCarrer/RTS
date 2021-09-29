using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CshaderTest : MonoBehaviour
{
    [SerializeField] ComputeShader Cshader;
    [SerializeField] RenderTexture rText;
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        rText = new RenderTexture(256, 256, 24);
        rText.enableRandomWrite = true;
        rText.Create();
        Cshader.SetTexture(0, "Result", rText);
        Cshader.SetInt("width", rText.width);
        Cshader.SetInt("height", rText.height);

        Cshader.Dispatch(0, rText.width / 8, rText.height / 8, 1);
        Graphics.Blit(rText, dest);
    }
}
