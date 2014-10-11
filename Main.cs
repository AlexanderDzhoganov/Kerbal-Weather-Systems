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


namespace Kerbal_Weather_Systems
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KerbalWeatherSystems : MonoBehaviour
    {
        //Private variables
        private static Rect MainGUI = new Rect(100,100,200,200);


        //Public variables

        //Boolean Variables
        public bool isWindAutomatic = false; //Value for automatic wind speed
        public bool isWindowOpen = true; //Value for GUI window open
        public bool showWindControls = true;
        public bool showRainControls = false;
        public bool showSnowControls = false;
        public bool showCloudsControls = false;
        public bool showStormsControls = false;

        //Integers
        public int windDirectionNumb;

        //Singles

        //Doubles
        public double Pressure = FlightGlobals.ActiveVessel.staticPressure;
        public double HighestPressure;
        public double GForce;
        public double vesselHeight = 0;

        //Floating point numbers
        public float windSpeed = 0.0f; //Wind speed, will probs turn into a double if possible.

        //Vectors
        public Vector3 windDirection;
        
        //Strings
        public String windDirectionLabel;
        //public String windSpeedString = "0.00f";

        
        //private static String File { get { return KSPUtil.ApplicationRootPath + "/GameData/KerbalWeatherSystems/Plugins/KerbalWeatherSystems.cfg"; } }
        /*
        * use the Awake() method instead of the constructor for initializing data because Unity uses
        * Serialization a lot.
        */
        public void KerbalWeatherSystemsMain()
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

            if (!HighLogic.LoadedSceneIsFlight)
                return;   

            Pressure = FlightGlobals.ActiveVessel.staticPressure;
            HighestPressure = FlightGlobals.getStaticPressure(0);
            GForce = FlightGlobals.ActiveVessel.geeForce;


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
                Debug.Log("[KWS] Exception! " + e.Message + e.StackTrace);

                return Vector3.zero;
            }
        
        }

        public void Save(ConfigNode node)
        {
            PluginConfiguration config = PluginConfiguration.CreateForType<KerbalWeatherSystems>();
            config.SetValue("Window Position", MainGUI);
            config.save();
        }

        public void Load(ConfigNode node)
        {
            PluginConfiguration config = PluginConfiguration.CreateForType<KerbalWeatherSystems>();
            config.load();
            MainGUI = config.GetValue<Rect>("Window Position");
        }

        //Called when the drawing happens
        private void OnDraw() 
        {
            
        }

        //Called when the GUI things happen
        void OnGUI()
        {
            MainGUI = GUILayout.Window(10, MainGUI, OnWindow, "Weather~");
        }

        // Called when the GUI window things happen
        void OnWindow(int windowId)
        {
            Pressure = FlightGlobals.getStaticPressure(FlightGlobals.ship_altitude); //gets the current pressure of the atmo the ship is in
            HighestPressure = FlightGlobals.getStaticPressure(0.0); //gets the highest pressure of the body the ship is in the SOI of
            vesselHeight = FlightGlobals.ship_altitude; //sets the vessel height as the altitude of the ship

            GUILayout.BeginHorizontal();
            GUILayout.Width(500);
            GUILayout.EndHorizontal();

            if (isWindowOpen == true)
            {
                

                if(GUILayout.Toggle(showWindControls,"Wind")) //Call the Wind Control Panel
                {
                    WindControls();
                }

                if (GUI.Button(new Rect(10, 55, 75, 25), "Rain")) //Call the Rain Control Panel
                {
                    RainControls();
                }
                if (GUI.Button(new Rect(10, 90, 75, 25), "Clouds")) //Call the Clouds Control Panel
                {
                    CloudControls();
                }
                if (GUI.Button(new Rect(10, 125, 75, 25), "Snow")) //Call the Snow Control Panel
                {
                    SnowControls();
                }
                if (GUI.Button(new Rect(10, 160, 75, 25), "Storms")) //Call the Storm Control Panel
                {
                    StormControls();
                }

            }
            else if (isWindowOpen == false) //If the GUI is closed
            {
                ClosedGUI();
            }
                    
            /*
            GUI.Button(new Rect(10, 45, 75, 25), "Rain"); //Rain Control Panel
            GUI.Button(new Rect(10, 80, 75, 25), "Clouds"); //Cloud Control Panel
            GUI.Button(new Rect(10, 115, 75, 25), "Snow"); //Snow Control Panel
            GUI.Button(new Rect(10, 150, 75, 25), "Storms"); //Storms Control panel
            */
        }
        public void ClosedGUI()
        {
            GUILayout.Window(10, MainGUI, OnWindow, "KsW");
            GUILayout.Height(0);
            GUILayout.Width(0);
            GUILayout.BeginHorizontal(GUILayout.Width(50));
            if (GUILayout.Button("KsW")) { isWindowOpen = true; } //Button to re-open the GUI
            GUILayout.BeginVertical(GUILayout.Height(50));
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

        void WindControls() //Wind Control Panel
        {
            
            Debug.Log("Weather: WindControls were pressed!");

            //MainGUI.width = 400;
            GUILayout.BeginHorizontal(GUILayout.Width(400));
            GUILayout.EndHorizontal();

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
                if (GUILayout.Button("X")) { isWindowOpen = false; MainGUI.height = 0; MainGUI.width = 0; } //Button for resizing the GUI
                GUILayout.Label("Windspeed: " + (windSpeed).ToString("0.00") + " kernauts");
                GUILayout.Label("Vessel Altitude: " + vesselHeight.ToString("0.00"));
                GUILayout.Label("\rCurrent Atmoshperic Pressure: " + Pressure.ToString("0.000"));
                GUILayout.Label("Highest Atmospheric Pressure: " + HighestPressure.ToString("0.000"));
                GUILayout.Label("InAtmo? : True");
                GUILayout.EndHorizontal();
                GUILayout.BeginVertical(GUILayout.Height(175));
                GUILayout.BeginHorizontal(GUILayout.Width(600));
                GUILayout.Label("Wind Direction: " + windDirectionLabel);
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUI.DragWindow();

            }
            else //if we are not in an atmosphere, show the non atmo GUI
            {
                GUILayout.BeginHorizontal(GUILayout.Width(600));
                if (GUILayout.Button("X")) { isWindowOpen = false; MainGUI.height = 0; MainGUI.width = 0; } //Button for Resizing the GUI
                GUILayout.Label("Windspeed: " + "0" + " kernauts");
                GUILayout.Label("Vessel Altitude: " + vesselHeight.ToString("0.00"));
                GUILayout.Label("\rCurrent Atmoshperic Pressure: " + Pressure.ToString("0.000"));
                GUILayout.Label("Highest Atmospheric Pressure: " + HighestPressure.ToString("0.000"));
                GUILayout.Label("InAtmo? : False");
                GUILayout.EndHorizontal();
                GUILayout.BeginVertical(GUILayout.Height(175));
                GUILayout.BeginHorizontal(GUILayout.Width(600));
                GUILayout.Label("Wind Direction: N/a");
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUI.DragWindow();
            }

            if (showWindControls == true) //Check if wind controls should be shown
            {
                Debug.Log("Weather: WindControls are true!");
                
            }

            else
            {
                Debug.Log("Weather: WindControls are false!");

                //Reset the GUI size
                //MainGUI.height = 0;
                MainGUI.width = 200;
                GUILayout.BeginHorizontal(GUILayout.Width(600));
                GUILayout.Label("Hello");
                GUILayout.EndHorizontal();
                GUI.DragWindow();

            }
        }
    
        void RainControls() //Rain Control Panel
        {

        }

        void CloudControls() //Cloud Control Panel
        {

        }

        void SnowControls() //Snow Control Panel
        {

        }

        void StormControls() //Storm Control Panel
        {

        }

    }
}
