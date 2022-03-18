using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    [CreateAssetMenu(fileName = "MapSectionData", menuName = "ScriptableObjects/MapSection", order = 1)]
    public class MapSectionScriptable : ScriptableObject
    {
        public bool tutorialUnlock;
        public SectionProperty[] gateInfoList = new SectionProperty[10];
    }

    [System.Serializable]
    public class SectionProperty
    {
        //public bool[] gateUnlock = { false, false, false, false};
        public GemsName gemsName;
        public bool zeroSectionUnlock = false;
        public bool firstSectionUnlock = false;
        public bool secondSectionUnlock = false;
        public bool thirdSectionUnlock = false;
        public bool fourthSectionUnlock = false;
    }

    
}

