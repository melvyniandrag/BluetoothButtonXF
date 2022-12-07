using BluetoothButtonXF.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BluetoothButtonXF
{
    public partial class MainPage : ContentPage
    {
        private double _width;
        private double _height;

        private BTDevice _selectedDevice;
        public BTDevice SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
            }
        }

        public MainPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<EventArgs>(this, App.BLUETOOTH_PERMISSION_IS_GRANTED, ListDevices);
            MessagingCenter.Subscribe<EventArgs>(this, App.BLUETOOTH_CONNECTION_SUCCESSFUL, GoToConnectedPage);
            App.BluetoothClassicService.ConfigureService();
            Task.Run(() => App.BluetoothClassicService.ListenForConnection());
            BindingContext = this;
        }

        private void ListDevices(EventArgs args)
        {
            PairedDeviceList.ItemsSource = App.BluetoothClassicService.GetPairedDevices();
        }

        private async void GoToConnectedPage(EventArgs args)
        {
            await Navigation.PushAsync(new ButtonPage());
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); //must be called

            if (_width != width || _height != height)
            {
                _width = width;
                _height = height;
                UpdateLayout();
            }
        }

        private void UpdateLayout()
        {
            if(_height > LayoutConstants.PC_SCREEN_HEIGHT)
            {
                SetTallScreenLayout();
            }
            else if(_width > LayoutConstants.PC_SCREEN_WIDTH)
            {
                SetWideScreenLayout();
            }
            else if( _height > _width)
            {
                SetPortraitLayout();
            }
            else // _width >= _height
            {
                SetLandscapeLayout();
            }
        }

        private void SetTallScreenLayout()
        {
            SetPortraitLayout();
        }

        private void SetWideScreenLayout()
        {
            SetLandscapeLayout();
        }

        private void SetPortraitLayout()
        {
            MainGridHeader.SetValue(Grid.RowProperty, 0);
            MainGridHeader.SetValue(Grid.RowSpanProperty, 1);
            MainGridHeader.SetValue(Grid.ColumnProperty, 0);
            MainGridHeader.SetValue(Grid.ColumnSpanProperty, 2);

            PairedDeviceListFrame.SetValue(Grid.RowProperty, 1);
            PairedDeviceListFrame.SetValue(Grid.RowSpanProperty, 1);
            PairedDeviceListFrame.SetValue(Grid.ColumnProperty, 0);
            PairedDeviceListFrame.SetValue(Grid.ColumnSpanProperty, 2);

            BluetoothTypeSelector.SetValue(Grid.RowProperty, 2);
            BluetoothTypeSelector.SetValue(Grid.RowSpanProperty, 1);
            BluetoothTypeSelector.SetValue(Grid.ColumnProperty, 0);
            BluetoothTypeSelector.SetValue(Grid.ColumnSpanProperty, 2);

            ConnectButton.SetValue(Grid.RowProperty, 3);
            ConnectButton.SetValue(Grid.RowSpanProperty, 1);
            ConnectButton.SetValue(Grid.ColumnProperty, 0);
            ConnectButton.SetValue(Grid.ColumnSpanProperty, 2);
        }

        private void SetLandscapeLayout()
        {
            MainGridHeader.SetValue(Grid.RowProperty, 0);
            MainGridHeader.SetValue(Grid.RowSpanProperty, 2);
            MainGridHeader.SetValue(Grid.ColumnProperty, 0);
            MainGridHeader.SetValue(Grid.ColumnSpanProperty, 1);

            PairedDeviceListFrame.SetValue(Grid.RowProperty, 1);
            PairedDeviceListFrame.SetValue(Grid.RowSpanProperty, 3);
            PairedDeviceListFrame.SetValue(Grid.ColumnProperty, 0);
            PairedDeviceListFrame.SetValue(Grid.ColumnSpanProperty, 1);


            BluetoothTypeSelector.SetValue(Grid.RowProperty, 2);
            BluetoothTypeSelector.SetValue(Grid.RowSpanProperty, 1);
            BluetoothTypeSelector.SetValue(Grid.ColumnProperty, 1);
            BluetoothTypeSelector.SetValue(Grid.ColumnSpanProperty, 1);

            ConnectButton.SetValue(Grid.RowProperty, 3);
            ConnectButton.SetValue(Grid.RowSpanProperty, 1);
            ConnectButton.SetValue(Grid.ColumnProperty, 1);
            ConnectButton.SetValue(Grid.ColumnSpanProperty, 1);
        }

        private void ConnectButton_Clicked(object sender, EventArgs args)
        {
            if(_selectedDevice != null)
            {
                App.BluetoothClassicService.ConnectDevice(_selectedDevice.DeviceMAC);
            }
        }
        
    }
}
