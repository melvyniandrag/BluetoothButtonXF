using BluetoothButtonXF.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(BluetoothButtonXF.UWP.BluetoothClassic_Service_UWP))]
namespace BluetoothButtonXF.UWP
{
    public class BluetoothClassic_Service_UWP : IBluetoothClassic_Service
    {
        public event EventHandler OnScanCompleted;
        public event EventHandler OnScanStarted;
        public event EventHandler OnDeviceDiscovered;
        public event EventHandler OnServicesDiscovered;
        public event EventHandler OnNotificationReceived;
        public event EventHandler OnDeviceDisconnected;
        public event EventHandler OnConnectionFailed;

        public void ClearDeviceConnections()
        {
            throw new NotImplementedException();
        }

        public void ConnectDevice(string identifier)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsBluetoothAvailable()
        {
            throw new NotImplementedException();
        }

        public void StartScan()
        {
            throw new NotImplementedException();
        }

        public void StopScan()
        {
            throw new NotImplementedException();
        }

        public Task<bool> WriteData(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
