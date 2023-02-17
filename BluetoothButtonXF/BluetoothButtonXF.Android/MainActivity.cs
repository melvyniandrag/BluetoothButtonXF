using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Widget;
using Xamarin.Forms;

namespace BluetoothButtonXF.Droid
{
    [Activity(Label = "BluetoothButtonXF", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.UiMode )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (requestCode == BluetoothClassic_Service_Droid.BLUETOOTH_PERMISSION_CODE)
            {
                if (grantResults.Length > 0 && grantResults[0] == 0)
                {
                    MessagingCenter.Send(EventArgs.Empty, App.BLUETOOTH_PERMISSION_IS_GRANTED);
                }
                else
                {
                    Toast.MakeText(this, "BT Permission DENIED", ToastLength.Short).Show();
                }
            }
        }
    }
}