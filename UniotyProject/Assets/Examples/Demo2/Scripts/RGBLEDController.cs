using System.Collections;
using System.Collections.Generic;
using Unioty.Controls;
using UnityEngine;

public class RGBLEDController : MonoBehaviour
{
    UniotyMasterController uniotyMaster;
    DeviceControl virtualRGB;
    Light rgbLight;

    public byte Host_ControlID_RGB = 0x01;

    void Start()
    {
        rgbLight = GetComponent<Light>();
        uniotyMaster = FindObjectOfType<UniotyMasterController>();
        virtualRGB = uniotyMaster.GetVirtualControl(Host_ControlID_RGB);
    }

    public void ChangeRGB(float dr, float dg, float db)
    {
        var r = rgbLight.color.r;
        var g = rgbLight.color.g;
        var b = rgbLight.color.b;

        r = Mathf.Clamp(r + dr, 0, 1f);
        g = Mathf.Clamp(g + dg, 0, 1f);
        b = Mathf.Clamp(b + db, 0, 1f);

        rgbLight.color = new Color(r, g, b);

        // Notify our IOT device
        virtualRGB.Payload = Payload.FromObject(new byte[] {
            GetR(), GetG(), GetB(),
        });
    }

    public byte GetR()
    {
        return (byte)(Mathf.Clamp(rgbLight.color.r, 0, 1f) * 255);
    }

    public byte GetG()
    {
        return (byte)(Mathf.Clamp(rgbLight.color.g, 0, 1f) * 255);
    }

    public byte GetB()
    {
        return (byte)(Mathf.Clamp(rgbLight.color.b, 0, 1f) * 255);
    }
}
