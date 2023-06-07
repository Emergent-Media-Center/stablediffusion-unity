using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

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
        var coro = StartCoroutine(instance.GenerateImage("horse"));
        yield return coro;
        GetComponent<Renderer>().material.mainTexture = instance.stableDiffusionImage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
