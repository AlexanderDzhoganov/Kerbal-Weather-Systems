//Honourable Mentions and contributions to the code:
//Cilph (he's an ass about it sometimes), TriggerAu (for helping me with a bunch of functionalities),
//DYJ, Scott Manley, taniwha, Majiir, Ippo, 
//Ferram for use of FARWIND code and API, as well as helping with coding.
//And very much thanks for Chris_W for bugtesting intensively and helping out with the code bunches~
//Very, very much thanks for use of code on Rbray's EVE mod, it really helps to get clouds going!

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using KSP.IO;
using ferram4;
using KerbalWeatherSystems.Extensions;

using Random = UnityEngine.Random;


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
        public bool isRaining = false; //Is it Raining?
        public bool isSnowing = false; //Is it Snowing?
        public bool isStorming = false; //Is it storming?

        //GUI Bool
        public bool isWindowOpen = true; //Value for GUI window open
        public bool showWindControls = false;
        public bool showRainControls = false;
        public bool showSnowControls = false;
        public bool showCloudControls = false;
        public bool showStormControls = false;
        //private static Type serverType = null; //The server stuff for the assembly version.
        //private static bool? installed = null; //Checks if FARWind is installed
        public bool showWindLines = false;

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
        private float Anger = 9001.0f; //You've found my easter egg!
        
        //Arrays
        public string[] WindGUIButtons = new string[3] {"Upwind", "DownWind", "WindDirection"};

        //Vectors
        public Vector3 windDirection;
        
        //Strings
        public String windDirectionLabel;
        public String windSpeedString = "1";

        //Test Variables
        private LineRenderer line = null;
        public float x;
        public float y;
        public float z;
        public string xstring = "1";
        public string ystring = "1";
        public string zstring = "1";
        
        
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

            Random.seed = (int)System.DateTime.Now.Ticks; //helps with the random process
            RenderingManager.AddToPostDrawQueue(0, OnDraw); //Draw the stuffs
            windDirectionNumb = Random.Range(1, 9); //Set wind direction

            Debug.Log("WIND: setting wind function"); //Write to debug
            FARWind.SetWindFunction(windStuff); //Set the WindFunction to the windStuff Function
            

        }
        
        //Called after Update()
        void LateUpdate()
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

            if (!HighLogic.LoadedSceneIsFlight)
                return;

            Part part = FlightGlobals.ActiveVessel.rootPart;

            if (windSpeed != 0.0f)
            {
                Vessel vessel = FlightGlobals.ActiveVessel;

                Vector3 Up = vessel.upAxis; //get the up relative to the surface
                Up.Normalize(); //normalize that shit
                Vector3 East = Vector3.Cross(vessel.mainBody.angularVelocity, Up); //Get the reverse East axis
                East.Normalize(); //Normalize that shit
                Vector3 North = Vector3.Cross( vessel.upAxis, East); //Get the reverse north axis
                North.Normalize();//Guess what? Normalize that shit
                

                //Defines the Wind Speed stuff
                switch (windDirectionNumb)
                {

                    case 1:
                        windDirectionLabel = "Northerly"; //Heading South: Wind going from North to South
                        windDirection = -North * windSpeed;

                        break;
                    case 2:
                        windDirectionLabel = "Easterly"; //Heading West: Wind going from East to West
                        windDirection = -East * windSpeed;

                        break;
                    case 3:
                        windDirectionLabel = "Southerly"; //Heading North: Wind going from South to North
                        windDirection = North * windSpeed;

                        break;
                    case 4:
                        windDirectionLabel = "Westerly"; //Heading East: Wind going from West to East
                        windDirection = East * windSpeed;

                        break;
                    case 5:
                        windDirectionLabel = "North Easterly"; //Heading South West: Wind going from North East to South West
                        windDirection = (-North + -East).normalized * windSpeed;

                        break;
                    case 6:
                        windDirectionLabel = "South Easterly"; //Heading North West: Wind going from South East to North West
                        windDirection = (North + -East).normalized * windSpeed;

                        break;
                    case 7:
                        windDirectionLabel = "South Westerly"; //Heading North East: Wind going from South West to North East
                        windDirection = (North + East).normalized * windSpeed;

                        break;
                    case 8:
                        windDirectionLabel = "North Westerly"; //Heading South East: Wind going from North West to South East
                        windDirection = (-North + East).normalized * windSpeed;

                        break;
                    default:
                        windDirectionLabel = "N/a";
                        windDirection.Zero(); //Zeroes the wind direction vector?
                        break;

                }
            }
        }
    
        //Do ALL the wind things!
        public Vector3 windStuff(CelestialBody body, Part part, Vector3 position)
        {
            Vessel vessel = FlightGlobals.ActiveVessel;
            Vector3 Up = vessel.upAxis; //get the up relative to the surface
            Up.Normalize(); //normalize that shit
            Vector3 East = Vector3.Cross(vessel.mainBody.angularVelocity, Up); //Get the reverse East axis
            East.Normalize(); //Normalize that shit
            Vector3 North = Vector3.Cross(vessel.upAxis, East); //Get the reverse north axis
            North.Normalize();//Guess what? Normalize that shit

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

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("X"); xstring = GUILayout.TextField(xstring);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Y"); ystring = GUILayout.TextField(ystring);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Z"); zstring = GUILayout.TextField(zstring);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Set"))
            {
                x = float.Parse(xstring);
                y = float.Parse(ystring);
                z = float.Parse(zstring);
                //FARWind.SetWindFunction(windStuff);
            }
            GUILayout.EndVertical();

        }
        //Displays the wind vector as a line from the GUI.
        public void WindVectorLine()
        {
            
                // First of all, create a GameObject to which LineRenderer will be attached.
                //GameObject obj = new GameObject("Line");
                Part part = FlightGlobals.ActiveVessel.rootPart;
                GameObject obj = new GameObject("Line");
                // Then create renderer itself...
                line = obj.AddComponent<LineRenderer>();
                line.transform.parent = transform; // ...child to our part...
                line.useWorldSpace = true; // ...and moving along with it (rather 
                // than staying in fixed world coordinates)
                line.transform.localPosition = Vector3.zero;
                line.transform.localEulerAngles = Vector3.zero;

                // Make it render a red to yellow triangle, 1 meter wide and 2 meters long
                line.material = new Material(Shader.Find("Particles/Additive"));
                line.SetColors(Color.red, Color.yellow);
                line.SetWidth(1, 0);
                line.SetVertexCount(2);
                line.SetPosition(0, part.transform.position);
                line.SetPosition(1, part.transform.position + windDirection); //Draws the end point in the direction of the wind
                
            
            
        }

            /*
            if (GUI.Button(new Rect(140, 190, 120, 25), "ShowWind"))
            {
                WindVectorLine();
            }
                    

                if (Pressure != 0) //If we are in atmosphere load the in atmo GUI
                {
                    
                    GUILayout.BeginHorizontal(GUILayout.Width(600));
                    GUILayout.Label("Windspeed: " + (windSpeed).ToString("0.00") + " m/s");
                    GUILayout.Label("Vessel Altitude: " + vesselHeight.ToString("0.00"));
                    GUILayout.Label("\rCurrent Atmoshperic Pressure: " + Pressure.ToString("0.000"));
                    GUILayout.Label("Highest Atmospheric Pressure: " + HighestPressure.ToString("0.000"));
                    GUILayout.Label("InAtmo? : True");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginVertical(GUILayout.Height(100));
                    GUILayout.BeginHorizontal(GUILayout.Width(600));
                    GUILayout.Label("Wind Direction: " + windDirectionLabel);
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUI.DragWindow();

                }
                else //if we are not in an atmosphere, show the non atmo GUI
                {
                    GUILayout.BeginHorizontal(GUILayout.Width(600));
                    GUILayout.Label("Windspeed: " + "0" + " m/s");
                    GUILayout.Label("Vessel Altitude: " + vesselHeight.ToString("0.00"));
                    GUILayout.Label("\rCurrent Atmoshperic Pressure: " + Pressure.ToString("0.000"));
                    GUILayout.Label("Highest Atmospheric Pressure: " + HighestPressure.ToString("0.000"));
                    GUILayout.Label("InAtmo? : False");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginVertical(GUILayout.Height(100));
                    GUILayout.BeginHorizontal(GUILayout.Width(600));
                    GUILayout.Label("Wind Direction: N/a");
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUI.DragWindow();
                }
        }
    
        // Return the current instance of the server, if any.
        /*
        public static FARWind Server
        {
            get
            {
                if (FARWindInstalled)
                {
                    object instance = serverType
                    .GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)
                    .GetValue(null, null);
                    return (FARWind)DuckTyping.Cast(typeof(FARWind), instance);
                }
                else
                    return null;
            }
         
        }

        //Finds the server type of the assembly
        private static Type FindServerType()
        {
            return AssemblyLoader.loadedAssemblies
            .SelectMany(a => a.assembly.GetExportedTypes())
            .SingleOrDefault(t => t.FullName == "ferram4.FARWind");
        }

        /// Checks if FARWind is installed and can be located
        public static bool FARWindInstalled
        {
            get
            {
                if (installed == null)
                {
                    serverType = FindServerType();
                    installed = !(serverType == null);
                }
                return (bool)installed;
            }
        }

         */
    }
}