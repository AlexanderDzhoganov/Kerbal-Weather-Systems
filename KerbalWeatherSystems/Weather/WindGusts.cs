using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Weather
{
    public class WindGusts
    {
        public static bool KillingWind = false;
        public static bool isWindStorm = false;
        public static bool stormEnded = false;

        void Update()
        {

        }

        void FixedUpdate()
        {
            
        }

        public static float KillWind(float windSpeed)
        {
           
            Debug.Log("Killing Wind");

            KillingWind = true;


            windSpeed = Mathf.MoveTowards(windSpeed, 0.00f, windSpeed / 10.0f);



            if (Mathf.Approximately(windSpeed, 0.0f) && KillingWind == true) 
            { 

                windSpeed = 0.00f;
                KillingWind = false;
                if(stormEnded == false)
                {
                    isWindStorm = true;
                    
                }
                
                Debug.Log("Wind Killed");
               

            }
            
            return windSpeed;

        }

        public static float WindGustStorm(float windSpeed, float MaxWindGustSpeed, float WindGustTime)
        {
            WindGustTime = Time.time;
       


            if(isWindStorm == true)
            {
                Debug.Log("Wind Storming");
                windSpeed = Mathf.MoveTowards(0, MaxWindGustSpeed, WindGustTime);

                if (windSpeed == MaxWindGustSpeed)
                {

                    Debug.Log("WindGust Strongest, dying now");
                    windSpeed = Mathf.MoveTowards(windSpeed, 0.00f, 0.5f);
                    KillWind(windSpeed);
                    stormEnded = true;
                    isWindStorm = false;

                }

            }

            return windSpeed;
        }

    }
}
