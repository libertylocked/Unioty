using UnityEngine;
using Unioty;

public class BallJumpController : MonoBehaviour
{
    UniotyMasterController uniotyMaster;
    Rigidbody rb;

    public byte DeviceID = 0x01;
    public byte ControlID_Button = 0x01;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        uniotyMaster = FindObjectOfType<UniotyMasterController>();
        uniotyMaster.GetDeviceControl(DeviceID, ControlID_Button).DataChanged += OnButtonDataReceived;
    }

    void Update ()
    {

    }

    void OnDestroy()
    {
        // Unsubscribe to input event
        // This does not remove the control from master's control map
        uniotyMaster.GetDeviceControl(DeviceID, ControlID_Button).DataChanged -= OnButtonDataReceived;
    }

    void OnButtonDataReceived(object sender, DataChangedEventArgs e)
    {
        // Move the ball upwards if the control state is 0x01 (pressed)
        // This event is not raised every frame - only when state changes
        if ((byte)e.Payload.Data == 1)
        {
            rb.AddForce(Vector3.up * 5.0f, ForceMode.VelocityChange);
        }
    }
}
