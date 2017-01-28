using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unioty;

public class BallMoveController : MonoBehaviour
{
    UniotyMasterController uniotyMaster;
    Rigidbody rb;
    float x = 0.5f, y = 0.5f;

    public byte DeviceID = 0x01;
    public byte ControlID_Joy_X = 0x03;
    public byte ControlID_Joy_Y = 0x04;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        uniotyMaster = FindObjectOfType<UniotyMasterController>();
        uniotyMaster.GetDeviceControl(DeviceID, ControlID_Joy_X).DataChanged += OnJoyXDataReceived;
        uniotyMaster.GetDeviceControl(DeviceID, ControlID_Joy_Y).DataChanged += OnJoyYDataReceived;
    }

    void FixedUpdate()
    {
        float velX = (x - 0.5f) * 2 * 0.1f;
        float velZ = (y - 0.5f) * 2 * 0.1f;
        rb.AddForce(Vector3.right * velX, ForceMode.VelocityChange);
        rb.AddForce(Vector3.forward * velZ, ForceMode.VelocityChange);
    }

    void OnDestroy()
    {
        // Unsubscribe to input event
        // This does not remove the control from master's control map
        uniotyMaster.GetDeviceControl(DeviceID, ControlID_Joy_X).DataChanged -= OnJoyXDataReceived;
        uniotyMaster.GetDeviceControl(DeviceID, ControlID_Joy_Y).DataChanged -= OnJoyYDataReceived;
    }

    void OnJoyXDataReceived(object sender, DataChangedEventArgs e)
    {
        x = (float)e.Payload.Data;
    }

    void OnJoyYDataReceived(object sender, DataChangedEventArgs e)
    {
        y = (float)e.Payload.Data;
    }
}
