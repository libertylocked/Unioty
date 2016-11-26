using UnityEngine;
using Unioty;

public class BallController : MonoBehaviour
{
    UniotyMasterScript uniotyMaster;
    Rigidbody rb;

    public byte DeviceID = 0x01;
    public byte ControlID_Button = 0x01;
    public byte ControlID_MagSwitch = 0x02;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        uniotyMaster = FindObjectOfType<UniotyMasterScript>();
        uniotyMaster.GetDeviceControl(DeviceID, ControlID_Button).DataReceived += OnButtonDataReceived;
        uniotyMaster.GetDeviceControl(DeviceID, ControlID_MagSwitch).DataReceived += OnMagSwitchDataReceived;
    }

    void Update ()
    {
        
    }

    void OnDestroy()
    {
        // Unsubscribe to input event
        // This does not remove the control from master's control map
        uniotyMaster.GetDeviceControl(DeviceID, ControlID_Button).DataReceived -= OnButtonDataReceived;
        uniotyMaster.GetDeviceControl(DeviceID, ControlID_MagSwitch).DataReceived -= OnMagSwitchDataReceived;
    }

    void OnButtonDataReceived(object sender, DataReceivedEventArgs e)
    {
        // Move the ball upwards if the control state is 0x01 (pressed)
        // This event is not raised every frame - only when state changes
        if ((byte)e.Payload.Data == 1)
        {
            rb.velocity = (Vector3.up * 5.0f);
        }
    }

    void OnMagSwitchDataReceived(object sender, DataReceivedEventArgs e)
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
