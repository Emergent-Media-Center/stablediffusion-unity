using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TexturePainter : MonoBehaviour
{
    public Texture2D texture;
    
    // Start is called before the first frame update
    // todo: load the last image painted by the user
    void Start()
    {
        texture = new Texture2D(2048, 2048);
        GetComponent<Image>().material.mainTexture = texture;
    }
    
    // todo: pass brush structure
    void Paint(Vector2Int center, float scale, BrushBase brush)
    {
        // todo: paint the texture and then apply it
        texture.Apply();
    }
}
