using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// todo: implement the interfaces
public class CircularBrush : BrushBase 
{
    public override void Draw(Texture2D tex, Vector2Int pos, float scale)
    {
        for (int x = -(int)scale; x < scale; x++)
        {
            if(pos.x + x > tex.width || pos.x + x < 0) continue;

            for(int y = -(int)scale; y < scale; y++)
            {
                if (pos.y + y > tex.height || pos.y + y < 0) continue;

                Vector2Int coord = new Vector2Int(x, y);

                //Outside of range
                if(coord.magnitude > scale)
                {
                    continue;
                }

                tex.SetPixel(pos.x + x, pos.y + y, color);
            }
        }
    }
}
