using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;


public enum APITypes
{
    txt2img = 0,
    img2img
}

public class StableDiffusion : MonoBehaviour
{
    private bool isReady = false;
    private Process process = null;
    
    private static StableDiffusion instance = null;
    public Texture2D stableDiffusionImage = null;
    public Texture2D inpaintMask = null;

    private string StableDiffusionAddress = "http://127.0.0.1:7860";
    
    // logging data structure
    public List<string> logs = new List<string>();
    // general semaphores, isstaring, isdownloading, isready, isrunning, etc

    public static StableDiffusion Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("StableDiffusionManager").AddComponent<StableDiffusion>();
                instance.StartCoroutine(instance.TestIfStableDiffusionIsRunning((done) =>
                {
                    if (done)
                    {
                        Debug.Log("StableDiffusion is running");
                        instance.isReady = true;
                    }
                    else
                    {
                        Debug.Log(
                            "StableDiffusion is not running. Do you forgot to run it? Refer to https://github.com/AUTOMATIC1111/stable-diffusion-webui");
                        instance.isReady = false;
                    }
                }));
            }
            return instance;
        }
    }
    
    public void Start() {
        if (instance == null) {
            instance = this;
        }
    }
    
    public IEnumerator TestIfStableDiffusionIsRunning(Action<bool> callback)
    {
        using (var www = UnityWebRequest.Get(StableDiffusionAddress + "/internal/ping"))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
                callback?.Invoke(false);
            }
            else
            {
                if (www.downloadHandler.text == "{}")
                    callback?.Invoke(true);
                else
                    callback?.Invoke(false);
            }
        }
    }

    // void SetExecutablePermission(string path)
    // {
    //     // todo: this is not working! please help!!
    //     FileInfo fileInfo = new FileInfo(path);
    //
    //     if (fileInfo.Exists)
    //     {
    //         // Get the current file attributes
    //         FileAttributes attributes = fileInfo.Attributes;
    //
    //         // Set the executable flag
    //         attributes |= FileAttributes.Normal;
    //         attributes |= FileAttributes.Archive;
    //         attributes |= FileAttributes.ReadOnly;
    //         attributes |= FileAttributes.Hidden;
    //         attributes |= FileAttributes.System;
    //         
    //         // Set the updated attributes
    //         fileInfo.Attributes = attributes;
    //         fileInfo.IsReadOnly = false;
    //
    //         ProcessStartInfo startInfo = new ProcessStartInfo();
    //         startInfo.FileName = "chmod";
    //         startInfo.Arguments = "+x \"" + path + "\"";
    //
    //         Process process = new Process();
    //         process.StartInfo = startInfo;
    //         process.Start();
    //         process.WaitForExit();
    //
    //         logs.Add("permission set");
    //     }
    //     else
    //     {
    //         Debug.LogError("File does not exist: " + path);
    //     }
    // }

    // public async UniTaskVoid StableDiffusionDownloader()
    // {
    //     try
    //     {
    //         // File.Delete(Application.persistentDataPath + Path.DirectorySeparatorChar + "stable-diffusion-webui.zip");
    //         // Directory.Delete(Application.persistentDataPath + Path.DirectorySeparatorChar + "stablediffusion", true);
    //         // ref. https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Post.html
    //         // ref. https://learn.microsoft.com/en-us/dotnet/api/system.io.file.exists?view=net-7.0
    //         // ref. unzip with  System.IO.Compression.FileSystem probably
    //
    //         // download the automatic1111 zip isnt already downloaded
    //         if (!File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "stable-diffusion-webui.zip"))
    //         {
    //             UnityWebRequest dlreq =
    //                 new UnityWebRequest(
    //                     "https://github.com/AUTOMATIC1111/stable-diffusion-webui/archive/refs/tags/v1.3.0.zip");
    //             dlreq.downloadHandler =
    //                 new DownloadHandlerFile(Application.persistentDataPath + Path.DirectorySeparatorChar +
    //                                         "stable-diffusion-webui.zip");
    //             var op = dlreq.SendWebRequest();
    //
    //             logs.Add("Downloaded");
    //
    //             while (!op.isDone)
    //             {
    //                 //here you can see download progress
    //                 logs.Add("StableDiffusion Download: " + (dlreq.downloadedBytes / 1000).ToString() + "KB");
    //                 await UniTask.Yield();
    //             }
    //
    //             if (dlreq.isNetworkError || dlreq.isHttpError)
    //             {
    //                 logs.Add(dlreq.error);
    //             }
    //             else
    //             {
    //                 logs.Add("download success");
    //             }
    //         }
    //
    //         logs.Add("stable diffusion zip file exists");
    //         
    //         // unzip if it is not unzipped already
    //         if (!Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "stablediffusion"))
    //         {
    //             Unity.SharpZipLib.Utils.ZipUtility.UncompressFromZip(
    //                 Application.persistentDataPath + Path.DirectorySeparatorChar + "stable-diffusion-webui.zip", 
    //                 null, 
    //                 Application.persistentDataPath + Path.DirectorySeparatorChar + "stablediffusion");
    //             // System.IO.Compression.ZipFile.ExtractToDirectory(
    //             //     Application.persistentDataPath + Path.DirectorySeparatorChar + "stable-diffusion-webui.zip",
    //             //     Application.persistentDataPath + Path.DirectorySeparatorChar + "stablediffusion");
    //             logs.Add("unzip success");
    //         }
    //         
    //         logs.Add("stable diffusion directory exists");
    //
    //         // todo: update stablediffusion
    //         // start the python server
    //         bool isWin = Application.platform == RuntimePlatform.WindowsEditor ||
    //                            Application.platform == RuntimePlatform.WindowsPlayer;
    //         
    //         // get first subdirectory
    //         string subdir =
    //             Directory.GetDirectories(Application.persistentDataPath + Path.DirectorySeparatorChar +
    //                                      "stablediffusion")[0];
    //         string webui = "webui." + (isWin ? "bat" : "sh");
    //
    //         string fullpath = subdir + Path.DirectorySeparatorChar + webui;
    //         logs.Add(fullpath);
    //
    //         // todo: improve this for windows and other platforms
    //         // make the webui executable
    //         if (!isWin)
    //             SetExecutablePermission(fullpath);
    //
    //         await UniTask.Yield();
    //
    //         string executer = isWin ? "cmd.exe" : "/bin/bash";
    //
    //         // run webui
    //         // wait for the server to be ready
    //         // start capturing the logs and outputting them to the console or other logging system
    //         logs.Add("before start");
    //         // process.Start();
    //         process = Process.Start(new ProcessStartInfo()
    //         {
    //             FileName = executer,
    //             Arguments = isWin ? "/c " + fullpath : fullpath,
    //             UseShellExecute = true,
    //             CreateNoWindow = true,
    //             WorkingDirectory = subdir
    //         });
    //         logs.Add("started");
    //         // while (!process.StandardOutput.EndOfStream)
    //         // {
    //         //     string line = process.StandardOutput.ReadLine();
    //         //     logs.Add(line);
    //         // }
    //
    //         process.WaitForExit();
    //         logs.Add("exit");
    //
    //         // change the ready state
    //         await UniTask.Yield();
    //     }
    //     catch (Exception e)
    //     {
    //         logs.Add(e.Message);
    //         logs.Add(e.StackTrace);
    //         throw;
    //     }
    // }
    
    // shutdown should be blocking
    private void StableDiffusionStop()
    {
        // stop and destroy references to 
        return;
    }

    public bool IsReady()
    {
        return isReady;
    }
    
    // todo: return a texture2d
    // todo: pass extra parameters needed by the AUTOMATIC1111 API. build the data structure for that
    public IEnumerator GenerateImage(string prompt, APITypes apiType, Dictionary<string, object> payload)
    {
        // ref. https://github.com/Cysharp/UniTask
        // ref. https://github.com/AUTOMATIC1111/stable-diffusion-webui/wiki/API
        // ref. https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Post.html

        int width = 512, height = 512;

        if(payload.TryGetValue("width", out object w))
        {
            width = (int)w;
        }

        if(payload.TryGetValue("height", out object h))
        {
            height = (int)h;
        }
        Debug.Log(width + "x" + height);

        // todo: improve and make it work as dictionary
        //string json = "{\"prompt\": \"" + prompt + "\",\"steps\": 50,\"cfg_scale\": 7,\"sampler_index\": \"Euler a\",\"width\": 512,\"height\": 512}";
        //note: JsonUtilities does not support dicts for some reason, so i am using newtonsoft json
        string json = JsonConvert.SerializeObject(payload);
        var bytes = Encoding.UTF8.GetBytes(json);
        Debug.Log(json);

        using (var www = new UnityWebRequest(StableDiffusionAddress + "/sdapi/v1/" + apiType.ToString(), "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bytes);
            www.downloadHandler = new DownloadHandlerBuffer();
            
            // www.certificateHandler = new ForceAcceptAllCertificates();
            www.SetRequestHeader("accept", "application/json");
            www.SetRequestHeader("Content-Type", "application/json");

            // Btw afaik you can simply
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) {
                Debug.LogError(www.error);
                yield break;
            }
            
            Debug.Log(www.downloadHandler.text);

            StableDiffusionTextToImageResponse jsonparsed = JsonUtility.FromJson<StableDiffusionTextToImageResponse>(www.downloadHandler.text);
            byte[] imagebytes = System.Convert.FromBase64String(jsonparsed.images[0]);

            if (stableDiffusionImage == null)
                stableDiffusionImage = new Texture2D(width,height);

            stableDiffusionImage.LoadImage(imagebytes);
            stableDiffusionImage.Apply();
            

            yield return null;
            // todo: extract the image from the response
            // todo: fill the variable stableDiffusionImage from the response above
            // https://docs.unity3d.com/ScriptReference/Texture2D.LoadRawTextureData.html
        }
    }


    // todo: return a texture2d
    // todo: pass extra parameters needed by the AUTOMATIC1111 API. build the data structure for that
    public IEnumerator TileImage(string prompt)
    {
        // ref. https://github.com/Cysharp/UniTask
        // ref. https://github.com/AUTOMATIC1111/stable-diffusion-webui/wiki/API
        // ref. https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Post.html


        Dictionary<string, object> promptDict = new Dictionary<string, object>();
        string[] images = { "data:image/png;base64," + Convert.ToBase64String(stableDiffusionImage.EncodeToPNG()) };
        promptDict["init_images"] = images;
        promptDict["mask"] = "data:image/png;base64," + Convert.ToBase64String(inpaintMask.EncodeToPNG());
        promptDict["prompt"] = prompt;
        promptDict["steps"] = 20;
        promptDict["width"] = 1024;
        promptDict["height"] = 512;
        promptDict["tiling"] = true;
        promptDict["denoising_strength"] = .75;
        promptDict["image_cfg_scale"] = 0;
        promptDict["mask_blur"] = 4;
        promptDict["inpainting_fill"] = 1;
        promptDict["inpaint_full_res"] = true;
        promptDict["inpaint_full_res_padding"] = 0;
        promptDict["inpainting_mask_invert"] = 0;
        promptDict["initial_noise_multiplier"] = 0;
        promptDict["seed"] = -1;
        promptDict["subseed"] = -1;
        promptDict["subseed_strength"] = 0;
        promptDict["seed_resize_from_h"] = -1;
        promptDict["seed_resize_from_w"] = -1;
        promptDict["sampler_name"] = "Euler a";
        promptDict["batch_size"] = 1;
        promptDict["n_iter"] = 1;
        promptDict["cfg_scale"] = 7;
        promptDict["sampler_index"] = "Euler a";
        //todo: use override settings to set model
        //Python:
        //"override_settings": {
        //  "sd_model_checkpoint": "v1-5-pruned-emaonly.safetensors [6ce0161689]"
        //}

    // todo: improve and make it work as dictionary
    //string json = "{\"prompt\": \"" + prompt + "\",\"steps\": 50,\"cfg_scale\": 7,\"sampler_index\": \"Euler a\",\"width\": 512,\"height\": 512}";
    //note: JsonUtilities does not support dicts for some reason, so i am using newtonsoft json
    string json = JsonConvert.SerializeObject(promptDict);
        var bytes = Encoding.UTF8.GetBytes(json);
        Debug.Log(json);

        using (var www = new UnityWebRequest(StableDiffusionAddress + "/sdapi/v1/img2img", "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bytes);
            www.downloadHandler = new DownloadHandlerBuffer();

            // www.certificateHandler = new ForceAcceptAllCertificates();
            www.SetRequestHeader("accept", "application/json");
            www.SetRequestHeader("Content-Type", "application/json");

            // Btw afaik you can simply
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                yield break;
            }

            Debug.Log(www.downloadHandler.text);

            StableDiffusionTextToImageResponse jsonparsed = JsonUtility.FromJson<StableDiffusionTextToImageResponse>(www.downloadHandler.text);
            byte[] imagebytes = System.Convert.FromBase64String(jsonparsed.images[0]);

            if (stableDiffusionImage == null)
                stableDiffusionImage = new Texture2D(1024, 512, TextureFormat.RGBA32, false);

            stableDiffusionImage.LoadImage(imagebytes);
            stableDiffusionImage.Apply();


            yield return null;
            // todo: extract the image from the response
            // todo: fill the variable stableDiffusionImage from the response above
            // https://docs.unity3d.com/ScriptReference/Texture2D.LoadRawTextureData.html
        }
    }

    private void OnApplicationQuit()
    {
        StableDiffusionStop();
        instance = null;
    }

    void Update()
    {
        var logsClone = logs; // to prevent concurrent modification
        foreach(string log in logsClone) {
            Debug.Log(log);
            logs.Remove(log); // needs improvement on concurrency from mainthead consuming and the producer thread logging
        }
    }
}
