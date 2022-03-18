using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    [CreateAssetMenu(fileName = "PuzzleData", menuName = "ScriptableObjects/PuzzleInfo", order = 4)]
    public class PuzzleScriptable : ScriptableObject
    {
        public int puzzleId;
        public GemsName gemsName;
        public int circleLevel;
        public int levelIndex;
        public int width;
        public int height;
        public int gridGap;
        public bool unlock;
        public string unlockConditionPuzzleId;
        public bool unlockGems;
        public bool clear;
        public DotType[] dotList;
        public int[] puzzleConnectedList;
        public PuzzleConnected[] connectedList;
        public ClearedEvent[] clearedEvents;
    }

    [System.Serializable]
    public class ClearedEvent
    {
        public EventName eventName;
        public string targetIndex;
    }

    [System.Serializable]
    public class PuzzleConnected
    {
        public int puzzleTargetId;
        public string puzzleConditionId;
    }

    public enum EventName
    {
        UnlockSection,
        UnlockBox,
        UnlockPuzzle,
        UnlockGems
    }


}

