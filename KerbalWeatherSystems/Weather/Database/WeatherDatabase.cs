//This shall be the giant class that holds all the weather data and info for each data cell on a planet.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

namespace Database
{
    public class WeatherDatabase
    {
        //Public Variables
        public static Cell Cell;

        //Private Variables
        //Bools
        private static bool isCellStorming;
        private static bool isCellRaining;
        private static bool isCellClouded;
        private static bool isCellSnowing;
        private static bool isCellDaytime;
        private static bool isCellHigherPressure;

        //Floats
        private static float offSetMultiplier;

        //Ints
        private static int CellID;
        private static int numberOfCells;

        //Doubles
        private static double cellPressure;
        private static double cellTemperature;
        private static double cellHumidity;
        private static double cellLatMin;
        private static double cellLatMax;
        private static double cellLongMin;
        private static double cellLongMax;
        private static double cellAltitude;

        //Strings
        private static string bodyName;

        //Vector3
        private static Vector3 cellWindDirTendancy;

        
        public static bool isCellHigherPressure(int CellIDA, int CellIDB)
        {
            double CellAPressure = Cell.Cell[CellIDA].Pressure;
            double CellBPressure = Cell.Cell[CellIDB].Pressure;

            if (CellAPressure > CellBPressure) { return true; }
            else { return false; }
            
        }

        public static int getNumberOfCells()
        {
            numberOfCells = Cell.Cell.Length;
            return numberOfCells;
        }

        public static double getCellAltitude(int CellID)
        {
            return cellAltitude;
        }

        public static double getCellLong()
        {
            return cellLongMax;
        }

        public static double getCellLat()
        {
            return cellLatMax;
        }

        public static double getCellHumidity()
        {
            return cellHumidity;
        }

        public static double getCellTemperature()
        {
            return cellTemperature;
        }

        public static double getCellPressure()
        {
            return cellPressure;
        }

        public static int getCellID()
        {
            return CellID;
        }

        public static float GetOffSetMultiplier()
        {
            try
            {
                CelestialBody body = FlightGlobals.currentMainBody;
                //offset multipliers for 1m/s of windspeed.
                if (body.bodyName == "Kerbin"){offSetMultiplier = 0.0000025f;}
                else if (body.bodyName == "Laythe"){offSetMultiplier = 0.00000011f;}
                else if (body.bodyName == "Duna"){offSetMultiplier = 0.000003f;}
                else if (body.bodyName == "Eve"){offSetMultiplier = -0.000001f;}
                else if (body.bodyName == "Jool") { offSetMultiplier = 0.000005f; }
            }
            catch
            {
                //Debug.Log("Null body");
                offSetMultiplier = -0.0000025f; //default to kerbin's offset rate
            }
            return offSetMultiplier;
        }
    }
}
