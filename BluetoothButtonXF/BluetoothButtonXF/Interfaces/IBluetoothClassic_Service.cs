using BluetoothButtonXF.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothButtonXF.Interfaces
{
    public interface IBluetoothClassic_Service
    {
        event EventHandler OnServicesDiscovered;
        event EventHandler OnNotificationReceived;
        event EventHandler OnDeviceDisconnected;
        event EventHandler OnConnectionFailed;

        Task<bool> IsBluetoothAvailable();

        List<BTDevice> GetPairedDevices();

        void DisconnectDevice();

        void ClearDeviceConnections();
        void ConnectDevice(string identifier);
        Task<bool> WriteData(byte[] data);

        void ListenForConnection();

        void ConfigureService();
    }
}