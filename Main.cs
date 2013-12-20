using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using KSP.IO;
using KsWeather.Extensions;

namespace KsWeather
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KsMain : MonoBehaviour
    {
        private static Rect _windowPosition = new Rect();
        public float windSpeed = 0.0f;
        public double vesselHeight = 0;
        double Pressure = FlightGlobals.ActiveVessel.staticPressure;
        public double HighestPressure = FlightGlobals.getStaticPressure(0);
        public bool windSpeedActive = true;

        private static String File { get { return KSPUtil.ApplicationRootPath + "/GameData/KsWeather/Plugins/KsWeatherConfig.cfg"; } }
/*
* use the Awake() method instead of the constructor for initializing data because Unity uses
* Serialization a lot.
*/
        public void KSMain()
        {
            
        }

        /*
* Called after the scene is loaded.
*/
        void Awake()
        {
            
        }

        /*
* Called next.
*/
        void Start()
        {
            
        }

        /*
* Called every frame
*/
        void Update()
        {
            
        }

        /*
* Called at a fixed time interval determined by the physics time step.
*/
        void FixedUpdate()
        {
          
        }

//*


        public void Save()
        {
            try
            {
                ConfigNode save = ConfigNode.CreateConfigFromObject(this);
                save.Save(File);
            }
            catch (Exception e) { Debug.Log("An error occurred while attempting to save: " + e.Message); }
        }

        public static Settings OnLoad()
        {
            ConfigNode load = ConfigNode.Load(File);
            Settings settings = new Settings();
            if (load == null)
            {
                return settings;
            }
            ConfigNode.LoadObjectFromConfig(settings, load);

            return settings;
        }

        private void OnDraw()
        {

            double Pressure = FlightGlobals.getStaticPressure(FlightGlobals.ship_altitude);

            if (Pressure != 0)
            {
                windSpeedActive = true;

            }
            else
            {

                windSpeedActive = false;
            }

                _windowPosition = GUILayout.Window(10, _windowPosition, OnWindow, "Wind Display");

            if (_windowPosition.x == 0 && _windowPosition.y == 0)
            {
                _windowPosition = _windowPosition.CenterScreen();
            }
        }

// Called when the GUI is loaded?
        void OnWindow(int windowId)
        {
            double Pressure = FlightGlobals.getStaticPressure(FlightGlobals.ship_altitude);
            double HighestPressure = FlightGlobals.getStaticPressure(0.0);
            vesselHeight = FlightGlobals.ship_altitude;

            if (Pressure != 0)
            {

                GUILayout.BeginHorizontal(GUILayout.Width(600));
                GUILayout.Label("windspeed: " + (windSpeed * 10) + " kernats");
                GUILayout.Label("Vessel Altitude: " + vesselHeight);
                GUILayout.Label("\rCurrent Atmoshperic Pressure: " + Pressure.ToString("0.000"));
                GUILayout.Label("Highest Atmospheric Pressure: " + HighestPressure.ToString("0.000"));
                GUILayout.Label("InAtmo? : True");
                GUILayout.EndHorizontal();
                GUI.DragWindow();
            }
            else
            {
                GUILayout.BeginHorizontal(GUILayout.Width(600));
                GUILayout.Label("windspeed: " + "0" + " kernats");
                GUILayout.Label("Vessel Altitude: " + vesselHeight);
                GUILayout.Label("\rCurrent Atmoshperic Pressure: " + Pressure.ToString("0.000"));
                GUILayout.Label("Highest Atmospheric Pressure: " + HighestPressure.ToString("0.000"));
                GUILayout.Label("InAtmo? : False");
                GUILayout.EndHorizontal();
                GUI.DragWindow();
            }
        }
    }
}
