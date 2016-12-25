using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RGB
{
    R,
    G,
    B,
}

public class RGBLEDColorButtonController : InteractableController
{
    public GameObject TargetLightInstance;
    public GameObject TextInstance;
    public RGB RGB;
    public bool Add;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void StartInteraction()
    {
        var rgbController = TargetLightInstance.GetComponent<RGBLEDController>();
        if (RGB == RGB.R)
        {
            if (Add) rgbController.ChangeRGB(0.1f, 0, 0);
            else rgbController.ChangeRGB(-0.1f, 0, 0);
            TextInstance.GetComponent<TextMesh>().text = rgbController.GetR().ToString();
        }
        if (RGB == RGB.G)
        {
            if (Add) rgbController.ChangeRGB(0, 0.1f, 0);
            else rgbController.ChangeRGB(0, -0.1f, 0);
            TextInstance.GetComponent<TextMesh>().text = rgbController.GetG().ToString();
        }
        if (RGB == RGB.B)
        {
            if (Add) rgbController.ChangeRGB(0, 0, 0.1f);
            else rgbController.ChangeRGB(0, 0, -0.1f);
            TextInstance.GetComponent<TextMesh>().text = rgbController.GetB().ToString();
        }
    }
}
