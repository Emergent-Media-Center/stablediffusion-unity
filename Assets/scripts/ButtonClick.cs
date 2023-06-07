using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{

	[SerializeField] public Button yourButton;
	[SerializeField] public GameObject promptField;

	void Start()
	{
		Button btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
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

		// call the dowload and wait for it to finish
		var coro = StartCoroutine(instance.GenerateImage(prompt));
		yield return coro;
		GetComponent<Renderer>().material.mainTexture = instance.stableDiffusionImage;
	}
}
