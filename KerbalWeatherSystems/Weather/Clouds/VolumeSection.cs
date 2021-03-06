﻿//Code copied from Rbray's mod EVE, all proper credits go to him for such hard work.

using Utils;
using OverlaySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Clouds
{
    class CloudParticle
    {
        private static System.Random Random = new System.Random();
        GameObject particle;
        public CloudParticle(Material cloudParticleMaterial, Transform parent, Vector3 pos, float magnitude)
        {
            //Debug.Log("Cloud Particle Called!"); Called on Space Center
            particle = new GameObject();
            particle.transform.parent = parent;
            particle.transform.localPosition = pos;
            Vector3 bodyPoint = parent.parent.InverseTransformPoint(particle.transform.position).normalized * magnitude;
            particle.transform.position = parent.parent.TransformPoint(bodyPoint);
            float x = 500 * ((float)Random.NextDouble() - .5f);
            float y = 500 * ((float)Random.NextDouble() - .5f);
            float z = 500 * ((float)Random.NextDouble() - .5f);
            particle.transform.localPosition += new Vector3(x, y, z);
            Vector3 worldUp = particle.transform.position - parent.parent.position;
            particle.transform.up = worldUp.normalized;
            x = 360f * (float)Random.NextDouble();
            y = 360f * (float)Random.NextDouble();
            z = 360f * (float)Random.NextDouble();
            particle.transform.localRotation = Quaternion.Euler(x, y, z);
            particle.transform.localScale = Vector3.one;
            particle.layer = OverlayManager.MACRO_LAYER;
            Vector3 up = particle.transform.InverseTransformDirection(worldUp);
            Quad.Create(particle, Random.Next(2500, 4500), Color.white, up);
            var mr = particle.AddComponent<MeshRenderer>();
            mr.sharedMaterial = cloudParticleMaterial;
            mr.castShadows = false; //used to be false
            mr.receiveShadows = false;
            mr.enabled = true;
        }
        public void Update(Texture2D tex)
        {
            //Debug.Log("Updating Texture"); is being called
            Vector3 point = particle.transform.parent.parent.InverseTransformPoint(particle.transform.position).normalized;
            float u = (float)(.5 + (Mathf.Atan2(point.z, point.x) / (2f * Mathf.PI)));
            float v = Mathf.Acos(-point.y) / Mathf.PI;
            Color color = tex.GetPixelBilinear(u, v);
            MeshFilter filter = particle.GetComponent<MeshFilter>();
            Mesh mesh = filter.mesh;
            mesh.colors = new Color[4]
            {
                new Color(color.r, color.g, color.b, color.a),
                new Color(color.r, color.g, color.b, color.a),
                new Color(color.r, color.g, color.b, color.a),
                new Color(color.r, color.g, color.b, color.a)
            };
        }
        internal void Update(Color color)
        {
            Debug.Log("Updating Colour");
            MeshFilter filter = particle.GetComponent<MeshFilter>();
            Mesh mesh = filter.mesh;
            mesh.colors = new Color[4]
            {
                new Color(color.r, color.g, color.b, color.a),
                new Color(color.r, color.g, color.b, color.a),
                new Color(color.r, color.g, color.b, color.a),
                new Color(color.r, color.g, color.b, color.a)
            };
        }
        internal void Destroy()
        {
            //Debug.Log("Particle Destroyed!"); Is being called
            GameObject.DestroyImmediate(particle);
        }
    }
    class VolumeSection
    {
        private static System.Random Random = new System.Random();
        GameObject segment;
        Vector3 offset;
        float magnitude;
        List<CloudParticle> Particles = new List<CloudParticle>();
        Texture2D texture;
        public Vector3 Center { get { return segment.transform.localPosition; } }
        public Vector3 Offset { get { return offset; } set { offset = value; } }
        public bool Enabled { get { return segment.activeSelf; } set { segment.SetActive(value); } }
        public VolumeSection(Texture2D tex, Material cloudParticleMaterial, Transform parent, Vector3 pos, float magnitude, Vector3 offset, float radius, int divisions)
        {
            texture = tex;
            segment = new GameObject();
            HexSeg hexGeometry = new HexSeg(radius, divisions);
            Reassign(pos, offset, magnitude, parent);
            List<Vector3> positions = hexGeometry.GetPoints();
            foreach (Vector3 position in positions)
            {
                //Debug.Log("Adding Particles..."); adding particles, they are added before volume is initialized...?
                Particles.Add(new CloudParticle(cloudParticleMaterial, segment.transform, position, magnitude));
            }
        }
        public VolumeSection(Material cloudParticleMaterial, Transform parent, Vector3 pos, float magnitude, Vector3 offset, float radius, int divisions)
        {
            texture = null;
            segment = new GameObject();
            HexSeg hexGeometry = new HexSeg(radius, divisions);
            Reassign(pos, offset, magnitude, parent);
            List<Vector3> positions = hexGeometry.GetPoints();
            foreach (Vector3 position in positions)
            {
                //Debug.Log("Adding Particles..."); adding particles, they are added before volume is initialized...?
                Particles.Add(new CloudParticle(cloudParticleMaterial, segment.transform, position, magnitude));
            }
        }
        public void Reassign(Vector3 pos, Vector3 offset, float magnitude = -1, Transform parent = null)
        {
            //Debug.Log("Reassign called"); Called on SpaceCenter
            if (parent != null)
            {
                //Debug.Log("Parent != null"); Called on SpaceCenter
                segment.transform.parent = parent;
            }
            this.offset = offset;
            if (magnitude > 0)
            {
                //Debug.Log("magnitude > 0"); Called on SpaceCenter
                this.magnitude = magnitude;
            }
            segment.transform.localPosition = pos;
            Vector3 worldUp = segment.transform.position - segment.transform.parent.position;
            segment.transform.up = worldUp.normalized;
            segment.transform.localScale = Vector3.one;
            segment.transform.Translate(offset);
            worldUp = segment.transform.position - segment.transform.parent.position;
            segment.transform.up = worldUp.normalized;
            segment.transform.localPosition = this.magnitude * segment.transform.localPosition.normalized;
        }
        public void Update()
        {
            foreach (CloudParticle particle in Particles)
            {
                particle.Update(texture);
                //Debug.Log("Texture Updated!"); Particle textures are being updated
            }
        }
        public void UpdateColor(Color color)
        {
            foreach (CloudParticle particle in Particles)
            {
                particle.Update(color);
                Debug.Log("Updated Color!");
            }
        }
        internal void Destroy()
        {
            foreach (CloudParticle particle in Particles)
            {
                //Debug.Log("Particle Destroyed"); Called during Flight
                particle.Destroy();
            }
            GameObject.DestroyImmediate(segment);
        }
    }
}
