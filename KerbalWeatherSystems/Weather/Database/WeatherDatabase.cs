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


        public static int GetCellAtLocation(CelestialBody body, double latitude, double longitude, double altitude)
        {
            return CellID;
        }

        public List<int> getCellNeighbours(CelestialBody body, int CellID)
        {
            List<int> Neighbours = new List<int>();

            if (CellID > 0) { Neighbours.Add(CellID - 1); } //Longitude neighbour
            if (CellID > 359) { Neighbours.Add(CellID - 360); } //Latitude neighbour
            if (CellID > 64799) { Neighbours.Add(CellID - 64800); } //Altitude neighbour

            if (CellID < Cell.KWSBODY[body].Count - 64800) { Neighbours.Add(CellID + 64800); }
            if (CellID < Cell.KWSBODY[body].Count - 360) { Neighbours.Add(CellID + 360); }
            if (CellID < Cell.KWSBODY[body].Count - 1) { Neighbours.Add(CellID + 1); }

            //Neighbours.Add(CellID + 64800);
            //Neighbours.Add(CellID + 360);
            //Neighbours.Add(CellID + 1);

            return Neighbours;
        }

        public static float GetCellWindSpeed(CelestialBody body, int CellID)
        {
            float cellWindSpeed = Cell.KWSBODY[body][CellID].WindSpeed;
            return cellWindSpeed;
        }

        public static Vector3 getCellWindDirection(CelestialBody body, int CellID)
        {
            Vector3 cellWindDirection = Cell.KWSBODY[body][CellID].CellWindDirection;
            return cellWindDirection;
        }
        
        public static bool isCellHigherPressure(CelestialBody body, int CellIDA, int CellIDB)
        {
            double CellAPressure = Cell.KWSBODY[body][CellIDA].Pressure;
            double CellBPressure = Cell.KWSBODY[body][CellIDB].Pressure;

            if (CellAPressure > CellBPressure) { return true; }
            else { return false; }
            
        }
        public static bool isCellHigherTemperature(CelestialBody body, int CellIDA, int CellIDB)
        {
            double CellATemperature = Cell.KWSBODY[body][CellIDA].Temperature;
            double CellBTemperature = Cell.KWSBODY[body][CellIDB].Temperature;

            if (CellATemperature > CellBTemperature) { return true; }
            else { return false; }

        }
        public static bool isCellMoreHumid(CelestialBody body, int CellIDA, int CellIDB)
        {
            double CellAHumidity = Cell.KWSBODY[body][CellIDA].Humidity;
            double CellBHumidity = Cell.KWSBODY[body][CellIDB].Humidity;

            if (CellAHumidity > CellBHumidity) { return true; }
            else { return false; }

        }

        public static int getNumberOfCells(CelestialBody body)
        {
            //numberOfCells = Cell.KWSBODY[body].Length;
            return numberOfCells;
        }

        public static double getCellAltitude(CelestialBody body, int CellID)
        {
            double cellAltitude = Cell.KWSBODY[body][CellID].Altitude;
            return cellAltitude;
        }

        public static double getCellLong(CelestialBody body, int CellID)
        {
            double Longitude = Cell.KWSBODY[body][CellID].Longitude;
            return Longitude;
        }

        public static double getCellLat(CelestialBody body, int CellID)
        {
            double Latitude = Cell.KWSBODY[body][CellID].Latitude;
            return Latitude;
        }

        public static double getCellHumidity(CelestialBody body, int CellID)
        {
            double Humidity = Cell.KWSBODY[body][CellID].Humidity;
            return Humidity;
        }

        public static double getCellTemperature(CelestialBody body, int CellID)
        {
            double Temperature = Cell.KWSBODY[body][CellID].Temperature;
            return Temperature;
        }

        public static double getCellPressure(CelestialBody body, int CellID)
        {
            double Pressure = Cell.KWSBODY[body][CellID].Pressure;
            return Pressure;
        }

        public static Vector3 getCellPosition(CelestialBody body, int CellID)
        {
            Vector3 CellPosition = Cell.KWSBODY[body][CellID].CellPosition;
            return CellPosition;
        }

        public static int getCellID(double latitude, double longitude, double altitude, CelestialBody body)
        {
            return CellID;
        }

        internal static float GetOffSetMultiplier()
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
