/*
 * Interactor Controller Script
 * Must be attached to a child object under Controller (left/right) object
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InteractorController : MonoBehaviour
{
    // game objects that can be interacted should have this tag
    const string TAG_INTERACTABLE = "Interactable";

    public EVRButtonId InteractButton = EVRButtonId.k_EButton_SteamVR_Trigger;

    SteamVR_TrackedObject trackedObj;

    void Start()
    {
        trackedObj = GetComponentInParent<SteamVR_TrackedObject>();
    }

    void Update()
    {

    }

    void OnTriggerStay(Collider other)
    {
        var interactable = other.gameObject.GetComponentInChildren<InteractableController>();

        var inputDevice = SteamVR_Controller.Input((int)trackedObj.index);
        if (inputDevice.GetPressDown(InteractButton) && interactable != null)
        {
            interactable.StartInteraction();
        }
    }
}
