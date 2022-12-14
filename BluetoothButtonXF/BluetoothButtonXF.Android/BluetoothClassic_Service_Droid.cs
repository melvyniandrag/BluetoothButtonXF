using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using BluetoothButtonXF.Interfaces;
using BluetoothButtonXF.Util;
using Java.IO;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(BluetoothButtonXF.Droid.BluetoothClassic_Service_Droid))]
namespace BluetoothButtonXF.Droid
{
    public class BluetoothClassic_Service_Droid : IBluetoothClassic_Service
    {
        private const String _NAME = "BluetoothButtonXF";
        private const String _UUID_STRING = "00001111-0000-1000-8000-008805050500";
        private static UUID _UUID = UUID.FromString(_UUID_STRING);

        private BluetoothDevice _device;
        private BluetoothSocket _socket = null;
        private System.IO.Stream _inputStream;
        private System.IO.Stream _outputStream;
        private BluetoothServerSocket _serverSocket = null;

        private enum BLUETOOTH_STATE
        {
            STATE_NONE,
            STATE_CONNECTED,
            STATE_CONNECTING,
            STATE_LISTEN
        }

        private BLUETOOTH_STATE _state = BLUETOOTH_STATE.STATE_NONE;

        public static int BLUETOOTH_PERMISSION_CODE = 100;

        public event EventHandler OnServicesDiscovered;
        public event EventHandler OnNotificationReceived;
        public event EventHandler OnDeviceDisconnected;
        public event EventHandler OnConnectionFailed;

        public void ClearDeviceConnections()
        {
            throw new NotImplementedException();
        }

        public void DisconnectDevice()
        {
            try
            {
                _inputStream?.Close();
                _inputStream?.Dispose();
            }
            catch(Exception e)
            {

            }
            
            try
            {
                _outputStream?.Close();
                _outputStream?.Dispose();
            }
            catch (Exception e)
            {

            }

            try
            {
                _socket?.Close();
                _socket?.Dispose();
            }
            catch (Exception e)
            {

            }

            try
            {
                _serverSocket?.Close();
                _serverSocket?.Dispose();
            }
            catch (Exception e)
            {

            }

            try
            {
                _device?.Dispose();
            }
            catch (Exception e)
            {

            }
        }

        public void ConnectDevice(string identifier)
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            _device = adapter.GetRemoteDevice(identifier);
            if (_device == null) return;
            try
            {
                _socket = _device.CreateRfcommSocketToServiceRecord(_UUID);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error creating rfcomm socket.");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                //OnConnectionFailed(this, new EventArgs());
                return;
            }
            if (_socket == null)
            {
                System.Diagnostics.Debug.WriteLine("Error creating rfcomm socket. Socket was null.");
                //OnConnectionFailed(this, new EventArgs());
                return;
            }
            try
            {
                //StopScan();
                _socket.Connect();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error connecting to device: " + identifier + " .");
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                try
                {
                    _socket.Close();
                }
                catch (Exception exInner)
                {
                    System.Diagnostics.Debug.WriteLine("Error closing socket");
                    System.Diagnostics.Debug.WriteLine(exInner.Message);
                }
                //OnConnectionFailed(this, new EventArgs());
                return;
            }
            try
            {
                _inputStream = _socket.InputStream;
                _outputStream = _socket.OutputStream;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error setting up streams");
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                OnConnectionFailed(this, new EventArgs());
                return;
            }
            //Task.Run(() => ReceiveBTClassicMessageLoop());
            //OnServicesDiscovered(this, new ServicesDiscoveredArgs(device?.Name, device?.Address));
            MessagingCenter.Send(EventArgs.Empty, App.BLUETOOTH_CONNECTION_SUCCESSFUL);
        }

        public Task<bool> IsBluetoothAvailable()
        {
            throw new NotImplementedException();
        }

        [Obsolete]
        public List<BTDevice> GetPairedDevices()
        {
            // WARNING: There should be a BluetoothManager.GetAdapter() method, but I dont see it here in Xamarin.Android. 
            // Context context = Android.App.Application.Context;
            // BluetoothManager bluetoothManager = context.GetSystemService(Context.BluetoothService) as BluetoothManager;
            // BluetoothAdapter bluetoothAdapter = bluetoothManager.GetAdapter();
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            List<BTDevice> macList = new List<BTDevice>();
            foreach(BluetoothDevice device in adapter.BondedDevices)
            {
                macList.Add(new BTDevice() { DeviceName = device.Name, DeviceMAC = device.Address });
            }
            return macList;
        }

        [Obsolete]
        public void ListenForConnection()
        {
            try
            {
                BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
                _serverSocket = adapter.ListenUsingRfcommWithServiceRecord(_NAME, _UUID);
            }
            catch(System.Exception btException)
            {
                System.Diagnostics.Debug.WriteLine(btException.Message);
                _state =  BLUETOOTH_STATE.STATE_NONE;
                return;
            }
            _state = BLUETOOTH_STATE.STATE_LISTEN;

            while(_state != BLUETOOTH_STATE.STATE_CONNECTED)
            {
                try
                {
                    _socket = _serverSocket?.Accept();
                    System.Diagnostics.Debug.WriteLine("accepted a connection!");
                }
                catch(System.Exception btException)
                {
                    break;
                }


                _state = BLUETOOTH_STATE.STATE_CONNECTED;
                _inputStream = _socket.InputStream;
                _outputStream = _socket.OutputStream;
            }
        }

        public Task<bool> WriteData(byte[] data)
        {
            throw new NotImplementedException(); 
        }

        public void ConfigureService()
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
            {
                string[] ANDROID_12_BLUETOOTH_PERMISSIONS = new string[]
                {
                    Manifest.Permission.BluetoothScan,
                    Manifest.Permission.BluetoothConnect,
                    Manifest.Permission.BluetoothAdvertise
                };
                ActivityCompat.RequestPermissions(Platform.CurrentActivity, ANDROID_12_BLUETOOTH_PERMISSIONS, BLUETOOTH_PERMISSION_CODE);
            }
            else
            {
                string[] BELOW_ANDROID_12_BLUETOOTH_PERMISSIONS = new string[]
                {
                    Manifest.Permission.AccessCoarseLocation,
                    Manifest.Permission.AccessFineLocation,

                };
                ActivityCompat.RequestPermissions(Platform.CurrentActivity, BELOW_ANDROID_12_BLUETOOTH_PERMISSIONS, BLUETOOTH_PERMISSION_CODE);
            }
        }
    }
}