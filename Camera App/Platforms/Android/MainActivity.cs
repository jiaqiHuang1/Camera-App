using Android.App;
using Android.Content.PM;
using Android.OS;
using Android;
using System.Runtime.Versioning;

namespace Camera_App
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // 检查并请求蓝牙权限（适用于 Android 6.0 及以上）
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                RequestBluetoothPermissions();
            }
        }

        // 请求蓝牙和位置权限的方法
        [SupportedOSPlatform("android31.0")]  // 仅适用于 Android 12（API 31）及以上版本
        private void RequestBluetoothPermissions()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                // 针对 Android 12 及以上的设备，使用新的权限模型
                RequestPermissions(new string[]
                {
                    Manifest.Permission.BluetoothScan,
                    Manifest.Permission.BluetoothConnect,
                    Manifest.Permission.AccessFineLocation
                }, 0);
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.M) // 针对 Android 6.0 到 11 版本的设备
            {
                RequestPermissions(new string[]
                {
                    Manifest.Permission.Bluetooth,
                    Manifest.Permission.BluetoothAdmin,
                    Manifest.Permission.AccessFineLocation
                }, 0);
            }
        }
    }
}
