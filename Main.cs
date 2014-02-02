//Honourable Mentions and contributions to the code:
//Cilph (he's an ass about it sometimes), TriggerAu (for helping me with a bunch of functionalities),
//DYJ, Scott Manley, taniwha, Majiir (for kethane co-op and lots of coding help), and many more!

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using KSP.IO;
using Kethane;
using KsWeather.Extensions;

namespace KsWeather
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KsMain : MonoBehaviour
    {
        private static Rect _windowPosition = new Rect();
        public float windForce = 0.0f;
        public float windMinimum = 0.0f;
        public float windMaximum = 3.0f;
        public float windInitial = 0.0f;
        public float windFinal = 0.0f;
        public Vector3 windDirection;
        public Vector3 windDrag;
        public float landDrag;
        public double GForce;
        public bool DrakeWarning = false;
        public bool DrakeCome = false;
        public double partDrag = 0.0;
        public float vesselDrag = 0.0f;
        public double vesselHeight = 0;
        public int windDirectionNumb;
        public String windDirectionLabel;
        public bool windActive = true;
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
            Cell cell = new Cell();
        }

        /*
* Called after the scene is loaded.
*/
        void Awake()
        {
            UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks;
            RenderingManager.AddToPostDrawQueue(0, OnDraw);
            windDirectionNumb = UnityEngine.Random.Range(1, 9);

            InvokeRepeating("windStuff", 1, 1);
            windSteppingStartTime = -100;    //-100 so it will reset on first run
            windSteppingDuration = 5.0; //wind step duration

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
            GForce = FlightGlobals.ActiveVessel.geeForce;


            if (!HighLogic.LoadedSceneIsFlight)
                return;



            if (GForce <= 4)
            {
                DrakeCome = false;
                DrakeWarning = false;
            }
            else if (GForce > 4 && GForce < 7)
            {
                DrakeWarning = true;
                DrakeCome = false;
            }
            else if (GForce >= 7)
            {
                DrakeWarning = false;
                DrakeCome = true;
            }
            else
            {
                DrakeCome = false;
                DrakeWarning = false;
            }

            


            if (windForce != 0.0f)
            {
                Vessel vessel = FlightGlobals.ActiveVessel;


                if (vessel != null)
                {

                    if (vessel.parts.Count > 0)
                    {
                        vesselDrag = (windDrag.magnitude * windForce) / 100;
                        if (windForce == 0)
                        {
                            vesselDrag = 0;
                        }
                        foreach (Part p in vessel.parts)
                        {

                            if (DrakeCome == true)
                            {
                                p.rigidbody.AddExplosionForce(100, windDirection, 10);
                            }
                            if (p.rigidbody == null) continue; 

                            var coeff = -0.5f * p.maximum_drag * vessel.atmDensity * FlightGlobals.DragMultiplier;
                            landDrag = ((float)(coeff * p.rigidbody.mass));
                            windDrag = (coeff * p.rigidbody.mass * vessel.srf_velocity * vessel.GetSrfVelocity().magnitude);

                            //VesselDrag = srfvel - windvel, then (goal*goal.magnitude - srfvel*srfvel.magnitude)
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
            if (!FlightGlobals.ActiveVessel.LandedOrSplashed)
            {
                switch (windDirectionNumb)
                {
                    case 1:
                        windDirectionLabel = "North";
                        windDirection.x = 0;
                        windDirection.y = windForce * ((vesselDrag / 5) * (windDrag.magnitude / 10));
                        windDirection.z = 0;
                        break;
                    case 2:
                        windDirectionLabel = "East";
                        windDirection.x = 0;
                        windDirection.y = 0;
                        windDirection.z = -windForce * ((vesselDrag / 5) * (windDrag.magnitude / 10));
                        break;
                    case 3:
                        windDirectionLabel = "South";
                        windDirection.x = 0;
                        windDirection.y = -windForce * ((vesselDrag / 5) * (windDrag.magnitude / 10));
                        windDirection.z = 0;
                        break;
                    case 4:
                        windDirectionLabel = "West";
                        windDirection.x = 0;
                        windDirection.y = 0;
                        windDirection.z = windForce * ((vesselDrag / 5) * (windDrag.magnitude / 10));
                        break;
                    case 5:
                        windDirectionLabel = "North East";
                        windDirection.x = 0;
                        windDirection.y = windForce * ((vesselDrag / 5) * (windDrag.magnitude / 10));
                        windDirection.z = -windForce * ((vesselDrag / 5) * (windDrag.magnitude / 10));
                        break;
                    case 6:
                        windDirectionLabel = "South East";
                        windDirection.x = 0;
                        windDirection.y = -windForce * ((vesselDrag / 5) * (windDrag.magnitude / 10));
                        windDirection.z = -windForce * ((vesselDrag / 5) * (windDrag.magnitude / 10));
                        break;
                    case 7:
                        windDirectionLabel = "South West";
                        windDirection.x = 0;
                        windDirection.y = -windForce * ((vesselDrag / 5) * (windDrag.magnitude / 10));
                        windDirection.z = windForce * ((vesselDrag / 5) * (windDrag.magnitude / 10));
                        break;
                    case 8:
                        windDirectionLabel = "North West";
                        windDirection.x = 0;
                        windDirection.y = windForce * ((vesselDrag / 5) * (windDrag.magnitude / 10));
                        windDirection.z = windForce * ((vesselDrag / 5) * (windDrag.magnitude / 10));
                        break;
                    default:
                        windDirectionLabel = "N/a";
                        break;
                }
            }
            else if (FlightGlobals.ActiveVessel.Landed)
            {
                switch (windDirectionNumb)
                {
                    case 1:
                        windDirectionLabel = "North";
                        windDirection.x = 0;
                        windDirection.y = windForce * (landDrag * 100);
                        windDirection.z = 0;
                        break;
                    case 2:
                        windDirectionLabel = "East";
                        windDirection.x = 0;
                        windDirection.y = 0;
                        windDirection.z = -windForce * (landDrag * 100);
                        break;
                    case 3:
                        windDirectionLabel = "South";
                        windDirection.x = 0;
                        windDirection.y = -windForce * (landDrag * 100);
                        windDirection.z = 0;
                        break;
                    case 4:
                        windDirectionLabel = "West";
                        windDirection.x = 0;
                        windDirection.y = 0;
                        windDirection.z = windForce * (landDrag * 100);
                        break;
                    case 5:
                        windDirectionLabel = "North East";
                        windDirection.x = 0;
                        windDirection.y = windForce * (landDrag * 100);
                        windDirection.z = -windForce * (landDrag * 100);
                        break;
                    case 6:
                        windDirectionLabel = "South East";
                        windDirection.x = 0;
                        windDirection.y = -windForce * (landDrag * 100);
                        windDirection.z = -windForce * (landDrag * 100);
                        break;
                    case 7:
                        windDirectionLabel = "South West";
                        windDirection.x = 0;
                        windDirection.y = -windForce * (landDrag * 100);
                        windDirection.z = windForce * (landDrag * 100);
                        break;
                    case 8:
                        windDirectionLabel = "North West";
                        windDirection.x = 0;
                        windDirection.y = windForce * (landDrag * 100);
                        windDirection.z = windForce * (landDrag * 100);
                        break;
                    default:
                        windDirectionLabel = "N/a";
                        break;
                }
            }
                
            else if (FlightGlobals.ActiveVessel.Splashed)
                switch (windDirectionNumb)
                {
                    case 1:
                        windDirectionLabel = "North";
                        windDirection.x = 0;
                        windDirection.y = windForce * (landDrag * 1000);
                        windDirection.z = 0;
                        break;
                    case 2:
                        windDirectionLabel = "East";
                        windDirection.x = 0;
                        windDirection.y = 0;
                        windDirection.z = -windForce * (landDrag * 1000);
                        break;
                    case 3:
                        windDirectionLabel = "South";
                        windDirection.x = 0;
                        windDirection.y = -windForce * (landDrag * 1000);
                        windDirection.z = 0;
                        break;
                    case 4:
                        windDirectionLabel = "West";
                        windDirection.x = 0;
                        windDirection.y = 0;
                        windDirection.z = windForce * (landDrag * 1000);
                        break;
                    case 5:
                        windDirectionLabel = "North East";
                        windDirection.x = 0;
                        windDirection.y = windForce * (landDrag * 1000);
                        windDirection.z = -windForce * (landDrag * 1000);
                        break;
                    case 6:
                        windDirectionLabel = "South East";
                        windDirection.x = 0;
                        windDirection.y = -windForce * (landDrag * 1000);
                        windDirection.z = -windForce * (landDrag * 1000);
                        break;
                    case 7:
                        windDirectionLabel = "South West";
                        windDirection.x = 0;
                        windDirection.y = -windForce * (landDrag * 1000);
                        windDirection.z = windForce * (landDrag * 1000);
                        break;
                    case 8:
                        windDirectionLabel = "North West";
                        windDirection.x = 0;
                        windDirection.y = windForce * (landDrag * 1000);
                        windDirection.z = windForce * (landDrag * 1000);
                        break;
                    default:
                        windDirectionLabel = "N/a";
                        break;
                }


        }

        //*
        Double windSteppingStartTime = -100;    //-100 so it will reset on first run
        Double windSteppingDuration = 5.0; //wind step duration
        Double windSteppingProgress;

        public void windStuff()
        {

            
            if (Pressure > HighestPressure * 0.7 || Pressure < HighestPressure * 0.3)
            {
                windMinimum = 0.0f;
                windMaximum = 6.0f;
 
                //windFinal = UnityEngine.Random.Range(0, 6) / 10.0f;
                if (windActive == false)
                {
 
                    // windForce = UnityEngine.Mathf.SmoothStep(windInitial, windFinal, 1.0f);
                    // windFinal = windInitial;
                    // windInitial = UnityEngine.Random.Range(0, 6) / 10.0f;
 
                    //calculate how long since the windSteppingStartTime was
                    windSteppingProgress = (Planetarium.GetUniversalTime() - windSteppingStartTime);
                    if (windSteppingProgress > windSteppingDuration)
                    {
                        //if we have been moving for longer than the duration
                        windSteppingStartTime = Planetarium.GetUniversalTime();                     //store the current game time
                        windSteppingProgress = 0;                                                                                                       //reset this
                        windInitial = windFinal;                                                    //Set the initial value to be whatever the final value was
                        windFinal = UnityEngine.Random.Range(windMinimum, windMaximum) / 10.0f;     //generate a new windFinal Value
                        windSteppingDuration = 2;
                    }
                    //Calc the new Force value based on how far from 0 to duration we have gone
                    windForce = UnityEngine.Mathf.SmoothStep(windInitial, windFinal, (float)(windSteppingProgress / windSteppingDuration));
                }
            }
            else
            {
                if (windActive == false)
                {
                    windMinimum = 3.0f;
                    windMaximum = 15.0f;
                    windSteppingProgress = (Planetarium.GetUniversalTime() - windSteppingStartTime);

                    if (windSteppingProgress > windSteppingDuration)
                    {
                        //if we have been moving for longer than the duration
                        windSteppingStartTime = Planetarium.GetUniversalTime();                     //store the current game time
                        windSteppingProgress = 0;                                                                                                       //reset this
                        windInitial = windFinal;                                                    //Set the initial value to be whatever the final value was
                        windFinal = UnityEngine.Random.Range(windMinimum, windMaximum) / 10.0f;     //generate a new windFinal Value
                        windSteppingDuration = 2;
                    }
                    //Calc the new Force value based on how far from 0 to duration we have gone
                    windForce = UnityEngine.Mathf.SmoothStep(windInitial, windFinal, (float)(windSteppingProgress / windSteppingDuration));
                }
            }
        
        }

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
            _windowPosition = GUILayout.Window(10, _windowPosition, OnWindow, "Weather~");

        }

        // Called when the GUI is loaded?
        void OnWindow(int windowId)
        {
            double Pressure = FlightGlobals.getStaticPressure(FlightGlobals.ship_altitude);
            double HighestPressure = FlightGlobals.getStaticPressure(0.0);
            vesselHeight = FlightGlobals.ship_altitude;
            if (GUI.Button(new Rect(10, 100, 150, 25), "Wind Speed Up"))
            {
                windForce += 0.1f;
            }
            if (GUI.Button(new Rect(170, 100, 150, 25), "Wind Speed Down"))
            {
                if (windForce > 0)
                {
                    windForce -= 0.1f;
                }
                else
                    windForce -= 0.0f;
            }
            if (GUI.Button(new Rect(330, 100, 150, 25), "Wind Speed Zero"))
            {
                windForce = 0;
            }

            if (GUI.Button(new Rect(490, 100, 150, 25), "Wind Direction"))
            {
                windDirectionNumb = UnityEngine.Random.Range(1, 9);
            }

            if (windActive == false)
            {
                if (GUI.Button(new Rect(10, 140, 120, 25), "Automatic Wind"))
                {
                    if (windActive == true)
                    {
                        windActive = false;
                    }
                    else if (windActive == false)
                    {
                        windActive = true;
                    }
                }
            }
            else
            {
                if (GUI.Button(new Rect(10, 140, 120, 25), "Manual Wind"))
                {
                    if (windActive == true)
                    {
                        windActive = false;
                    }
                    else if (windActive == false)
                    {
                        windActive = true;
                    }
                }
            }

            if (GUI.Button(new Rect(10, 200, 150, 25), "Wind Direction"))
            {
                windDirectionNumb = UnityEngine.Random.Range(1, 9);
            }

            if (Pressure != 0)
            {
                if (DrakeWarning == false && DrakeCome == false)
                {
                    GUILayout.BeginHorizontal(GUILayout.Width(600));
                    GUILayout.Label("Windspeed: " + (windForce * 10).ToString("0.00") + " kernauts");
                    GUILayout.Label("Vessel Altitude: " + vesselHeight.ToString("0.00"));
                    GUILayout.Label("\rCurrent Atmoshperic Pressure: " + Pressure.ToString("0.000"));
                    GUILayout.Label("Highest Atmospheric Pressure: " + HighestPressure.ToString("0.000"));
                    GUILayout.Label("InAtmo? : True");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginVertical(GUILayout.Height(100));
                    GUILayout.BeginHorizontal(GUILayout.Width(600));
                    GUILayout.Label("Vessel Drag: " + vesselDrag.ToString("0.0000"));
                    GUILayout.Label("Wind Direction: " + windDirectionLabel);
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUI.DragWindow();
                }
                else if (DrakeWarning == true)
                {
                    GUILayout.BeginHorizontal(GUILayout.Width(600));
                    GUILayout.Label("Windspeed: " + (windForce * 10).ToString("0.00") + " kernauts");
                    GUILayout.Label("Vessel Altitude: " + vesselHeight.ToString("0.00"));
                    GUILayout.Label("\rCurrent Atmoshperic Pressure: " + Pressure.ToString("0.000"));
                    GUILayout.Label("Highest Atmospheric Pressure: " + HighestPressure.ToString("0.000"));
                    GUILayout.Label("InAtmo? : True");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginVertical(GUILayout.Height(100));
                    GUILayout.BeginHorizontal(GUILayout.Width(600));
                    GUILayout.Label("Vessel Drag: " + vesselDrag.ToString("0.0000"));
                    GUILayout.Label("Wind Direction: " + windDirectionLabel);
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUI.DragWindow();
                }
                else if (DrakeCome == true)
                {
                    GUILayout.BeginHorizontal(GUILayout.Width(150));
                    GUILayout.Label("Good Boy~");
                    GUILayout.EndHorizontal();
                    GUI.DragWindow();
                }


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
                GUILayout.Label("Vessel Drag: " + vesselDrag.ToString("0.0000"));
                GUILayout.Label("Wind Direction: N/a");
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUI.DragWindow();
            }
        }
    }
}
