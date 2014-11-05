//Code copied from Rbray's mod EVE, all proper credits go to him for such hard work

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;

namespace KWSManager
{
    public interface IKWSObject : INamed
    {
        ConfigNode ConfigNode { get; }
        String Body { get; }
        void LoadConfigNode(ConfigNode node, String body);
        ConfigNode GetConfigNode();
        void Apply();
        void Remove();
    }
}
