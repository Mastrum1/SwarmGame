using Es.InkPainter;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(InkCanvas))]
public class WinCondition : MonoBehaviour
{
    private InkCanvas _inkCavas;
    public float CheckRate = 2f;

    public int PixelCheckCountPerFrame = 50;
    public float ValueThreshold = 0.15f;


    public float WinThreshold = 0.7f;
    public float CurrentValue;

    //private void Awake()
    //{
    //    _inkCavas = GetComponent<InkCanvas>();
    //    _inkCavas.RenderTextureEventInterval = CheckRate;
    //    _inkCavas.OnRenderTextureUpdated += CheckWinCondition;
    //}



    //private void CheckWinCondition(object sender, RenderTexture e)
    //{
    //    StartCoroutine(CheckWinCondition(e));
    //}

    //private IEnumerator CheckWinCondition(RenderTexture e)
    //{        
    //    Texture2D tex = new Texture2D(e.width, e.height);
    //    var old_rt = RenderTexture.active;
    //    RenderTexture.active = e;

    //    tex.ReadPixels(new Rect(0, 0, e.width, e.height), 0, 0);
    //    tex.Apply();

    //    RenderTexture.active = old_rt;

    //    var pixels = tex.GetPixels();
        
    //    float sum = 0;

    //    for (int i = 0; i < pixels.Length; i++)
    //    {
    //        if(i % PixelCheckCountPerFrame == 0) yield return null;


    //        if (pixels[i].b<= ValueThreshold )
    //        {
    //            continue;
    //        }
    //        else
    //        {
    //            sum += Mathf.Clamp01(pixels[i].b / (pixels[i].r + pixels[i].g));
    //        }
    //    }
    //    CurrentValue = sum / pixels.Length;
    //}
}
