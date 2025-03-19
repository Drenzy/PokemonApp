using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Android;

namespace PokemonApp
{
    // MainActivity serves as the entry point for your Android MAUI app.
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode)]
    public class MainActivity : MauiAppCompatActivity
    {
        // OnCreate is called when your activity is first created.
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Request storage permissions from the user when the app starts.
            RequestStoragePermission();
        }

        const int REQUEST_STORAGE_ID = 0;

        // Method to request storage permissions at runtime (required by Android >=6.0)
        void RequestStoragePermission()
        {
            // Check if WRITE_EXTERNAL_STORAGE permission is already granted
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != Permission.Granted)
            {
                // If permission isn't granted, explicitly request both WRITE and READ storage permissions.
                ActivityCompat.RequestPermissions(this,
                    new string[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage },
                    REQUEST_STORAGE_ID);
            }
        }
    }
}
