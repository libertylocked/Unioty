using UnityEngine;
using Unioty;

public class BallJumpController : MonoBehaviour
{
    UniotyMaster uniotyMaster;
    Rigidbody rb;
    bool flyUp = false;

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
        if (flyUp)
        {
            rb.velocity = (Vector3.up * 5.0f);
        }
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
        flyUp = e.State == 0x01;
    }
}
