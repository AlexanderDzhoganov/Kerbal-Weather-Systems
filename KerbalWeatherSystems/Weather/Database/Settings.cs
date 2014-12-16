using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

namespace Database
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class Settings : MonoBehaviour
    {
        internal static int cellDefinitionAlt = 2500;
        internal static double cellDefinitionWidth = 1;
    }
}
