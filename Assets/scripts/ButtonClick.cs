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

        // call the dowload and wait for it to finish
        var coro = StartCoroutine(instance.GenerateImage(prompt));
		yield return coro;

		var tileCoro = StartCoroutine (instance.TileImage(prompt));
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
