using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.Bluetooth.BluetoothClass;
using System.Threading.Tasks;

namespace WearOSC.Services
{
    public class OSCMainService
    {
      
        public static OSCService osc = new OSCService();

        public OSCMainService()
        {
            //osc.MessageReceived += MessageAdd;
            //bleService.HeartBeatRecieved += HeartBeat;
            //bleService.StepsDataRecieved += Steps;

            //deviceSensors.RotationReceived += RotationSend;
            //band.SleepStatusChanged += SleepChange;


        }


     

       public async  static void HeartBeat(int _beat)
        {
           
        
             
                   if(_beat!=0)
                    await osc.SendFloatAsync("/avatar/parameters/HeartBeat", ClampAndNormalize(0,255,_beat));




        }


   
       static float ClampAndNormalize(int min, int max, float value)
        {

            var newres = value - min;
            if (max < min) max = min + 1;
            float result = Math.Clamp((float)newres, 0, max - min);
            result = result / (max - min);
            return result;

        }
 

    }
}