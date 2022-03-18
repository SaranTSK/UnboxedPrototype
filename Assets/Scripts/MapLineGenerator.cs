using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class MapLineGenerator : MonoBehaviour
    {
        [SerializeField] private float animatedDuration;

        private LineRenderer line;
        private MapGenerator generator;

        private List<List<string>> lineSections;
        private List<string> lineNoneSection;

        private bool isSectionUpdated;
        private bool isLineUpdated;

        public void Initialize()
        {
            //line = transform.GetComponent<LineRenderer>();
            line = ObjectAsset.instance.GetPref(PrefType.MapLine).GetComponent<LineRenderer>();
            generator = GetComponent<MapGenerator>();
            CreateLineSectionList();
        }

        public void LineGenerate(Transform parent)
        {
            int dataLenght = DataAsset.instance.GetBoxDataLenght();

            for(int i = 0; i < dataLenght; i++)
            {
                BoxScriptable boxInfo = DataAsset.instance.GetBoxData(i);
                GameObject box = new GameObject();
                box.name = "Box " + boxInfo.boxId;
                box.transform.parent = parent;

                int connected = boxInfo.connectedList.Length;

                if (connected > 0)
                {
                    for (int k = 0; k < connected; k++)
                    {
                        int target = boxInfo.connectedList[k].boxTargetId;
                        //BoxScriptable targetBoxInfo = DataAsset.instance.GetBoxData(target);
                        if (target < dataLenght)
                        {
                            BoxScriptable targetBoxInfo = DataAsset.instance.GetBoxData(target);

                            string lineId = i + "_" + k;
                            GemsName gems = CheckLineSection(boxInfo, targetBoxInfo);
                            AddChildLineList(gems, (int)targetBoxInfo.boxSection, lineId);

                            Transform startPoint = generator.GetBoxPosition(boxInfo.boxId);
                            Transform endPoint = generator.GetBoxPosition(target);

                            DrawLine(startPoint, endPoint, box.transform);

                            string condition = boxInfo.connectedList[k].puzzleConditionId;

                            if (CheckPuzzleCleared(condition))
                            {
                                LineRenderer line = GetLine(i, k);
                                line.startColor = Color.green;
                                line.endColor = Color.green;
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Missing box id: " + target);
                        }

                        
                    }
                }
            }

            CheckUnlockedSection();
        }

        public IEnumerator LineShowUp(int index)
        {
            isSectionUpdated = false;

            // TODO: index show incorrect line
            Debug.Log("Get line parent: " + index);
            List<string> tempList = (index == -1) ? lineNoneSection : lineSections[index];

            Debug.Log("Count: " + tempList.Count);

            for (int i = 0; i < tempList.Count; i++)
            {
                yield return new WaitForSecondsRealtime(0.5f);

                //Debug.Log("String: " + tempList[i]);
                int boxId = int.Parse(tempList[i].Split('_').GetValue(0).ToString());
                int lineId = int.Parse(tempList[i].Split('_').GetValue(1).ToString());

                generator.GetParentBoxLine(boxId).gameObject.SetActive(true);

                // TODO: Play show line animations
                GetLine(boxId, lineId).gameObject.SetActive(true);
                StartCoroutine(AnimateLine(GetLine(boxId, lineId)));
            }

            isSectionUpdated = !isSectionUpdated;
        }

        public void SkipSectionUpdate()
        {
            isSectionUpdated = true;
        }

        public bool IsSectionUpdated()
        {
            return isSectionUpdated;
        }

        // Check eeach line connected
        public IEnumerator LineAndBoxUpdate(Queue<string> indexQueue)
        {
            isLineUpdated = false;

            int dataLenght = indexQueue.Count;

            for (int i = 0; i < dataLenght; i++)
            {
                yield return new WaitForSecondsRealtime(0.5f);

                string value = indexQueue.Dequeue();
                //Debug.Log("Queue value: " + value);

                int boxId = int.Parse(value.Split('_').GetValue(0).ToString());
                int lineId = int.Parse(value.Split('_').GetValue(1).ToString());

                

                LineRenderer line = GetLine(boxId, lineId);
                line.startColor = Color.green;
                line.endColor = Color.green;

                BoxScriptable box = DataAsset.instance.GetBoxData(boxId);
                int targetId = box.connectedList[lineId].boxTargetId;
                ChangeCemareFocus(targetId);
                generator.GetBoxPosition(targetId).GetComponent<BoxController>().UpdateSprite();
            }

            yield return new WaitUntil(() => indexQueue.Count == 0);
            isLineUpdated = !isLineUpdated;
        }

        // Check each line connected
        public IEnumerator LineOnlyUpdate(Queue<string> indexQueue)
        {
            isLineUpdated = false;

            int dataLenght = indexQueue.Count;

            for (int i = 0; i < dataLenght; i++)
            {
                yield return new WaitForSecondsRealtime(0.5f);

                string value = indexQueue.Dequeue();

                int boxId = int.Parse(value.Split('_').GetValue(0).ToString());
                int lineId = int.Parse(value.Split('_').GetValue(1).ToString());

                LineRenderer line = GetLine(boxId, lineId);
                line.startColor = Color.green;
                line.endColor = Color.green;

                BoxScriptable box = DataAsset.instance.GetBoxData(boxId);
                int targetId = box.connectedList[lineId].boxTargetId;
                ChangeCemareFocus(targetId);
            }

            yield return new WaitUntil(() => indexQueue.Count == 0);
            isLineUpdated = !isLineUpdated;
        }

        private void ChangeCemareFocus(int id)
        {
            Transform boxTransform = generator.GetBoxPosition(id);
            LevelManager.instance.FocusAtUnlockPosition(boxTransform.position);
        }

        public void SkipLineUpdate()
        {
            isLineUpdated = true;
        }

        public bool IsLineUpdated()
        {
            return isLineUpdated;
        }

        //public IEnumerator LineShowUp(Queue<string> indexQueue)
        //{
        //    int dataLenght = indexQueue.Count;

        //    for (int i = 0; i < dataLenght; i++)
        //    {
        //        string index = indexQueue.Dequeue();

        //        if(index.ToLower() == "tutorial")
        //        {
        //            StartCoroutine(CheckSectionLine(-1));
        //        }
        //        else
        //        {
        //            StartCoroutine(CheckSectionLine(int.Parse(index)));
        //        }
        //    }

        //    yield return new WaitUntil(() => indexQueue.Count == 0);
        //    isUpdated = !isUpdated;
        //}

        // TODO: Draw double line for background line and animated line
        private void DrawLine(Transform startPoint, Transform endPoint, Transform parent)
        {
            if(startPoint != null && endPoint != null)
            {
                LineRenderer lineRenderer = Instantiate(line);
                lineRenderer.transform.parent = parent;

                Vector3 startPos = new Vector3(startPoint.position.x, startPoint.position.y, 1);
                Vector3 endPos = new Vector3(endPoint.position.x, endPoint.position.y, 1);

                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, endPos);
            }
            else
            {
                Debug.LogWarning("Missing box transform");
            }
        }

        private IEnumerator AnimateLine(LineRenderer line)
        {
            float startTime = Time.time;

            Vector3 startPos = line.GetPosition(0);
            Vector3 endPos = line.GetPosition(1);

            Vector3 pos = startPos;

            while(pos != endPos)
            {
                float t = (Time.time - startTime) / animatedDuration;
                pos = Vector3.Lerp(startPos, endPos, t);
                line.SetPosition(1, pos);

                yield return null;
            }
        }

        private LineRenderer GetLine(int boxId, int lineIndex)
        {
            Transform parent = generator.GetParentBoxLine(boxId);
            LineRenderer line = parent.GetChild(lineIndex).GetComponent<LineRenderer>();

            return line;
        }

        private void CheckUnlockedSection()
        {
            MapSectionScriptable mapData = DataAsset.instance.GetMapData();

            // Check none section
            //if(!mapData.tutorialUnlock)
            //{
            //    CheckSectionBoxes(DataAsset.instance.GetBoxListTutorialSection());
            //}

            // Check color section
            int dataLenght = DataAsset.instance.GetBoxDataLenght();

            for (int i = 0; i < dataLenght; i++)
            {
                
                BoxScriptable box = DataAsset.instance.GetBoxData(i);
                int gemsId = (int)box.boxGems;

                if (i == 0)
                {
                    CheckSectionUnlocked(i, mapData.tutorialUnlock);
                    //generator.GetParentBoxLine(i).gameObject.SetActive(mapData.tutorialUnlock);
                }
                else
                {
                    if(!box.isGate)
                    {
                        switch (box.boxSection)
                        {
                            case SectionName.Tutorial:
                                int id = box.boxId; // Calculate gems id for tutorial level
                                bool value = mapData.gateInfoList[id - 1].zeroSectionUnlock && mapData.gateInfoList[id].zeroSectionUnlock; //Check 2 colors
                                CheckSectionUnlocked(i, value); break;

                            case SectionName.Zero: CheckSectionUnlocked(i, mapData.gateInfoList[gemsId - 1].zeroSectionUnlock); break;
                            case SectionName.First: CheckSectionUnlocked(i, mapData.gateInfoList[gemsId - 1].firstSectionUnlock); break;
                            case SectionName.Second: CheckSectionUnlocked(i, mapData.gateInfoList[gemsId - 1].secondSectionUnlock); break;
                            case SectionName.Third: CheckSectionUnlocked(i, mapData.gateInfoList[gemsId - 1].thirdSectionUnlock); break;
                            case SectionName.Fourth: CheckSectionUnlocked(i, mapData.gateInfoList[gemsId - 1].fourthSectionUnlock); break;
                        }
                    }
                    else
                    {
                        switch (box.boxSection)
                        {
                            case SectionName.Zero:
                                int id = box.boxId - 24; // Calculate gems id for level 4
                                bool value = mapData.gateInfoList[id - 1].firstSectionUnlock && mapData.gateInfoList[id].firstSectionUnlock; //Check 2 colors
                                CheckSectionUnlocked(i, value); break;

                            case SectionName.First: CheckSectionUnlocked(i, mapData.gateInfoList[gemsId - 1].secondSectionUnlock); break;
                            case SectionName.Second: CheckSectionUnlocked(i, mapData.gateInfoList[gemsId - 1].thirdSectionUnlock); break;
                            case SectionName.Third: CheckSectionUnlocked(i, mapData.gateInfoList[gemsId - 1].fourthSectionUnlock); break;
                        }
                    }
                    
                }

                //if (!mapData.gateInfoList[i].zeroSectionUnlock)
                //{
                //    List<int> idList = DataAsset.instance.GetBoxListBySection(gems, (int)SectionName.Zero);
                //    CheckSectionBoxes(idList);
                //}

                //if (!mapData.gateInfoList[i].firstSectionUnlock)
                //{
                //    List<int> idList = DataAsset.instance.GetBoxListBySection(gems, (int)SectionName.First);
                //    CheckSectionBoxes(idList);
                //}

                //if (!mapData.gateInfoList[i].secondSectionUnlock)
                //{
                //    List<int> idList = DataAsset.instance.GetBoxListBySection(gems, (int)SectionName.Second);
                //    CheckSectionBoxes(idList);
                //}

                //if (!mapData.gateInfoList[i].thirdSectionUnlock)
                //{
                //    List<int> idList = DataAsset.instance.GetBoxListBySection(gems, (int)SectionName.Third);
                //    CheckSectionBoxes(idList);
                //}

                //if (!mapData.gateInfoList[i].fourthSectionUnlock)
                //{
                //    List<int> idList = DataAsset.instance.GetBoxListBySection(gems, (int)SectionName.Fourth);
                //    CheckSectionBoxes(idList);
                //}
            }
        }

        private void CheckSectionUnlocked(int id, bool value)
        {
            int length = generator.GetParentBoxLine(id).childCount;
            for (int i = 0; i < length; i++)
            {
                Transform child = generator.GetParentBoxLine(id).GetChild(i);
                child.gameObject.SetActive(value);
            }

            generator.GetParentBoxLine(id).gameObject.SetActive(value);
        }

        private void CreateLineSectionList()
        {
            lineNoneSection = new List<string>();
            lineSections = new List<List<string>>();

            MapSectionScriptable mapData = DataAsset.instance.GetMapData();
            for (int i = 0; i < mapData.gateInfoList.Length * Constant.MAX_SECTIONS; i++)
            {
                lineSections.Add(new List<string>());
            }
        }

        private void AddChildLineList(GemsName gems, int section, string lineId)
        {
            if (lineSections != null)
            {
                if (gems != GemsName.None)
                {
                    int index = (((int)gems - 1) * Constant.MAX_SECTIONS) + section;
                    if (lineSections[index] != null)
                    {
                        lineSections[index].Add(lineId);
                    }
                }
                else
                {
                    lineNoneSection.Add(lineId);
                }

            }
        }

        //private void CheckUnlockedPuzzle()
        //{
        //    int dataLenght = DataAsset.instance.GetBoxDataLenght();

        //    for (int i = 0; i < dataLenght; i++)
        //    {
        //        BoxScriptable boxInfo = DataAsset.instance.GetBoxData(i);

        //        int connected = boxInfo.connectedList.Length;

        //        if (connected > 0)
        //        {
        //            for (int k = 0; k < connected; k++)
        //            {
        //                string target = boxInfo.connectedList[k].puzzleConditionId;

        //                if (CheckPuzzleCleared(target))
        //                {
        //                    LineRenderer line = GetLine(i, k);
        //                    line.startColor = Color.green;
        //                    line.endColor = Color.green;
        //                }
        //            }
        //        }
        //    }
        //}

        private bool CheckPuzzleCleared(string target)
        {
            string[] split = target.Split('_');

            for (int i = 0; i < split.Length; i++)
            {
                int index;

                if (int.TryParse(split[i], out index))
                {
                    int dataLenght = DataAsset.instance.GetPuzzleDataLenght();
                    if (index < dataLenght)
                    {
                        PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(index);

                        if (puzzle.clear)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Puzzle " + index + " has missing data");
                    }
                }
            }

            return false;
        }

        private GemsName CheckLineSection(BoxScriptable fisrtBox, BoxScriptable secondBox)
        {
            if(fisrtBox.boxGems == secondBox.boxGems || (fisrtBox.boxGems != GemsName.None && secondBox.boxGems == GemsName.None))
            {
                return fisrtBox.boxGems;
            }
            else if(fisrtBox.boxGems == GemsName.None && secondBox.boxGems != GemsName.None)
            {
                return secondBox.boxGems;
            }
            else
            {
                return GemsName.None;
            }
        }
    }
}

