using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class StableDiffusion : MonoBehaviour
{
    private Process process = null;
    
    private static StableDiffusion instance = null;
    public Texture2D stableDiffusionImage = null;

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
                instance.StableDiffusionStart().Forget();
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
    
    public async UniTaskVoid StableDiffusionStart()
    {
        try
        {
            // File.Delete(Application.persistentDataPath + Path.DirectorySeparatorChar + "stable-diffusion-webui.zip");
            // Directory.Delete(Application.persistentDataPath + Path.DirectorySeparatorChar + "stablediffusion", true);
            // ref. https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Post.html
            // ref. https://learn.microsoft.com/en-us/dotnet/api/system.io.file.exists?view=net-7.0
            // ref. unzip with  System.IO.Compression.FileSystem probably

            // download the automatic1111 zip isnt already downloaded
            if (!File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "stable-diffusion-webui.zip"))
            {
                UnityWebRequest dlreq =
                    new UnityWebRequest(
                        "https://github.com/AUTOMATIC1111/stable-diffusion-webui/archive/refs/tags/v1.3.0.zip");
                dlreq.downloadHandler =
                    new DownloadHandlerFile(Application.persistentDataPath + Path.DirectorySeparatorChar +
                                            "stable-diffusion-webui.zip");
                var op = dlreq.SendWebRequest();

                logs.Add("Downloaded");

                while (!op.isDone)
                {
                    //here you can see download progress
                    logs.Add("StableDiffusion Download: " + (dlreq.downloadedBytes / 1000).ToString() + "KB");
                    await UniTask.Yield();
                }

                if (dlreq.isNetworkError || dlreq.isHttpError)
                {
                    logs.Add(dlreq.error);
                }
                else
                {
                    logs.Add("download success");
                }
            }

            // unzip if it is not unzipped already
            if (!Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "stablediffusion"))
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(
                    Application.persistentDataPath + Path.DirectorySeparatorChar + "stable-diffusion-webui.zip",
                    Application.persistentDataPath + Path.DirectorySeparatorChar + "stablediffusion");
                logs.Add("unzip success");
            }

            // todo: update stablediffusion
            // start the python server
            bool isWin = Application.platform == RuntimePlatform.WindowsEditor ||
                               Application.platform == RuntimePlatform.WindowsPlayer;
            
            // get first subdirectory
            string subdir =
                Directory.GetDirectories(Application.persistentDataPath + Path.DirectorySeparatorChar +
                                         "stablediffusion")[0];
            string webui = "webui." + (isWin ? "bat" : "sh");

            string fullpath = subdir + Path.DirectorySeparatorChar + webui;
            logs.Add(fullpath);

            // todo: improve this for windows and other platforms
            // make the webui executable
            if (!isWin)
            {
                process = new Process()
                {
                    StartInfo =
                    {
                        FileName = "chmod",
                        Arguments = "+x " + fullpath
                    },
                };
                process.Start();
                process.WaitForExit();
                logs.Add("chmod success");
            }
            
            await UniTask.Yield();

            // run webui
            process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    // WorkingDirectory = subdir,
                    // FileName = webui,
                    FileName = fullpath,
                    Arguments = "--api --disable-console-progressbars --nowebui",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                },
            };

            // wait for the server to be ready
            // start capturing the logs and outputting them to the console or other logging system
            logs.Add("before start");
            process.Start();
            logs.Add("started");
            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                logs.Add(line);
            }

            // change the ready state
            await UniTask.Yield();
        }
        catch (Exception e)
        {
            logs.Add(e.Message);
            logs.Add(e.StackTrace);
            throw;
        }
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

    void Update()
    {
        var logsClone = logs; // to prevent concurrent modification
        foreach(string log in logsClone) {
            Debug.Log(log);
            logs.Remove(log); // needs improvement on concurrency from mainthead consuming and the producer thread logging
        }
    }
}
