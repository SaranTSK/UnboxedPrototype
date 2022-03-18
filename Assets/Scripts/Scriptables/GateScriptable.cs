using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    [CreateAssetMenu(fileName = "GateData", menuName = "ScriptableObjects/GateInfo", order = 5)]
    public class GateScriptable : ScriptableObject
    {
        public CircleGenerate gate;
        public GateProperty red;
        public GateProperty orange;
        public GateProperty yellow;
        public GateProperty green;
        public GateProperty turquoise;
        public GateProperty navy;
        public GateProperty violet;
        public GateProperty pink;
        public GateProperty black;
        public GateProperty white;
    }

    [System.Serializable]
    public class GateProperty
    {
        public bool clear;
        public int[] boxClueId;

        public GateProperty()
        {
            clear = false;
            boxClueId = new int[0];
        }
    }
}

