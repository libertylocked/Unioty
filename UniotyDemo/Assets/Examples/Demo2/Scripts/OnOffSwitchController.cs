using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffSwitchController : InteractableController
{
    public GameObject SwitchTargetInstance;

    IOTLEDController ledController;

    void Start()
    {
        ledController = GameObject.Find("IOT LED Control").GetComponent<IOTLEDController>();
    }

    void Update()
    {

    }

    public override void StartInteraction()
    {
        SwitchTargetInstance.SetActive(!SwitchTargetInstance.activeSelf);

        // Notify our LED controller
        if (ledController != null)
        {
            ledController.CheckLightsAndUpdateLED();
        }
    }
}
