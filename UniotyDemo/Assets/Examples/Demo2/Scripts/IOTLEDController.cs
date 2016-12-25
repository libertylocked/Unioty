using System.Collections;
using System.Collections.Generic;
using Unioty.Controls;
using UnityEngine;

public class IOTLEDController : MonoBehaviour
{
    UniotyMasterController uniotyMaster;
    DeviceControl ledVirtualControl;
    GameObject light1, light2, light3, light4;

    public byte Host_ControlID_LED = 0x01;

    void Start()
    {
        light1 = GameObject.Find("Light 1");
        light2 = GameObject.Find("Light 2");
        light3 = GameObject.Find("Light 3");
        light4 = GameObject.Find("Light 4");

        uniotyMaster = FindObjectOfType<UniotyMasterController>();
        ledVirtualControl = uniotyMaster.GetVirtualControl(Host_ControlID_LED);
    }

    public void CheckLightsAndUpdateLED()
    {
        // Turn on LED if all lights are enabled
        if (light1.activeSelf && light2.activeSelf &&
            light3.activeSelf && light4.activeSelf)
        {
            ledVirtualControl.Payload = Payload.FromObject(1);
        }
        else
        {
            ledVirtualControl.Payload = Payload.FromObject(0);
        }
    }
}
