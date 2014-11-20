//This shall be the giant class that holds all the weather data and info for each data cell on a planet.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

namespace Database
{
    class WeatherDatabase
    {
        private static float offSetMultiplier;

        public static float GetOffSetMultiplier(CelestialBody body)
        {
           //offset multipliers for 1m/s of windspeed.
            if(body.bodyName == "Kerbin")
            {
                offSetMultiplier = -0.0000025f;
            }
            else if(body.bodyName == "Laythe")
            {
                offSetMultiplier = 0.00000011f;
            }
            else if(body.bodyName == "Duna")
            {
                offSetMultiplier = 0.000003f;
            }
            else if(body.bodyName == "Eve")
            {
                offSetMultiplier = -0.000001f;
            }
            else if(body.bodyName == "Jool")
            {
                offSetMultiplier = 0.000005f;
            }

            return offSetMultiplier;
        }
    }
}
