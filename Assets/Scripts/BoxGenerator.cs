using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class BoxGenerator : MonoBehaviour
    {
        private GameObject boxSymbolPref;
        private CircleGeneratorScriptable circleGenData;
        private BoxScriptable boxInfo;
        private BoxLineGenerator lineGenerator;

        private bool isPuzzleUpdated;

        public void Initailize()
        {
            lineGenerator = GetComponent<BoxLineGenerator>();

            if (LevelManager.instance.GetBoxInfo() != null)
            {
                boxInfo = LevelManager.instance.GetBoxInfo();
                boxSymbolPref = ObjectAsset.instance.GetPref(PrefType.BoxSymbol);
                circleGenData = DataAsset.instance.GetCircleData((int)boxInfo.circleGenerate);
            }

            GeneratePuzzleFrame();
        }

        // Selected puzzle and line show up
        public IEnumerator UpdatePuzzleAndLineQueue(Queue<string> queue)
        {
            isPuzzleUpdated = false;

            int dataLength = queue.Count;
            if (dataLength > 0)
            {
                for (int i = 0; i < dataLength; i++)
                {
                    yield return new WaitForSecondsRealtime(0.5f);
                    lineGenerator.PuzzleAndLineUpdate(queue.Dequeue());
                }
            }
            else
            {
                lineGenerator.SkippedUpdated();
            }

            yield return new WaitUntil(() => lineGenerator.IsUpdated() && queue.Count == 0);
            isPuzzleUpdated = !isPuzzleUpdated;
        }

        public IEnumerator UpdatePuzzleLineQueue(Queue<string> queue)
        {
            isPuzzleUpdated = false;
            int dataLength = queue.Count;
            if (dataLength > 0)
            {
                for (int i = 0; i < dataLength; i++)
                {
                    yield return new WaitForSecondsRealtime(0.5f);
                    lineGenerator.OnlyLineUpdate(queue.Dequeue());
                }
            }
            else
            {
                lineGenerator.SkippedUpdated();
            }

            yield return new WaitUntil(() => lineGenerator.IsUpdated() && queue.Count == 0);
            isPuzzleUpdated = !isPuzzleUpdated;
        }

        public IEnumerator UpdatePuzzleQueue(Queue<int> queue)
        {
            isPuzzleUpdated = false;

            int dataLength = queue.Count;
            for (int i = 0; i < dataLength; i++)
            {
                yield return new WaitForSecondsRealtime(0.5f);
                PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(queue.Dequeue());
                Transform parent = LevelManager.instance.GetPuzzleTransform(puzzle.circleLevel, puzzle.levelIndex);
                Debug.Log("Puzzle " + puzzle.name + "|circle: " + puzzle.circleLevel + "|index: " + puzzle.levelIndex + "|Parent: " + parent.name);
                parent.GetComponent<PuzzleController>().UpdateSprite();
            }

            yield return new WaitUntil(() => lineGenerator.IsUpdated() && queue.Count == 0);
            isPuzzleUpdated = !isPuzzleUpdated;
        }

        public IEnumerator ResetLinePuzzleQueue(Queue<string> queue)
        {
            isPuzzleUpdated = false;
            int dataLength = queue.Count;
            if (dataLength > 0)
            {
                for (int i = 0; i < dataLength; i++)
                {
                    yield return new WaitForSecondsRealtime(0.5f);
                    lineGenerator.ResetLineOnly(queue.Dequeue());
                }
            }
            else
            {
                lineGenerator.SkippedUpdated();
            }

            yield return new WaitUntil(() => lineGenerator.IsUpdated() && queue.Count == 0);
            isPuzzleUpdated = !isPuzzleUpdated;
        }

        public bool IsPuzzleUpdated()
        {
            return isPuzzleUpdated;
        }

        private void GeneratePuzzleFrame()
        {
            for (int level = 0; level < circleGenData.circle.Length; level++)
            {
                CircleProperty circle = circleGenData.circle[level];
                int amount = circle.splitSection;
                float angle = 360 / amount;

                GameObject parentGo = new GameObject("Level " + level);
                LevelManager.instance.AddChildBox(parentGo.transform);

                for (int point = 0; point < amount; point++)
                {
                    float theta = angle * point + circle.modifyAngle;

                    GameObject go = Instantiate(boxSymbolPref);
                    go.transform.position = GeneratorCalculator.TransformCalculate(theta, circle.circleRadius);
                    go.transform.parent = parentGo.transform;
                    go.name = "Puzzle_" + point;

                    PuzzleController puzzleController = go.GetComponent<PuzzleController>();

                    if(boxInfo.puzzleList[point] != null)
                    {
                        int index;

                        if (boxInfo.isGate && boxInfo.boxSection != SectionName.Zero)
                        {
                            index = point;
                        }
                        else
                        {
                            index = (level == 0) ? 0 : (amount * (level - 1)) + point + 1;
                        }
                        
                        int puzzleId = DataAsset.instance.GetPuzzleDataKey(boxInfo.puzzleList[index]);
                        PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(puzzleId);

                        puzzleController.Initailize(puzzle, puzzleId);
                        //LevelManager.instance.AddChildPuzzleId(puzzleId);
                    }
                    else
                    {
                        Debug.LogWarning(boxInfo.boxName + "|Level " + level + "|Box " + point + " has missing data");
                        puzzleController.Initailize(ScriptableObject.CreateInstance<PuzzleScriptable>(), 0);
                    }
                }
            }

            GeneratePuzzleLine();
        }

        private void GeneratePuzzleLine()
        {
            lineGenerator.Initialize();
            lineGenerator.LineGenerate(boxInfo);
        }


        // TODO: For moock up generator

        public void Initailize(BoxScriptable _boxInfo)
        {
            lineGenerator = GetComponent<BoxLineGenerator>();

            boxInfo = _boxInfo;
            boxSymbolPref = ObjectAsset.instance.GetPref(PrefType.BoxSymbol);
            circleGenData = DataAsset.instance.GetCircleData((int)_boxInfo.circleGenerate);

            GenerateMockupPuzzleFrame();
        }

        private void GenerateMockupPuzzleFrame()
        {
            GameObject boxParent = new GameObject("BoxInfo");
            List<PuzzleScriptable> puzzleList = new List<PuzzleScriptable>();

            for (int level = 0; level < circleGenData.circle.Length; level++)
            {
                CircleProperty circle = circleGenData.circle[level];
                int amount = circle.splitSection;
                float angle = 360 / amount;

                GameObject parentGo = new GameObject("Level " + level);
                parentGo.transform.parent = boxParent.transform;

                //TODO: Change to real index from DataAsset
                for (int point = 0; point < amount; point++)
                {
                    float theta = angle * point + circle.modifyAngle;

                    GameObject go = Instantiate(boxSymbolPref);
                    go.transform.position = GeneratorCalculator.TransformCalculate(theta, circle.circleRadius);
                    go.transform.parent = parentGo.transform;
                    go.name = "Puzzle_" + point;

                    //TODO: New code
                    PuzzleController puzzleController = go.GetComponent<PuzzleController>();

                    if (boxInfo.puzzleList[point] != null)
                    {
                        int index;

                        if (boxInfo.isGate && boxInfo.boxSection != SectionName.Zero)
                        {
                            index = point;
                        }
                        else
                        {
                            index = (level == 0) ? 0 : (amount * (level - 1)) + point + 1;
                        }

                        PuzzleScriptable puzzle = Instantiate(boxInfo.puzzleList[index]);
                        puzzleController.Initailize(puzzle, puzzle.puzzleId);
                        puzzleList.Add(puzzle);
                    }
                    else
                    {
                        Debug.LogWarning(boxInfo.boxName + "|Level " + level + "|Box " + point + " has missing data");
                        puzzleController.Initailize(ScriptableObject.CreateInstance<PuzzleScriptable>(), 0);
                        puzzleList.Add(ScriptableObject.CreateInstance<PuzzleScriptable>());
                    }
                }
            }

            lineGenerator.Initialize();
            lineGenerator.LineMockupGenerate(puzzleList, boxParent.transform);
        }
    }
}

