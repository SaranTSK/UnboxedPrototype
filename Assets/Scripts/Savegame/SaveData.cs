using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    [System.Serializable]
    public class SaveData
    {
        public string playerName;
        public bool tutorialUnlock;
        public List<string> unlockedSection;
        public List<int> unlockedBox;
        public List<string> unlockedBoxGems;
        public List<int> clearedBox;
        public List<int> unlockedPuzzle;
        public List<int> unlockedPuzzleGems;
        public List<int> clearedPuzzle;
        public List<int> unlockedGems;
        public List<int> clearedGate;

        public SaveData(PlayerData player)
        {
            playerName = player.playerName;
            tutorialUnlock = player.tutorialUnlock;
            unlockedSection = player.unlockedSection;
            unlockedBox = player.unlockedBox;
            unlockedBoxGems = player.unlockedBoxGems;
            clearedBox = player.clearedBox;
            unlockedPuzzle = player.unlockedPuzzle;
            unlockedPuzzleGems = player.unlockedPuzzleGems;
            clearedPuzzle = player.clearedPuzzle;
            unlockedGems = player.unlockedGems;
            clearedGate = player.clearedGate;
        }
    }
}

