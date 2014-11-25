using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

namespace Modules
{
    public class AnemScienceModule : ModuleScienceExperiment, IScienceDataContainer
    {
        //reference body index constants
        public const int REF_BODY_KERBIN = 1;
        public const int REF_BODY_EVE = 5;
        public const int REF_BODY_DUNA = 6;
        public const int REF_BODY_JOOL = 8;
        public const int REF_BODY_LAYTHE = 9;

        [KSPField(isPersistant = true)]
        new public bool isEnabled;
        [KSPField(isPersistant = true)]
        public bool isRunning;
        [KSPField(isPersistant = true)]
        public float sciencetoadd;
        [KSPField(isPersistant = true)]
        public string resultTitle;
        [KSPField(isPersistant = true)]
        public string resultString;
        [KSPField(isPersistant = true)]
        public float transmitValue;
        [KSPField(isPersistant = true)]
        public float recoveryValue;
        [KSPField(isPersistant = true)]
        public float dataSize;
        [KSPField(isPersistant = true)]
        public float refValue;
        [KSPField(isPersistant = true)]
        new public string experimentID = "anemometerReading";
        [KSPField(isPersistant = true)]
        public int scienceCount;
        [KSPField(isPersistant = true)]
        public bool dataGend;
        [KSPField(isPersistant = true)]
        public float xmitScalar;
        [KSPField(isPersistant = false)]
        public string deployEventName;
        [KSPField(isPersistant = false)]
        public string reviewEventName;
        [KSPField(isPersistant = false)]
        public string resetEventName;

        protected ScienceData scienceData;
        internal ModableExperimentResultDialogPage merdp;


        private float scienceRate;

        [KSPEvent(guiName = "Deploy", active = true, guiActive = true)]
        public void DeployExperiment()
        {
            //Debug.Log("Deploy");
            dataGend = generateScienceData();
            ReviewData();
            Deployed = true;
            cleanUpScienceData();
            
        }

        [KSPAction("Deploy")]
        public void DeployAction(KSPActionParam actParams)
        {
            DeployExperiment();
        }

        [KSPEvent(guiName = "Reset", active = true, guiActive = true)]
        public void ResetExperiment()
        {
            //Debug.Log("Reset");
            if (scienceData != null)
            {
                DumpData(scienceData);
            }
            Deployed = false;
        }

        [KSPAction("Reset")]
        new public void ResetAction(KSPActionParam actParams)
        {
            ResetExperiment();
        }

        [KSPEvent(guiName = "Review Data", active = true, guiActive = true)]
        public void ReviewData()
        {
            //Debug.Log("Review Data");
            if (scienceData != null)
            {
                if (merdp == null || !dataGend)
                {
                    ExperimentsResultDialog.DisplayResult(merdp = new ModableExperimentResultDialogPage(base.part, this.scienceData, this.scienceData.transmitValue, 0, false, "", true, false, new Callback<ScienceData>(this.endExperiment), new Callback<ScienceData>(this.keepData), new Callback<ScienceData>(this.sendDataToComms), new Callback<ScienceData>(this.sendDataToLab)));
                    merdp.setUpScienceData(resultTitle, resultString, transmitValue, recoveryValue, dataSize, xmitScalar, refValue);
                }
                else
                {
                    ExperimentsResultDialog.DisplayResult(merdp);
                }
            }
            else
            {
                ResetExperiment();
            }
        }

        public override void OnUpdate()
        {
            Events["DeployExperiment"].guiName = deployEventName;
            Events["ResetExperiment"].guiName = resetEventName;
            Events["ReviewData"].guiName = reviewEventName;
            Events["DeployExperiment"].active = !Deployed;
            Events["ResetExperiment"].active = Deployed;
            Events["ReviewData"].active = Deployed;
            Actions["DeployAction"].guiName = deployEventName;
            if (scienceData == null)
            {
                //Debug.Log("Data is null");
                Deployed = false;
            }
        }

        protected void sendDataToComms(ScienceData scienceData)
        {
            //Debug.Log("Sending data to comms");
            List<IScienceDataTransmitter> list = base.vessel.FindPartModulesImplementing<IScienceDataTransmitter>();
            if (list.Any<IScienceDataTransmitter>() && scienceData != null && dataGend)
            {
                merdp = null;
                List<ScienceData> list2 = new List<ScienceData>();
                list2.Add(scienceData);
                list.OrderBy(new Func<IScienceDataTransmitter, float>(ScienceUtil.GetTransmitterScore)).First<IScienceDataTransmitter>().TransmitData(list2);
                endExperiment(scienceData);
            }
        }

        protected void sendDataToLab(ScienceData scienceData)
        {

        }

        protected void keepData(ScienceData scienceData)
        {

        }

        protected bool generateScienceData()
        {
            Debug.Log("Generating Science Data");
            ScienceExperiment experiment = ResearchAndDevelopment.GetExperiment(experimentID);
            if (experiment == null) { Debug.Log("Experiment is null"); return false; }

            if(sciencetoadd > 0)
            {
                resultTitle = experiment.experimentTitle;
                resultString = "You keep a very close eye on one of the scoops, counting it's rotations carefully.";

                transmitValue = sciencetoadd;
                recoveryValue = sciencetoadd * 0.1f;
                dataSize = 100f;
                xmitDataScalar = 1f;

                ScienceSubject subject = ResearchAndDevelopment.GetExperimentSubject(experiment, ScienceUtil.GetExperimentSituation(vessel), vessel.mainBody, "");
                subject.scienceCap = 167 * getScienceMultiplier(vessel.mainBody.flightGlobalsIndex);
                refValue = subject.scienceCap;

                scienceData = new ScienceData(sciencetoadd, 1f, 1f, subject.id, "Anemometer Data");

                return true;
            }

            return false;
        }

        public override void OnStart(PartModule.StartState state)
        {
            if (state == StartState.Editor) { return; }
            base.OnStart(state);
        }

        public override void OnSave(ConfigNode node)
        {
            Debug.Log("Saving");
            if(scienceData != null)
            {
                ConfigNode scienceNode = node.AddNode("ScienceData");
                scienceData.Save(scienceNode);
                Debug.Log("Saved!");
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            Debug.Log("Loading");
            if(node.HasNode("ScienceData"))
            {
                ConfigNode scienceNode = node.GetNode("ScienceData");
                scienceData = new ScienceData(scienceNode);
                Debug.Log("Loaded!");
            }

        }

        public void DumpData(ScienceData scienceData)
        {
            //Debug.Log("Dump Data");
            if(scienceData == this.scienceData)
            {
                this.scienceData = null;
                merdp = null;
                resultString = null;
                transmitValue = 0;
                recoveryValue = 0;
                Deployed = false;
            }
        }

        protected void endExperiment(ScienceData scienceData)
        {
            //Debug.Log("End Experiment");
            DumpData(scienceData);
        }

        public ScienceData[] GetData()
        {
            //Debug.Log("Get Data");
            if (scienceData != null) { return new ScienceData[] { scienceData }; }
            else { return new ScienceData[0]; }
        }

        public int GetScienceCount()
        {
            //Debug.Log("Get Science Count");
            if(scienceData != null)
            {
                return 1;
            }

            return 0;
        }

        public bool isRerunnable()
        {
            return true;
        }

        new void ReviewDataItem(ScienceData scienceData)
        {
            //Debug.Log("Review Data Item");
            if (scienceData == this.scienceData)
            {
                ReviewData();
            }
        }

        void cleanUpScienceData()
        {
            //Debug.Log("Clean up Science");
            sciencetoadd = 1.2f;
        }

        public float getScienceMultiplier(int refBody)
        {
            float multiplier = 1;
            if (refBody == REF_BODY_DUNA) { multiplier = 7f;}
            else if (refBody == REF_BODY_EVE) { multiplier = 8f;}
            else if (refBody == REF_BODY_JOOL) { multiplier = 9f;}
            else if (refBody == REF_BODY_KERBIN) { multiplier = 1f;}
            else if (refBody == REF_BODY_LAYTHE) { multiplier = 11f;}

            return multiplier;
        }

    }
}
