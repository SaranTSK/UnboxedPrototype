using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    [CreateAssetMenu(fileName = "BoxData", menuName = "ScriptableObjects/BoxInfo", order = 3)]
    public class BoxScriptable : ScriptableObject
    {
        public int boxId;
        public string boxName;
        public bool isGate;
        public bool isClue;
        public GemsName clueGems;
        public GemsName boxGems;
        public GemsTier gemsTier;
        public SectionName boxSection;
        public FrameRarity rarity;
        public bool unlock;
        public string unlockConditionPuzzleId;
        public bool clear;
        public GemsUnlock[] gemsUnlockList;
        public int[] boxConnectedList;
        public BoxConnected[] connectedList;
        public CircleGenerate circleGenerate;
        public PuzzleScriptable[] puzzleList;
        //public int circleGenIndex;
        //public int minPuzzleId = -1;
        //public int maxPuzzleId = -1;

        //public GemsName[] gemsList = new GemsName[6];
        //public GameObject boxIconPref;
        //public PuzzleSectionScriptable[] puzzleInfoData;
    }

    [System.Serializable]
    public class BoxConnected
    {
        public int boxTargetId;
        public string puzzleConditionId;
    }

    [System.Serializable]
    public class GemsUnlock
    {
        public GemsName gems;
        public bool unlock;
    }
}

