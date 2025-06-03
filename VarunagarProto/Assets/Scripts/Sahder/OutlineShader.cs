using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineShader : MonoBehaviour
{
    public Image targetImage;
    public Texture2D sourceTexture;
    public Color outlineColor = Color.black;
    [Range(1, 10)]
    public int outlineThickness = 1;

    void Start()
    {
        Texture2D readable = MakeReadable(sourceTexture);
        Texture2D outlinedTexture = AddPreciseOutline(readable, outlineColor, outlineThickness);

        Sprite outlinedSprite = Sprite.Create(
            outlinedTexture,
            new Rect(0, 0, outlinedTexture.width, outlinedTexture.height),
            new Vector2(0.5f, 0.5f),
            100f
        );

        targetImage.sprite = outlinedSprite;
    }

    Texture2D AddPreciseOutline(Texture2D original, Color outlineColor, int thickness)
    {
        int w = original.width;
        int h = original.height;
        Texture2D newTex = new Texture2D(w, h);
        Color32[] pixels = original.GetPixels32();
        Color32[] newPixels = new Color32[pixels.Length];

        for (int i = 0; i < pixels.Length; i++)
            newPixels[i] = pixels[i];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int i = x + y * w;

                if (pixels[i].a == 0)
                {
                    bool nearEdge = false;

                    for (int dy = -thickness; dy <= thickness; dy++)
                    {
                        for (int dx = -thickness; dx <= thickness; dx++)
                        {
                            int nx = x + dx;
                            int ny = y + dy;
                            if (nx < 0 || nx >= w || ny < 0 || ny >= h)
                                continue;

                            int ni = nx + ny * w;
                            if (pixels[ni].a > 0)
                            {
                                nearEdge = true;
                                break;
                            }
                        }
                        if (nearEdge) break;
                    }

                    if (nearEdge)
                        newPixels[i] = outlineColor;
                }
            }
        }

        newTex.SetPixels32(newPixels);
        newTex.Apply();
        return newTex;
    }
    Texture2D MakeReadable(Texture2D source)
    {
        RenderTexture tmp = RenderTexture.GetTemporary(
            source.width, source.height, 0,
            RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

        Graphics.Blit(source, tmp);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmp;

        Texture2D readableTex = new Texture2D(source.width, source.height);
        readableTex.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
        readableTex.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmp);

        return readableTex;
    }
}
