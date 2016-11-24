using UnityEngine;
using Unioty;

public class BallJumpController : MonoBehaviour
{
    UniotyMaster uniotyMaster;
    Rigidbody rb;
    bool buttonDownPrev = false;
    bool buttonDown = false;

    public byte DeviceID = 0x01;
    public byte ControlID = 0x01;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        uniotyMaster = FindObjectOfType<UniotyMaster>();
        uniotyMaster.GetDeviceControl(DeviceID, ControlID).InputReceived += OnInputReceived;
    }

    void Update ()
    {
        // Jump on rising edge of the button state (0 -> 1)
        if (buttonDown && !buttonDownPrev)
        {
            rb.velocity = (Vector3.up * 5.0f);
        }
        buttonDownPrev = buttonDown;
    }

    void OnDestroy()
    {
        // Unsubscribe to input event
        // This does not remove the control from master's control map
        uniotyMaster.GetDeviceControl(DeviceID, ControlID).InputReceived -= OnInputReceived;
    }

    void OnInputReceived(object sender, InputReceivedEventArgs e)
    {
        // Move the ball upwards if the control state is 0x01 (pressed)
        // This event may not be raised every frame - need to save the state
        buttonDown = e.State == 0x01;
    }
}
