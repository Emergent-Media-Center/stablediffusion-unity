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

        // wait to get ready
        while (!instance.IsReady())
            yield return null;

        // call the dowload and wait for it to finish
        var coro = StartCoroutine(instance.GenerateImage("horse", null));
        yield return coro;
        Texture2D text = instance.stableDiffusionImage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
