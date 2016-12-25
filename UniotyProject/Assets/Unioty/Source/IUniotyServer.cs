namespace Unioty
{
    interface IUniotyServer
    {
        int Port { get; }
        void Start();
        void Stop();
    }
}
