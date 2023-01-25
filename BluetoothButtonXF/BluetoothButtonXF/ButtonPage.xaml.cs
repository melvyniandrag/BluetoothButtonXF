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

        private Color POPPED_BUTTON_COLOR = Color.Green;
        private Color PRESSED_BUTTON_COLOR = Color.Red;

        private int POPPED_BUTTON_SIZE = 200;
        private int PRESSED_BUTTON_SIZE = 150;


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

        private int _buttonWidth = 200;
        private int _buttonHeight = 200;
        private int _buttonRadius = 100;

        public int ButtonWidth
        {
            get => _buttonWidth;
            set {
                _buttonWidth = value;
                OnPropertyChanged(nameof(ButtonWidth));
            }
        }

        public int ButtonHeight
        {
            get => _buttonHeight;
            set
            {
                _buttonHeight = value;
                OnPropertyChanged(nameof(ButtonHeight));
            }
        }

        public int ButtonCornerRadius
        {
            get => _buttonRadius;
            set
            {
                _buttonRadius = value;
                OnPropertyChanged(nameof(ButtonCornerRadius));
            }
        }

        public enum ButtonStatus { 
            POPPED,
            PRESSED
        }

        private ButtonStatus _buttonStatus;


        public ButtonPage()
        {
            InitializeComponent();
            _buttonStatus = ButtonStatus.POPPED;
            MessagingCenter.Subscribe<EventArgs>(this, App.BLUETOOTH_CONNECTION_LOST, PopPage);
            MessagingCenter.Subscribe<EventArgs>(this, App.REMOTE_BUTTON_PRESS_RECEIVED, HandleRemoteButtonPress);
            BindingContext = this;
            Task.Run(async () => {
                //await Task.Delay(DELAY_BEFORE_HEARTBEAT);
                App.BluetoothClassicService.SendHeartBeat();
            });

        }


        public ButtonPage(ButtonStatus status)
        {
            InitializeComponent();

            MessagingCenter.Subscribe<EventArgs>(this, App.BLUETOOTH_CONNECTION_LOST, PopPage);
            MessagingCenter.Subscribe<EventArgs>(this, App.REMOTE_BUTTON_PRESS_RECEIVED, HandleRemoteButtonPress);
            BindingContext = this;
            _buttonStatus = status;
            UpdateUI();
        }

        private void UpdateUI()
        {
            Device.BeginInvokeOnMainThread(() => { 
                switch (_buttonStatus)
                {
                    case ButtonStatus.PRESSED:
                        {
                            ButtonWidth = PRESSED_BUTTON_SIZE;
                            ButtonHeight = PRESSED_BUTTON_SIZE;
                            ButtonCornerRadius = PRESSED_BUTTON_SIZE / 2; 
                            ButtonStatusColor = PRESSED_BUTTON_COLOR;
                            break;
                        }
                    default:
                        {
                            ButtonWidth = POPPED_BUTTON_SIZE;
                            ButtonHeight = POPPED_BUTTON_SIZE;
                            ButtonCornerRadius = POPPED_BUTTON_SIZE / 2;
                            ButtonStatusColor = POPPED_BUTTON_COLOR;
                            break;
                        }
                }
            });
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

        private void HandleRemoteButtonPress(EventArgs e)
        {
            if (_buttonStatus.Equals(ButtonStatus.PRESSED)){
                _buttonStatus = ButtonStatus.POPPED;
                UpdateUI();
            }
        }

        private void GameButton_Clicked(object sender, EventArgs e)
        {
            if (_buttonStatus.Equals(ButtonStatus.POPPED))
            {
                App.BluetoothClassicService.SendButtonPressEvent();
                _buttonStatus = ButtonStatus.PRESSED;
                UpdateUI();
            }
        }

    }
}