using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class PuzzleController : MonoBehaviour
    {
        private PuzzleScriptable puzzleInfo;
        private Grid grid;
        private LevelManager levelManager;
        private PuzzleLineGenerator lineGenerator;
        private GameObject dotPref;

        private int puzzleId;
        private SpriteRenderer icon;

        public void Initailize(PuzzleScriptable _puzzleInfo, int _puzzleId)
        {
            puzzleInfo = _puzzleInfo;
            puzzleId = _puzzleId;
            lineGenerator = GetComponent<PuzzleLineGenerator>();

            dotPref = ObjectAsset.instance.GetPref(PrefType.Dot);
            levelManager = LevelManager.instance;

            icon = GetComponent<SpriteRenderer>();
            SetIconSprite();
        }

        public void OnClick()
        {
            if(puzzleInfo.unlock && puzzleInfo.unlockGems)
            {
                LoadingPuzzle();
                levelManager.SetPuzzleIndex(puzzleInfo.circleLevel, puzzleInfo.levelIndex);
            }
            else
            {
                PopupCenterData popup = new PopupCenterData(PopupCenterType.SingleButtonText, "ALERT", "Puzzle [ID-" + puzzleId + "] is locked!", false);
                QueueManager.instance.AddPopupCenterQueue(popup);
                QueueManager.instance.ExecutePopupCenterQueue();
                Debug.LogWarning("Puzzle locked!!!");
            }
            
        }

        public void UpdateSprite()
        {
            Debug.Log("Update puzzle: " + puzzleInfo.name + " = " + puzzleInfo.unlock + "| Gems unlock = " + puzzleInfo.unlockGems);
            int puzzleId = DataAsset.instance.GetPuzzleDataKey(puzzleInfo);
            puzzleInfo = DataAsset.instance.GetPuzzleDataValue(puzzleId);
            SetIconSprite();
        }

        private void SetIconSprite()
        {
            if (!puzzleInfo.unlock && !puzzleInfo.unlockGems)
            {
                icon.sprite = SpriteAsset.instance.GetLockSprite(0);
            }
            else if(!puzzleInfo.unlock && puzzleInfo.unlockGems)
            {
                icon.sprite = SpriteAsset.instance.GetLockSprite((int)puzzleInfo.gemsName);
            }
            else
            {
                icon.sprite = SpriteAsset.instance.GetPuzzleSprite(puzzleInfo.gemsName);
            }

        }

        private void LoadingPuzzle()
        {
            Debug.Log("Loading...");
            UIManager.instance.UpdateHeaderText(puzzleId.ToString());
            levelManager.EnterPuzzle();

            grid = new Grid();
            grid.Initailize(puzzleInfo.width, puzzleInfo.height, puzzleInfo.gridGap, dotPref.GetComponent<SpriteRenderer>());

            // Add temp data to LineGenerator object
            lineGenerator.Initialize(grid);

            // Add temp data to LevelManager object
            levelManager.SetGrid(grid);
            levelManager.CreateDotArray(puzzleInfo.width * puzzleInfo.height);

            GridGenerate();
        }

        private void GridGenerate()
        {
            for (int x = 0; x < grid.GridArray.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GridArray.GetLength(1); y++)
                {
                    int index = GeneratorCalculator.GetDotArrayIndex(x, y, grid.GridArray.GetLength(0), grid.GridArray.GetLength(1));
                    GameObject go = Instantiate(dotPref);
                    DotType dotType = puzzleInfo.dotList[index];

                    go.transform.position = grid.GetCellWorldPosition(x, y);
                    levelManager.AddChildPuzzle(go.transform);
                    go.name = "Grid_" + x + "_" + y;

                    Dot dot = go.GetComponent<Dot>();
                    dot.Initialize(index, dotType, DotState.Empty);
                    lineGenerator.AddChildDot(dot);

                    levelManager.AddChildDotList(dot);
                    levelManager.AddDotArray(index, dot);
                    
                }
            }

            lineGenerator.LineGenerate();
            levelManager.AddChildPuzzle(lineGenerator.linePerent.transform);
            levelManager.CreatePuzzleResult();
        }

        public void OnCleared()
        {
            PopupCenterData popup = new PopupCenterData(PopupCenterType.SingleButtonText, "CONGRATS", "You clear puzzle [ID-" + puzzleId + "]", false);
            QueueManager.instance.AddPopupCenterQueue(popup);
            QueueManager.instance.ExecutePopupCenterQueue();
            // Reload data after saved
            //puzzleInfo = DataAsset.instance.GetPuzzleData(puzzleInfo.puzzleId);
            if (!puzzleInfo.clear)
            {
                CheckClearedEvent();
            }
        }

        private void CheckClearedEvent()
        {
            GameManager.instance.SaveClearedPuzzle(puzzleId);

            // Add to unlocked queue if it have other connection
            //if (puzzleInfo.connectedList.Length > 0 && puzzleId < Constant.MIN_UNLOCK_GEMS_PUZZLE_ID)
            //{
            //    UnlockedManager.instance.AddConnectedPuzzleQueue(puzzleId);
            //}

            int length = puzzleInfo.clearedEvents.Length;

            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    EventName eventName = puzzleInfo.clearedEvents[i].eventName;
                    string index = puzzleInfo.clearedEvents[i].targetIndex;

                    switch (eventName)
                    {
                        case EventName.UnlockSection: UnlockedSection(index); break;
                        case EventName.UnlockBox: UnlockedBox(int.Parse(index)); break;
                        case EventName.UnlockPuzzle: UnlockedPuzzle(int.Parse(index)); break;
                        case EventName.UnlockGems: UnlockedGems(index); break;
                    }
                }

                UnlockedManager.instance.ExecuteGemsQueue();
            }

            if (puzzleInfo.circleLevel == 0 && puzzleInfo.levelIndex == 0 && puzzleId > Constant.MIN_UNLOCK_GEMS_PUZZLE_ID)
            {
                if(!levelManager.GetBoxInfo().isGate || (levelManager.GetBoxInfo().isGate && levelManager.GetBoxInfo().boxSection == SectionName.Zero))
                {
                    PopupCenterData popup = new PopupCenterData(PopupCenterType.SingleButtonText, "CONGRATS", "Now you can drop gems for unlock other puzzle", true);
                    QueueManager.instance.AddPopupCenterQueue(popup);
                }
            }
        }

        private void UnlockedSection(string index)
        {
            UnlockedManager.instance.AddSectionQueue(index);

            if (index.ToLower() == "tutorial")
            {
                GameManager.instance.SaveTutorialUnlock(true);
            }
            else
            {
                GemsName gemsName = (GemsName)int.Parse(index.Split('_').GetValue(0).ToString());
                int section = int.Parse(index.Split('_').GetValue(1).ToString());

                GameManager.instance.SaveUnlockedSection(gemsName, section);
            }
        }

        private void UnlockedBox(int index)
        {
            if(LevelManager.instance.CheckBoxUnlocked(index))
            {
                Debug.Log("Box " + index + " Unlocked!");
                string value = CheckBoxQueueValue(index);
                if(value != null)
                {
                    UnlockedManager.instance.AddBoxQueue(value);
                }
                
                GameManager.instance.SaveUnlockedBox(index);

                // Unlock first puzzle of new box
                BoxScriptable box = DataAsset.instance.GetBoxData(index);
                int puzzleId = DataAsset.instance.GetPuzzleDataKey(box.puzzleList[0]);
                GameManager.instance.SaveUnlockedPuzzle(puzzleId);
            }
            else
            {
                // TODO: If box doesn't unlock show only line update
                string value = CheckBoxQueueValue(index);
                if (value != null)
                {
                    UnlockedManager.instance.AddLineQueue(value);
                }
            }
        }

        private string CheckBoxQueueValue(int index)
        {
            // Use parent box
            //int index = LevelManager.instance.GetBoxId();
            //BoxScriptable box = DataAsset.instance.GetBoxData(index);

            BoxScriptable boxInfo = LevelManager.instance.GetBoxInfo();

            for(int i = 0; i < boxInfo.connectedList.Length; i++)
            {
                int id = int.Parse(boxInfo.connectedList[i].puzzleConditionId);
                int targetId = boxInfo.connectedList[i].boxTargetId;
                if(id == puzzleId && targetId == index)
                {
                    return boxInfo.boxId + "_" + i;
                }
            }

            return null;
        }

        private void UnlockedPuzzle(int index)
        {
            if(CheckPuzzleUnlock(index))
            {
                string queueValue = puzzleId + "_" + GetTargetPuzzleLineIndex(index);
                UnlockedManager.instance.AddPuzzleAndLineQueue(queueValue);
                GameManager.instance.SaveUnlockedPuzzle(index);
            }
            else
            {
                string queueValue = puzzleId + "_" + GetTargetPuzzleLineIndex(index);
                UnlockedManager.instance.AddPuzzleLineQueue(queueValue);
            }
            
        }

        private bool CheckPuzzleUnlock(int index)
        {
            PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(index);
            bool isUnlock = false;

            if(puzzle.unlockConditionPuzzleId.Length > 0)
            {
                string[] id = puzzle.unlockConditionPuzzleId.Split('_');

                for (int i = 0; i < id.Length; i++)
                {
                    int puzzleId = int.Parse(id[i]);
                    PuzzleScriptable targetPuzzle = DataAsset.instance.GetPuzzleDataValue(puzzleId);

                    if (!targetPuzzle.clear)
                    {
                        return false;
                    }
                    else
                    {
                        isUnlock = true;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Puzzle " + puzzle.name + " is missing unlock condition");
            }

            if(isUnlock)
            {
                isUnlock = CheckPuzzleGemsUnlock(puzzle.gemsName);
            }
            
            return isUnlock;
        }

        private bool CheckPuzzleGemsUnlock(GemsName gems)
        {
            //int index = LevelManager.instance.GetBoxId();
            //BoxScriptable box = DataAsset.instance.GetBoxData(index);

            BoxScriptable boxInfo = LevelManager.instance.GetBoxInfo();

            if(boxInfo.gemsUnlockList.Length > 0)
            {
                for (int i = 0; i < boxInfo.gemsUnlockList.Length; i++)
                {
                    if (gems != GemsName.None)
                    {
                        if (gems == boxInfo.gemsUnlockList[i].gems)
                        {
                            return boxInfo.gemsUnlockList[i].unlock;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        public bool CheckPuzzleCleared(int index)
        {
            PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(index);
            bool isClear = false;

            if (puzzle.unlockConditionPuzzleId.Length > 0)
            {
                string[] id = puzzle.unlockConditionPuzzleId.Split('_');

                for (int i = 0; i < id.Length; i++)
                {
                    int puzzleId = int.Parse(id[i]);
                    PuzzleScriptable targetPuzzle = DataAsset.instance.GetPuzzleDataValue(puzzleId);

                    if (!targetPuzzle.clear)
                    {
                        return false;
                    }
                    else
                    {
                        isClear = true;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Puzzle " + puzzle.name + " is missing unlock condition");
            }

            return isClear;
        }

        private int GetTargetPuzzleLineIndex(int index)
        {
            for(int i = 0; i < puzzleInfo.puzzleConnectedList.Length; i++)
            {
                if (index == puzzleInfo.puzzleConnectedList[i])
                {
                    return i;
                }
            }
            return 0;
        }

        private void UnlockedGems(string index)
        {
            GemsTier tier = (GemsTier)int.Parse(index.Split('_').GetValue(0).ToString());
            int id = int.Parse(index.Split('_').GetValue(1).ToString());

            GameManager.instance.SaveUnlockedGems(tier, id);
            UnlockedManager.instance.AddGemsQueue(id);
        }


        // TODO: For mock up generator

        public void Initailize(PuzzleScriptable _puzzleInfo)
        {
            puzzleInfo = _puzzleInfo;
            puzzleId = _puzzleInfo.puzzleId;
            lineGenerator = GetComponent<PuzzleLineGenerator>();

            dotPref = ObjectAsset.instance.GetPref(PrefType.Dot);

            InitializeMockupPuzzle();
        }

        private void InitializeMockupPuzzle()
        {
            Debug.Log("Loading...");

            grid = new Grid();
            grid.Initailize(puzzleInfo.width, puzzleInfo.height, puzzleInfo.gridGap, dotPref.GetComponent<SpriteRenderer>());

            lineGenerator.Initialize(grid);

            MockupGridGenerate();
        }

        private void MockupGridGenerate()
        {
            for (int x = 0; x < grid.GridArray.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GridArray.GetLength(1); y++)
                {
                    int index = GeneratorCalculator.GetDotArrayIndex(x, y, grid.GridArray.GetLength(0), grid.GridArray.GetLength(1));
                    GameObject go = Instantiate(dotPref);
                    DotType dotType = puzzleInfo.dotList[index];

                    go.transform.position = grid.GetCellWorldPosition(x, y);
                    go.name = "Grid_" + x + "_" + y;

                    Dot dot = go.GetComponent<Dot>();
                    dot.Initialize(index, dotType, DotState.Empty);
                    lineGenerator.AddChildDot(dot);
                }
            }

            lineGenerator.LineGenerate();
        }
    }
}

