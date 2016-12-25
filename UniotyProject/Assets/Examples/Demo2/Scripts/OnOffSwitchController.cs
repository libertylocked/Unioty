using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffSwitchController : InteractableController
{
    public GameObject SwitchTargetInstance;

    void Start()
    {

    }

    void Update()
    {

    }

    public override void StartInteraction()
    {
        SwitchTargetInstance.SetActive(!SwitchTargetInstance.activeSelf);
    }
}
