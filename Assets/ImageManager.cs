using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageManager : MonoBehaviour
{
    private static Dictionary<string, Sprite> cachedSprites = new Dictionary<string, Sprite>();

    void Start()
    {
        
    }

    public static Sprite GetSprite(string name)
    {
        if (cachedSprites.ContainsKey(name)) return cachedSprites[name];

        var sprite = Resources.Load<Sprite>("Images/" + name);
        cachedSprites[name] = sprite;

        return sprite;
    }
}
