using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxGenerator : MonoBehaviour
{
    [SerializeField] Texture2D texture;
    [SerializeField] Material matSkybox;

    [ContextMenu("AverageColor")]
    public void AverageCapColor()
    {
        if(texture == null)
        {
            Debug.LogError("No texture specified");
            return;
        }

        if(!texture.isReadable)
        {
            Debug.LogError("Check \"Read/Write\" under advanced settings in this texture");
            return;
        }

        if(matSkybox == null)
        {
            Debug.LogError("No material specified");
            return;
        }

        Color col = Color.black;

        //Loop over top pixels of texture and average the resulting color
        for(int i = 0; i < texture.width; i++)
        {
            col += texture.GetPixel(i, texture.height - 1);
        }
        col /= texture.width;

        matSkybox.SetTexture("_Panorama", texture);
        matSkybox.SetColor("_CapColor", col);
    }
}
