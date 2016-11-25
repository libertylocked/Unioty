using UnityEngine;
using Unioty;

public class BallController : MonoBehaviour
{
    UniotyMasterScript uniotyMaster;
    Rigidbody rb;

    bool buttonDownPrev = false;
    bool buttonDown = false;
    float scale = 1.0f;

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
        // Jump on rising edge of the button state (0 -> 1)
        if (buttonDown && !buttonDownPrev)
        {
            rb.velocity = (Vector3.up * 5.0f);
        }
        buttonDownPrev = buttonDown;

        // Make ball larger if mag switch is high
        transform.localScale = new Vector3(scale, scale, scale);
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
        buttonDown = ((byte)e.Payload == 1);
    }

    void OnMagSwitchDataReceived(object sender, DataReceivedEventArgs e)
    {
        if ((byte)e.Payload == 1)
        {
            scale = 2.0f;
        }
        else
        {
            scale = 1.0f;
        }
    }
}
