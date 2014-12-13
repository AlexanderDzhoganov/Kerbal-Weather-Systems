//Wind gusts will be "random"
//Note: ITCZ is Easterlies, Hadley cell is North Easterlies and South Easterlies, 
//Ferrel Cell is South Westerlies, Polar Cell is North Easterlies

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;


namespace Weather
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Wind : MonoBehaviour
    {
        public static bool KillingWind = false;
        public static bool isWindStorm = false;
        public static bool stormEnded = false;
        public static float WindGustTime1;
        public static int windDirectionNumb;
        public static Vector3 windDirection = new Vector3(1.0f, 0.0f, 0.0f);
        public static float windSpeed = HeadMaster.windSpeed;
        public static string WindDirectionLabel = string.Empty;
        public static Vector3 vesselVector;
        public static Vector3 resultantWindVector;
        public static Vector3 localWindVector;
        public static double bodyWindSpeedMultiplier;
        public static Vector3 windGustDirection;

        private static double CurrentAtmoPressure;
        private static double HighestAtmoPressure;
        //private static float RecipDeltaAtmoPressure;
        private static double Longitude = HeadMaster.Longitude;
        private static double Latitude = HeadMaster.Latitude;
        private static float Temperature = HeadMaster.Temperature;
        private static double Altitude = HeadMaster.Altitude;
        private static float AtmoDensity;
        private static Vector3 TradeWindTendancy;
        private static float tradeWindSpeed;
        private static bool isWindGust = false;
        private static Vector3 WindGust;
        private static bool isWindChanging = false;
        private static float MaxWindGustSpeed;
        private static float WindGustTime;
        private double windTime;
        private float targetWindSpeed;
        private static bool isWindVectorChanging = true;

        public void Awake()
        {
            windDirection.x = 1;
            windDirection.y = 0;
            windDirection.z = 0;
            windTime = UnityEngine.Random.Range(0,60);
        }

        void Update()
        {

        }

        double second = 0.0f;
        private static double timeUntilGust = 0.0f;
        public void FixedUpdate()
        {

            //Debug.Log("FixedUpdate");
            if (!HighLogic.LoadedSceneIsFlight)
                return;

            Vessel vessel = FlightGlobals.ActiveVessel;

            CurrentAtmoPressure = FlightGlobals.getStaticPressure(Altitude);
            HighestAtmoPressure = FlightGlobals.getStaticPressure(0.0);
            //RecipDeltaAtmoPressure = (float)(1 / (HighestAtmoPressure - CurrentAtmoPressure));
            AtmoDensity = (float)vessel.atmDensity;

            //Data collection
            Longitude = HeadMaster.Longitude;
            Latitude = HeadMaster.Latitude;
            Temperature = HeadMaster.Temperature;
            Altitude = HeadMaster.Altitude;
            //windSpeed = HeadMaster.windSpeed;
            MaxWindGustSpeed = HeadMaster.MaxWindGustSpeed;
            WindGustTime = HeadMaster.WindGustTime;
            //windSpeed = ChangeWindSpeed(windSpeed, windDirection.magnitude);

            second += 0.02;
            windTime -= 0.02;
            timeUntilGust -= 0.02;
            if(second >= 1.0)
            {
                //do things every second
                //Debug.Log(windTime);
                //Debug.Log(windSpeed);
                //if (isWindChanging == true) { Debug.Log("Wind Changing"); }
                //Debug.Log(timeUntilGust);
                //Debug.Log(windGustDirection);
                //Debug.Log(FlightGlobals.getCoriolisAcc(windDirection, FlightGlobals.currentMainBody));
                second = 0; 
            }

            if(windTime <= 0.0)
            {
                //Do the wind gust change
                targetWindSpeed = UnityEngine.Random.Range(0, 25);
                windTime = UnityEngine.Random.Range(0, 25);
                windSpeed = ChangeWindSpeed(windSpeed, targetWindSpeed);
                isWindChanging = true;
                //windSecond = 0;
            }

            if(timeUntilGust <= 0.0)
            {
                isWindGust = true;
                GenerateWindGust(windGustDirection);
            }
            else { isWindGust = false;}


            if (KillingWind == true) { windSpeed = KillWind(windSpeed); }
            if (isWindStorm == true) { windSpeed = WindGustStorm(windSpeed, MaxWindGustSpeed, WindGustTime); }
            if (isWindChanging == true) { windSpeed = ChangeWindSpeed(windSpeed, targetWindSpeed); }
            if (isWindGust == true) { windGustDirection = ChangeWindVector(windDirection, windGustDirection); }

        }

        public static Vector3 WindStuff()
        {

            Vessel vessel = FlightGlobals.ActiveVessel;
            CelestialBody orbitingBody = FlightGlobals.currentMainBody;
            String orbitingBodyName = orbitingBody.bodyName;
            Vector3 Up = vessel.upAxis; //get the up relative to the surface
            Up.Normalize(); //normalize that shit
            Vector3 East = Vector3.Cross(vessel.mainBody.angularVelocity, Up); //Get the reverse East axis
            East.Normalize(); //Normalize that shit
            Vector3 North = Vector3.Cross(East, vessel.upAxis); //Get the reverse north axis
            North.Normalize();//Guess what? Normalize that shit


            windDirection = windGustDirection + localWindVector + WindDirTendancy(Latitude, orbitingBody) + WindGust;
            //windDirection.x = Mathf.Max(windGustDirection.x, localWindVector.x, WindDirTendancy(Latitude, orbitingBody).x, WindGust.x);
            //windDirection.y = Mathf.Max(windGustDirection.y, localWindVector.y, WindDirTendancy(Latitude, orbitingBody).y, WindGust.y);
            //windDirection.z = Mathf.Max(windGustDirection.z, localWindVector.z, WindDirTendancy(Latitude, orbitingBody).z, WindGust.z);
            //windDirection.Normalize(); //Surprise! Normalize!
            windDirection = windDirection.normalized * windSpeed;

            try
            {
                return windDirection;
            }
            catch (Exception e)
            {
                Debug.Log("[KWS] Exception! " + e.Message + e.StackTrace);

                return Vector3.zero;
            }
        }

        private static Vector3 WindDirTendancy(double latitude, CelestialBody body)
        {
            Vessel vessel = FlightGlobals.ActiveVessel;
            CelestialBody orbitingBody = FlightGlobals.currentMainBody;
            String orbitingBodyName = orbitingBody.bodyName;
            Vector3 Up = vessel.upAxis; //get the up relative to the surface
            Up.Normalize(); //normalize that shit
            Vector3 East = Vector3.Cross(vessel.mainBody.angularVelocity, Up); //Get the reverse East axis
            East.Normalize(); //Normalize that shit
            Vector3 North = Vector3.Cross(East, vessel.upAxis); //Get the reverse north axis
            North.Normalize();//Guess what? Normalize that shit
            Vector3 coriolisAcc = FlightGlobals.getCoriolisAcc(TradeWindTendancy, body);

            //Debug.Log(coriolisAcc);
            
            if (latitude >= -5 && latitude <= 5)
            {
                if (body.name == "Kerbin")//Easterly trade wind at the Inter-Tropical Convergence zone
                {
                    //FlightGlobals.getCoriolisAcc(TradeWindTendancy, body);
                    //Debug.Log("Wind is Easterly!");
                    //This area is nominally of lower pressure
                    TradeWindTendancy = -East * ((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 2;
                }
                else //kerbin like weather patterns
                {
                    TradeWindTendancy = -East * ((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 2;
                }
            }
            else if(latitude > 5 && latitude <= 10)
            {
                if (body.name == "Kerbin") //Hadley Cell, North Easterly trade wind
                {
                    //TradeWindTendancy = Vector3.Cross(North * (windSpeed * (float)CurrentAtmoPressure), coriolisAcc);// * RecipDeltaAtmoPressure);
                    TradeWindTendancy = (North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 5;

                }
                else //Kerbin like weather patterns
                {
                    TradeWindTendancy = (North + -East).normalized;
                    windDirectionNumb = 5;
                }
            }
            else if(latitude > 10 && latitude <= 15)
            {
                if (body.name == "Kerbin") //Hadley Cell, North Easterly trade wind
                {
                    TradeWindTendancy = (North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 5;
                }
                else
                {
                    TradeWindTendancy = (North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 5;
                }
            }
            else if(latitude > 15 && latitude <= 20)
            {
                if (body.name == "Kerbin")//Hadley Cell, Northerly trade wind.
                {
                    TradeWindTendancy = North; // *((float)CurrentAtmoPressure);// * RecipDeltaAtmoPressure);
                    windDirectionNumb = 1;
                }
                else
                {
                    TradeWindTendancy = North; // *((float)CurrentAtmoPressure);// * RecipDeltaAtmoPressure);
                    windDirectionNumb = 1;
                }
            }
            else if(latitude > 20 && latitude <= 25)
            {
                if (body.name == "Kerbin")//Hadley Cell - Sub-Tropical Ridge, North Westerly trade wind
                {
                    TradeWindTendancy = (North + East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }
                else
                {
                    TradeWindTendancy = (North + East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }
            }
            else if(latitude > 25 && latitude <= 30)
            {
                if (body.name == "Kerbin")//Sub-tropical Ridge, Westerlies
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }
                else
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }
            }
            else if(latitude > 30 && latitude <= 35)
            {
                if (body.name == "Kerbin")//Sub-tropical Ridge - Ferrel Cell, Mid-Latitude. Westerlies
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }
                else
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }
            }
            else if(latitude > 35 && latitude <= 40)
            {
                if (body.name == "Kerbin")//Sub-tropical Ridge - Ferrel Cell, Mid-Latitude. Westerlies
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }
                else
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }
            }
            else if(latitude > 40 && latitude <= 45)
            {
                if (body.name == "Kerbin")//Ferrel cell, Mid-Latitude. South Westerlies
                {
                    TradeWindTendancy = (-North + East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }
                else
                {
                    TradeWindTendancy = (-North + East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }
            }
            else if(latitude > 45 && latitude <= 50)
            {
                if (body.name == "Kerbin")//Ferrel Cell, Mid-Latitude, South westerlies
                {
                    TradeWindTendancy = (-North + East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }
                else
                {
                    TradeWindTendancy = (-North + East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }
            }
            else if(latitude > 50 && latitude <= 55)
            {
                if (body.name == "Kerbin")//Ferrel Cell, Mid-Latitude, South Westerlies
                {
                    TradeWindTendancy = (-North + East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }
                else
                {
                    TradeWindTendancy = (-North + East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }

            }
            else if(latitude > 55 && latitude <= 60)
            {
                if (body.name == "Kerbin")//Ferrel cell - Polar Front, Entering Jet stream, Westerlies
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }
                else
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }
            }
            else if(latitude > 60 && latitude <= 65)
            {
                if (body.name == "Kerbin")//Polar Front - Polar Vortex, leaving Jet Stream, Westerlies
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }
                else
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }
            }
            else if(latitude > 65 && latitude <= 70)
            {
                if (body.name == "Kerbin")//Polar Vortex, North Easterlies
                {
                    TradeWindTendancy = (North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 5;
                }
                else
                {
                    TradeWindTendancy = (North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 5;
                }

            }
            else if(latitude > 70 && latitude <= 75)
            {
                if (body.name == "Kerbin")//Polar vortex, North Easterlies
                {
                    TradeWindTendancy = (North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 5;
                }
                else
                {
                    TradeWindTendancy = (North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 5;
                }
            }
            else if(latitude > 75 && latitude <= 80)
            {
                if (body.name == "Kerbin")//Polar vortex, North Easterlies
                {
                    TradeWindTendancy = (North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 5;
                }
                else
                {
                    TradeWindTendancy = (North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 5;
                }
            }
            else if(latitude > 80 && latitude <= 85)
            {
                if (body.name == "Kerbin") //Polar vortex, North Easterlies
                {
                    TradeWindTendancy = (North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 5;
                }
                else
                {
                    TradeWindTendancy = (North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 5;
                }
            }
            else if(latitude > 85 && latitude <= 90)
            {
                if (body.name == "Kerbin") //Polar Limit, Northerlies
                {
                    TradeWindTendancy = North; // *((float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 1;
                }
                else
                {
                    TradeWindTendancy = North; // *((float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 1;
                }
            }

                //Southern Hemisphere
            else if (latitude < -5 && latitude >= -10)
            {
                if (body.name == "Kerbin")//ITCZ - Hadley Cell, South Easterlies
                {
                    TradeWindTendancy = (-North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 6;
                }
                else
                {
                    TradeWindTendancy = (-North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 6;
                }
            }
            else if (latitude < -10 && latitude >= -15)
            {
                if (body.name == "Kerbin")//Hadley Cell, South Easterlies
                {
                    TradeWindTendancy = (-North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 6;
                }
                else
                {
                    TradeWindTendancy = (-North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 6;
                }
            }
            else if (latitude < -15 && latitude >= -20)
            {
                if (body.name == "Kerbin")//Hadley Cell, Southerlies
                {
                    TradeWindTendancy = -North; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }
                else
                {
                    TradeWindTendancy = -North; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }
            }
            else if (latitude < -20 && latitude >= -25)
            {
                if (body.name == "Kerbin")//Hadley Cell, Southerlies
                {
                    TradeWindTendancy = -North; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }
                else
                {
                    TradeWindTendancy = -North; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }
            }
            else if (latitude < -25 && latitude >= -30)
            {
                if (body.name == "Kerbin")//Hadley Cell - Sub-Tropical Ridge, North Westerlies
                {
                    TradeWindTendancy = (North + East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }
                else
                {
                    TradeWindTendancy = (North + East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }
            }
            else if (latitude < -30 && latitude >= -35)
            {
                if (body.name == "Kerbin")//Sub-Tropical Ridge, Westerlies
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }
                else
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }
            }
            else if (latitude < -35 && latitude >= -40)
            {
                if (body.name == "Kerbin")//Sub-Tropical Ridge - Ferrel Cell, North Westerlies
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }
                else
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }
            }
            else if (latitude < -40 && latitude >= -45)
            {
                if (body.name == "Kerbin")//Ferrel Cell, South Westerlies
                {
                    TradeWindTendancy = (-North + East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }
                else
                {
                    TradeWindTendancy = (-North + East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }

            }
            else if (latitude < -45 && latitude >= -50)
            {
                if (body.name == "Kerbin")//Ferrel Cell, North Westerlies
                {
                    TradeWindTendancy = (North + East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }
                else
                {
                    TradeWindTendancy = (North + East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }
            }
            else if (latitude < -50 && latitude >= -55)
            {
                if (body.name == "Kerbin")//Ferrel Cell, North Westerlies
                {
                    TradeWindTendancy = (North + East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }
                else
                {
                    TradeWindTendancy = (North + East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }
            }
            else if (latitude < -55 && latitude >= -60)
            {
                if (body.name == "Kerbin")//Polar Front, Entering Jet Stream, Westerlies
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }
                else
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }
            }
            else if (latitude < -60 && latitude >= -65)
            {
                if (body.name == "Kerbin")//Polar Vortex, Leaving Jet Stream, Westerlies
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }
                else
                {
                    TradeWindTendancy = East; // *((float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

            }
            else if (latitude < -65 && latitude >= -70)
            {
                if (body.name == "Kerbin")//Polar Vortex, South Easterlies
                {
                    TradeWindTendancy = (-North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 6;
                }
                else
                {
                    TradeWindTendancy = (-North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 6;
                }
            }
            else if (latitude < -70 && latitude >= -75)
            {
                if (body.name == "Kerbin")//Polar Vortex, South Easterlies
                {
                    TradeWindTendancy = (-North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 6;
                }
                else
                {
                    TradeWindTendancy = (-North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 6;
                }
            }
            else if (latitude < -75 && latitude >= -80)
            {
                if (body.name == "Kerbin")//Polar Vortex, South Easterlies
                {
                    TradeWindTendancy = (-North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 6;
                }
                else
                {
                    TradeWindTendancy = (-North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 6;
                }
            }
            else if (latitude < -80 && latitude >= -85)
            {
                if (body.name == "Kerbin")//Polar Vortex, South Easterlies
                {
                    TradeWindTendancy = (North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 6;
                }
                else
                {
                    TradeWindTendancy = (-North + -East).normalized; // *((float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 6;
                }
            }
            else if (latitude < -85 && latitude >= -90)
            {
                if (body.name == "Kerbin")//Polar Maximum, Southerlies
                {
                    TradeWindTendancy = -North; // *((float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }
                else
                {
                    TradeWindTendancy = -North; // *((float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }
            }
            else
            {
                Debug.Log("KWS: Orbiting body doesn't contain atmosphere or has not been added to climate database");
            }
            return TradeWindTendancy;
        }

        private static void GenerateWindGust(Vector3 windDirection)
        {
            //Idea: randomly changing values of x,y,z , then lerping smoothly to the new set of data, then after that
            //keep going for a set of time with the process repeating.
            if(isWindGust == true)
            {
                timeUntilGust = UnityEngine.Random.Range(0, 10);
                float numberx = UnityEngine.Random.Range(-20, 20);
                float numbery = UnityEngine.Random.Range(-20, 20);
                //float numberz = UnityEngine.Random.Range(-10, 10);
                //float windGustSpeed = UnityEngine.Random.Range(0,10);

                WindGust = new Vector3(numberx, numbery, 0);
                isWindVectorChanging = true;
                ChangeWindVector(windDirection, WindGust);

            }

            else
            {
                WindGust.Zero();
            }

            
        }

        public static Vector3 ChangeWindVector(Vector3 initialVector, Vector3 targetVector)
        {
            
            if(initialVector != targetVector)
            {
                initialVector = Vector3.Lerp(initialVector, targetVector, 0.1f);
                windGustDirection = initialVector;

                return initialVector;
            }
            else
            {
                isWindVectorChanging = false;
            }

            return windGustDirection;
        }

        public static float ChangeWindSpeed(float initialSpeed, float targetSpeed)
        {
            //Debug.Log("Changing Wind Speed");
            //initialSpeed = Mathfx.Berp(initialSpeed, targetSpeed,deltaTime);
            
            if(initialSpeed < targetSpeed)
            {
                initialSpeed = Mathf.Lerp(initialSpeed, targetSpeed, 0.0075f);
                windSpeed = initialSpeed;
            }
            else if(initialSpeed > targetSpeed)
            {
                initialSpeed = Mathf.Lerp(initialSpeed, targetSpeed, 0.0075f);
                windSpeed = initialSpeed;
            }
            
            if(Mathf.Approximately(windSpeed, targetSpeed))
            {
                isWindChanging = false;
            }

            //Debug.Log(initialSpeed);
            return windSpeed;
        }

        public static float KillWind(float windSpeed)
        {
           
            //Debug.Log("Killing Wind");

            KillingWind = true;


            windSpeed = Mathf.MoveTowards(windSpeed, 0.00f, windSpeed * 0.1f);
            


            if (Mathf.Approximately(windSpeed, 0.0f)) 
            {

                windSpeed = 0f;
                KillingWind = false;
                
                
                Debug.Log("Wind Killed");
               

            }
            
            return windSpeed;

        }

        public static float WindGustStorm(float windSpeed, float MaxWindGustSpeed, float WindGustTime)
        {
            //Debug.Log("Wind Storming");
            windSpeed = Mathf.MoveTowards(windSpeed, MaxWindGustSpeed, windSpeed * 0.05f);



            if (Mathf.Approximately(windSpeed, MaxWindGustSpeed))
            {
                //float WindGustTime1 = WindGustTime;

                WindGustTime1 -= 0.02f; //WindGustTime - float.Parse(Planetarium.fetch.fixedDeltaTime.ToString());
                //WindGustTime -= TimeWarp.fixedDeltaTime;
                //Debug.Log(WindGustTime1.ToString());
                if (WindGustTime1 <= 0)
                {
                    Debug.Log("WindGust Strongest, dying now");
                    isWindStorm = false;
                    KillWind(windSpeed);
                    stormEnded = true;
                }

            }
            return windSpeed;
        }

    }
}
