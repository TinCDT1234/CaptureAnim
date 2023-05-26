using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CaptureAnim : MonoBehaviour
{
    public float stepDuration = 0.1f;
    public int numberOfPictures = 50;
    public Vector2Int cellPixelSize = new Vector2Int(100,100);
    public Camera captureCamera;

    public int atlasDimension;
    public Vector2Int atlasPixelSize;
    public Texture2D targetPNG; //gross png
    public Vector2Int currentAtlasPos;
   
    private void Awake()
    {
        captureCamera = Camera.main;

        atlasDimension = Mathf.CeilToInt(Mathf.Sqrt(numberOfPictures));

        atlasPixelSize = cellPixelSize * atlasDimension;
        currentAtlasPos = new Vector2Int(0, atlasPixelSize.y - cellPixelSize.y);

        targetPNG = new Texture2D(atlasPixelSize.x, atlasPixelSize.y, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point
        };
    }

    public void CaptureDynamic()
    {

        ClearAtlas(targetPNG, Color.clear);

        CaptureRecursion(0);
        
    }

    public void CaptureRecursion(int count)
    {
        CaptureSingle(count);

        if(count>=50) 
        {
            SaveCapture(targetPNG);
            return;
        }

        LeanTween.delayedCall(stepDuration, () => { CaptureRecursion(count + 1); });
    }

    public void CaptureSingle(int count)
    {
        var rtFrame = new RenderTexture(cellPixelSize.x, cellPixelSize.y, 24, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Point,
            antiAliasing = 1,
            hideFlags = HideFlags.HideAndDontSave
        };

        captureCamera.targetTexture = rtFrame;

        //
        captureCamera.backgroundColor = Color.clear;
        captureCamera.Render();
        Graphics.SetRenderTarget(rtFrame);
        targetPNG.ReadPixels(new Rect(0, 0, rtFrame.width, rtFrame.height), currentAtlasPos.x, currentAtlasPos.y);
        targetPNG.Apply();

        captureCamera.targetTexture = null;
        captureCamera.Render();
        Graphics.SetRenderTarget(null);

        currentAtlasPos.x += cellPixelSize.x;

        if ((count + 1) % atlasDimension == 0)
        {
            currentAtlasPos.x = 0;
            currentAtlasPos.y -= cellPixelSize.y;
        }
    }
    private void ClearAtlas(Texture2D texture, Color color)
    {
        var pixels = new Color[texture.width * texture.height];
        for (var i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
    }

    public void SaveCapture(Texture2D diffuseMap)
    {
        var diffusePath = EditorUtility.SaveFilePanel("Save Capture", "", "NewCapture", "png");
        File.WriteAllBytes(diffusePath, diffuseMap.EncodeToPNG());
    }
}
