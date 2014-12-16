//Solar flux = 1.405kW/m^2
//~5.67e-8 W/m^2/K^4 is the

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP.IO;
using UnityEngine;

namespace Temperature
{
    class Heating
    {
        public float netFlux;
        public float solarFlux;
        public float emissiveFlux;
        public bool isSunlight;
        public float temperature;
        public double latitude;

        public void calculateNetFlux()
        {
            if (isSunlight == true) { solarFlux = (float)Math.Cos(latitude); }
            else{solarFlux = 0;}



            netFlux = solarFlux - emissiveFlux;
            temperature += netFlux * 0.35f;
        }
    }
}
