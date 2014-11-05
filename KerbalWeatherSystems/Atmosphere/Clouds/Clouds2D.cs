using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using KSP.IO;
using Utils;
using KWSManager;

using Random = UnityEngine.Random;

namespace Atmosphere
{

    public class Clouds2DMaterial : KWSMaterialManager
    {
        //Material data goes here
        [Persistent]
        Color _Color = new Color(1, 1, 1, 1);
        [Persistent]
        String _MainTex = "";
        [Persistent]
        String _DetailTex = "";
        [Persistent]
        float _FalloffPow = 2f;
        [Persistent]
        float _FalloffScale = 3f;
        [Persistent]
        float _DetailScale = 100f;
        [Persistent]
        Vector3 _DetailOffset = new Vector3(0, 0, 0);
        [Persistent]
        float _DetailDist = 0.000002f;
        [Persistent]
        float _MinLight = .5f;
        [Persistent]
        float _FadeDist = 8f;
        [Persistent]
        float _FadeScale = 0.00375f;
        [Persistent]
        float _RimDist = 0.0001f;
        [Persistent]
        float _RimDistSub = 1.01f;
        [Persistent]
        float _InvFade = .008f;
        
    }

    class Clouds2D
    {
        GameObject CloudMesh; //Mesh for the clouds.
        Material CloudMaterial; //Material the cloud will be made of.
        Projector ShadowProjector = null; //Used to project the texture's properties onto the particles.
        GameObject ShadowProjectorGO = null; //The shadow projector's gameobject.

        [Persistent] //means it persists through scene change.
        float detailSpeed;
        [Persistent]
        Vector3 offset = new Vector3(0, 0, 0); //the offset of the texture
        [Persistent]
        bool shadow = true; //shadows?
        [Persistent]
        Vector3 shadowOffset = new Vector3(0, 0, 0); //offset of the shadows
        [Persistent]
        Clouds2DMaterial macroCloudMaterial; //Large scale cloud material
        [Persistent]
        Clouds2DMaterial scaledCloudMaterial; //Small scale cloud material

        bool isScaled = false;
        public bool Scaled //Scaled layer things
        {
            get { return CloudMesh.layer == KWSManagerClass.SCALED_LAYER; } //get and return the scaled layer (EVE's = 10)
            set //set the value
            {
                if (isScaled != value) //if there is a value
                {
                    scaledCloudMaterial.ApplyMaterialProperties(CloudMaterial); //Apply the materialistic properties
                    float scale = (float)(1000f / celestialBody.Radius); 
                    CloudMaterial.DisableKeyword("SOFT_DEPTH_ON");
                    Reassign(KWSManager.KWSManagerClass.SCALED_LAYER, scaledCelestialTransform, scale);
                }
                else //Set the macrocloud material
                {
                    macroCloudMaterial.ApplyMaterialProperties(CloudMaterial); //apply the macro material properties.
                    CloudMaterial.EnableKeyword("SOFT_DEPTH_ON");
                    Reassign(KWSManager.KWSManagerClass.MACRO_LAYER, celestialBody.transform, 1); //reassign
                }
                isScaled = value;
            }
        }
         
        CelestialBody celestialBody = null; 
        Transform scaledCelestialTransform = null; //Set the scaled Celestialbody transform
        Transform sunTransform; //Set the transform in relation to the sun?
        float radius; //Radius of the main body to use as a ratio to set the layer height.
        float radiusScale; //The scaled ratio of the body radius?
        float globalPeriod; //The global period of things?
        float mainPeriodOffset; //The offset of the main period
        float shadowPeriod; //the period of the shadow

        private static Shader cloudShader = null; //Material shader for the cloud.
        private static Shader CloudShader //Function to get and set the cloud shader.
        {
            get
            {
                if (cloudShader == null) //If there is no cloudShader loaded.
                {
                    Assembly assembly = Assembly.GetExecutingAssembly(); //Currently executing assembly.
                    cloudShader = KWSManager.KWSManagerClass.GetShader(assembly, "Atmosphere.Shaders.Compiled-SphereCloud.shader"); //get the shader from the assembly
                } 
                return cloudShader; //return the cloud shader.
            }
        }

        private static Shader cloudShadowShader = null;  //Set the Cloud shadow shader to null
        private static Shader CloudShadowShader //function to get and set the cloud shadow shader.
        {
            get
            {
                if (cloudShadowShader == null) //If the cloudShadowShader is null
                {
                    Assembly assembly = Assembly.GetExecutingAssembly(); //Currently executing assembly.
                    cloudShadowShader = KWSManagerClass.GetShader(assembly, "Atmosphere.Shaders.Compiled-CloudShadow.shader"); //Get and set the cloudshadow shader from the assembly
                } 
                return cloudShadowShader; //Returns the cloudShadowShader.
            }
        }

        //Applies the Cloud shader, perhaps could change the speed to be the magnitude of a vector?
        internal void Apply(CelestialBody celestialBody, Transform scaledCelestialTransform, float radius, float speed)
        {
            Remove(); //call the remove method to remove the clouds
            this.celestialBody = celestialBody; 
            this.scaledCelestialTransform = scaledCelestialTransform;
            CloudMaterial = new Material(CloudShader); //Cloud material used.
            HalfSphere hp = new HalfSphere(radius, CloudMaterial); //Refers to the Halfsphere class.
            CloudMesh = hp.GameObject; //Sets the cloud mesh as the half sphere
            Scaled = true; //is it scaled?
            this.radius = radius; //gets the radius of the things
            float circumference = 2f * Mathf.PI * radius;
            globalPeriod = (speed + detailSpeed) / circumference;
            mainPeriodOffset = (-detailSpeed) / circumference; //The main offset of the period
            shadowPeriod = (speed) / circumference; //sets the shadow period

            if (shadow) //If shadows are true
            {
                ShadowProjectorGO = new GameObject(); //Sets the game object for the shadow projector
                ShadowProjector = ShadowProjectorGO.AddComponent<Projector>(); //Add the component for the projector onto the game object
                ShadowProjector.nearClipPlane = 10;
                ShadowProjector.fieldOfView = 60; //projector's field of view
                ShadowProjector.aspectRatio = 1; //aspect ratio of the projection
                ShadowProjector.orthographic = true; //is orthographic view or perspective?
                ShadowProjector.transform.parent = celestialBody.transform;
                ShadowProjector.material = new Material(CloudShadowShader); //Set the Shadow projector material as the cloudshader material.
            }
            sunTransform = Sun.Instance.sun.transform; //sets the transform in relation to the sun.
        }

        //Reassigns the cloud layer
        public void Reassign(int layer, Transform parent, float scale)
        {
            CloudMesh.transform.parent = parent; //sets the parent transform
            CloudMesh.transform.localPosition = Vector3.zero; //zeroes for localposition
            CloudMesh.transform.localScale = scale * Vector3.one; //Sets the transform scale
            CloudMesh.layer = layer;

            radiusScale = radius * scale; //scales the radius
            float worldRadiusScale = Vector3.Distance(parent.transform.TransformPoint(Vector3.up * radiusScale), parent.transform.TransformPoint(Vector3.zero)); //gets the scale of the world radius

            if (ShadowProjector != null) //if there is a shadow projector
            {
                float dist = (float)(2 * worldRadiusScale);
                ShadowProjector.farClipPlane = dist;
                ShadowProjector.orthographicSize = worldRadiusScale; //sets the scale of the world radius.

                ShadowProjector.transform.parent = parent;
                //ShadowProjector.transform.localScale = scale * Vector3.one; //commented out for a reason.
                ShadowProjector.material.SetTexture("_ShadowTex", CloudMaterial.mainTexture); //set the shadow texture
                ShadowProjectorGO.layer = layer; //sets the shadowprojector gameobject's layer as the cloud mesh layer.
                
                if (layer == KWSManagerClass.MACRO_LAYER) //if the layer is = the macro layer
                {
                    ShadowProjector.ignoreLayers = ~((1 << 19) | (1 << 15) | 2 | 1); //layers to ignore
                    sunTransform = KWSManagerClass.GetCelestialBody(Sun.Instance.sun.bodyName).transform; //Sun transform
                }
                else //set the layer as the second scaled layer
                {
                    ShadowProjectorGO.layer = KWSManagerClass.SCALED_LAYER2;
                    ShadowProjector.ignoreLayers = ~(1 << KWSManagerClass.SCALED_LAYER2);// ~((1 << 29) | (1 << 23) | (1 << 18) | (1 << 10) | (1 << 9));
                    sunTransform = KWSManagerClass.GetScaledTransform(Sun.Instance.sun.bodyName);
                    //Atmosphere.Log("Camera mask: " + ScaledCamera.Instance.camera.cullingMask);
                }
                 
            }
        }

        //Removes/nullifies the cloud meshes and shadow projectors
        public void Remove()
        {
            if (CloudMesh != null)
            {
                CloudMesh.transform.parent = null;
                GameObject.DestroyImmediate(CloudMesh);
                CloudMesh = null;
            }
            if (ShadowProjector != null)
            {
                ShadowProjector.transform.parent = null;
                GameObject.DestroyImmediate(ShadowProjector);
                ShadowProjector = null;
            }
        }

        //Update the rotation of the mesh.
        internal void UpdateRotation(Quaternion rotation)
        {
            if (rotation != null)
            {
                SetMeshRotation(rotation);
                if (ShadowProjector != null) //If there is a shadowprojector
                {
                    Vector3 sunDirection = Vector3.Normalize(ShadowProjector.transform.parent.InverseTransformDirection(Sun.Instance.sunDirection));//sunTransform.position));
                    sunDirection.Normalize(); //normalize the vector
                    ShadowProjector.transform.localPosition = radiusScale * -sunDirection; //local position of the shadow projector
                    ShadowProjector.transform.forward = Sun.Instance.sunDirection; //transforms the shadowprojector forward to the sun's direction.
                }
            }
            SetTextureOffset();
        }

        //sets the mesh rotation
        private void SetMeshRotation(Quaternion rotation)
        {
            CloudMesh.transform.localRotation = rotation; //variable for the local rotation
            double ut = Planetarium.GetUniversalTime(); 
            double x = (ut * globalPeriod); //makes the mesh rotate along the x axis
            x -= (int)x; //does the rotating
            CloudMesh.transform.Rotate(CloudMesh.transform.parent.TransformDirection(Vector3.up), (float)(360f * x), Space.World);
            Quaternion rotationForMatrix = CloudMesh.transform.localRotation;
            CloudMesh.transform.localRotation = rotation;
            Matrix4x4 mtrx = Matrix4x4.TRS(Vector3.zero, rotationForMatrix, new Vector3(1, 1, 1));
            CloudMaterial.SetMatrix(KWSManagerClass.ROTATION_PROPERTY, mtrx); //Sets the matrix
        }

        //Sets the Texture Offset
        private void SetTextureOffset()
        {
            double ut = Planetarium.GetUniversalTime();
            double x = (ut * mainPeriodOffset);
            x -= (int)x; //does the rotating of x axis
            Vector2 texOffset = new Vector2((float)x + offset.x, offset.y); //2D texture offset by period
            CloudMaterial.SetVector(KWSManagerClass.MAINOFFSET_PROPERTY, texOffset);

            if (ShadowProjector != null)
            {
                Vector4 texVect = ShadowProjector.transform.localPosition.normalized;
                texVect.w = -(float)x; //rotate the texture on the x axis.
                ShadowProjector.material.SetVector(KWSManagerClass.SHADOWOFFSET_PROPERTY, texVect);
            }
        }
    }
}
