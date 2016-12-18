using UnityEngine;
using Unioty;
using Unioty.Controls;

public class BallController : MonoBehaviour
{
    UniotyMasterController uniotyMaster;
    Rigidbody rb;
    DeviceControl virtualControl; // this virtual control is the height control
    float lastHeight = 0f;

    public byte DeviceID = 0x01;
    public byte ControlID_Button = 0x01;
    public byte ControlID_MagSwitch = 0x02;
    public byte Host_ControlID_Height = 0x00;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        uniotyMaster = FindObjectOfType<UniotyMasterController>();
        uniotyMaster.GetDeviceControl(DeviceID, ControlID_Button).DataChanged += OnButtonDataReceived;
        uniotyMaster.GetDeviceControl(DeviceID, ControlID_MagSwitch).DataChanged += OnMagSwitchDataReceived;
        virtualControl = uniotyMaster.GetVirtualControl(Host_ControlID_Height);
    }

    void Update ()
    {
        var height = rb.position.y;
        if (lastHeight < 5f && height >= 5f)
        {
            virtualControl.Payload = Payload.FromObject(1);
        }
        else if (lastHeight >= 5f && height < 5f)
        {
            virtualControl.Payload = Payload.FromObject(0);
        }
        lastHeight = height;
    }

    void OnDestroy()
    {
        // Unsubscribe to input event
        // This does not remove the control from master's control map
        uniotyMaster.GetDeviceControl(DeviceID, ControlID_Button).DataChanged -= OnButtonDataReceived;
        uniotyMaster.GetDeviceControl(DeviceID, ControlID_MagSwitch).DataChanged -= OnMagSwitchDataReceived;
    }

    void OnButtonDataReceived(object sender, DataChangedEventArgs e)
    {
        // Move the ball upwards if the control state is 0x01 (pressed)
        // This event is not raised every frame - only when state changes
        if ((byte)e.Payload.Data == 1)
        {
            rb.velocity = (Vector3.up * 5.0f);
        }
    }

    void OnMagSwitchDataReceived(object sender, DataChangedEventArgs e)
    {
        // Like the button event, this is also only raised when the state changes
        float scale = 1.0f;
        if ((byte)e.Payload.Data == 1)
        {
            scale = 2.0f;
        }
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
