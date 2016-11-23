using UnityEngine;
using Unioty;

public class BallJumpController : MonoBehaviour
{
    UniotyMaster uniotyMaster;
    Rigidbody rb;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        uniotyMaster = FindObjectOfType<UniotyMaster>();
        uniotyMaster.GetDeviceControl(0x01, 0x01).InputReceived += OnInputReceived;
    }

    void Update ()
    {
    
    }

    void OnInputReceived(object sender, InputReceivedEventArgs e)
    {
        if (e.State == 0x01)
        {
            rb.velocity = (Vector3.up * 10.0f);
        }
    }
}
