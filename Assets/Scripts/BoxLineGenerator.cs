using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class BoxLineGenerator : MonoBehaviour
    {
        private LineRenderer line;
        private LevelManager levelManager;
        private bool isUpdated;
        //private List<LineRenderer> lines;

        public void Initialize()
        {
            line = ObjectAsset.instance.GetPref(PrefType.MapLine).GetComponent<LineRenderer>();
            levelManager = LevelManager.instance;
            isUpdated = false;
        }

        public void LineGenerate(BoxScriptable boxInfo)
        {
            //List<int> idList = levelManager.GetPuzzleIdList();
            //int dataLenght = idList.Count;

            for (int i = 0; i < boxInfo.puzzleList.Length; i++)
            {
                int puzzleId = DataAsset.instance.GetPuzzleDataKey(boxInfo.puzzleList[i]);
                PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(puzzleId);

                int connected = puzzle.puzzleConnectedList.Length;

                if (connected > 0)
                {
                    for (int k = 0; k < connected; k++)
                    {
                        //PuzzleScriptable targetPuzzle = DataAsset.instance.GetPuzzleDataValue(puzzle.connectedList[k].puzzleTargetId);

                        PuzzleScriptable targetPuzzle = DataAsset.instance.GetPuzzleDataValue(puzzle.puzzleConnectedList[k]);

                        Transform startPoint = levelManager.GetPuzzleTransform(puzzle.circleLevel, puzzle.levelIndex);
                        Transform endPoint = levelManager.GetPuzzleTransform(targetPuzzle.circleLevel, targetPuzzle.levelIndex);

                        DrawLine(startPoint, endPoint, startPoint);

                        //string condtion = puzzle.connectedList[k].puzzleConditionId;
                        Transform parent = levelManager.GetPuzzleTransform(targetPuzzle.circleLevel, targetPuzzle.levelIndex);
                        PuzzleController puzzleController = parent.GetComponent<PuzzleController>();

                        if (puzzleController.CheckPuzzleCleared(puzzle.puzzleConnectedList[k]))
                        {
                            LineRenderer line = GetLine(puzzle, k);
                            line.startColor = Color.green;
                            line.endColor = Color.green;
                        }
                    }
                }
            }
        }

        //public IEnumerator AllLineUpdate(Queue<int> indexQueue)
        //{
        //    isUpdated = false;

        //    Debug.Log("Line update queue count: " + indexQueue.Count);

        //    for (int i = 0; i < indexQueue.Count; i++)
        //    {
        //        PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(indexQueue.Dequeue());
        //        Debug.Log("Puzzle: " + puzzle.name);

        //        int connected = puzzle.connectedList.Length;

        //        if (connected > 0)
        //        {
        //            for (int k = 0; k < connected; k++)
        //            {
        //                yield return new WaitForSecondsRealtime(0.5f);

        //                Debug.Log("Check " + i + " and " + k);

        //                LineRenderer line = GetLine(puzzle, k);
        //                line.startColor = Color.green;
        //                line.endColor = Color.green;

        //                PuzzleScriptable targetPuzzle = DataAsset.instance.GetPuzzleDataValue(puzzle.connectedList[k].puzzleTargetId);
        //                Transform parent = levelManager.GetPuzzleTransform(targetPuzzle.circleLevel, targetPuzzle.levelIndex);
        //                parent.GetComponent<PuzzleController>().UpdateSprite();
        //            } 
        //        }
        //    }

        //    yield return new WaitUntil(() => indexQueue.Count == 0);
        //    isUpdated = !isUpdated;
        //}

        public void PuzzleAndLineUpdate(string queueValue)
        {
            isUpdated = false;

            Debug.Log("Puzzle and line update queue value: " + queueValue);
            int puzzleId = int.Parse(queueValue.Split('_').GetValue(0).ToString());
            int puzzleIndex = int.Parse(queueValue.Split('_').GetValue(1).ToString());
            
            PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(puzzleId);

            LineRenderer line = GetLine(puzzle, puzzleIndex);
            line.startColor = Color.green;
            line.endColor = Color.green;

            //PuzzleScriptable targetPuzzle = DataAsset.instance.GetPuzzleDataValue(puzzle.connectedList[puzzleIndex].puzzleTargetId);
            PuzzleScriptable targetPuzzle = DataAsset.instance.GetPuzzleDataValue(puzzle.puzzleConnectedList[puzzleIndex]);
            Transform parent = levelManager.GetPuzzleTransform(targetPuzzle.circleLevel, targetPuzzle.levelIndex);
            parent.GetComponent<PuzzleController>().UpdateSprite();

            isUpdated = !isUpdated;
        }

        public void OnlyLineUpdate(string queueValue)
        {
            isUpdated = false;

            Debug.Log("Only line update queue value: " + queueValue);
            int puzzleId = int.Parse(queueValue.Split('_').GetValue(0).ToString());
            int puzzleIndex = int.Parse(queueValue.Split('_').GetValue(1).ToString());

            PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(puzzleId);

            LineRenderer line = GetLine(puzzle, puzzleIndex);
            line.startColor = Color.green;
            line.endColor = Color.green;

            isUpdated = !isUpdated;
        }

        public void ResetLineOnly(string queueValue)
        {
            Debug.Log("Reset line queue value: " + queueValue);
            int puzzleId = int.Parse(queueValue.Split('_').GetValue(0).ToString());
            int puzzleIndex = int.Parse(queueValue.Split('_').GetValue(1).ToString());

            PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(puzzleId);

            LineRenderer line = GetLine(puzzle, puzzleIndex);
            line.startColor = Color.white;
            line.endColor = Color.white;
        }

        public bool IsUpdated()
        {
            return isUpdated;
        }

        public void SkippedUpdated()
        {
            isUpdated = true;
        }

        private void DrawLine(Transform startPoint, Transform endPoint, Transform parent)
        {
            if (startPoint != null && endPoint != null)
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
                Debug.LogWarning("Missing puzzle transform");
            }
        }

        //private bool CheckPuzzleCleared(string target)
        //{
        //    if(target.Length > 0)
        //    {
        //        string[] split = target.Split('_');

        //        for (int i = 0; i < split.Length; i++)
        //        {
        //            int index = int.Parse(split[i]);
        //            PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(index);

        //            if (puzzle.clear)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogWarning("Puzzle " + name + " is missing unlock condition");
        //    }
           

        //    return false;
        //}

        private LineRenderer GetLine(PuzzleScriptable puzzle, int lineIndex)
        {
            Transform parent = levelManager.GetPuzzleTransform(puzzle.circleLevel, puzzle.levelIndex);
            LineRenderer line = parent.GetChild(lineIndex).GetComponent<LineRenderer>();

            return line;
        }


        // TODO: For moock up generator

        public void LineMockupGenerate(List<PuzzleScriptable> puzzleList, Transform parent)
        {
            Debug.Log(puzzleList.Count);

            for (int i = 0; i < puzzleList.Count; i++)
            {
                PuzzleScriptable puzzle = puzzleList[i];

                int connected = puzzle.puzzleConnectedList.Length;

                if (connected > 0)
                {
                    for (int k = 0; k < connected; k++)
                    {
                        //int index = puzzle.connectedList[k].puzzleTargetId;
                        int index = puzzle.puzzleConnectedList[k];
                        Debug.Log("Index: " + index);
                        PuzzleScriptable targetPuzzle = puzzleList[index];

                        Transform startPoint = parent.GetChild(puzzle.circleLevel).GetChild(puzzle.levelIndex);
                        Transform endPoint = parent.GetChild(targetPuzzle.circleLevel).GetChild(targetPuzzle.levelIndex);

                        DrawLine(startPoint, endPoint, startPoint);

                        //string condtion = puzzle.connectedList[k].puzzleConditionId;
                        Transform puzzleParent = levelManager.GetPuzzleTransform(targetPuzzle.circleLevel, targetPuzzle.levelIndex);
                        PuzzleController puzzleController = parent.GetComponent<PuzzleController>();

                        if (puzzleController.CheckPuzzleCleared(targetPuzzle.puzzleId))
                        {
                            LineRenderer line = GetLine(puzzle, k);
                            line.startColor = Color.green;
                            line.endColor = Color.green;
                        }
                    }
                }
            }
        }
    }
}

