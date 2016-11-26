namespace Unioty
{
    /// <summary>
    /// A callback function that is invoked after data from a Unioty device is received and parsed
    /// </summary>
    /// <param name="devID">Device ID</param>
    /// <param name="ctrlID">Control ID</param>
    /// <param name="payload">The payload data</param>
    public delegate void DataReceivedCallback(byte devID, byte ctrlID, Payload payload);
}
