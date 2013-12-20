using UnityEngine;
using KSP.IO;

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

        /*
* Called when the game is leaving the scene (or exiting). Perform any clean up work here.
*/
        void OnDestroy()
        {
            
        }
    }
}
