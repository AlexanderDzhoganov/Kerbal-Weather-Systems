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

        //Bools
        public bool isCellStorming;
        public bool isCellRaining;
        public bool isCellClouded;
        public bool isCellSnowing;
        public bool isCellDaytime;
        //private static bool isCellHigherPressure;

        //Ints
        

        //Doubles
        /*
        public static double cellPressure;
        public static double cellTemperature;
        public static double cellHumidity;
        public static double cellLat;
        public static double cellLatMax;
        public static double cellLong;
        public static double cellLongMax;
        public static double cellAltitude;
         */

        //Floats

        //Strings
        

        //Vector2

        //Vector3
        

        //Arrays


        //Private Variables
        //Bools

        //Ints
        private static int CellID;
        private static int numberOfCells;

        //Doubles

        //Floats
        private static float offSetMultiplier;

        //Strings
        private string bodyName;

        //Vector2

        //Vector3
        private Vector3 cellWindDirTendancy;

        //Arrays

        
        public static bool isCellHigherPressure(int CellIDA, int CellIDB)
        {
            double CellAPressure = 1; // = Cell.Cell[CellIDA].Pressure;
            double CellBPressure = 2; // = Cell.Cell[CellIDB].Pressure;

            if (CellAPressure > CellBPressure) { return true; }
            else { return false; }
            
        }

        public static int getNumberOfCells(CelestialBody Body)
        {
            //numberOfCells = Cell.Cell.Length;
            return numberOfCells;
        }

        public static double getCellAltitude(int CellID)
        {
            double cellAltitude = Cell.Cells[CellID].Altitude;
            return cellAltitude;
        }

        public static double getCellLong(int CellID)
        {
            double Longitude = Cell.Cells[CellID].Longitude;
            return Longitude;
        }

        public static double getCellLat(int CellID)
        {
            double Latitude = Cell.Cells[CellID].Latitude;
            return Latitude;
        }

        public static double getCellHumidity(int CellID)
        {
            double Humidity = Cell.Cells[CellID].Humidity;
            return Humidity;
        }

        public static double getCellTemperature(int CellID)
        {
            double Temperature = Cell.Cells[CellID].Temperature;
            return Temperature;
        }

        public static double getCellPressure(int CellID)
        {
            double Pressure = Cell.Cells[CellID].Pressure;
            return Pressure;
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
