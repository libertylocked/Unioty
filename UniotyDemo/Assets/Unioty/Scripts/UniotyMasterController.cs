using UnityEngine;
using Unioty;

public class UniotyMasterController : MonoBehaviour
{
    public int Port = 25556;

    UniotyServerUpdater serverUpdater;

    #region Public methods
    /// <summary>
    /// Gets the device control in the control map. If it's not in the map already, it'll be created.
    /// </summary>
    /// <param name="devID"></param>
    /// <param name="ctrlID"></param>
    /// <returns></returns>
    public DeviceControl GetDeviceControl(byte devID, byte ctrlID)
    {
        return serverUpdater.GetDeviceControl(devID, ctrlID);
    }
    #endregion

    void Awake()
    {
        serverUpdater = new UniotyServerUpdater(Port, Debug.LogFormat);
        serverUpdater.Start();
    }

    void Update()
    {
        serverUpdater.Update();
    }

    void OnDestroy()
    {
        serverUpdater.Stop();
    }
}
