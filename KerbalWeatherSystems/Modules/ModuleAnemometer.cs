using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using Weather;

namespace Modules
{
    public class ModuleAnemometer : PartModule
    {

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "WindSpeed: ", guiFormat = "F2", isPersistant = true)]
        public string windSpeedString = "";
        [KSPField]
        public double powerConsumption;

        static float windSpeed;
        bool isDisplayOn;

        public override void OnStart(StartState state)
        {
            //Debug.Log("OnStart");
            windSpeedString = windSpeed.ToString();
        }

        public override void OnUpdate()
        {
            //Debug.Log("Update Called");
            windSpeed = getWindSpeed(HeadMaster.windSpeed);
            if(isDisplayOn == true)
            {
                if (HeadMaster.inAtmosphere == true) { windSpeedString = ((windSpeed * 3.6).ToString("0.000") + " km/h"); }
                else { windSpeedString = "0.000 km/h"; }
                
            }
            else
            {
                windSpeedString = "Display is off!";
            }

            
 	        base.OnUpdate();
        }

        [KSPEvent(active = true, guiActive = true, guiActiveEditor = false, guiName = "Toggle Display")]
        public void ToggleDisplay()
        {
            isDisplayOn = !isDisplayOn;
        }
        /*
        [KSPEvent(active = true, guiActive = true, guiActiveEditor = false, guiName = "Log Wind Data")]
        public void doScience()
        {
            
            //AnemScienceModule
            Debug.Log("Science was done!");

        }        
        */
        

        float getWindSpeed(float windSpeed)
        {

            windSpeed = Wind.windSpeed;

            return windSpeed;
        }

    }
}
