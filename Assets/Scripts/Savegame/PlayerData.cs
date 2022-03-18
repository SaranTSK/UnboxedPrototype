using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class PlayerData
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

        public PlayerData(SaveData save)
        {
            playerName = save.playerName;
            tutorialUnlock = save.tutorialUnlock;
            unlockedSection = save.unlockedSection;
            unlockedBox = save.unlockedBox;
            unlockedBoxGems = save.unlockedBoxGems;
            clearedBox = save.clearedBox;
            unlockedPuzzle = save.unlockedPuzzle;
            unlockedPuzzleGems = save.unlockedPuzzleGems;
            clearedPuzzle = save.clearedPuzzle;
            unlockedGems = save.unlockedGems;
            clearedGate = save.clearedGate;
        }

        public PlayerData()
        {
            playerName = "";
            tutorialUnlock = false;
            unlockedSection = new List<string>();
            unlockedBox = new List<int>();
            unlockedBoxGems = new List<string>();
            clearedBox = new List<int>();
            unlockedPuzzle = new List<int>();
            unlockedPuzzleGems = new List<int>();
            clearedPuzzle = new List<int>();
            unlockedGems = new List<int>();
            clearedGate = new List<int>();

            CreateUnlockData(UnlockedData.Zero);
            unlockedBox.Add(101);
        }

        private const int PUZZLE_LENGTH_INIT_START = 0;
        private const int PUZZLE_LENGTH_INIT_END = 6;

        private const int BOX_LENGTH_TUTORIAL_START = 1;
        private const int BOX_LENGTH_TUTORIAL_END = 5;
        private const int PUZZLE_LENGTH_TUTORIAL_START = 7;
        private const int PUZZLE_LENGTH_TUTORIAL_END = 41;
        private const int BOX_TUTORIAL_CONNECTED_START = 6;
        private const int BOX_TUTORIAL_CONNECTED_END = 15;

        private const int BOX_LENGTH_ZERO_START = 6;
        private const int BOX_LENGTH_ZERO_END = 30;
        private const int PUZZLE_LENGTH_ZERO_START = 42;
        private const int PUZZLE_LENGTH_ZERO_END = 216;
        private const int BOX_ZERO_CONNECTED_START = 31;
        private const int BOX_ZERO_CONNECTED_END = 40;

        private const int BOX_LENGTH_FIRST_START = 31;
        private const int BOX_LENGTH_FIRST_END = 40;
        private const int PUZZLE_LENGTH_FIRST_START = 217;
        private const int PUZZLE_LENGTH_FIRST_END = 217;

        private enum UnlockedData
        {
            None,
            Init,
            Tutorial,
            Zero,
            First
        }

        private void CreateUnlockData(UnlockedData data)
        {
            switch(data)
            {
                case UnlockedData.None:
                    unlockedBox.Add(0);
                    unlockedPuzzle.Add(0);
                    break;

                case UnlockedData.Init:
                    UnlockInitSection();
                    break;

                case UnlockedData.Tutorial:
                    UnlockInitSection();
                    UnlockTutorialSection();
                    break;

                case UnlockedData.Zero:
                    UnlockInitSection();
                    UnlockTutorialSection();
                    UnlockZeroSection();
                    CreateUnlockedGemsTies(GemsTier.E);
                    break;

                case UnlockedData.First:
                    UnlockInitSection();
                    UnlockTutorialSection();
                    UnlockZeroSection();
                    UnlockFirstSection();
                    CreateUnlockedGemsTies(GemsTier.E);
                    break;
            }
        }

        private void CreateUnlockedSection(string[] sections)
        {
            for (int i = 0; i < sections.Length; i++)
            {
                unlockedSection.Add(sections[i]);
            }
        }

        private void CreateUnlockedBox(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                unlockedBox.Add(i);
                clearedBox.Add(i);
            }
        }

        private void CreateUnlockedPuzzle(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                unlockedPuzzle.Add(i);
                unlockedPuzzleGems.Add(i);
                clearedPuzzle.Add(i);
            }
        }

        private void CraeteUnlockedConnected(int start, int end, int[] centers)
        {
            for (int i = start; i <= end; i++)
            {
                unlockedBox.Add(i);
            }

            for (int i = 0; i < centers.Length; i++)
            {
                unlockedPuzzle.Add(centers[i]);
            }
        }

        private void CreateUnlockedGemsTies(GemsTier tier)
        {
            int dataLength = (int)tier * Constant.MAX_GEMS;

            for(int i = 0; i < dataLength; i++)
            {
                unlockedGems.Add(i);
            }
        }

        private void UnlockInitSection()
        {
            tutorialUnlock = true;
            unlockedBox.Add(0);
            clearedBox.Add(0);
            CreateUnlockedPuzzle(PUZZLE_LENGTH_INIT_START, PUZZLE_LENGTH_INIT_END);

            int[] centerPuzzle = { 7, 14, 21, 28, 35 };
            CraeteUnlockedConnected(BOX_LENGTH_TUTORIAL_START, BOX_LENGTH_TUTORIAL_END, centerPuzzle);
        }

        private void UnlockTutorialSection()
        {
            string[] unlockSection = {"1_0", "2_0", "3_0", "4_0", "5_0", "6_0", "7_0", "8_0", "9_0", "10_0", };
            CreateUnlockedSection(unlockSection);
            CreateUnlockedBox(BOX_LENGTH_TUTORIAL_START, BOX_LENGTH_TUTORIAL_END);
            CreateUnlockedPuzzle(PUZZLE_LENGTH_TUTORIAL_START, PUZZLE_LENGTH_TUTORIAL_END);

            int[] centerPuzzle = {42, 49, 56, 63, 70, 77, 84, 91, 98, 105 };
            CraeteUnlockedConnected(BOX_TUTORIAL_CONNECTED_START, BOX_TUTORIAL_CONNECTED_END, centerPuzzle);

        }

        private void UnlockZeroSection()
        {
            string[] unlockSection = { "1_1", "2_1", "3_1", "4_1", "5_1", "6_1", "7_1", "8_1", "9_1", "10_1", };
            CreateUnlockedSection(unlockSection);
            CreateUnlockedBox(BOX_LENGTH_ZERO_START, BOX_LENGTH_ZERO_END);
            CreateUnlockedPuzzle(PUZZLE_LENGTH_ZERO_START, PUZZLE_LENGTH_ZERO_END);

            int[] centerPuzzle = { 217, 230, 243, 256, 269, 282, 295, 308, 321, 334 };
            CraeteUnlockedConnected(BOX_ZERO_CONNECTED_START, BOX_ZERO_CONNECTED_END, centerPuzzle);
        }

        private void UnlockFirstSection()
        {
            string[] unlockSection = { };
            CreateUnlockedSection(unlockSection);
            CreateUnlockedBox(BOX_LENGTH_FIRST_START, BOX_LENGTH_FIRST_END);
            CreateUnlockedPuzzle(PUZZLE_LENGTH_FIRST_START, PUZZLE_LENGTH_FIRST_END);

            // TODO: Add connected puzzle
        }
    }
}

