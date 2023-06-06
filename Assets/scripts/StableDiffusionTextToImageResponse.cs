using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct StableDiffusionTextToImageResponse
{
    [SerializeField]
    public List<string> images;
}
