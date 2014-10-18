using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using KSP.IO;

using Random = UnityEngine.Random;

namespace KerbalWeatherSystems.Extensions
{

    public class Clouds2DMaterial
    {
        //Material data goes here
    }

    class Clouds2D
    {
        GameObject CloudMesh; //Mesh for the clouds.
        Material CloudMaterial; //Material the cloud will be made of.
        Projector ShadowProjector = null;
        GameObject ShadowProjectorGO = null;

        [Persistent] //means it persists through scene change.
        float detailSpeed;
        [Persistent]
        Vector3 offset = new Vector3(0, 0, 0);
        [Persistent]
        bool shadow = false;
        [Persistent]
        Vector3 shadowOffset = new Vector3(0, 0, 0);
        [Persistent]
        Clouds2DMaterial macroCloudMaterial;
        [Persistent]
        Clouds2DMaterial scaledCloudMaterial;


    }
}
