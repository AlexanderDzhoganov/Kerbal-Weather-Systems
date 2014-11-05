//Code copied from Rbray's mod EVE, all proper creditation goes to him for such work.

using KWSManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Atmosphere
{
    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class AtmosphereManager : GenericKWSManager<AtmosphereObject>
    {
        protected override ObjectType objectType { get { return ObjectType.PLANET | ObjectType.MULTIPLE; } }
        protected override String configName { get { return "KWS_ATMOSPHERE"; } }
        protected override void ApplyConfigNode(ConfigNode node, String body)
        {
            GameObject go = new GameObject();
            AtmosphereObject newObject = go.AddComponent<AtmosphereObject>();
            go.transform.parent = KWSManagerClass.GetCelestialBody(body).bodyTransform;
            newObject.LoadConfigNode(node, body);
            ObjectList.Add(newObject);
            newObject.Apply();
        }
        protected override void Clean()
        {
            foreach (AtmosphereObject obj in ObjectList)
            {
                obj.Remove();
                GameObject go = obj.gameObject;
                go.transform.parent = null;
                GameObject.DestroyImmediate(obj);
                GameObject.DestroyImmediate(go);
            }
            ObjectList.Clear();
        }
    }
}
