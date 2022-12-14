using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BluetoothButtonXF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ButtonPage : ContentPage
    {

        private Color _buttonStatusColor = Color.Green;
        public Color ButtonStatusColor
        {
            get => _buttonStatusColor;
            set
            {
                _buttonStatusColor = value;
                OnPropertyChanged(nameof(ButtonStatusColor));
            }

        }
        public ButtonPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<EventArgs>(this, App.BLUETOOTH_CONNECTION_LOST, PopPage);
            BindingContext = this;
        }

        private void DisconnectButton_Clicked(object sender, EventArgs e)
        {
            App.BluetoothClassicService.DisconnectDevice();
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = await Navigation.PopAsync();
            });
        }

        protected override bool OnBackButtonPressed()
        {
            App.BluetoothClassicService.DisconnectDevice();
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = await Navigation.PopAsync();
            });
            return true;
        }

        private void PopPage(EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _ = await Navigation.PopAsync();
            });
        }

    }
}