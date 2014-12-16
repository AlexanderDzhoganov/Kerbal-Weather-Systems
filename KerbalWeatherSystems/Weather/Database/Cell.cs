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
    public class CellStartup : MonoBehaviour
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

        //private List<Cell> cells = new List<Cell>();
        private Array KWSBody;
        //private Array Cells;
        

        internal void getCellVariables()
        {

        }

        

        
    }

    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class Cell : MonoBehaviour, IConfigNode
    {


        //Private Variables
        private static CelestialBody body = FlightGlobals.Bodies[0];
        //private string bodyName;
        //private Vector3 cellWindDirTendancy;
        //private static Vector3 cellPosition;
        private static int numberOfCells;
        //private static int numberOfKWSBodies;
        private CelestialBody cellBody;

        //Public Variables
        //public int numberOfCells;
        public bool isCellStorming;
        public bool isCellRaining;
        public bool isCellClouded;
        public bool isCellSnowing;
        public bool isCellDaytime;
        public bool isCellHigherPressure;

        public double Pressure;
        public float Temperature;
        public double Humidity;
        public double Latitude;
        //public double LatMax;
        public double Longitude;
        //public double LongMax;
        public double Altitude;

        public float WindSpeed;

        public Vector3 CellWindDirection;
        public Vector3 CellPosition;

        public static Cell[] Cells = new Cell[numberOfCells];
        //public static Array KWSBODY = new Array[numberOfKWSBodies];
        public static Dictionary<CelestialBody, List<Cell>> KWSBODY = new Dictionary<CelestialBody, List<Cell>>();



        void Awake()
        {
            //Debug.Log("Awake!");
            GenerateNewCells();
        }
        void FixedUpdate()
        {
            //Debug.Log("Fixed Update");
            UpdateCellData();
        }

        public void Save(ConfigNode node)
        {

        }
        public void Load(ConfigNode node)
        {

        }

        static int CellID;
        internal static void GenerateNewCells()
        {
            Debug.Log("Generating New Cells!");
            double altitude = 0;

            foreach (CelestialBody body in FlightGlobals.Bodies)
            {
                if (body.atmosphere) //Check if the body has atmosphere
                {
                    Debug.Log(body.name + " Has Atmosphere!");
                    KWSBODY[body] = new List<Cell>();
                    ///*
                    for (altitude = 0; altitude < 12500; altitude += 2500)
                    {
                        for (double latitude = -90; latitude <= 90; latitude += Settings.cellDefinitionWidth)//iterate through the latitudes
                        {
                            for (double longitude = -180; longitude < 180; longitude += (Settings.cellDefinitionWidth))//iterate through the longitudes
                            {
                                Cell newCell = new Cell(latitude, longitude, altitude, body);
                                newCell.Latitude = latitude;
                                newCell.Longitude = longitude;
                                newCell.Altitude = altitude;
                                newCell.cellBody = body;
                                newCell.CellPosition = body.GetWorldSurfacePosition(latitude, longitude, altitude);
                                newCell.Temperature = FlightGlobals.getExternalTemperature((float)newCell.Altitude, body);
                                newCell.Pressure = FlightGlobals.getStaticPressure(newCell.CellPosition);
                                //Cells[CellID] = newCell;
                                //cells.Add(newCell);
                                KWSBODY[body].Add(newCell);
                                //Debug.Log(body + ", " + CellID + " : " + KWSBODY[body][CellID].Latitude + ", " + KWSBODY[body][CellID].Longitude + ", " + KWSBODY[body][CellID].Altitude);
                                numberOfCells++;
                                CellID++;

                            }
                        }
                        //Debug.Log("Cell Altitude: " + altitude);
                        //Debug.Log(FlightGlobals.getExternalTemperature((float)altitude, body));
                    }
                    //*/
                    /*
                    for (altitude = 0; altitude < body.maxAtmosphereAltitude; altitude += Settings.cellDefinitionAlt )
                    {
                        for (double latitude = -90; latitude < 90; latitude += Settings.cellDefinitionWidth)//iterate through the latitudes
                        {
                            for (double longitude = -180; longitude < 180; longitude += (Settings.cellDefinitionWidth))//iterate through the longitudes
                            {
                                Cell newCell = new Cell(latitude, longitude, altitude);
                                newCell.Latitude = latitude;
                                newCell.Longitude = longitude;
                                newCell.Altitude = altitude;
                                newCell.cellBody = body;

                                //Cells[CellID] = newCell;
                                //cells.Add(newCell);
                                KWSBODY[body].Add(newCell);
                                //Debug.Log(body + ", " + CellID + " : " + KWSBODY[body][CellID].Latitude + ", " + KWSBODY[body][CellID].Longitude + ", " + KWSBODY[body][CellID].Altitude);
                                numberOfCells++;
                                CellID++;

                            }
                        }
                    }
                    */
                    /*
                    for (double latitude = -90; latitude < 90; latitude += Settings.cellDefinitionWidth)//iterate through the latitudes
                    {
                        for (double longitude = -180; longitude < 180; longitude += (Settings.cellDefinitionWidth))//iterate through the longitudes
                        {
                            Cell newCell = new Cell(latitude, longitude, altitude);
                            newCell.Latitude = latitude;
                            newCell.Longitude = longitude;
                            newCell.Altitude = altitude;
                            newCell.cellBody = body;

                            //Cells[CellID] = newCell;
                            //cells.Add(newCell);
                            KWSBODY[body].Add(newCell);
                            //Debug.Log(body + ", " + CellID + " : " + KWSBODY[body][CellID].Latitude + ", " + KWSBODY[body][CellID].Longitude + ", " + KWSBODY[body][CellID].Altitude);
                            numberOfCells++;
                            CellID++;

                        }
                    }
                    */
                    CellID = 0;
                    numberOfCells = 0;
                    altitude = 0;
                    //if (altitude < body.maxAtmosphereAltitude) { altitude += Settings.cellDefinitionAlt; }//Up the altitude so that a new layer of cells can be created
                    //else { CellID = 0; numberOfCells = 0; altitude = 0; }
                }
                else
                {
                    Debug.Log(body.name + " Doesn't Have Atmosphere!");
                    CellID = 0;
                    numberOfCells = 0;
                    altitude = 0;
                }
            }


        }

        public Cell(double latitude, double longitude, double altitude, CelestialBody body)
        {
            CellPosition = body.GetWorldSurfacePosition(latitude, longitude, altitude);
            //Temperature = 12;
            Temperature = FlightGlobals.getExternalTemperature(CellPosition);
            Pressure = FlightGlobals.getStaticPressure(CellPosition);
            Altitude = altitude;
        }

        public void UpdateCellData()
        {
            //CelestialBody TestBody = FlightGlobals.Bodies[1];
            //KWSBODY[TestBody][0].Temperature = FlightGlobals.getExternalTemperature(KWSBODY[TestBody][0].CellPosition);
            //Debug.Log(KWSBODY[TestBody][0].Temperature);
        }

        

    }
 
}
