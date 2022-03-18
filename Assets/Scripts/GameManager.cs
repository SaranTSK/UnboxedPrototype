using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unboxed
{
    public enum GameState
    {
        Menu,
        Simulation,
        BoxLevel,
        Puzzle
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        //public BoxScriptable boxInfo { get; private set; }
        public GameState currentState { get; private set; }

        private PlayerData playerData;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(instance);
            }
        }

        private void Start()
        {
            SetGameState(GameState.Menu);
            UIManager.instance.Initialize();
            LevelManager.instance.Initialize();
            StartCoroutine(InitializeGame());
        }

        private IEnumerator InitializeGame()
        {
            UIManager.instance.IsLoading(true);
            yield return new WaitUntil(() => SpriteAsset.instance.IsLoaded() == true);
            yield return new WaitUntil(() => DataAsset.instance.IsLoaded() == true);
            yield return new WaitUntil(() => ObjectAsset.instance.IsLoaded() == true);
            LoadGame();
            yield return new WaitUntil(() => DataAsset.instance.IsSaveLoaded() == true);
            LevelManager.instance.EnterSimulation();
            ClueManager.instance.Initialize();
        }

        private void LoadGame()
        {
            CheckPlayerData();
            DataAsset.instance.LoadSave(playerData);
        }

        private void SaveGame()
        {
            SaveManager.SavePlayer(playerData);
        }

        private void CheckPlayerData()
        {
            Debug.Log("Savegame: " + SaveManager.LoadPlayer());

            if (SaveManager.LoadPlayer() != null)
            {
                playerData = new PlayerData(SaveManager.LoadPlayer());
            }
            else
            {
                playerData = new PlayerData();
            }
        }

        public void SaveTutorialUnlock(bool value)
        {
            playerData.tutorialUnlock = value;
            DataAsset.instance.UpdateTutorialUnlock(value);
            SaveGame();
        }

        public void SaveClearedPuzzle(int index)
        {
            playerData.clearedPuzzle.Add(index);
            playerData.clearedPuzzle.Sort();
            DataAsset.instance.UpdateClearedPuzzleData(index);
            SaveGame();
        }

        public void SaveUnlockedPuzzle(int index)
        {
            playerData.unlockedPuzzle.Add(index);
            playerData.unlockedPuzzle.Sort();
            DataAsset.instance.UpdateUnlockedPuzzleData(index);
            SaveGame();
        }

        public void SaveUnlockedPuzzleGems(int index)
        {
            playerData.unlockedPuzzleGems.Add(index);
            playerData.unlockedPuzzleGems.Sort();
            DataAsset.instance.UpdateUnlockedPuzzelGemsData(index);
            SaveGame();
        }

        public void SaveClearedBox(int index)
        {
            playerData.clearedBox.Add(index);
            playerData.clearedBox.Sort();
            DataAsset.instance.UpdateClearedBoxData(index);
            SaveGame();
        }

        public void SaveUnlockedBox(int index)
        {
            playerData.unlockedBox.Add(index);
            playerData.unlockedBox.Sort();
            DataAsset.instance.UpdateUnlockedBoxData(index);
            SaveGame();
        }

        public void SaveUnlockedBoxGems(int key, int index)
        {
            string value = key + "_" + index;
            playerData.unlockedBoxGems.Add(value);
            playerData.unlockedBoxGems.Sort();
            DataAsset.instance.UpdateUnlockedBoxGemsData(key, index);
            SaveGame();
        }

        public void SaveUnlockedSection(GemsName gems, int section)
        {
            string unlock = gems.ToString() + "_" + section.ToString();
            playerData.unlockedSection.Add(unlock);
            playerData.unlockedSection.Sort();
            DataAsset.instance.UpdateUnlockedSectionData(gems, section);
            SaveGame();
        }

        public void SaveUnlockedGems(GemsTier tier, int index)
        {
            playerData.unlockedGems.Add(index);
            playerData.unlockedSection.Sort();
            DataAsset.instance.UpdateUnlockedGems(tier, index);
            SaveGame();
        }

        public void SetGameState(GameState state)
        {
            currentState = state;
        }

        public void SaveClearedGate(SectionName section, GemsName gems)
        {
            int value = (((int)section - 2) * Constant.MAX_GEMS) + ((int)gems - 1);
            playerData.clearedGate.Add(value);
            playerData.clearedGate.Sort();
            ClueManager.instance.UpdateClearedGateData(section, gems);
            SaveGame();
        }

        //public void SetBoxInfo(BoxScriptable info)
        //{
        //    boxInfo = info;
        //}

        //public void ResetBoxInfo()
        //{
        //    if(boxInfo != null)
        //    {
        //        Destroy(boxInfo);
        //    }

        //    boxInfo = ScriptableObject.CreateInstance<BoxScriptable>();
        //}

        public void LoadingScene(int index)
        {
            SceneManager.LoadScene(index);
        }
    }
}

