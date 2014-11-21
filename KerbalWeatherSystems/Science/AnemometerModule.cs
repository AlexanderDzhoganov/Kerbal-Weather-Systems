using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using Weather;

namespace Science
{
    public class AnemometerModule : PartModule
    {
        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "WindSpeed: ", guiFormat = "F2", isPersistant = true)]
        float windSpeed;

        double animationPlaySpeed;
        bool isDisplayOn;

        public override void OnUpdate()
        {
            getWindSpeed(Wind.windSpeed);
            base.OnUpdate();
        }
        
        [KSPEvent(active = true, guiActive = true, guiActiveEditor = false, guiName = "Log Wind Speed Data")]
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
