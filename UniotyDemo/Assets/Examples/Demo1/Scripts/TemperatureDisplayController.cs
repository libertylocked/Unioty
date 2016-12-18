using UnityEngine;
using Unioty;

public class TemperatureDisplayController : MonoBehaviour
{
    UniotyMasterController uniotyMaster;
    TextMesh textMesh;

    public byte DeviceID = 0x01;
    public byte ControlID_HDC1000 = 0x03;

    void Start()
    {
        uniotyMaster = FindObjectOfType<UniotyMasterController>();
        uniotyMaster.GetDeviceControl(DeviceID, ControlID_HDC1000).DataChanged += OnHDCDataReceived;
        textMesh = GetComponent<TextMesh>();
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        uniotyMaster.GetDeviceControl(DeviceID, ControlID_HDC1000).DataChanged -= OnHDCDataReceived;
    }


    void OnHDCDataReceived(object sender, DataChangedEventArgs e)
    {
        float temperature = (float)e.Payload.Data;
        textMesh.text = string.Format("{0} C", temperature);
    }
}
