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
        private bool BluetoothPermissionGranted = false;

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
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            MessagingCenter.Subscribe<EventArgs>(this, App.BLUETOOTH_PERMISSION_IS_GRANTED, StartBluetoothActivities);
            MessagingCenter.Subscribe<EventArgs>(this, App.BLUETOOTH_CONNECTION_SUCCESSFUL, GoToConnectedPage);
            MessagingCenter.Subscribe<EventArgs>(this, App.BLUETOOTH_CONNECTION_FROM_REMOTE_DEVICE_SUCCESSFUL, GoToConnectedPageForListener);
            MessagingCenter.Subscribe<EventArgs>(this, App.BLUETOOTH_CONNECTION_FROM_REMOTE_DEVICE_SUCCESSFUL, GoToConnectedPageForListener);
            MessagingCenter.Subscribe<EventArgs>(this, App.THEME_CHANGED, updateColors);
            App.BluetoothClassicService.ConfigureService();
            BindingContext = this;
        }

        private void StartBluetoothActivities(EventArgs args)
        {
            BluetoothPermissionGranted = true;
            PairedDeviceList.ItemsSource = App.BluetoothClassicService.GetPairedDevices(); // list connectable devices
            updateColorsForDeviceList();
            Task.Run(() => App.BluetoothClassicService.ListenForConnection());             // and listen for incoming connections
        }



        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void GoToConnectedPage(EventArgs args)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PushAsync(new ButtonPage());
            });
        }

        private void GoToConnectedPageForListener(EventArgs args)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PushAsync(new ButtonPage(ButtonPage.ButtonStatus.PRESSED));
            });
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
                Task.Run(() => {
                    App.BluetoothClassicService.ConnectDevice(_selectedDevice.DeviceMAC);
                });
            }
        }

        public void SelectedDevice_Changed(object sender, SelectionChangedEventArgs args)
        {
            var selection = args.CurrentSelection;
            if ((selection == null) || (selection.Count == 0))
            {
                return;
            }
            else
            {
                foreach(BTDevice device in PairedDeviceList.ItemsSource)
                {
                    if(device == (BTDevice)selection[0])
                    {
                        device.IsSelected = true;
                    }
                    else
                    {
                        device.IsSelected = false;
                    }
                    device.OnPropertyChanged();
                }
                updateColorsForDeviceList();
            }
        }

        private void updateColors( EventArgs args)
        {
            updateColorsForDeviceList();
        }

        private void updateColorsForDeviceList()
        {
            OSAppTheme theme = Application.Current.RequestedTheme;

            foreach (BTDevice device in PairedDeviceList.ItemsSource)
            {
                if (device.IsSelected)
                {
                    if (theme == OSAppTheme.Dark)
                    {
                        device.TextColor = LayoutConstants.LIST_SELECTED_TEXT_DARK;
                        device.BackgroundColor = LayoutConstants.LIST_SELECTED_BG_DARK;
                    }
                    else
                    {
                        device.TextColor = LayoutConstants.LIST_SELECTED_TEXT_LIGHT;
                        device.BackgroundColor = LayoutConstants.LIST_SELECTED_BG_LIGHT;
                    }
                }
                else
                {
                    if (theme == OSAppTheme.Dark)
                    {
                        device.TextColor = LayoutConstants.LIST_UNSELECTED_TEXT_DARK;
                        device.BackgroundColor = LayoutConstants.LIST_UNSELECTED_BG_DARK;
                    }
                    else
                    {
                        device.TextColor = LayoutConstants.LIST_UNSELECTED_TEXT_LIGHT;
                        device.BackgroundColor = LayoutConstants.LIST_UNSELECTED_BG_LIGHT;
                    }
                }
                device.OnPropertyChanged();
            }
        }

    }
}
