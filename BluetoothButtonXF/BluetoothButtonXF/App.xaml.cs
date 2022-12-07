using BluetoothButtonXF.Interfaces;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BluetoothButtonXF
{
    public partial class App : Application
    {
        public static IBluetoothClassic_Service BluetoothClassicService;
        public const string BLUETOOTH_PERMISSION_IS_GRANTED = "User has granted bluetooth permission";
        public const string BLUETOOTH_CONNECTION_SUCCESSFUL = "Bluetooth Connection Successful";

        public App()
        {
            InitializeComponent();
            BluetoothClassicService = DependencyService.Get<IBluetoothClassic_Service>();

            MainPage = new NavigationPage( new MainPage() );
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
