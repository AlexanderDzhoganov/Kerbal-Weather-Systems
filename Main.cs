//Honourable Mentions and contributions to the code:
//Cilph (he's an ass about it sometimes), TriggerAu (for helping me with a bunch of functionalities),
//DYJ, Scott Manley, taniwha, Majiir, Ippo, 
//Ferram for use of FARWIND code and API, as well as helping with coding.
//And very much thanks for Chris_W for bugtesting intensively and helping out with the code bunches~

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using KSP.IO;
using ferram4;
using KsWeather.Extensions;

namespace KsWeather
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KsMain : MonoBehaviour
    {
        //Private variables
        private static Rect _windowPosition = new Rect();


        //Public variables

        //Boolean Variables
        public bool isWindAutomatic = true; //Value for automatic wind speed
        public bool isWindowOpen = true; //Value for GUI window open

        //Integers
        public int windDirectionNumb;

        //Singles

        //Doubles
        double Pressure = FlightGlobals.ActiveVessel.staticPressure;
        public double HighestPressure = FlightGlobals.getStaticPressure(0);
        public double GForce;
        public double vesselHeight = 0;

        //Floating point numbers
        public float windSpeed = 0.0f; //Wind speed, will probs turn into a double if possible.

        //Vectors
        public Vector3 windDirection;
        
        //Strings
        public String windDirectionLabel;
        

        
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
            UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks; //helps with the random process
            RenderingManager.AddToPostDrawQueue(0, OnDraw); //Draw the stuffs
            windDirectionNumb = UnityEngine.Random.Range(1, 9); //Set wind direction

            Debug.Log("WIND: setting wind function"); //Write to debug
            FARWind.SetWindFunction(windStuff); //Set the WindFunction to the windStuff Function

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

            if (windSpeed != 0.0f)
            {
                Vessel vessel = FlightGlobals.ActiveVessel;
                
                if (vessel != null)
                {

                    if (vessel.parts.Count > 0)
                    {
                        
                        foreach (Part p in vessel.parts)
                        {

                            if (p.rigidbody != null)
                            {
                                
                                    if (p.physicalSignificance != Part.PhysicalSignificance.NONE)
                                    {

                                        //p.rigidbody.AddForce(windDirection); // adds force and drag unto each part

                                    }
                            }
                        }
                        
                        //Part testPart = vessel.parts[0];
                        //testPart.rigidbody.AddForce(forceDirection * windForce);
                    }
                    else
                    {
                        //Debug.Log("KSweatherSystem: activeVessel parts count is < 0");
                    }
                }
                else
                {
                    //Debug.Log("KSweatherSystem: activeVessel is null");
                }
            }

            //Does the defining of the wind direction
            switch (windDirectionNumb)
            {
                case 1:
                    windDirectionLabel = "South";
                    windDirection.x = 0;
                    windDirection.y = windSpeed;
                    windDirection.z = 0;
                    break;
                case 2:
                    windDirectionLabel = "West";
                    windDirection.x = 0;
                    windDirection.y = 0;
                    windDirection.z = -windSpeed;
                    break;
                case 3:
                    windDirectionLabel = "North";
                    windDirection.x = 0;
                    windDirection.y = -windSpeed;
                    windDirection.z = 0;
                    break;
                case 4:
                    windDirectionLabel = "East";
                    windDirection.x = 0;
                    windDirection.y = 0;
                    windDirection.z = windSpeed;
                    break;
                case 5:
                    windDirectionLabel = "South West";
                    windDirection.x = 0;
                    windDirection.y = windSpeed;
                    windDirection.z = -windSpeed;
                    break;
                case 6:
                    windDirectionLabel = "North West";
                    windDirection.x = 0;
                    windDirection.y = -windSpeed;
                    windDirection.z = -windSpeed;
                    break;
                case 7:
                    windDirectionLabel = "North East";
                    windDirection.x = 0;
                    windDirection.y = -windSpeed;
                    windDirection.z = windSpeed;
                    break;
                case 8:
                    windDirectionLabel = "South East";
                    windDirection.x = 0;
                    windDirection.y = windSpeed;
                    windDirection.z = windSpeed;
                    break;
                default:
                    windDirectionLabel = "N/a";
                    windDirection.Zero(); //Zeroes the wind direction vector?
                    break;
            }

        }
        
        //Do ALL the wind things!
        public Vector3 windStuff(CelestialBody body, Part part, Vector3 position)
        {

            try
            {
                return windDirection; //return the windDirection vector stuff
            }

            catch (Exception e)
            {
                Debug.Log("[KsWeather] Exception! " + e.Message + e.StackTrace);

                return Vector3.zero;
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

        //Called when the drawing happens
        private void OnDraw() 
        {

            double Pressure = FlightGlobals.getStaticPressure(FlightGlobals.ship_altitude);

        }

        //Called when the GUI things happen
        void OnGUI()
        {
            _windowPosition = GUILayout.Window(10, _windowPosition, OnWindow, "Weather~");

        }

        // Called when the GUI window things happen
        void OnWindow(int windowId)
        {
            double Pressure = FlightGlobals.getStaticPressure(FlightGlobals.ship_altitude); //gets the current pressure of the atmo the ship is in
            double HighestPressure = FlightGlobals.getStaticPressure(0.0); //gets the highest pressure of the body the ship is in the SOI of
            vesselHeight = FlightGlobals.ship_altitude; //sets the vessel height as the altitude of the ship

            
            if (isWindowOpen == true)
            {
                if (GUI.Button(new Rect(10, 100, 150, 25), "Wind Speed Up")) //Turns up wind speed
                {
                    windSpeed += 1.0f;
                }
                if (GUI.Button(new Rect(170, 100, 150, 25), "Wind Speed Down")) //Turns down wind speed
                {
                    if (windSpeed > 0) //makes sure that you cant have negative windspeed
                    {
                        windSpeed -= 1.0f;
                    }
                    else
                        windSpeed -= 1.0f;
                }
                if (GUI.Button(new Rect(330, 100, 150, 25), "Wind Speed Zero")) //Zeroes wind speed
                {
                    windSpeed = 0.0f;
                }

                if (GUI.Button(new Rect(490, 100, 125, 25), "Wind Direct.")) //Changes the wind direction
                {
                    windDirectionNumb = UnityEngine.Random.Range(1, 9);
                }

                if (isWindAutomatic == false)
                {
                    if (GUI.Button(new Rect(10, 140, 120, 25), "Automatic Wind")) //turns on/off automatic wind 
                    {
                        if (isWindAutomatic == true)
                        {
                            isWindAutomatic = false;
                        }
                        else if (isWindAutomatic == false)
                        {
                            isWindAutomatic = true;
                        }
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(10, 140, 120, 25), "Manual Wind"))
                    {
                        if (isWindAutomatic == true)
                        {
                            isWindAutomatic = false;
                        }
                        else if (isWindAutomatic == false)
                        {
                            isWindAutomatic = true;
                        }
                    }
                }

                if (Pressure != 0) //If we are in atmosphere load the in atmo GUI
                {
                    
                    GUILayout.BeginHorizontal(GUILayout.Width(600));
                    if (GUILayout.Button("X")) { isWindowOpen = false; _windowPosition.height = 0; _windowPosition.width = 0; }
                    GUILayout.Label("Windspeed: " + (windSpeed).ToString("0.00") + " kernauts");
                    GUILayout.Label("Vessel Altitude: " + vesselHeight.ToString("0.00"));
                    GUILayout.Label("\rCurrent Atmoshperic Pressure: " + Pressure.ToString("0.000"));
                    GUILayout.Label("Highest Atmospheric Pressure: " + HighestPressure.ToString("0.000"));
                    GUILayout.Label("InAtmo? : True");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginVertical(GUILayout.Height(100));
                    GUILayout.BeginHorizontal(GUILayout.Width(600));
                    GUILayout.Label("Wind Direction: " + windDirectionLabel);
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUI.DragWindow();


                }
                else //if we are not in an atmosphere, show the non atmo GUI
                {
                    GUILayout.BeginHorizontal(GUILayout.Width(600));
                    if (GUILayout.Button("X")) { isWindowOpen = false; _windowPosition.height = 0; _windowPosition.width = 0; }
                    GUILayout.Label("Windspeed: " + "0" + " kernauts");
                    GUILayout.Label("Vessel Altitude: " + vesselHeight.ToString("0.00"));
                    GUILayout.Label("\rCurrent Atmoshperic Pressure: " + Pressure.ToString("0.000"));
                    GUILayout.Label("Highest Atmospheric Pressure: " + HighestPressure.ToString("0.000"));
                    GUILayout.Label("InAtmo? : False");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginVertical(GUILayout.Height(50));
                    GUILayout.BeginHorizontal(GUILayout.Width(600));
                    GUILayout.Label("Wind Direction: N/a");
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUI.DragWindow();
                }
            }
            
            else if (isWindowOpen == false)
            {
                GUILayout.Window(10, _windowPosition, OnWindow, "KsW");
                GUILayout.Height(0);
                GUILayout.Width(0);
                GUILayout.BeginHorizontal(GUILayout.Width(50));
                if (GUILayout.Button("KsW")) { isWindowOpen = true; }
                GUILayout.BeginVertical(GUILayout.Height(50));
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUI.DragWindow();
            }
            
        }
    }
}
