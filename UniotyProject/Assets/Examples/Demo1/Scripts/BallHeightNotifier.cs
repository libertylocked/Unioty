using UnityEngine;
using Unioty;
using Unioty.Controls;

public class BallHeightNotifier : MonoBehaviour
{
    UniotyMasterController uniotyMaster;
    Rigidbody rb;
    DeviceControl heightVirtualControl;
    float lastHeight = 0f;

    public byte Host_ControlID_Height = 0x01;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        uniotyMaster = FindObjectOfType<UniotyMasterController>();
        heightVirtualControl = uniotyMaster.GetVirtualControl(Host_ControlID_Height);
    }

    void Update()
    {
        var height = rb.position.y;
        if (lastHeight < 5f && height >= 5f)
        {
            heightVirtualControl.Payload = Payload.FromObject(1);
        }
        else if (lastHeight >= 5f && height < 5f)
        {
            heightVirtualControl.Payload = Payload.FromObject(0);
        }
        lastHeight = height;
    }
}
