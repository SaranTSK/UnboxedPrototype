using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class MapGenerator : MonoBehaviour
    {
        //[SerializeField] private GameObject boxIconPref;
        //[SerializeField] private LevelSectionScriptable[] levelInfoData;

        private GameObject boxIconPref;
        private GameObject tutorialSectionPref;
        private GameObject circleSectionPref;
        private MapSectionScriptable mapSectionData;
        private CircleGeneratorScriptable circleGenData;
        private MapLineGenerator lineGenerator;

        private bool isSectionUpdated;
        private bool isLineUpdated;

        private GameObject parentGo;

        private void Start()
        {
            lineGenerator = GetComponent<MapLineGenerator>();
        }

        public void Initailize()
        {
            CreateInstace();
            GenerateSection();
            GenerateBoxFrame();
            GenerateLine();
        }

        public Transform GetBoxPosition(int id)
        {
            int dataLenght = DataAsset.instance.GetBoxDataLenght();
            if (id < dataLenght)
            {
                return parentGo.transform.GetChild(id);
            }
            else
            {
                return null;
            }
        }

        public Transform GetParentBoxLine(int boxId)
        {
            return transform.GetChild(2).GetChild(boxId);
        }

        public void HideMapLine()
        {
            transform.GetChild(2).gameObject.SetActive(false);
        }

        public void ShowMapLine()
        {
            Debug.Log("Show Map Line");
            transform.GetChild(2).gameObject.SetActive(true);
        }

        public void ShowUnlockedBoxes()
        {
            int boxLength = DataAsset.instance.GetBoxDataLenght();

            for (int i = 0; i < boxLength; i++)
            {
                BoxScriptable box = DataAsset.instance.GetBoxData(i);

                if (GeneratorCalculator.IsSectionUnlocked(box.boxGems, box.boxSection))
                {
                    GetBoxPosition(box.boxId).gameObject.SetActive(true);
                }
            }
        }

        private Transform GetParentSection(int section)
        {
            return transform.GetChild(0).GetChild(section);
        }

        public IEnumerator SectionUpdate(Queue<string> queue)
        {
            isSectionUpdated = false;

            int dataLenght = queue.Count;
            if (dataLenght > 0)
            {
                for (int i = 0; i < dataLenght; i++)
                {
                    yield return new WaitForSecondsRealtime(0.5f);

                    string index = queue.Dequeue();

                    if (index.ToLower() == "tutorial")
                    {
                        // Update section sprite
                        Debug.Log("Tutorial unlocked!");
                        GetParentSection(0).GetComponent<MapTutorialSectionController>().UpdateSection();
                        StartCoroutine(lineGenerator.LineShowUp(-1));
                    }
                    else
                    {
                        int gems = int.Parse(index.Split('_').GetValue(0).ToString());
                        int section = int.Parse(index.Split('_').GetValue(1).ToString());

                        int target = ((gems - 1) * Constant.MAX_SECTIONS) + section;

                        GetParentSection(gems).GetComponent<MapSectionController>().UpdateSection(gems - 1, section);
                        StartCoroutine(lineGenerator.LineShowUp(target));
                    }
                }
            }
            else
            {
                lineGenerator.SkipSectionUpdate();
            }

            //Debug.Log("Section update queue: " + queue.Count + " | Updated: " + lineGenerator.IsSectionUpdated());

            yield return new WaitUntil(() => queue.Count == 0 && lineGenerator.IsSectionUpdated());

            isSectionUpdated = !isSectionUpdated;
            //UnlockedManager.instance.ExecuteSectionQueue();
        }

        public bool IsSectionUpdated()
        {
            return isSectionUpdated;
        }

        public IEnumerator LineAndBoxUpdate(Queue<string> queue)
        {
            isLineUpdated = false;

            if(queue.Count > 0)
            {
                StartCoroutine(lineGenerator.LineAndBoxUpdate(queue));
            }
            else
            {
                lineGenerator.SkipLineUpdate();
            }
            

            yield return new WaitUntil(() => lineGenerator.IsLineUpdated());

            isLineUpdated = !isLineUpdated;
            //UnlockedManager.instance.ExecuteBoxQueue();
        }

        public IEnumerator LineOnlyUpdate(Queue<string> queue)
        {
            isLineUpdated = false;
            if (queue.Count > 0)
            {
                StartCoroutine(lineGenerator.LineOnlyUpdate(queue));
            }
            else
            {
                lineGenerator.SkipLineUpdate();
            }

            yield return new WaitUntil(() => lineGenerator.IsLineUpdated());

            isLineUpdated = !isLineUpdated;
            //UnlockedManager.instance.ExecuteLineQueue();
        }

        public bool IsLineUpdated()
        {
            return isLineUpdated;
        }

        private void CreateInstace()
        {
            boxIconPref = ObjectAsset.instance.GetPref(PrefType.BoxIcon);
            tutorialSectionPref = ObjectAsset.instance.GetPref(PrefType.TutorialSection);
            circleSectionPref = ObjectAsset.instance.GetPref(PrefType.CircleSection);
            mapSectionData = DataAsset.instance.GetMapData();
            circleGenData = DataAsset.instance.GetCircleData(0);
        }

        private void GenerateBoxFrame()
        {
            parentGo = new GameObject();
            parentGo.name = "BoxData";
            parentGo.transform.parent = transform;

            int dataLenght = DataAsset.instance.GetBoxDataLenght();

            for (int level = 0; level < circleGenData.circle.Length; level++)
            {
                Debug.Log("Level " + level);
                CircleProperty circle = circleGenData.circle[level];
                int amount = circleGenData.circle[level].splitSection;
                float angle = 360 / amount;

                //GameObject parentGo = new GameObject();
                //parentGo.name = "Level " + level;
                //parentGo.transform.parent = mainParentGo.transform;

                //Check missing data
                //if (!(level < levelInfoData.Length))
                //{
                //    Debug.LogWarning("Level " + level + " has missing data");
                //}

               
                for (int point = 0; point < amount; point++)
                {
                    float theta = angle * point + circle.modifyAngle;
                    int boxId = GeneratorCalculator.GetBoxArrayIndex(circleGenData, level, point);

                    GameObject go = Instantiate(boxIconPref);
                    go.transform.position = GeneratorCalculator.TransformCalculate(theta, circle.circleRadius);
                    go.transform.parent = parentGo.transform;
                    go.name = "Box_" + boxId;

                    //TODO: change to real index

                    //if(level < levelInfoData.Length)
                    //{
                    //    if (point < levelInfoData[level].boxInfo.Length)
                    //    {
                    //        boxController.Initailize(levelInfoData[level].boxInfo[point]);
                    //    }
                    //    else
                    //    {
                    //        Debug.LogWarning("Level " + level + "|Box " + point + " has missing data");
                    //        boxController.Initailize(ScriptableObject.CreateInstance<BoxScriptable>());
                    //    }
                    //}

                    BoxController boxController = go.GetComponent<BoxController>();

                    if (boxId < dataLenght)
                    {
                        boxController.Initailize(DataAsset.instance.GetBoxData(boxId));
                    }
                    else
                    {
                        Debug.LogWarning("Level " + level + "|Box " + boxId + " has missing data");
                        boxController.Initailize(ScriptableObject.CreateInstance<BoxScriptable>());
                    }
                }
            }

        }

        private void GenerateSection()
        {
            GameObject mainParentGo = new GameObject();
            mainParentGo.name = "SectionData";
            mainParentGo.transform.parent = transform;

            GameObject goTutorial = Instantiate(tutorialSectionPref);
            goTutorial.GetComponent<MapTutorialSectionController>().Initialize(mapSectionData.tutorialUnlock);
            goTutorial.transform.parent = mainParentGo.transform;
            goTutorial.name = "Section Tutorial";

            for (int i = 0; i < mapSectionData.gateInfoList.Length; i++)
            {
                int angle = i * 36;
                GameObject go = Instantiate(circleSectionPref);
                go.GetComponent<MapSectionController>().Initailize(mapSectionData.gateInfoList[i], angle - 18);
                go.transform.parent = mainParentGo.transform;
                go.name = "Section " + i;
            }
        }

        private void GenerateLine()
        {
            GameObject lineParent = new GameObject();
            lineParent.name = "LineData";
            lineParent.transform.parent = transform;

            lineGenerator.Initialize();
            lineGenerator.LineGenerate(lineParent.transform);
        }

    }
}


