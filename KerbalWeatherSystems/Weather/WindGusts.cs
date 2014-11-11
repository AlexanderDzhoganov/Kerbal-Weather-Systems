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
        public static float WindGustTime1;

        void Update()
        {

        }

        void FixedUpdate()
        {
            
        }

        public static float KillWind(float windSpeed)
        {
           
            //Debug.Log("Killing Wind");

            KillingWind = true;


            windSpeed = Mathf.MoveTowards(windSpeed, 0.00f, windSpeed / 10.0f);



            if (Mathf.Approximately(windSpeed, 0.0f)) 
            { 

                windSpeed = 0.001f;
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

            if(isWindStorm == true)
            {
                //Debug.Log("Wind Storming");
                windSpeed = Mathf.MoveTowards(windSpeed, MaxWindGustSpeed, windSpeed * 0.01f);

                

                if (Mathf.Approximately(windSpeed, MaxWindGustSpeed))
                {
                    //float WindGustTime1 = WindGustTime;

                    WindGustTime1 -= 0.02f; //WindGustTime - float.Parse(Planetarium.fetch.fixedDeltaTime.ToString());
                    //WindGustTime -= TimeWarp.fixedDeltaTime;
                    //Debug.Log(WindGustTime1.ToString());
                    if(WindGustTime1 <= 0)
                    {
                        Debug.Log("WindGust Strongest, dying now");
                        isWindStorm = false;
                        KillWind(windSpeed);
                        stormEnded = true;
                    }
                    
                }
                return windSpeed;

            }

            return windSpeed;
        }

    }
}
