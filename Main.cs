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
        public float windForce = 0.0f;
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
            UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks;
            RenderingManager.AddToPostDrawQueue(0, OnDraw);
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
            int caseSwitch = 1;
            Pressure = FlightGlobals.ActiveVessel.staticPressure;
            HighestPressure = FlightGlobals.getStaticPressure(0);

            if (!HighLogic.LoadedSceneIsFlight)
                return;
            if (Pressure > HighestPressure * 0.7 || Pressure < HighestPressure * 0.3)
            {
                caseSwitch = 1;
            }
            else
            {
                caseSwitch = 2;
            }
            switch (caseSwitch)
            {
                case 1:
                    
                    {
                        windForce = UnityEngine.Random.Range(0, 3) / 10.0f;
                    }
                    break;
                default:
                    {
                        windForce = UnityEngine.Random.Range(3, 7) / 10.0f;
                    }
                    break;

            }

            if (windForce != 0f)
            {
                Vessel vessel = FlightGlobals.ActiveVessel;
                if (vessel != null)
                {

                    if (vessel.parts.Count > 0)
                    {
                        foreach (Part p in vessel.parts)
                        {
                            p.rigidbody.AddForce(0, 0, windForce);// * p.maximum_drag);
                        }
                        //Part testPart = vessel.parts[0];
                        //testPart.rigidbody.AddForce(forceDirection * windForce);
                    }
                    else
                    {
                        //Debug.Log("FSweatherSystem: activeVessel parts count is < 0");
                    }
                }
                else
                {
                    //Debug.Log("FSweatherSystem: activeVessel is null");
                }
            }
        }

//*


        public void Save(ConfigNode node)
        {
            PluginConfiguration config = PluginConfiguration.CreateForType<KsMain>();
            config.SetValue("Window Position", _windowPosition);
            config.save();
        }

        public void Load(ConfigNode node)
        {
             PluginConfiguration config = PluginConfiguration.CreateForType<KsMain>();
            config.load();
            _windowPosition = config.GetValue<Rect>("Window Position");
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

        }

        void OnGUI()
        {
            _windowPosition = GUILayout.Window(10, _windowPosition, OnWindow, "KsWindDetector");
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
                GUILayout.Label("windspeed: " + (windForce * 10) + " kernauts");
                GUILayout.Label("Vessel Altitude: " + vesselHeight.ToString("0.00"));
                GUILayout.Label("\rCurrent Atmoshperic Pressure: " + Pressure.ToString("0.000"));
                GUILayout.Label("Highest Atmospheric Pressure: " + HighestPressure.ToString("0.000"));
                GUILayout.Label("InAtmo? : True");
                GUILayout.EndHorizontal();
                GUI.DragWindow();
            }
            else
            {
                GUILayout.BeginHorizontal(GUILayout.Width(600));
                GUILayout.Label("windspeed: " + "0" + " kernauts");
                GUILayout.Label("Vessel Altitude: " + vesselHeight.ToString("0.00"));
                GUILayout.Label("\rCurrent Atmoshperic Pressure: " + Pressure.ToString("0.000"));
                GUILayout.Label("Highest Atmospheric Pressure: " + HighestPressure.ToString("0.000"));
                GUILayout.Label("InAtmo? : False");
                GUILayout.EndHorizontal();
                GUI.DragWindow();
            }
        }
    }
}
