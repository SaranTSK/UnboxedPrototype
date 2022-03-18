using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class ClueManager : MonoBehaviour
    {
        public static ClueManager instance;

        [SerializeField] private MapGenerator mapGenerator;
        [SerializeField] private GateScriptable[] gateData;

        private LineRenderer clueLine;
        private ClueData[] clueDatas;
        private LineRenderer[] lines;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(instance);
            }
        }

        public void Initialize()
        {
            clueDatas = new ClueData[Constant.MAX_GEMS];
            for(int i = 0; i < clueDatas.Length; i++)
            {
                clueDatas[i] = new ClueData();
            }

            lines = new LineRenderer[Constant.MAX_GEMS * 4];

            CreateClueData();
            CraeteClueLine();
            HideAllLineClue();
        }

        public List<int> GetClueList(GemsName gems, Clue clue)
        {
            switch (clue)
            {
                case Clue.FirstClue: return clueDatas[(int)gems - 1].firstClue;
                case Clue.SecondClue: return clueDatas[(int)gems - 1].secondClue;
                case Clue.ThirdClue: return clueDatas[(int)gems - 1].thirdClue;
                case Clue.GemsClue: return clueDatas[(int)gems - 1].gemsClue;
            }

            return null;
        }

        public void EnterAnalysis()
        {
            HideNonClueBox();
            HideAllLineClue();
            ShowClueLine(GemsName.None);
        }

        public void ExitAnalysis()
        {
            HideAllLineClue();
        }

        public void ShowClueLine(GemsName gems)
        {
            if(gems == GemsName.None)
            {
                int gemsId = (int)gems;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i % 4 != 3 && IsClueUnlocked((GemsName)(gemsId / 4) + 1, i % 4))
                    {
                        lines[i].gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                lines[((int)gems * 4) - 1].gameObject.SetActive(true);
            }
            
        }

        public void LoadGateData(List<int> datas)
        {
            DataAsset.instance.SetGateLoaded(false);

            foreach (int index in datas)
            {
                int gateIndex = index * 10;
                GemsName gems = (GemsName)(index / 10);

                switch (gems)
                {
                    case GemsName.Red:
                        gateData[gateIndex].red.clear = true; break;

                    case GemsName.Orange:
                        gateData[gateIndex].orange.clear = true; break;

                    case GemsName.Yellow:
                        gateData[gateIndex].yellow.clear = true; break;

                    case GemsName.Green:
                        gateData[gateIndex].green.clear = true; break;

                    case GemsName.Turquoise:
                        gateData[gateIndex].turquoise.clear = true; break;

                    case GemsName.Navy:
                        gateData[gateIndex].navy.clear = true; break;

                    case GemsName.Violet:
                        gateData[gateIndex].violet.clear = true; break;

                    case GemsName.Pink:
                        gateData[gateIndex].pink.clear = true; break;

                    case GemsName.Black:
                        gateData[gateIndex].black.clear = true; break;

                    case GemsName.White:
                        gateData[gateIndex].white.clear = true; break;
                }
            }

            DataAsset.instance.SetGateLoaded(true);
        }

        public bool IsGateCleared(SectionName section, GemsName gems)
        {
            int gateIndex = (int)section - 1;
            switch (gems)
            {
                case GemsName.Red: return gateData[gateIndex].red.clear;
                case GemsName.Orange: return gateData[gateIndex].orange.clear;
                case GemsName.Yellow: return gateData[gateIndex].yellow.clear;
                case GemsName.Green: return gateData[gateIndex].green.clear;
                case GemsName.Turquoise: return gateData[gateIndex].turquoise.clear;
                case GemsName.Navy: return gateData[gateIndex].navy.clear;
                case GemsName.Violet: return gateData[gateIndex].violet.clear;
                case GemsName.Pink: return gateData[gateIndex].pink.clear;
                case GemsName.Black: return gateData[gateIndex].black.clear;
                case GemsName.White: return gateData[gateIndex].white.clear;
            }

            return false;
        }

        public GateProperty GetGateData(SectionName section, GemsName gems)
        {
            int gateIndex = (int)section - 1;
            switch(gems)
            {
                case GemsName.Red:          return gateData[gateIndex].red;
                case GemsName.Orange:       return gateData[gateIndex].orange;
                case GemsName.Yellow:       return gateData[gateIndex].yellow;
                case GemsName.Green:        return gateData[gateIndex].green;
                case GemsName.Turquoise:    return gateData[gateIndex].turquoise;
                case GemsName.Navy:         return gateData[gateIndex].navy;
                case GemsName.Violet:       return gateData[gateIndex].violet;
                case GemsName.Pink:         return gateData[gateIndex].pink;
                case GemsName.Black:        return gateData[gateIndex].black;
                case GemsName.White:        return gateData[gateIndex].white;
            }

            return null;
        }

        public void UpdateClearedGateData(SectionName section, GemsName gems)
        {
            int gateIndex = (int)section - 1;
            switch (gems)
            {
                case GemsName.Red:      gateData[gateIndex].red.clear = true; break;
                case GemsName.Orange:   gateData[gateIndex].orange.clear = true; break;
                case GemsName.Yellow:   gateData[gateIndex].yellow.clear = true; break;
                case GemsName.Green:    gateData[gateIndex].green.clear = true; break;
                case GemsName.Turquoise:  gateData[gateIndex].turquoise.clear = true; break;
                case GemsName.Navy:     gateData[gateIndex].navy.clear = true; break;
                case GemsName.Violet:   gateData[gateIndex].violet.clear = true; break;
                case GemsName.Pink:     gateData[gateIndex].pink.clear = true; break;
                case GemsName.Black:    gateData[gateIndex].black.clear = true; break;
                case GemsName.White:    gateData[gateIndex].white.clear = true; break;
            }
        }

        private void HideNonClueBox()
        {
            int boxLength = DataAsset.instance.GetBoxDataLenght();

            for (int i = 0; i < boxLength; i++)
            {
                BoxScriptable box = DataAsset.instance.GetBoxData(i);

                //Debug.Log("Section unlocked box id: " + box.boxId + " = " + GeneratorCalculator.IsSectionUnlocked(box.boxGems, box.boxSection));

                if (!GeneratorCalculator.IsSectionUnlocked(box.boxGems, box.boxSection))
                {
                    mapGenerator.GetBoxPosition(box.boxId).gameObject.SetActive(false);
                }
                else if (!box.isClue)
                {
                    mapGenerator.GetBoxPosition(box.boxId).gameObject.SetActive(false);
                }
            }
        }

        private void HideAllLineClue()
        {
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i].gameObject.SetActive(false);
            }
        }

        private void CreateClueData()
        {
            int boxLength = DataAsset.instance.GetBoxDataLenght();
            
            for (int i = 0; i < boxLength; i++)
            {
                BoxScriptable box = DataAsset.instance.GetBoxData(i);
                if(box.isClue)
                {
                    GemsName gems = box.boxGems;
                    if (gems != GemsName.None)
                    {
                        SectionName section = box.boxSection;
                        switch (section)
                        {
                            case SectionName.None:
                            case SectionName.Tutorial:
                            case SectionName.Zero:
                            case SectionName.First:
                                clueDatas[(int)gems - 1].firstClue.Add(i);
                                break;

                            case SectionName.Second:
                                clueDatas[(int)gems - 1].secondClue.Add(i);
                                break;

                            case SectionName.Third:
                                clueDatas[(int)gems - 1].thirdClue.Add(i);
                                break;
                        }

                    }
                }

                for (int index = 0; index < box.puzzleList.Length; index++)
                {
                    int puzzleId = DataAsset.instance.GetPuzzleDataKey(box.puzzleList[index]);
                    PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(puzzleId);

                    if (puzzle.gemsName != GemsName.None)
                    {
                        clueDatas[(int)puzzle.gemsName - 1].gemsClue.Add(box.boxId);
                    }
                }

                //int startPuzzle = box.minPuzzleId;
                //int endPuzzle = box.maxPuzzleId;

                //if(startPuzzle >= 0 && endPuzzle >= 0)
                //{
                //    for (int index = startPuzzle; index <= endPuzzle; index++)
                //    {
                //        PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleData(index);
                //        if (puzzle.gemsName != GemsName.None)
                //        {
                //            clueDatas[(int)puzzle.gemsName - 1].gemsClue.Add(box.boxId);
                //        }
                //    }
                //}
            }
        }

        private void CraeteClueLine()
        {
            clueLine = ObjectAsset.instance.GetPref(PrefType.PlayerLine).GetComponent<LineRenderer>();

            for (int i = 0; i < clueDatas.Length; i++)
            {
                CreateLine(i, Clue.FirstClue);
                CreateLine(i, Clue.SecondClue);
                CreateLine(i, Clue.ThirdClue);
                CreateLine(i, Clue.GemsClue);
            }
        }

        private void CreateLine(int i, Clue clue)
        {
            LineRenderer line = Instantiate(clueLine);
            line.name = (GemsName)(i + 1) + "_" + clue;
            line.positionCount = GetClueLength(i, clue);
            line.transform.parent = transform;

            if(GetClueLength(i, clue) > 0)
            {
                DrawLine(line, GetClueList((GemsName)i + 1, clue));
            }
            
            lines[(i * 4) + (int)clue] = line;
        }

        private int GetClueLength(int i, Clue clue)
        {
            switch(clue)
            {
                case Clue.FirstClue: return clueDatas[i].firstClue.Count;
                case Clue.SecondClue: return clueDatas[i].secondClue.Count;
                case Clue.ThirdClue: return clueDatas[i].thirdClue.Count;
                case Clue.GemsClue: return clueDatas[i].gemsClue.Count;
            }

            return 0;
        }

        private bool IsSectionUnlocked(GemsName gems, int sectionId)
        {
            MapSectionScriptable mapData = DataAsset.instance.GetMapData();

            int index = (int)gems - 1;
            if(sectionId < 5 && gems != GemsName.None)
            {
                switch (sectionId)
                {
                    case 0: return mapData.gateInfoList[index].zeroSectionUnlock;
                    case 1: return mapData.gateInfoList[index].firstSectionUnlock;
                    case 2: return mapData.gateInfoList[index].secondSectionUnlock;
                    case 3: return mapData.gateInfoList[index].thirdSectionUnlock;
                    case 4: return mapData.gateInfoList[index].fourthSectionUnlock;
                }
            }
            
            return false;
        }

        private bool IsClueUnlocked(GemsName gems, int clue)
        {
            MapSectionScriptable mapData = DataAsset.instance.GetMapData();

            int index = (int)gems - 1;
            if(gems != GemsName.None)
            {
                switch ((Clue)clue)
                {
                    case Clue.FirstClue: return mapData.gateInfoList[index].zeroSectionUnlock && mapData.gateInfoList[index].firstSectionUnlock;
                    case Clue.SecondClue: return mapData.gateInfoList[index].secondSectionUnlock;
                    case Clue.ThirdClue: return mapData.gateInfoList[index].thirdSectionUnlock;
                    case Clue.GemsClue: return false;
                }
            }

            return false;
        }

        private void DrawLine(LineRenderer line, List<int> boxId)
        {
            for(int i = 0; i < line.positionCount; i++)
            {
                //Debug.Log("Box: " + boxId[i]);
                line.SetPosition(i, mapGenerator.GetBoxPosition(boxId[i]).position);
            }
        }
    }
}

