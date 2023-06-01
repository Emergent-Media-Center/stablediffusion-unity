using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class testScript : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        Debug.Log("start!");
        StableDiffusion instance = StableDiffusion.Instance;
        Texture2D control = null;
        Texture2D result;
        // call the dowload and wait for it to finish
        var coro = StartCoroutine(instance.GenerateImage("test", control));
        yield return coro;
        Texture2D text = instance.stableDiffusionImage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
