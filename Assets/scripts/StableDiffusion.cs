using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class StableDiffusion : MonoBehaviour
{
    private static StableDiffusion instance = null;
    public Texture2D stableDiffusionImage = null;
    
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

        // download the automatic1111 zip isnt already downloaded
        if (!File.Exists(Application.persistentDataPath + Path.PathSeparator + "stable-diffusion-webui.zip"))
        {
            UnityWebRequest dlreq =
                new UnityWebRequest(
                    "https://github.com/AUTOMATIC1111/stable-diffusion-webui/archive/refs/tags/v1.3.0.zip");
            dlreq.downloadHandler =
                new DownloadHandlerFile(Application.persistentDataPath + Path.PathSeparator + "stable-diffusion-webui.zip");
            var op = dlreq.SendWebRequest();
            
            while (!op.isDone)
            {
                //here you can see download progress
                Debug.Log( "StableDiffusion Download: " + (dlreq.downloadedBytes / 1000).ToString() + "KB");
                yield return null;
            }
            
            if (dlreq.isNetworkError || dlreq.isHttpError)
            {
                Debug.Log(dlreq.error);
            }
            else
            {
                Debug.Log("download success");
            }
        }
        
        // unzip if it is not unzipped already
        if (!Directory.Exists(Application.persistentDataPath + Path.PathSeparator + "stablediffusion")) {
            System.IO.Compression.ZipFile.ExtractToDirectory(Application.persistentDataPath + Path.PathSeparator + "stable-diffusion-webui.zip", Application.persistentDataPath + Path.PathSeparator + "stablediffusion");
            Debug.Log("unzip success");
        }
        
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
    public IEnumerator GenerateImage(string prompt, Texture2D controlImage)
    {
        // ref. https://github.com/Cysharp/UniTask
        // ref. https://github.com/AUTOMATIC1111/stable-diffusion-webui/wiki/API
        // ref. https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Post.html

        WWWForm form = new WWWForm();
        form.AddField("json", "{\"prompt\": \"maltese puppy\",\"steps\": 5}");

        UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:7860/sdapi/v1/txt2img", form);
        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);
        
        // todo: extract the image from the response
        // todo: fill the variable stableDiffusionImage from the response above
        // https://docs.unity3d.com/ScriptReference/Texture2D.LoadRawTextureData.html
    }
    
    private void OnApplicationQuit()
    {
        StableDiffusionStop();
        instance = null;
    }
}
