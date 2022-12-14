using BluetoothButtonXF.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothButtonXF.Interfaces
{
    public interface IBluetoothClassic_Service
    {
        event EventHandler OnButtonPushReceived;
        event EventHandler OnHeartbeatReceived;
        event EventHandler OnDeviceDisconnected;
        event EventHandler OnConnectionFailed;

        Task<bool> IsBluetoothAvailable();

        List<BTDevice> GetPairedDevices();

        void DisconnectDevice();

        void ClearDeviceConnections();
        void ConnectDevice(string identifier);
        void WriteData(byte[] data);

        void ListenForConnection();

        void ConfigureService();
    }
}