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
using Android.Content;

namespace WearOSC
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : WearableActivity
    {
        public  const string hr_intent = "hr_update";
        TextView hrtext;
        EditText ip;
        Button Go;
        Button Stop;
        HRService HRService ;
        HRTextReceiver HRrecv;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_main);
            HRService = new HRService(this.ApplicationContext);
            ip = FindViewById<EditText>(Resource.Id.text);
            hrtext = FindViewById<TextView>(Resource.Id.text2);
            Go = FindViewById<Button>(Resource.Id.go);
            Stop= FindViewById<Button>(Resource.Id.stopit);
            Stop.Click += StopClick;
            Go.Click += GoClick;
            HRrecv = new HRTextReceiver(hrtext);
            var intentFilter = new IntentFilter(hr_intent);

            RegisterReceiver(HRrecv, intentFilter);
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
    
        }
    }
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class HRTextReceiver : BroadcastReceiver
    {
        public TextView hrtext;
        public HRTextReceiver()
        {

        }
        public HRTextReceiver(TextView text)
        {
            hrtext = text;
        }
        public override void OnReceive(Context context, Intent intent)
        {

            hrtext.Text = intent?.GetStringExtra("HR");
        }
    }
}


