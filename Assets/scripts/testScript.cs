using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StableDiffusion instance = StableDiffusion.Instance;
        Texture2D control;
        Texture2D result;
        //result = await instance.GenerateImage("test", control);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
