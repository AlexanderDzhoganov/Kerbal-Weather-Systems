using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Info
{
    class Info
    {
        //Solar flux = 1.405kW/m^2
        //Effective Solar radiation reaching earth's surface = SolarFlux * (1-albedo) / 4
        //~5.67e-8 W/m^2/K^4 is the stefan-holtzmann constant thing.
        //0.000000056704 is the Stefan constant shit.

        //Pressure gradient force can be represented using Newton's Second Law: F = ma
        //F = -dP * dA = p * dA * dz * a, where F is force, dP is the pressure difference, dA is the surface area
        //height is dz, and the mass of the parcel can be expressed as m = p * dA * dz, where p is the density.
        //The resulting accelleration from the pressure gradient is then: 
        //a = -1/p * dP/dz
        //or it can be expressed more precisely as below, with general pressure P,
        //a(vector) = -1/p * Nabla(P)
        //Where Nabla(P) is the Pressure Gradient between two points.

        //The density of dry air can be calculated using the ideal gas law as a function of temperature and pressure,
        //p = pressure / R sub(Specific) * T
        //Where R sub(Specific) is the specific gas constant for dry air (J/(kg*)) = 287.058, and T is is absolute temperature (Kelvin).
        //and pressure is in Absolute pressure(Pa)

        //The pressure KSP gives is in standard atmospheres, which at 0m(sea level) is 1atm = 101,325 Pascals.
        //So the above equation could be written as p = (101325 * (pressure)) / 287.058 * Temperature
        //Or a more KWS! specific equation would be Density = (101325 *(Cell.Pressure)) / 287.058 * Cell.Temperature;



    }
}
