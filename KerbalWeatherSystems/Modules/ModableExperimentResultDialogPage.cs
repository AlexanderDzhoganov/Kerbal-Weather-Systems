using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modules
{
    internal class ModableExperimentResultDialogPage : ExperimentResultDialogPage
    {
        Callback<ScienceData> onDiscardData;
        Callback<ScienceData> onTransmitData;
        Callback<ScienceData> onKeepData;

        public ModableExperimentResultDialogPage(Part host, ScienceData experimentData, float xmitDataScalar, float labDataBoost, bool showTransmitWarning, string transmitWarningMessage, bool showLabOption, bool showResetOption, Callback<ScienceData> onDiscardData, Callback<ScienceData> onKeepData, Callback<ScienceData> onTransmitData, Callback<ScienceData> onSendToLab) : base(host,experimentData,xmitDataScalar,labDataBoost,showTransmitWarning,transmitWarningMessage,showResetOption, showLabOption, onDiscardData, onKeepData, onTransmitData, onSendToLab)
        {

        }
        public void setUpScienceData(string experimentTitle, string experimentResults, float transmitValue, float recoveryValue, float dataSize, float xmitScalar, float refValue)
        {
            this.title = experimentTitle;
            this.resultText = experimentResults;
            //this.transmitValue = valueAfterTransmit;
            this.valueAfterTransmit = transmitValue;
            this.valueAfterRecovery = recoveryValue;
            this.dataSize = dataSize;
            this.xmitDataScalar = xmitScalar;
            this.refValue = transmitValue;
            this.scienceValue = recoveryValue;
            this.transmitValue = transmitValue;
        }
    }
}
