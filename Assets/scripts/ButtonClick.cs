using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{

	[SerializeField] public Button yourButton;
	[SerializeField] public GameObject promptField;
	[SerializeField] Texture2D inpaintMask;
	[SerializeField] Material skyboxMaterial;
	[SerializeField] Texture2D defaultImage;

	void Start()
	{
		Button btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
        skyboxMaterial.SetTexture("_Panorama", defaultImage);
    }

    private void OnDestroy()
    {
        skyboxMaterial.SetTexture("_Panorama", defaultImage);
    }

    void TaskOnClick()
	{
		Debug.Log("done");
		StartCoroutine(makeImage(promptField.GetComponent<TMP_InputField>().text));

	}
	IEnumerator makeImage(string prompt)
	{
		Debug.Log("start!");
		StableDiffusion instance = StableDiffusion.Instance;

		// wait to get ready
		while (!instance.IsReady())
			yield return null;

		instance.inpaintMask = inpaintMask;

        Dictionary<string, object> promptDict = new Dictionary<string, object>();
        promptDict["prompt"] = prompt;
        promptDict["steps"] = 20;
        promptDict["cfg_scale"] = 7;
        promptDict["sampler_index"] = "Euler a";
        promptDict["width"] = 1024;
        promptDict["height"] = 512;

        // call the dowload and wait for it to finish
        var coro = StartCoroutine(instance.GenerateImage(prompt, APITypes.txt2img, promptDict));
		yield return coro;

        Dictionary<string, object> imgDict = new Dictionary<string, object>();
        string[] images = { "data:image/png;base64," + Convert.ToBase64String(instance.stableDiffusionImage.EncodeToPNG()) };
        imgDict["init_images"] = images;
        imgDict["mask"] = "data:image/png;base64," + Convert.ToBase64String(inpaintMask.EncodeToPNG());
        imgDict["prompt"] = prompt;
        imgDict["steps"] = 20;
        imgDict["width"] = 1024;
        imgDict["height"] = 512;
        imgDict["tiling"] = true;
        imgDict["denoising_strength"] = .75;
        imgDict["image_cfg_scale"] = 0;
        imgDict["mask_blur"] = 4;
        imgDict["inpainting_fill"] = 1;
        imgDict["inpaint_full_res"] = true;
        imgDict["inpaint_full_res_padding"] = 0;
        imgDict["inpainting_mask_invert"] = 0;
        imgDict["initial_noise_multiplier"] = 0;
        imgDict["seed"] = -1;
        imgDict["subseed"] = -1;
        imgDict["subseed_strength"] = 0;
        imgDict["seed_resize_from_h"] = -1;
        imgDict["seed_resize_from_w"] = -1;
        imgDict["sampler_name"] = "Euler a";
        imgDict["batch_size"] = 1;
        imgDict["n_iter"] = 1;
        imgDict["cfg_scale"] = 7;
        imgDict["sampler_index"] = "Euler a";
        //todo: use override settings to set model
        //Python:
        //"override_settings": {
        //  "sd_model_checkpoint": "v1-5-pruned-emaonly.safetensors [6ce0161689]"
        //}

        var tileCoro = StartCoroutine (instance.GenerateImage(prompt, APITypes.img2img, imgDict));
        yield return tileCoro;

        if (instance.stableDiffusionImage == null)
        {
            Debug.LogError("No texture specified");
            yield break;
        }

        if (skyboxMaterial == null)
        {
            Debug.LogError("No material specified");
            yield break;
        }

        skyboxMaterial.SetTexture("_Panorama", instance.stableDiffusionImage);
    }
}
