//Solar flux = 1.405kW/m^2
//Effective Solar radiation reaching earth's surface = SolarFlux * (1-albedo) / 4
//~5.67e-8 W/m^2/K^4 is the stefan-holtzmann constant thing.
//0.000000056704 is the Stefan constant shit.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP.IO;
using UnityEngine;
using Database;

namespace Temperature
{
    class Heating
    {
        public float netFlux;
        public float solarFlux;
        public float emissiveFlux;
        public bool isSunlight;
        internal static float temperature;
        public double latitude;
        public float albedo;

        public void calculateNetFlux()
        {
            if (isSunlight == true) { solarFlux = (float)Math.Cos(latitude); }
            else{solarFlux = 0;}

            //emissiveFlux = stefan-boltzmann constant * (((temperature in kelvin)^4) - spaceTemp ^ 4);
            //emissiveFlux = 0.000000056704 * ((Cell.temperature + 273.15)^4 - (2.7)^4);
            //emissiveFlux = 0.000000056704 * ((WeatherDatabase.getCellTemperature(body, CellID)) ^ 4 - (2.7) ^ 4);

            netFlux = solarFlux - emissiveFlux;
            temperature += netFlux * albedo;
            //Cell.Temperature += netFlux * Cell.albedo; //Where cell albedo is affected by if there are clouds, ocean, desert or land there.
            //The lighter the colour of ground, the higher the albedo. Where clouds give a 0.35 albedo (the highest afaik)
        }
    }
}
