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
        public Vector3 windDirection;
        public double partDrag = 0.0;
        public float vesselDrag = 0.0f;
        public double vesselHeight = 0;
        public int windDirectionNumb;
        public String windDirectionLabel;
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
            windDirectionNumb = UnityEngine.Random.Range(0, 9);
            
               
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
            Pressure = FlightGlobals.ActiveVessel.staticPressure;
            HighestPressure = FlightGlobals.getStaticPressure(0);

            if (!HighLogic.LoadedSceneIsFlight)
                return;
           
            if (Pressure > HighestPressure * 0.7 || Pressure < HighestPressure * 0.3)
            {
                windForce = UnityEngine.Random.Range(0, 3) / 10.0f;
            }
            else
            {
                windForce = UnityEngine.Random.Range(3, 7) / 10.0f;
            }
            

            if (windForce != 0f)
            {
                Vessel vessel = FlightGlobals.ActiveVessel;
                if (vessel != null)
                {

                    if (vessel.parts.Count > 0)
                    {
                        vesselDrag = FlightGlobals.ActiveVessel.Parts.Count * (vessel.rigidbody.angularDrag / 10);
                        foreach (Part p in vessel.parts)
                        {
                            p.rigidbody.AddForce(windDirection); // adds force and drag unto each part
                            
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

            switch (windDirectionNumb)
            {
                case 1:
                    windDirectionLabel = "North";
                    windDirection.x = 0;
                    windDirection.y = windForce * (vesselDrag / 10);
                    windDirection.z = 0;
                    break;
                case 2:
                    windDirectionLabel = "East";
                    windDirection.x = 0;
                    windDirection.y = 0;
                    windDirection.z = -windForce * (vesselDrag / 10);
                    break;
                case 3:
                    windDirectionLabel = "South";
                    windDirection.x = 0;
                    windDirection.y = -windForce * (vesselDrag / 10);
                    windDirection.z = 0;
                    break;
                case 4:
                    windDirectionLabel = "West";
                    windDirection.x = 0;
                    windDirection.y = 0;
                    windDirection.z = windForce * (vesselDrag / 10);
                    break;
                case 5:
                    windDirectionLabel = "North East";
                    windDirection.x = 0;
                    windDirection.y = windForce * (vesselDrag / 10);
                    windDirection.z = -windForce * (vesselDrag / 10);
                    break;
                case 6:
                    windDirectionLabel = "South East";
                    windDirection.x = 0;
                    windDirection.y = -windForce * (vesselDrag / 10);
                    windDirection.z = -windForce * (vesselDrag / 10);
                    break;
                case 7:
                    windDirectionLabel = "South West";
                    windDirection.x = 0;
                    windDirection.y = -windForce * (vesselDrag / 10);
                    windDirection.z = windForce * (vesselDrag / 10);
                    break;
                case 8:
                    windDirectionLabel = "North West";
                    windDirection.x = 0;
                    windDirection.y = windForce * (vesselDrag / 10);
                    windDirection.z = windForce * (vesselDrag / 10);
                    break;
                default:
                    windDirectionLabel = "N/a";
                    break;
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
                GUILayout.Label("Windspeed: " + (windForce * 10) + " kernauts");
                GUILayout.Label("Vessel Altitude: " + vesselHeight.ToString("0.00"));
                GUILayout.Label("\rCurrent Atmoshperic Pressure: " + Pressure.ToString("0.000"));
                GUILayout.Label("Highest Atmospheric Pressure: " + HighestPressure.ToString("0.000"));
                GUILayout.Label("InAtmo? : True");
                GUILayout.EndHorizontal();
                GUILayout.BeginVertical(GUILayout.Height(50));
                GUILayout.BeginHorizontal(GUILayout.Width(600));
                GUILayout.Label("Vessel Drag: " + vesselDrag.ToString("0.000"));
                GUILayout.Label("Wind Direction: " + windDirectionLabel);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUI.DragWindow();
            }
            else
            {
                GUILayout.BeginHorizontal(GUILayout.Width(600));
                GUILayout.Label("Windspeed: " + "0" + " kernauts");
                GUILayout.Label("Vessel Altitude: " + vesselHeight.ToString("0.00"));
                GUILayout.Label("\rCurrent Atmoshperic Pressure: " + Pressure.ToString("0.000"));
                GUILayout.Label("Highest Atmospheric Pressure: " + HighestPressure.ToString("0.000"));
                GUILayout.Label("InAtmo? : False");
                GUILayout.EndHorizontal();
                GUILayout.BeginVertical(GUILayout.Height(50));
                GUILayout.BeginHorizontal(GUILayout.Width(600));
                GUILayout.Label("Vessel Drag: " + vesselDrag.ToString("0.000"));
                GUILayout.Label("Wind Direction: N/a");
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUI.DragWindow();
            }
        }
    }
}
