﻿//Code copied from Rbray's mod EVE, all proper credits go to him for such hard work

using KWSManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PQSManager
{
    public class PQSWrapper : PQS, IKWSObject
    {
        [Persistent]
        float deactivateDistance = 175000;
        String body;
        ConfigNode node;
        float cameraDistance;
        public String Name { get { return body; } set { } }
        public ConfigNode ConfigNode { get { return node; } }
        public String Body { get { return body; } }
        public void LoadConfigNode(ConfigNode node, String body)
        {
            ConfigNode.LoadObjectFromConfig(this, node);
            this.node = node;
            this.body = body;
            name = node.name;
        }
        public ConfigNode GetConfigNode()
        {
            return ConfigNode.CreateConfigFromObject(this, new ConfigNode(body));
        }
        public void Apply()
        {
            CelestialBody cb = KWSManagerClass.GetCelestialBody(body);
            cameraDistance = (float)cb.Radius + deactivateDistance;
            this.isActive = false;
            this.relativeTargetPosition = Vector3.zero;
        }
        public void Remove() { }
        protected void Start()
        {
            this.isActive = false;
        }
        protected void Update()
        {
            if (HighLogic.LoadedScene == GameScenes.FLIGHT && FlightCamera.fetch != null)
            {
                FlightCamera cam = FlightCamera.fetch;
                float dist = Vector3.Distance(cam.mainCamera.transform.position, this.transform.position);
                if (dist < cameraDistance)
                {
                    if (!this.isActive)
                    {
                        this.isActive = true;
                        PQSMod[] mods = this.GetComponentsInChildren<PQSMod>();
                        foreach (PQSMod mod in mods)
                        {
                            mod.OnSphereActive();
                        }
                    }
                }
                else
                {
                    if (this.isActive)
                    {
                        this.isActive = false;
                        PQSMod[] mods = this.GetComponentsInChildren<PQSMod>();
                        foreach (PQSMod mod in mods)
                        {
                            mod.OnSphereInactive();
                        }
                    }
                }
                if (cam.Target != null)
                {
                    this.relativeTargetPosition = this.transform.InverseTransformPoint(cam.Target.position);
                    this.target = cam.Target;
                }
            }
            else
            {
                this.isActive = false;
            }
        }
    }
}
