using UnityEngine;
using Unioty;

public class BallController : MonoBehaviour
{
    ControllerHubListener inputListener;
    Rigidbody rb;

    void Start () {
        rb = GetComponent<Rigidbody>();
        inputListener = FindObjectOfType<ControllerHubListener>();
        inputListener.InputReceived += OnInputReceived;
    }

    void Update () {
    
    }

    void OnInputReceived(object sender, InputReceiveEventArgs e)
    {
        if (e.DeviceID == 0x01 && e.ControlID == 0x01)
        {
            if (e.State == 0x01)
            {
                rb.velocity = (Vector3.up * 10.0f);
            }
        }
    }
}
