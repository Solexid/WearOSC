using Android.App;
using Android.Content;
using Android.OS;
using Android.Hardware;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.OS.PowerManager;

namespace WearOSC.Services
{
    [Service]
    public class HRService : Service,ISensorEventListener2
    {
        const string stop_it = "pls_die";
        Sensor hrSensor;
        SensorManager sensorManager;
        WakeLock wakeLock;
        public HRService()
        {

        }
        BReceiver receiver = new BReceiver();
        static Context thisContext;
        static string ip = "192.168.1.229";
        public HRService(Context cont)
        {
            thisContext = cont;

        }

        public override IBinder OnBind(Intent intent)
        {
           
            return null;
        }
        public override void OnCreate()
        {
            base.OnCreate();

            var intentFilter = new IntentFilter(stop_it);
            RegisterReceiver(receiver, intentFilter);
            sensorManager = (SensorManager)thisContext.GetSystemService(Context.SensorService);
            hrSensor = sensorManager.GetDefaultSensor(SensorType.HeartRate);
            sensorManager.RegisterListener(this, hrSensor, SensorDelay.Normal, 1000);
            wakeLock = (GetSystemService(Context.PowerService) as PowerManager).NewWakeLock
            (WakeLockFlags.Full, "WearOSC::BackgroundStreaming");

            wakeLock?.Acquire();
            OSCMainService.osc.SetIpAndRestart("192.168.1.229");

        }
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 228228;
        public void StartWorker()
        {
            
            var intent = new Intent(Android.App.Application.Context, typeof(HRService));
          
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                Android.App.Application.Context.StartForegroundService(intent);

            }
            else
            {
                Android.App.Application.Context.StartService(intent);
            }
        }
        public override bool StopService(Intent name)
        {
            wakeLock?.Release();
            return base.StopService(name);  
        }
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var stopIntent = new Intent(stop_it);
            
            var pendingIntentStopAction = PendingIntent.GetBroadcast(this, SERVICE_RUNNING_NOTIFICATION_ID, stopIntent, PendingIntentFlags.UpdateCurrent); 

            CreateNotificationChannel();
            string messageBody = "SendingData to host";
            var notification = new Notification.Builder(this, "228")
            .SetContentTitle("Osc worker service").AddAction(new Notification.Action(Resource.Drawable.close_button,"Stop OSC",pendingIntentStopAction))
            .SetContentText(messageBody)
            .SetSmallIcon(Resource.Drawable.abc_switch_track_mtrl_alpha)
            .SetOngoing(true)
            .Build();
            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
          
            return StartCommandResult.Sticky;
        }
        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel("228", "OscService", NotificationImportance.Default)
            {
                Description = "Osc worker service"
            };
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        public override void OnDestroy()
        {
            wakeLock?.Release();
            sensorManager.UnregisterListener(this, hrSensor);
            UnregisterReceiver(receiver);

          base.OnDestroy();
        }
        public void StopWorker()
        {
            var stopIntent = new Intent(stop_it);
          
           SendBroadcast(stopIntent);
            
            var intent = new Intent(Android.App.Application.Context, typeof(HRService));
            Android.App.Application.Context.StopService(intent);
         
        }

        public void RestartWorker(string _ip ="192.168.1.229")
        {
            ip = _ip;

             var intent = new Intent(Android.App.Application.Context, typeof(HRService));
            Android.App.Application.Context.StopService(intent);
            StartWorker();

        }
        void ISensorEventListener.OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
            
        }

        void ISensorEventListener2.OnFlushCompleted(Sensor sensor)
        {
           
        }

        void ISensorEventListener.OnSensorChanged(SensorEvent e)
        {
            var hrIntent = new Intent(MainActivity.hr_intent);
            hrIntent.PutExtra("HR", e?.Values?[0].ToString());
            SendBroadcast(hrIntent);
            OSCMainService.HeartBeat((Int32)e?.Values?[0]);
        }
    }
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class BReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
    }
}