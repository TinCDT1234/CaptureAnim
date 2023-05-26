using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CaptureSingle : MonoBehaviour
{
    private Vector2Int _cellSize = new Vector2Int(512,512);
    public Camera _camera;
    public void CaptureScreen()
    {
        var diffuseMap = new Texture2D(_cellSize.x, _cellSize.y, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point
        };
        var rtFrame = new RenderTexture(_cellSize.x, _cellSize.y, 24, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Point,
            antiAliasing = 1,
            hideFlags = HideFlags.HideAndDontSave
        };

        _camera.targetTexture = rtFrame;

        //
        _camera.backgroundColor = Color.clear;
        _camera.Render() ;
        Graphics.SetRenderTarget(rtFrame);
        diffuseMap.ReadPixels(new Rect(0, 0, rtFrame.width, rtFrame.height), 0, 0);

        diffuseMap.Apply();

        // Reset the camera to render to the screen
        _camera.targetTexture = null;
        _camera.Render();
        Graphics.SetRenderTarget(null);

        SaveCapture(diffuseMap);

    }

    public void SaveCapture(Texture2D diffuseMap)
    {
        var diffusePath = EditorUtility.SaveFilePanel("Save Capture", "", "NewCapture", "png");
        File.WriteAllBytes(diffusePath, diffuseMap.EncodeToPNG());
    }
}
