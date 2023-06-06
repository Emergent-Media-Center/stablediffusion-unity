using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct StableDiffusionTextToImageQuery
{
    [SerializeField]
    string prompt; 
    int steps;
    int cfg_scale;
    string sampler_index;
    string width;
    string height;
}
