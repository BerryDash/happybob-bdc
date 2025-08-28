using System;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class Tools
{
    public static Sprite GetIconForUser(BigInteger user)
    {
        if (user == 1)
        {
            return Resources.Load<Sprite>("Icons/Icons/bird_-1");
        }
        else if (user == 2)
        {
            return Resources.Load<Sprite>("Icons/Icons/bird_-2");
        }
        else if (user == 4)
        {
            return Resources.Load<Sprite>("Icons/Icons/bird_-3");
        }
        else if (user == 3)
        {
            return Resources.Load<Sprite>("Icons/Icons/bird_-4");
        }
        else
        {
            return Resources.Load<Sprite>("Icons/Icons/bird_1");
        }
    }

    public static string FormatWithCommas(string number)
    {
        try
        {
            return FormatWithCommas(BigInteger.Parse(number));
        }
        catch
        {
            return number;
        }
    }

    public static string FormatWithCommas(BigInteger number)
    {
        return string.Format("{0:N0}", number);
    }

    public static void UpdateStatusText(TMP_Text statusText, string message, Color color)
    {
        statusText.text = message;
        statusText.color = color;
    }

    public static void RenderFromBase64(string base64, Image targetImage)
    {
        byte[] imageData = Convert.FromBase64String(base64);
        Texture2D tex = new(2, 2, TextureFormat.ARGB32, false);
        if (!tex.LoadImage(imageData)) return;

        tex.filterMode = FilterMode.Point;
        tex.Apply(false, false);

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new UnityEngine.Vector2(0.5f, 0.5f));
        targetImage.sprite = sprite;
    }

    public static void RenderFromBase64(string base64, SpriteRenderer targetImage)
    {
        byte[] imageData = Convert.FromBase64String(base64);
        Texture2D tex = new(2, 2, TextureFormat.ARGB32, false);
        if (!tex.LoadImage(imageData)) return;

        tex.filterMode = FilterMode.Point;
        tex.Apply(false, false);

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new UnityEngine.Vector2(0.5f, 0.5f));
        targetImage.sprite = sprite;
    }
}