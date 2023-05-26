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
    public Vector2Int cellSize = new Vector2Int(100,100);
    public Camera captureCamera;

    public int numberOfCells;
    public Vector2Int atlasSize;
    public Texture2D diffuseMap; //gross png
    public Vector2Int currentAtlasPos;
   
    private void Awake()
    {
        captureCamera = Camera.main;

        numberOfCells = Mathf.CeilToInt(Mathf.Sqrt(numberOfPictures));

        atlasSize = cellSize * numberOfCells;
        currentAtlasPos = new Vector2Int(0, atlasSize.y - cellSize.y);

        diffuseMap = new Texture2D(atlasSize.x, atlasSize.y, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point
        };
    }

    public void CaptureDynamic()
    {

        ClearAtlas(diffuseMap, Color.clear);

        CaptureRecursion(0);
        
    }

    public void CaptureRecursion(int count)
    {
        CaptureSingle(count);
        if(count>=50) 
        {
            SaveCapture(diffuseMap);
            return;
        }

        LeanTween.delayedCall(stepDuration, () => { CaptureRecursion(count + 1); });


    }

    public void CaptureSingle(int count)
    {
        var rtFrame = new RenderTexture(cellSize.x, cellSize.y, 24, RenderTextureFormat.ARGB32)
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
        diffuseMap.ReadPixels(new Rect(0, 0, rtFrame.width, rtFrame.height), currentAtlasPos.x, currentAtlasPos.y);
        diffuseMap.Apply();

        captureCamera.targetTexture = null;
        captureCamera.Render();
        Graphics.SetRenderTarget(null);

        currentAtlasPos.x += cellSize.x;

        if ((count + 1) % numberOfCells == 0)
        {
            currentAtlasPos.x = 0;
            currentAtlasPos.y -= cellSize.y;
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
