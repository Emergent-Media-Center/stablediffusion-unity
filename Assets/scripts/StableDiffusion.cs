using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class StableDiffusion : MonoBehaviour
{
    private static StableDiffusion instance = null;
    
    // logging data structure
    // general semaphores, isstaring, isdownloading, isready, isrunning, etc

    public static StableDiffusion Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("StableDiffusionManager").AddComponent<StableDiffusion>();
                instance.StartCoroutine(instance.StableDiffusionStart());
            }
            return instance;
        }
    }
    
    public void Awake() {
        if (instance == null)
        {
            instance = this;
        }
    }

    private IEnumerator StableDiffusionStart()
    {
        // ref. https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Post.html
        // ref. https://learn.microsoft.com/en-us/dotnet/api/system.io.file.exists?view=net-7.0
        // ref. unzip with  System.IO.Compression.FileSystem probably

        // download and unzip the automatic1111 zip isnt already downloaded
        // https://github.com/AUTOMATIC1111/stable-diffusion-webui/archive/refs/tags/v1.3.0.zip
        // start the python server
        // wait for the server to be ready
        // start capturing the logs and outputting them to the console or other logging system
        // change the ready state
        yield return null;
    }
    
    // shutdown should be blocking
    private void StableDiffusionStop()
    {
        // stop and destroy references to 
        return;
    }

    public IEnumerator<bool> IsReady()
    {
        yield return false;
    }
    
    // todo: return a texture2d
    // todo: pass extra parameters needed by the AUTOMATIC1111 API. build the data structure for that
    public IEnumerator<Texture2D> GenerateImage(string prompt, Texture2D controlImage)
    {
        // ref. https://github.com/AUTOMATIC1111/stable-diffusion-webui/wiki/API
        // ref. https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Post.html

        WWWForm form = new WWWForm();
        form.AddField("json", "{\"prompt\": \"maltese puppy\",\"steps\": 5}");

        UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:7860/sdapi/v1/txt2img", form);
        UnityWebRequestAsyncOperation req = www.SendWebRequest();

        Debug.Log(www.GetResponseHeader(""));


        yield return null;
    }
    
    private void OnApplicationQuit()
    {
        StableDiffusionStop();
        instance = null;
    }
}
