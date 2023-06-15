using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo: create the interface that all brushes should follow
public abstract class BrushBase
{
    public Color color = Color.black;

    public abstract void Draw(Texture2D tex, Vector2Int pos, float scale);
}
