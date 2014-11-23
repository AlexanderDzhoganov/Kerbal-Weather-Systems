//Wind gusts will be "random"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;


namespace Weather
{
    public class Wind
    {
        public static bool KillingWind = false;
        public static bool isWindStorm = false;
        public static bool stormEnded = false;
        public static float WindGustTime1;
        public static int windDirectionNumb;
        public static Vector3 windDirection = new Vector3(1.0f, 0.0f, 0.0f);
        public static float windSpeed = 1.0f;
        public static string WindDirectionLabel = string.Empty;
        public static Vector3 vesselVector;
        public static Vector3 resultantWindVector;
        public static Vector3 localWindVector;

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

        public void Awake()
        {
            windDirection.x = 1;
            windDirection.y = 0;
            windDirection.z = 0;
        }

        void Update()
        {

        }

        public static void FixedUpdate()
        {

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
            windSpeed = HeadMaster.windSpeed;

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

            windDirection.x = Mathf.Max(localWindVector.x, WindDirTendancy(Latitude, orbitingBody).x);
            windDirection.y = Mathf.Max(localWindVector.y, WindDirTendancy(Latitude, orbitingBody).y);
            windDirection.z = Mathf.Max(localWindVector.z, WindDirTendancy(Latitude, orbitingBody).z);

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
            Vector3d coriolisAcc = FlightGlobals.getCoriolisAcc(WindDirTendancy(Latitude, body), body);

            //Check for orbiting body and apply proper wind patterns
            if (orbitingBodyName == "Kerbin")
            {
                if(Latitude >= -5 && Latitude < 0)
                {

                }
                //Trade wind stuff below
                if (Latitude >= 0 && Latitude <= 5) //Easterly trade wind at the Inter-Tropical Convergence zone
                {
                    FlightGlobals.getCoriolisAcc(TradeWindTendancy, body);
                    //Debug.Log("Wind is Easterly!");
                    //This area is nominally of lower pressure
                    TradeWindTendancy = Vector3.Cross((-North * (windSpeed * (float)CurrentAtmoPressure)), coriolisAcc); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 2;
                }

                if (Latitude > 5 && Latitude <= 15) //Hadley Cell, North Easterly trade wind
                {
                    TradeWindTendancy = (North + -East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 5;
                }

                if (Latitude > 15 && Latitude <= 20) //Hadley Cell, Northerly trade wind.
                {
                    TradeWindTendancy = North * (windSpeed * (float)CurrentAtmoPressure);// * RecipDeltaAtmoPressure);
                    windDirectionNumb = 1;
                }

                if (Latitude > 20 && Latitude <= 27) //Hadley Cell - Sub-Tropical Ridge, North Westerly trade wind
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }

                if (Latitude > 27 && Latitude <= 33) //Sub-tropical Ridge, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude > 33 && Latitude <= 40) //Sub-Tropical Ridge - Ferrel Cell, Mid-Latitude. Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude > 40 && Latitude <= 80) //Ferrel Cell - Polar Cell/Vortex, High Latitude. High Pressure, North Westerlies
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }

                if (Latitude > 80 && Latitude <= 90) //Extreme high polar region, High Pressure, Northerlies
                {
                    TradeWindTendancy = North * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 1;
                }


                //Southern hemisphere
                if (Latitude < -5 && Latitude >= -15) //ITCZ - Hadley Cell, South Easterlies 
                {
                    TradeWindTendancy = (-North + -East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 6;
                }

                if (Latitude < -15 && Latitude >= -20) //Hadley Cell, Southerlies
                {
                    TradeWindTendancy = -North * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }

                if (Latitude < -20 && Latitude >= -27) //Hadley Cell - Sub-Tropical Ridge, North Westerlies
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }

                if (Latitude < -27 && Latitude >= -33) //Sub-Tropical Ridge, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude < -33 && Latitude >= -40)//Sub-Tropical Ridge - Ferrel Cell, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude < -40 && Latitude >= -80) //Ferrel Cell - Polar Cell/Vortex, High Latitude. High Pressure, South Westerlies
                {
                    TradeWindTendancy = (-North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }

                if (Latitude < -80 && Latitude >= -90) //Polar Cell, High Latitude, High Pressure, Southerlies
                {
                    TradeWindTendancy = -North * (windSpeed * (float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }
                /*
                //Trade wind stuff below
                if (Latitude >= -5 && Latitude <= 5) //Easterly trade wind at the Inter-Tropical Convergence zone
                {
                    FlightGlobals.getCoriolisAcc(TradeWindTendancy, body);
                    //Debug.Log("Wind is Easterly!");
                    //This area is nominally of lower pressure
                    TradeWindTendancy = -East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 2;
                }

                if (Latitude > 5 && Latitude <= 15) //Hadley Cell, North Easterly trade wind
                {
                    TradeWindTendancy = (North + -East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 5;
                }

                if (Latitude > 15 && Latitude <= 20) //Hadley Cell, Northerly trade wind.
                {
                    TradeWindTendancy = North * (windSpeed * (float)CurrentAtmoPressure);// * RecipDeltaAtmoPressure);
                    windDirectionNumb = 1;
                }

                if (Latitude > 20 && Latitude <= 27) //Hadley Cell - Sub-Tropical Ridge, North Westerly trade wind
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }

                if (Latitude > 27 && Latitude <= 33) //Sub-tropical Ridge, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude > 33 && Latitude <= 40) //Sub-Tropical Ridge - Ferrel Cell, Mid-Latitude. Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude > 40 && Latitude <= 80) //Ferrel Cell - Polar Cell/Vortex, High Latitude. High Pressure, North Westerlies
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }

                if (Latitude > 80 && Latitude <= 90) //Extreme high polar region, High Pressure, Northerlies
                {
                    TradeWindTendancy = North * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 1;
                }


                //Southern hemisphere
                if (Latitude < -5 && Latitude >= -15) //ITCZ - Hadley Cell, South Easterlies 
                {
                    TradeWindTendancy = (-North + -East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 6;
                }

                if (Latitude < -15 && Latitude >= -20) //Hadley Cell, Southerlies
                {
                    TradeWindTendancy = -North * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }

                if (Latitude < -20 && Latitude >= -27) //Hadley Cell - Sub-Tropical Ridge, North Westerlies
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }

                if (Latitude < -27 && Latitude >= -33) //Sub-Tropical Ridge, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude < -33 && Latitude >= -40)//Sub-Tropical Ridge - Ferrel Cell, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude < -40 && Latitude >= -80) //Ferrel Cell - Polar Cell/Vortex, High Latitude. High Pressure, South Westerlies
                {
                    TradeWindTendancy = (-North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }

                if (Latitude < -80 && Latitude >= -90) //Polar Cell, High Latitude, High Pressure, Southerlies
                {
                    TradeWindTendancy = -North * (windSpeed * (float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }
                 */ 
            }

            else if (orbitingBodyName == "Duna")
            {
                //Trade wind stuff below
                if (Latitude >= -5 && Latitude <= 5) //Easterly trade wind at the Inter-Tropical Convergence zone
                {
                    //Debug.Log("Wind is Easterly!");
                    //This area is nominally of lower pressure
                    TradeWindTendancy = -East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 2;
                }

                if (Latitude > 5 && Latitude <= 15) //Hadley Cell, North Easterly trade wind
                {
                    TradeWindTendancy = (North + -East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 5;
                }

                if (Latitude > 15 && Latitude <= 20) //Hadley Cell, Northerly trade wind.
                {
                    TradeWindTendancy = North * (windSpeed * (float)CurrentAtmoPressure);// * RecipDeltaAtmoPressure);
                    windDirectionNumb = 1;
                }

                if (Latitude > 20 && Latitude <= 27) //Hadley Cell - Sub-Tropical Ridge, North Westerly trade wind
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }

                if (Latitude > 27 && Latitude <= 33) //Sub-tropical Ridge, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude > 33 && Latitude <= 40) //Sub-Tropical Ridge - Ferrel Cell, Mid-Latitude. Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude > 40 && Latitude <= 80) //Ferrel Cell - Polar Cell/Vortex, High Latitude. High Pressure, North Westerlies
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }

                if (Latitude > 80 && Latitude <= 90) //Extreme high polar region, High Pressure, Northerlies
                {
                    TradeWindTendancy = North * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 1;
                }


                //Southern hemisphere
                if (Latitude < -5 && Latitude >= -15) //ITCZ - Hadley Cell, South Easterlies 
                {
                    TradeWindTendancy = (-North + -East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 6;
                }

                if (Latitude < -15 && Latitude >= -20) //Hadley Cell, Southerlies
                {
                    TradeWindTendancy = -North * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }

                if (Latitude < -20 && Latitude >= -27) //Hadley Cell - Sub-Tropical Ridge, North Westerlies
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }

                if (Latitude < -27 && Latitude >= -33) //Sub-Tropical Ridge, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude < -33 && Latitude >= -40)//Sub-Tropical Ridge - Ferrel Cell, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude < -40 && Latitude >= -80) //Ferrel Cell - Polar Cell/Vortex, High Latitude. High Pressure, South Westerlies
                {
                    TradeWindTendancy = (-North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }

                if (Latitude < -80 && Latitude >= -90) //Polar Cell, High Latitude, High Pressure, Southerlies
                {
                    TradeWindTendancy = -North * (windSpeed * (float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }
            }

            else if (orbitingBodyName == "Eve")
            {
                //Trade wind stuff below
                if (Latitude >= -5 && Latitude <= 5) //Easterly trade wind at the Inter-Tropical Convergence zone
                {
                    //Debug.Log("Wind is Easterly!");
                    //This area is nominally of lower pressure
                    TradeWindTendancy = -East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 2;
                }

                if (Latitude > 5 && Latitude <= 15) //Hadley Cell, North Easterly trade wind
                {
                    TradeWindTendancy = (North + -East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 5;
                }

                if (Latitude > 15 && Latitude <= 20) //Hadley Cell, Northerly trade wind.
                {
                    TradeWindTendancy = North * (windSpeed * (float)CurrentAtmoPressure);// * RecipDeltaAtmoPressure);
                    windDirectionNumb = 1;
                }

                if (Latitude > 20 && Latitude <= 27) //Hadley Cell - Sub-Tropical Ridge, North Westerly trade wind
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }

                if (Latitude > 27 && Latitude <= 33) //Sub-tropical Ridge, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude > 33 && Latitude <= 40) //Sub-Tropical Ridge - Ferrel Cell, Mid-Latitude. Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude > 40 && Latitude <= 80) //Ferrel Cell - Polar Cell/Vortex, High Latitude. High Pressure, North Westerlies
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }

                if (Latitude > 80 && Latitude <= 90) //Extreme high polar region, High Pressure, Northerlies
                {
                    TradeWindTendancy = North * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 1;
                }


                //Southern hemisphere
                if (Latitude < -5 && Latitude >= -15) //ITCZ - Hadley Cell, South Easterlies 
                {
                    TradeWindTendancy = (-North + -East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 6;
                }

                if (Latitude < -15 && Latitude >= -20) //Hadley Cell, Southerlies
                {
                    TradeWindTendancy = -North * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }

                if (Latitude < -20 && Latitude >= -27) //Hadley Cell - Sub-Tropical Ridge, North Westerlies
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }

                if (Latitude < -27 && Latitude >= -33) //Sub-Tropical Ridge, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude < -33 && Latitude >= -40)//Sub-Tropical Ridge - Ferrel Cell, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude < -40 && Latitude >= -80) //Ferrel Cell - Polar Cell/Vortex, High Latitude. High Pressure, South Westerlies
                {
                    TradeWindTendancy = (-North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }

                if (Latitude < -80 && Latitude >= -90) //Polar Cell, High Latitude, High Pressure, Southerlies
                {
                    TradeWindTendancy = -North * (windSpeed * (float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }
            }

            else if (orbitingBodyName == "Laythe")
            {
                //Trade wind stuff below
                if (Latitude >= -5 && Latitude <= 5) //Easterly trade wind at the Inter-Tropical Convergence zone
                {
                    //Debug.Log("Wind is Easterly!");
                    //This area is nominally of lower pressure
                    TradeWindTendancy = -East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 2;
                }

                if (Latitude > 5 && Latitude <= 15) //Hadley Cell, North Easterly trade wind
                {
                    TradeWindTendancy = (North + -East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 5;
                }

                if (Latitude > 15 && Latitude <= 20) //Hadley Cell, Northerly trade wind.
                {
                    TradeWindTendancy = North * (windSpeed * (float)CurrentAtmoPressure);// * RecipDeltaAtmoPressure);
                    windDirectionNumb = 1;
                }

                if (Latitude > 20 && Latitude <= 27) //Hadley Cell - Sub-Tropical Ridge, North Westerly trade wind
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }

                if (Latitude > 27 && Latitude <= 33) //Sub-tropical Ridge, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude > 33 && Latitude <= 40) //Sub-Tropical Ridge - Ferrel Cell, Mid-Latitude. Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude > 40 && Latitude <= 80) //Ferrel Cell - Polar Cell/Vortex, High Latitude. High Pressure, North Westerlies
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }

                if (Latitude > 80 && Latitude <= 90) //Extreme high polar region, High Pressure, Northerlies
                {
                    TradeWindTendancy = North * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 1;
                }


                //Southern hemisphere
                if (Latitude < -5 && Latitude >= -15) //ITCZ - Hadley Cell, South Easterlies 
                {
                    TradeWindTendancy = (-North + -East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 6;
                }

                if (Latitude < -15 && Latitude >= -20) //Hadley Cell, Southerlies
                {
                    TradeWindTendancy = -North * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }

                if (Latitude < -20 && Latitude >= -27) //Hadley Cell - Sub-Tropical Ridge, North Westerlies
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }

                if (Latitude < -27 && Latitude >= -33) //Sub-Tropical Ridge, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude < -33 && Latitude >= -40)//Sub-Tropical Ridge - Ferrel Cell, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude < -40 && Latitude >= -80) //Ferrel Cell - Polar Cell/Vortex, High Latitude. High Pressure, South Westerlies
                {
                    TradeWindTendancy = (-North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }

                if (Latitude < -80 && Latitude >= -90) //Polar Cell, High Latitude, High Pressure, Southerlies
                {
                    TradeWindTendancy = -North * (windSpeed * (float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }
            }

            else if (orbitingBodyName == "Jool")
            {
                //Trade wind stuff below
                if (Latitude >= -5 && Latitude <= 5) //Easterly trade wind at the Inter-Tropical Convergence zone
                {
                    //Debug.Log("Wind is Easterly!");
                    //This area is nominally of lower pressure
                    TradeWindTendancy = -East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 2;
                }

                if (Latitude > 5 && Latitude <= 15) //Hadley Cell, North Easterly trade wind
                {
                    TradeWindTendancy = (North + -East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure); 
                    windDirectionNumb = 5;
                }

                if (Latitude > 15 && Latitude <= 20) //Hadley Cell, Northerly trade wind.
                {
                    TradeWindTendancy = North * (windSpeed * (float)CurrentAtmoPressure);// * RecipDeltaAtmoPressure);
                    windDirectionNumb = 1;
                }

                if (Latitude > 20 && Latitude <= 27) //Hadley Cell - Sub-Tropical Ridge, North Westerly trade wind
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }

                if (Latitude > 27 && Latitude <= 33) //Sub-tropical Ridge, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude > 33 && Latitude <= 40) //Sub-Tropical Ridge - Ferrel Cell, Mid-Latitude. Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude > 40 && Latitude <= 80) //Ferrel Cell - Polar Cell/Vortex, High Latitude. High Pressure, North Westerlies
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 8;
                }

                if (Latitude > 80 && Latitude <= 90) //Extreme high polar region, High Pressure, Northerlies
                {
                    TradeWindTendancy = North * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 1;
                }


                //Southern hemisphere
                if (Latitude < -5 && Latitude >= -15) //ITCZ - Hadley Cell, South Easterlies 
                {
                    TradeWindTendancy = (-North + -East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 6;
                }

                if (Latitude < -15 && Latitude >= -20) //Hadley Cell, Southerlies
                {
                    TradeWindTendancy = -North * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }

                if (Latitude < -20 && Latitude >= -27) //Hadley Cell - Sub-Tropical Ridge, North Westerlies
                {
                    TradeWindTendancy = (North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }

                if (Latitude < -27 && Latitude >= -33) //Sub-Tropical Ridge, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude < -33 && Latitude >= -40)//Sub-Tropical Ridge - Ferrel Cell, Westerlies
                {
                    TradeWindTendancy = East * (windSpeed * (float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 4;
                }

                if (Latitude < -40 && Latitude >= -80) //Ferrel Cell - Polar Cell/Vortex, High Latitude. High Pressure, South Westerlies
                {
                    TradeWindTendancy = (-North + East).normalized * (windSpeed * (float)CurrentAtmoPressure); // * RecipDeltaAtmoPressure);
                    windDirectionNumb = 7;
                }

                if (Latitude < -80 && Latitude >= -90) //Polar Cell, High Latitude, High Pressure, Southerlies
                {
                    TradeWindTendancy = -North * (windSpeed * (float)CurrentAtmoPressure); //* RecipDeltaAtmoPressure);
                    windDirectionNumb = 3;
                }
            }

            else
            {
                Debug.Log("KWS: Orbiting body doesn't contain atmosphere or has not been added to climate database");
            }
            return TradeWindTendancy;
        }

        public void WindGusts(float windSpeed, Vector3 windDirection)
        {



        }

        public static float KillWind(float windSpeed)
        {
           
            //Debug.Log("Killing Wind");

            KillingWind = true;


            windSpeed = Mathf.MoveTowards(windSpeed, 0.00f, windSpeed / 10.0f);



            if (Mathf.Approximately(windSpeed, 0.0f)) 
            { 

                windSpeed = 0.001f;
                KillingWind = false;
                if(stormEnded == false)
                {
                    isWindStorm = true;
                    
                }
                
                Debug.Log("Wind Killed");
               

            }
            
            return windSpeed;

        }

        public static float WindGustStorm(float windSpeed, float MaxWindGustSpeed, float WindGustTime)
        {

            if(isWindStorm == true)
            {
                //Debug.Log("Wind Storming");
                windSpeed = Mathf.MoveTowards(windSpeed, MaxWindGustSpeed, windSpeed * 0.05f);

                

                if (Mathf.Approximately(windSpeed, MaxWindGustSpeed))
                {
                    //float WindGustTime1 = WindGustTime;

                    WindGustTime1 -= 0.02f; //WindGustTime - float.Parse(Planetarium.fetch.fixedDeltaTime.ToString());
                    //WindGustTime -= TimeWarp.fixedDeltaTime;
                    //Debug.Log(WindGustTime1.ToString());
                    if(WindGustTime1 <= 0)
                    {
                        Debug.Log("WindGust Strongest, dying now");
                        isWindStorm = false;
                        KillWind(windSpeed);
                        stormEnded = true;
                    }
                    
                }
                return windSpeed;

            }

            return windSpeed;
        }

    }
}
