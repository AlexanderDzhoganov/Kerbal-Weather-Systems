//note: Circumference of anemometer is 2.75m
//1m/s wind would cause a 0.363636m/s rotation which means a play speed of 0.36%
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using Weather;

namespace Science
{
    public class ModuleAnemometer : PartModule
    {

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "WindSpeed: ", guiFormat = "F2", isPersistant = true)]
        public string windSpeedString = "";

        float windSpeed;
        double animationPlaySpeed;
        bool isDisplayOn;

        public override void OnStart(StartState state)
        {
            //Debug.Log("OnStart");
            windSpeedString = windSpeed.ToString();
        }

        public override void OnUpdate()
        {
            //Debug.Log("Update Called");
            windSpeed = getWindSpeed(Wind.windSpeed);
            if(isDisplayOn == true)
            {
                windSpeedString = (windSpeed.ToString() + " m/s");
            }
            else
            {
                windSpeedString = "Display is off!";
            }
 	        base.OnUpdate();
        }
        
        
        [KSPEvent(active = true, guiActive = true, guiActiveEditor = false, guiName = "Log Wind Data")]
        public void doScience()
        {

            Debug.Log("Science was done!");

        }

        [KSPEvent(active = true, guiActive = true, guiActiveEditor = false, guiName = "Toggle Display")]
        public void ToggleDisplay()
        {
            isDisplayOn = !isDisplayOn;
        }

        float getWindSpeed(float windSpeed)
        {

            windSpeed = Wind.windSpeed;

            return windSpeed;
        }

    }
}
