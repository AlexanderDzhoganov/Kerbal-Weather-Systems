//Each Cell will have properties to it, all ranging in variable types
//Example being Cell[CellID].Pressure, and Cell[CellID].isStorming. where pressure is a double, and isStorming is a bool.
//Cell Startup will be called once, and will attempt to grab all the cell's data from the persistence file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

namespace Database
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    class CellStartup : MonoBehaviour
    {
        //Public Variables
        //public Array[] Cell = new Cell[];


        //Private Variables
        private static bool isCellStorming;
        private static bool isCellRaining;
        private static bool isCellClouded;
        private static bool isCellSnowing;
        private static bool isCellDaytime;
        private static bool isCellHigherPressure;

        private static int CellID;
        private static int numberOfCells;

        private static double cellPressure;
        private static double cellTemperature;
        private static double cellHumidity;
        private static double cellLatMin;
        private static double cellLatMax;
        private static double cellLongMin;
        private static double cellLongMax;

        private static string bodyName;

        private static Vector3 cellWindDirTendancy;
        private static Vector3 cellPosition;

        private CelestialBody body;

        private static List<Cell> cells = new List<Cell>();

        internal void getCellVariables()
        {

        }

        int i;
        double altitude = 0;
        internal void GenerateNewCells()
        {
            if(body.atmosphere)
            {
                for(double latitude = -90; latitude < 90; latitude += 0.1)
                {
                    for(double longitude = -180; longitude < 180; longitude += 0.1)
                    {
                        Cell newCell = new Cell(latitude, longitude, altitude);
                        newCell.numberOfCells++;
                        cells.Add(newCell);
                        
                    }
                }

            }
            else {i++; body = FlightGlobals.Bodies[i];}

        }

        
    }

    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class Cell : MonoBehaviour 
    {
        

        //Private Variables
        private CelestialBody body = FlightGlobals.Bodies[0];
        private string bodyName;

        //Public Variables
        public int numberOfCells;
        public bool isCellStorming;
        public bool isCellRaining;
        public bool isCellClouded;
        public bool isCellSnowing;
        public bool isCellDaytime;
        public bool isCellHigherPressure;

        public int CellID;


        public double cellPressure;
        public double cellTemperature;
        public double cellHumidity;
        public double cellLatMin;
        public double cellLatMax;
        public double cellLongMin;
        public double cellLongMax;

        private Vector3 cellWindDirTendancy;
        private Vector3 cellPosition;



        int i;
        public Cell(double latitude, double longitude, double altitude)
        {
            cellPosition = body.GetWorldSurfacePosition(latitude, longitude, altitude);
            cellTemperature = FlightGlobals.getExternalTemperature(cellPosition);
            cellPressure = FlightGlobals.getStaticPressure(cellPosition);
        }

    }
}
