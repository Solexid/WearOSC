using System;
using Android.Hardware;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Activity;
using Android.Runtime;
using Android.Content.PM;
using WearOSC.Services;
using Android;
using Android.Support.V4.App;

namespace WearOSC
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : WearableActivity,ISensorEventListener
    {
        EditText ip;
        Button Go;
        Button Stop;
        HRService HRService ;
        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            throw new NotImplementedException();
        }

        public void OnSensorChanged(SensorEvent e)
        {
            throw new NotImplementedException();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_main);
            HRService = new HRService(this.ApplicationContext);
            ip = FindViewById<EditText>(Resource.Id.text);
            Go = FindViewById<Button>(Resource.Id.go);
            Stop= FindViewById<Button>(Resource.Id.stopit);
            Stop.Click += StopClick;
            Go.Click += GoClick;
            ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.BodySensors, Manifest.Permission.ForegroundService, Manifest.Permission.WakeLock , Manifest.Permission.Internet }, 0);
         
            SetAmbientEnabled();
        }

        private void StopClick(object sender, EventArgs e)
        {
           
            HRService?.StopWorker();
        }

        private void GoClick(object sender, EventArgs e)
        {
            
            HRService.RestartWorker(ip.Text);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //global::Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}


