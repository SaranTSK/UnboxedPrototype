using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unboxed
{
    public class DataAsset : MonoBehaviour
    {
        public static DataAsset instance;

        [SerializeField] private MapSectionScriptable mapData;
        [SerializeField] private CircleGeneratorScriptable[] circleGenData;
        [SerializeField] private BoxScriptable[] boxData;
        [SerializeField] private PuzzleScriptable[] puzzleData;

        private MapSectionScriptable mapDataTemp;
        private CircleGeneratorScriptable[] circleGenDataTemp;
        private GemsTierData gemsTierData;
        private Dictionary<int, BoxScriptable> boxDict;
        private Dictionary<int, PuzzleScriptable> puzzleDict;

        //private List<List<int>> boxIdSectionTemp;
        //private List<int> boxIdTutorialGateTemp;
        //private List<int> boxIdTutorialSectionTemp;

        private bool mapLoaded;
        private bool boxLoaded;
        private bool puzzleLoaded;
        private bool gemsTierLoaded;
        private bool gateLoaded;

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
            CreateInstanceData();
        }

        public MapSectionScriptable GetMapData()
        {
            return mapDataTemp;
        }

        public int GetMapDataLenght()
        {
            return mapDataTemp.gateInfoList.Length;
        }

        public CircleGeneratorScriptable GetCircleData(int index)
        {
            return circleGenDataTemp[index];
        }

        public BoxScriptable GetBoxData(int key)
        {
            return boxDict[key];
        }

        public int GetBoxDataLenght()
        {
            return boxDict.Count;
        }

        public PuzzleScriptable GetPuzzleDataValue(int key)
        {
            return puzzleDict[key];
        }

        public int GetPuzzleDataKey(PuzzleScriptable puzzle)
        {
            //if(puzzleDict.ContainsValue(puzzle))
            //{
            //    Debug.Log(puzzle.name);
            //    return int.Parse(puzzle.name.Split('_').GetValue(1).ToString());
            //}
            //return 0;
            //Debug.Log(puzzle.name);
            return int.Parse(puzzle.name.Split('_').GetValue(1).ToString());
        }

        public int GetPuzzleDataLenght()
        {
            return puzzleDict.Count;
        }

        public bool[] GetGemsTierData(GemsTier tier)
        {
            switch (tier)
            {
                case GemsTier.E: return gemsTierData.tierE;
                case GemsTier.D: return gemsTierData.tierD;
                case GemsTier.C: return gemsTierData.tierC;
                case GemsTier.B: return gemsTierData.tierB;
                case GemsTier.A: return gemsTierData.tierA;
                case GemsTier.S: return gemsTierData.tierS;
            }

            return null;
        }

        public bool IsLoaded()
        {
            return mapDataTemp != null &&
                circleGenDataTemp.Length == circleGenData.Length &&
                boxDict.Count == boxData.Length;
                //puzzleDataTemp.Length == puzzleData.Length;
        }

        public bool IsSaveLoaded()
        {
            return mapLoaded && boxLoaded && puzzleLoaded && gemsTierLoaded && gateLoaded;
        }

        public void LoadSave(PlayerData player)
        {
            // Check tutorial unlock
            mapDataTemp.tutorialUnlock = player.tutorialUnlock;

            if (player.unlockedSection != null && player.unlockedSection.Count > 0)
            {
                LoadSectionData(player.unlockedSection);
            }
            else
            {
                mapLoaded = true;
            }

            if (player.unlockedBox != null && player.unlockedBox.Count > 0)
            {
                LoadBoxData(player.unlockedBox, player.unlockedBoxGems, player.clearedBox);
            }
            else
            {
                boxLoaded = true;
            }

            if (player.unlockedPuzzle != null && player.unlockedPuzzle.Count > 0)
            {
                LoadPuzzleData(player.unlockedPuzzle, player.unlockedPuzzleGems, player.clearedPuzzle);
            }
            else
            {
                puzzleLoaded = true;
            }

            if(gemsTierData != null && player.unlockedGems.Count > 0)
            {
                LoadGemsTierData(player.unlockedGems);
            }
            else
            {
                gemsTierLoaded = true;
            }

            if(player.clearedGate.Count > 0)
            {
                ClueManager.instance.LoadGateData(player.clearedGate);
            }
            else
            {
                gateLoaded = true;
            }

        }

        public void SetGateLoaded(bool value)
        {
            gateLoaded = value;
        }

        private void LoadSectionData(List<string> datas)
        {
            foreach(string data in datas)
            {
                int index = int.Parse(data.Split('_').GetValue(0).ToString());
                int section = int.Parse(data.Split('_').GetValue(1).ToString());

                SectionProperty sectionProperty = mapDataTemp.gateInfoList[index - 1];

                switch(section)
                {
                    case 0: sectionProperty.zeroSectionUnlock = true; break;
                    case 1: sectionProperty.firstSectionUnlock = true; break;
                    case 2: sectionProperty.secondSectionUnlock = true; break;
                    case 3: sectionProperty.thirdSectionUnlock = true; break;
                    case 4: sectionProperty.fourthSectionUnlock = true; break;
                }
            }

            mapLoaded = true;
        }

        private void LoadBoxData(List<int> unlockedList, List<string> unlockedGemsList, List<int> clearedList)
        {
            foreach (int key in unlockedList)
            {
                boxDict[key].unlock = true;
            }

            foreach (string value in unlockedGemsList)
            {
                int key = int.Parse(value.Split('_').GetValue(0).ToString());
                int index = int.Parse(value.Split('_').GetValue(1).ToString());

                boxDict[key].gemsUnlockList[index].unlock = true;
            }

            foreach (int key in clearedList)
            {
                boxDict[key].clear = true;
            }

            boxLoaded = true;
        }

        private void LoadPuzzleData(List<int> unlockedList, List<int> unlockedGemsList, List<int> clearedList)
        {
            foreach (int key in unlockedList)
            {
                puzzleDict[key].unlock = true;
            }

            foreach (int key in unlockedGemsList)
            {
                puzzleDict[key].unlockGems = true;
            }

            foreach (int key in clearedList)
            {
                puzzleDict[key].clear = true;
            }

            puzzleLoaded = true;
        }

        private void LoadGemsTierData(List<int> unlockedList)
        {
            foreach (int data in unlockedList)
            {
                GemsTier tier = (GemsTier)(data / Constant.MAX_GEMS) + 1;

                int index = data - (((int)tier - 1) * Constant.MAX_GEMS);

                Debug.Log("Data " + data + " | Tier " + tier + " | Index " + index);

                switch (tier)
                {
                    case GemsTier.E: gemsTierData.tierE[index] = true; break;
                    case GemsTier.D: gemsTierData.tierD[index] = true; break;
                    case GemsTier.C: gemsTierData.tierC[index] = true; break;
                    case GemsTier.B: gemsTierData.tierB[index] = true; break;
                    case GemsTier.A: gemsTierData.tierA[index] = true; break;
                    case GemsTier.S: gemsTierData.tierS[index] = true; break;
                }
            }

            gemsTierLoaded = true;
        }

        private void CreateInstanceData()
        {
            // Create map instance data
            mapDataTemp = Instantiate(mapData);

            circleGenDataTemp = new CircleGeneratorScriptable[circleGenData.Length];
            for (int i = 0; i < circleGenData.Length; i++)
            {
                circleGenDataTemp[i] = Instantiate(circleGenData[i]);
            }

            // Create box and puzzle instance data
            boxDict = new Dictionary<int, BoxScriptable>();
            puzzleDict = new Dictionary<int, PuzzleScriptable>();
            int puzzleKey = 0;
            for (int i = 0; i < boxData.Length; i++)
            {
                boxDict.Add(i, Instantiate(boxData[i]));

                int puzzleLength = boxData[i].puzzleList.Length;

                if (puzzleLength > 0)
                {
                    for (int k = 0; k < puzzleLength; k++)
                    {
                        puzzleDict.Add(puzzleKey, Instantiate(boxData[i].puzzleList[k]));
                        puzzleDict[puzzleKey].name = boxData[i].puzzleList[k].name;
                        puzzleDict[puzzleKey].unlockGems = (boxData[i].gemsUnlockList.Length > 0) ? CheckUnlockPuzzleGemsValue(puzzleDict[puzzleKey]) : true;
                        puzzleKey++;
                    }
                }
                else
                {
                    Debug.LogWarning(boxData[i].name + " has missing puzzleList data");
                }

            }

            gemsTierData = new GemsTierData();
        }

        private bool CheckUnlockPuzzleGemsValue(PuzzleScriptable puzzle)
        {
            if(puzzle.gemsName == GemsName.None)
            {
                return true;
            }

            return false;
        }

        public void UpdateTutorialUnlock(bool value)
        {
            mapDataTemp.tutorialUnlock = value;
        }

        public void UpdateUnlockedBoxData(int index)
        {
            boxDict[index].unlock = true;
        }

        public void UpdateUnlockedBoxGemsData(int key, int index)
        {
            boxDict[key].gemsUnlockList[index].unlock = true;
        }

        public void UpdateClearedBoxData(int index)
        {
            boxDict[index].clear = true;
        }

        public void UpdateUnlockedPuzzleData(int index)
        {
            puzzleDict[index].unlock = true;
        }

        public void UpdateUnlockedPuzzelGemsData(int index)
        {
            puzzleDict[index].unlockGems = true;
        }

        public void UpdateClearedPuzzleData(int index)
        {
            puzzleDict[index].clear = true;
        }

        public void UpdateUnlockedSectionData(GemsName gems, int sectionId)
        {
            int index = (int)gems - 1;
            switch (sectionId)
            {
                case 0: mapDataTemp.gateInfoList[index].zeroSectionUnlock = true; break;
                case 1: mapDataTemp.gateInfoList[index].firstSectionUnlock = true; break;
                case 2: mapDataTemp.gateInfoList[index].secondSectionUnlock = true; break;
                case 3: mapDataTemp.gateInfoList[index].thirdSectionUnlock = true; break;
                case 4: mapDataTemp.gateInfoList[index].fourthSectionUnlock = true; break;
            }
        }

        public void UpdateUnlockedGems(GemsTier tier, int index)
        {
            int id = index - (((int)tier - 1) * Constant.MAX_GEMS);

            Debug.Log("Unlock gems " + tier.ToString() + " id " + id);
            switch (tier)
            {
                case GemsTier.E: gemsTierData.tierE[id] = true; break;
                case GemsTier.D: gemsTierData.tierD[id] = true; break;
                case GemsTier.C: gemsTierData.tierC[id] = true; break;
                case GemsTier.B: gemsTierData.tierB[id] = true; break;
                case GemsTier.A: gemsTierData.tierA[id] = true; break;
                case GemsTier.S: gemsTierData.tierS[id] = true; break;
            }
        }

        public void UpdateTempUnlockedPuzzleGemsData(int index, bool isUnlock)
        {
            puzzleDict[index].unlockGems = isUnlock;
        }

        //public List<int> GetBoxListBySection(GemsName gems, int sectionId)
        //{
        //    // TODO: Fix this number
        //    if (IsPlayableSection(sectionId))
        //    {
        //        int index = (((int)gems - 1) * 4) + sectionId;
        //        return boxIdSectionTemp[index];
        //    }
            
        //    return new List<int>();
        //}

        //private bool IsPlayableSection(int sectionId)
        //{
        //    SectionName section = (SectionName)sectionId;
        //    switch(section)
        //    {
        //        case SectionName.Zero:
        //        case SectionName.First:
        //        case SectionName.Second:
        //        case SectionName.Third:
        //        case SectionName.Fourth:
        //            return true;
        //    }

        //    return false;
        //}

        //public List<int> GetBoxListTutorialSection()
        //{
        //    return boxIdTutorialSectionTemp;
        //}

        //private void CreateChildBoxList()
        //{
        //    boxIdSectionTemp = new List<List<int>>();

        //    for (int i = 0; i < mapData.gateInfoList.Length * 4; i++)
        //    {
        //        boxIdSectionTemp.Add(new List<int>());
        //    }
        //}

        //private void AddChildBoxList(GemsName gems, int sectionId, int boxId)
        //{
        //    if(boxIdSectionTemp != null)
        //    {
        //        int index = (((int)gems - 1) * 4) + sectionId;
        //        if (boxIdSectionTemp[index] != null)
        //        {
        //            boxIdSectionTemp[index].Add(boxId);
        //        }
        //    }
        //}
    }

    public class GemsTierData
    {
        public bool[] tierE;
        public bool[] tierD;
        public bool[] tierC;
        public bool[] tierB;
        public bool[] tierA;
        public bool[] tierS;

        public GemsTierData()
        {
            tierE = new bool[Constant.MAX_GEMS];
            tierD = new bool[Constant.MAX_GEMS];
            tierC = new bool[Constant.MAX_GEMS];
            tierB = new bool[Constant.MAX_GEMS];
            tierA = new bool[Constant.MAX_GEMS];
            tierS = new bool[Constant.MAX_GEMS];
        }
    }
}

