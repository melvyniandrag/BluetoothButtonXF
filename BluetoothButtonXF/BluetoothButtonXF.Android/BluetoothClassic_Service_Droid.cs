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
using Javax.Security.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using static AndroidX.RecyclerView.Widget.RecyclerView;

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

        private int _lastHeartbeat = 0;
        private bool _receivedHeartbeat = false;

        private const int _HEARTBEAT_TIMER = 5;

        private enum BLUETOOTH_STATE
        {
            STATE_NONE,
            STATE_CONNECTED,
            STATE_CONNECTING,
            STATE_LISTEN
        }

        private BLUETOOTH_STATE _state = BLUETOOTH_STATE.STATE_NONE;

        public static int BLUETOOTH_PERMISSION_CODE = 100;

        public event EventHandler OnButtonPushReceived;
        public event EventHandler OnHeartbeatReceived;
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
                _inputStream = null;
            }
            catch(Exception e)
            {

            }
            
            try
            {
                _outputStream?.Close();
                _outputStream = null;
            }
            catch (Exception e)
            {

            }

            try
            {
                _socket?.Close();
                _socket = null;
            }
            catch (Exception e)
            {

            }

            try
            {
                _serverSocket?.Close();
                _serverSocket = null;
            }
            catch (Exception e)
            {

            }

            try
            {
                _device = null;
            }
            catch (Exception e)
            {

            }
            _state = BLUETOOTH_STATE.STATE_NONE;
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
                //OnConnectionFailed(this, new EventArgs());
                return;
            }
            try
            {
                _serverSocket?.Close();
                _serverSocket = null;
            }
            catch (Exception e)
            {

            }
            _state = BLUETOOTH_STATE.STATE_CONNECTED;
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            _lastHeartbeat = secondsSinceEpoch;
            Task.Run(() => ReceiveDataLoop());
            Task.Run(() => HeartbeatLoop());
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
        public async void ListenForConnection()
        {
            _state = BLUETOOTH_STATE.STATE_LISTEN;
            while (true)
            {
                if (_state != BLUETOOTH_STATE.STATE_CONNECTED)
                {
                    try
                    {
                        BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
                        _serverSocket = adapter.ListenUsingRfcommWithServiceRecord(_NAME, _UUID);
                        _socket = _serverSocket?.Accept();
                    }
                    catch (System.Exception btException)
                    {
                        _state = BLUETOOTH_STATE.STATE_LISTEN;
                        System.Diagnostics.Debug.WriteLine("[BluetoothClassicService_Droid]::ListenForConnection() \n" + btException.Message);
                        continue;
                    }
                    _state = BLUETOOTH_STATE.STATE_CONNECTED;
                    _inputStream = _socket.InputStream;
                    _outputStream = _socket.OutputStream;
                    TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                    int secondsSinceEpoch = (int)t.TotalSeconds;
                    _lastHeartbeat = secondsSinceEpoch;
                    Task.Run(() => ReceiveDataLoop());
                    Task.Run(() => HeartbeatLoop());
                    MessagingCenter.Send(EventArgs.Empty, App.BLUETOOTH_CONNECTION_FROM_REMOTE_DEVICE_SUCCESSFUL);
                }
                await Task.Delay(1000);
            }
        }
        
        private async void HeartbeatLoop()
        {
            while (true)
            {
                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                int secondsSinceEpoch = (int)t.TotalSeconds;
                if (_receivedHeartbeat)
                {
                    _lastHeartbeat = secondsSinceEpoch;
                    _receivedHeartbeat = false;
                    SendHeartBeat();
                    await Task.Delay(1000);
                }
                else if((secondsSinceEpoch - _lastHeartbeat) > _HEARTBEAT_TIMER)
                {
                    DisconnectDevice(); // this should cause everything else to stop working.
                    break;
                }
                else
                {
                    // We didnt get a heartbeat and the timer didnt run out. Lets keep waiting.
                    await Task.Delay(1000);
                }
                
            }
        }
        
        public void WriteData(byte[] data)
        {
            try
            {
                _outputStream.Write(data);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private const int MESSAGE_SIZE = 1;
        
        private async void ReceiveDataLoop()
        {
            byte[] receivedBytes = new byte[MESSAGE_SIZE];
            while (true)
            {
                try
                {
                    bool success = await readStream(_inputStream, 1, receivedBytes);
                    if (!success)
                    {
                        DisconnectDevice();
                        MessagingCenter.Send(EventArgs.Empty, App.BLUETOOTH_CONNECTION_LOST);
                        break;
                    }
                    if (receivedBytes[0] == 0x00)
                    {
                        _receivedHeartbeat = true;
                    }
                    else if(receivedBytes[0] == 0x01)
                    {
                        MessagingCenter.Send(EventArgs.Empty, App.REMOTE_BUTTON_PRESS_RECEIVED);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("[BluetoothClassicService_Droid::ReceiveDataLoop()]: \n" + ex.Message);
                    return;
                }
            }
        }

        private async Task<bool> readStream(System.IO.Stream s, int count, byte[] dest)
        {
            if (s == null)
            {
                return false;
            }
            try
            {
                int offset = 0;
                int numRead = 0;
                if (count == 0)
                {
                    return true;
                }
                do
                {
                    numRead = await s.ReadAsync(dest, offset, count);
                    System.Diagnostics.Debug.WriteLine("Read " + numRead + " bytes int array offset " + offset + ". Expecting to read " + count + "bytes in total.");
                    offset += numRead;
                    count -= numRead;
                } while (count > 0);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error reading stream");
                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }
            return true;
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

        public void SendButtonPressEvent()
        {
            byte[] data = { 0x01 };
            WriteData(data);
        }

        public void SendHeartBeat()
        {
            byte[] data = { 0x00 };
            WriteData(data);
        }
    }
}