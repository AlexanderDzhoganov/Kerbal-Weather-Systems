//animationPlaySpeed = windSpeed * 0.363636f;
//note: Circumference of anemometer is 2.75m
//1m/s wind would cause a 0.363636m/s rotation which means a play speed of 0.36%
//name of animation for rotation is anemRotationSpeed

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using Weather;

namespace Animations
{
    class KWSTurbineAnimationGeneric : PartModule
    {
        [KSPField]
        public string animationName;
        [KSPField]
        public bool goToBeginningWhenStopped = true;
        [KSPField]
        public int layer = 1;

        private Animation anim;
        [Persistent]
        public bool isAnimating;

        public void Update()
        {
            if (HeadMaster.inAtmosphere == true) 
            { 
                //Debug.Log("inAtmosphere = true"); 
                isAnimating = true; 
            }
            else 
            { 
                isAnimating = false; 
                //Debug.Log("Isanimating = false"); 
                anim[animationName].speed = 0f; 
            }
            //Debug.Log(isAnimating);
            //Debug.Log(HeadMaster.inAtmosphere);
            //Debug.Log(Wind.windSpeed);
            if (HeadMaster.windSpeed < 0.001f || HeadMaster.windSpeed == 0f || HeadMaster.inAtmosphere == false)
            {
                //Debug.Log("Windspeed is 0");
                anim[animationName].speed = 0f;
                //anim[animationName].speed = HeadMaster.windSpeed;
                isAnimating = false;
            }
            else
            {
                anim[animationName].speed = (HeadMaster.windSpeed)* 0.3636f;
                isAnimating = true;
            }

            //Debug.Log("Update");
            setPlayMode(isAnimating);
            //Debug.Log("SetPlayMode");
            if (isAnimating == true)
            {
                //Debug.Log(anim[animationName].speed);
                //Debug.Log(Wind.windSpeed);
                anim.Play(animationName);

            }
            else if (isAnimating == false)
            {
                //Debug.Log("Animation Stopped");
                if (goToBeginningWhenStopped)
                {
                    anim[animationName].normalizedTime = 0f;
                }

                anim[animationName].speed = 0f;
            }
            
        }

        private void setPlayMode(bool isAnimating)
        {
            
        }

        public override void OnAwake()
        {
            isAnimating = true;
            base.OnAwake();
        }

        public override void OnStart(PartModule.StartState state)
        {
            //Debug.Log("OnStart");
            //isAnimating = true;
            anim = part.FindModelAnimators(animationName).FirstOrDefault();
            if (anim != null)
            {
                anim[animationName].layer = layer;
                //anim[animationName].speed = Wind.windSpeed * 0.363636f;
                anim.wrapMode = WrapMode.Loop;
                //setPlayMode(isAnimating);
            }
            else
            {
                Debug.Log("Could not find anim " + animationName);
            }
        }
    }
}
