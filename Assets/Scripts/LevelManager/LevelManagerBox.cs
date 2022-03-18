using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public partial class LevelManager : MonoBehaviour
    {
        private BoxScriptable boxInfo;

        public void SetBoxInfo(BoxScriptable _boxInfo)
        {
            boxInfo = _boxInfo;
        }

        public BoxScriptable GetBoxInfo()
        {
            return boxInfo;
        }

        public void ResetBoxInfo()
        {
            boxInfo = null;
        }

        public void EnterBox()
        {
            UIManager.instance.IsLoading(true);
            if (GameManager.instance.currentState == GameState.Simulation)
            {
                InitBoxMap();
                FocusAtBox();
                EnterBoxUI();

                UIManager.instance.UpdateHeaderText(GetBoxInfo().boxName);

                StartCoroutine(LoadingDelay());

            }
            else if (GameManager.instance.currentState == GameState.Puzzle)
            {
                RemovePuzzleMap();
                FocusAtBox();
                EnterBoxUI();

                UIManager.instance.UpdateHeaderText();

                StartCoroutine(LoadingDelay());
                UnlockedManager.instance.ExecuteBoxPuzzleQueue();
                QueueManager.instance.ExecutePopupCenterQueue();
            }

            GameManager.instance.SetGameState(GameState.BoxLevel);
        }

        private void InitBoxMap()
        {
            box = new GameObject("BoxTemp");
            box.gameObject.SetActive(true);

            boxGenerator.Initailize();

            mapGenerator.gameObject.SetActive(false);
        }

        private void RemoveBoxMap()
        {
            Destroy(box);
            mapGenerator.gameObject.SetActive(true);
        }

        private void EnterBoxUI()
        {
            //BoxScriptable boxInfo = DataAsset.instance.GetBoxData(GetBoxId());

            if (IsBoxReadyToGemsDropped())  // && !firstTimeEnter
            {
                // TODO: Set drop position
                InitBoxFooterGemsUI();
            }
            else if (IsBoxGate())
            {
                // TODO: Set drop position
                bool isGateCleared = ClueManager.instance.IsGateCleared(GetBoxInfo().boxSection, GetBoxInfo().boxGems);
                if(!isGateCleared)
                {
                    InitBoxFooterGemsUI();
                }
                else
                {
                    UIManager.instance.SwitchFooter(FooterType.TextFooter);
                }
                
            }
            else
            {
                UIManager.instance.SwitchFooter(FooterType.TextFooter);
            }
        }

        public void InitBoxFooterGemsUI()
        {
            //BoxScriptable boxInfo = DataAsset.instance.GetBoxData(GetBoxId());
            int puzzleId = DataAsset.instance.GetPuzzleDataKey(GetBoxInfo().puzzleList[0]);

            PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(puzzleId);
            Vector3 position = GetPuzzlePosition(puzzle.levelIndex, puzzle.circleLevel);

            UIManager.instance.SwitchFooter(FooterType.BoxFooter);
            UIManager.instance.InitGemsSlot(GetBoxInfo().gemsTier, position);
        }

        private Vector3 GetPuzzlePosition(int level, int circle)
        {
            if(box != null)
            {
                return box.transform.GetChild(level).GetChild(circle).position;
            }
            else
            {
                return Vector3.zero;
            }
        }

        private bool IsBoxReadyToGemsDropped()
        {
            int puzzleId = DataAsset.instance.GetPuzzleDataKey(GetBoxInfo().puzzleList[0]);
            PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(puzzleId);

            return GetBoxInfo().gemsTier != GemsTier.None && puzzle.clear && !IsAllGemsMatched();
        }

        // ---------- Handel gate puzzle ----------

        private int currentGatePuzzle;

        private bool IsBoxGate()
        {
            return GetBoxInfo().isGate && GetBoxInfo().boxSection != SectionName.Zero;
        }

        private bool IsGateGemsMatch(GemsName gems)
        {
            GateProperty gateProperty = ClueManager.instance.GetGateData(GetBoxInfo().boxSection, GetBoxInfo().boxGems);
            BoxScriptable clueBox = DataAsset.instance.GetBoxData(gateProperty.boxClueId[currentGatePuzzle]);

            if(clueBox.isClue && clueBox.clueGems != GemsName.None)
            {
                return gems == clueBox.clueGems;
            }
            else
            {
                Debug.LogWarning("Box " + clueBox.name + " missing clue gems data or not a clue box");
                return false;
            }
            
        }

        private bool IsAllGateGamesMatched()
        {
            //GateProperty gateProperty = ClueManager.instance.GetGateData(GetBoxInfo().boxSection, GetBoxInfo().boxGems);

            for(int i = 0; i < GetBoxInfo().puzzleList.Length; i++)
            {
                int puzzleId = DataAsset.instance.GetPuzzleDataKey(GetBoxInfo().puzzleList[i]);
                PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(puzzleId);

                if (!puzzle.unlockGems)
                {
                    return false;
                }
            }

            return true;
        }

        private void CheckGateGemsPuzzle()
        {
            // TODO: Change how to check gems with clue value?
            //GateProperty gateProperty = ClueManager.instance.GetGateData(GetBoxInfo().boxSection, GetBoxInfo().boxGems);

            //PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(gateProperty.boxClueId[currentGatePuzzle]);
            //int currentPuzzleId = DataAsset.instance.GetPuzzleDataKey(puzzle);

            if(currentGatePuzzle < GetBoxInfo().puzzleList.Length - 1)
            {
                int puzzleId = DataAsset.instance.GetPuzzleDataKey(GetBoxInfo().puzzleList[currentGatePuzzle]);
                DataAsset.instance.UpdateTempUnlockedPuzzleGemsData(puzzleId, true);

                string queueValue = puzzleId + "_0";
                UnlockedManager.instance.AddPuzzleLineQueue(queueValue);
                UnlockedManager.instance.ExecuteBoxPuzzleQueue();

                currentGatePuzzle++;
                int nextPuzzleId = DataAsset.instance.GetPuzzleDataKey(GetBoxInfo().puzzleList[currentGatePuzzle]);
                PuzzleScriptable nextPuzzle = DataAsset.instance.GetPuzzleDataValue(nextPuzzleId);
                Vector3 newPos = GetPuzzlePosition(nextPuzzle.circleLevel, nextPuzzle.levelIndex);
                UIManager.instance.UpdateGemsDropPosition(newPos);
            }
            else
            {
                int puzzleId = DataAsset.instance.GetPuzzleDataKey(GetBoxInfo().puzzleList[currentGatePuzzle]);
                DataAsset.instance.UpdateTempUnlockedPuzzleGemsData(puzzleId, true);
            }
            

            if (IsAllGateGamesMatched())
            {
                SaveGateGemsData();
                UIManager.instance.SwitchFooter(FooterType.TextFooter);
            }
        }

        private void SaveGateGemsData()
        {
            for (int i = 0; i < GetBoxInfo().gemsUnlockList.Length; i++)
            {
                int puzzleId = DataAsset.instance.GetPuzzleDataKey(GetBoxInfo().puzzleList[i]);
                if(i == 0)
                {
                    GameManager.instance.SaveUnlockedPuzzle(puzzleId);
                }

                GameManager.instance.SaveUnlockedPuzzleGems(puzzleId);
                UnlockedManager.instance.AddPuzzleQueue(puzzleId);
                GameManager.instance.SaveUnlockedBoxGems(GetBoxInfo().boxId, i);
            }

            GameManager.instance.SaveClearedGate(GetBoxInfo().boxSection, GetBoxInfo().boxGems);
            UnlockedManager.instance.ExecuteBoxPuzzleQueue();
        }

        private void ClearGateGemsProgress()
        {
            //GateProperty gateProperty = ClueManager.instance.GetGateData(GetBoxInfo().boxSection, GetBoxInfo().boxGems);

            if(currentGatePuzzle != 0)
            {
                for (int i = currentGatePuzzle; i >= 0; i--)
                {
                    //PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(gateProperty.boxClueId[currentGatePuzzle]);
                    int puzzleId = DataAsset.instance.GetPuzzleDataKey(GetBoxInfo().puzzleList[i]);
                    DataAsset.instance.UpdateTempUnlockedPuzzleGemsData(puzzleId, false);

                    // TODO: Add reset line color follow this index
                    string queueValue = puzzleId + "_0";
                    UnlockedManager.instance.AddPuzzleLineResetQueue(queueValue);
                }
                UnlockedManager.instance.ExecuteBoxPuzzleQueue();
                currentGatePuzzle = 0;

                int startPuzzleId = DataAsset.instance.GetPuzzleDataKey(GetBoxInfo().puzzleList[currentGatePuzzle]);
                PuzzleScriptable startPuzzle = DataAsset.instance.GetPuzzleDataValue(startPuzzleId);
                Vector3 newPos = GetPuzzlePosition(startPuzzle.circleLevel, startPuzzle.levelIndex);
                UIManager.instance.UpdateGemsDropPosition(newPos);
            }
        }

        // ---------- Handel gate puzzle ----------

        public void OnDropGems(int gemsIndex)
        {
            GemsName gems = (GemsName)gemsIndex + 1;

            if(IsBoxGate())
            {
                if(IsGateGemsMatch(gems))
                {
                    if(!IsAllGateGamesMatched())
                    {
                        CheckGateGemsPuzzle();
                    }
                }
                else
                {
                    // TODO: Reset all gems clue check
                    ClearGateGemsProgress();
                    PopupCenterData popup = new PopupCenterData(PopupCenterType.SingleButtonText, "NOTICE", "This gems doesn't matched", false);

                    QueueManager.instance.AddPopupCenterQueue(popup);
                    QueueManager.instance.ExecutePopupCenterQueue();
                }
            }
            else
            {
                if (IsGemsMatched(gems))
                {
                    // TODO: Play matched animation

                    if (!IsGemsUnlocked(gems))
                    {
                        GameManager.instance.SaveUnlockedGems(GetBoxInfo().gemsTier, gemsIndex);
                        GameManager.instance.SaveUnlockedBoxGems(GetBoxInfo().boxId, GetBoxGemsIndex(GetBoxInfo(), gems));
                        UnlockMatchGemsPuzzle(gems);

                        PopupCenterData popup = new PopupCenterData(PopupCenterType.SingleButtonText, "NOTICE", "Unlock new puzzle", false);

                        QueueManager.instance.AddPopupCenterQueue(popup);
                        QueueManager.instance.ExecutePopupCenterQueue();
                    }
                    else
                    {
                        Debug.Log("Puzzle already unlocked");
                    }
                }
                else
                {
                    // TODO: Play unmatched animation
                    PopupCenterData popup = new PopupCenterData(PopupCenterType.SingleButtonText, "NOTICE", "This gems doesn't matched", false);

                    QueueManager.instance.AddPopupCenterQueue(popup);
                    QueueManager.instance.ExecutePopupCenterQueue();
                }
            }
        }

        private int GetBoxGemsIndex(BoxScriptable box, GemsName gems)
        {
            if(box.gemsUnlockList.Length > 0)
            {
                for(int i = 0; i < box.gemsUnlockList.Length; i++)
                {
                    if(gems == box.gemsUnlockList[i].gems)
                    {
                        return i;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Missing box gems index in box id " + box.boxId);
            }

            return 0;
        }

        private void UnlockMatchGemsPuzzle(GemsName gems)
        {
            int centerPuzzleId = DataAsset.instance.GetPuzzleDataKey(GetBoxInfo().puzzleList[0]);
            PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(centerPuzzleId);

            CheckConnectedPuzzles(puzzle.puzzleConnectedList, centerPuzzleId, gems);

            UnlockedManager.instance.ExecuteBoxPuzzleQueue();
        }

        private void CheckConnectedPuzzles(int[] puzzles, int centerPuzzleId, GemsName gems)
        {
            for (int i = 0; i < puzzles.Length; i++)
            {
                PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(puzzles[i]);
                int currentPuzzleId = DataAsset.instance.GetPuzzleDataKey(puzzle);

                if (puzzle.gemsName == gems)
                {
                    int firstPuzzleId = DataAsset.instance.GetPuzzleDataKey(GetBoxInfo().puzzleList[0]);
                    if (centerPuzzleId == firstPuzzleId)
                    {
                        Debug.Log("Add puzzle " + currentPuzzleId + " to puzzle and line queue");
                        string queueValue = centerPuzzleId + "_" + i;
                        UnlockedManager.instance.AddPuzzleAndLineQueue(queueValue);
                        GameManager.instance.SaveUnlockedPuzzle(currentPuzzleId);
                        GameManager.instance.SaveUnlockedPuzzleGems(currentPuzzleId);
                    }
                    else
                    {
                        Debug.Log("Add puzzle " + currentPuzzleId + " to puzzle queue");
                        UnlockedManager.instance.AddPuzzleQueue(currentPuzzleId);
                        GameManager.instance.SaveUnlockedPuzzleGems(currentPuzzleId);
                    }
                }

                if (puzzle.puzzleConnectedList.Length > 0)
                {
                    CheckConnectedPuzzles(puzzle.puzzleConnectedList, currentPuzzleId, gems);
                }
            }
        }

        private bool IsGemsUnlocked(GemsName gems)
        {
            for (int i = 0; i < GetBoxInfo().gemsUnlockList.Length; i++)
            {
                if (gems == GetBoxInfo().gemsUnlockList[i].gems)
                    return GetBoxInfo().gemsUnlockList[i].unlock;
            }

            return false;
        }

        private bool IsAllGemsMatched()
        {
            for (int i = 0; i < GetBoxInfo().puzzleList.Length; i++)
            {
                int puzzleId = DataAsset.instance.GetPuzzleDataKey(GetBoxInfo().puzzleList[i]);
                PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(puzzleId);

                if (puzzle.gemsName != GemsName.None && !puzzle.unlock)
                    return false;
            }

            return true;
        }

        private bool IsGemsMatched(GemsName gems)
        {
            //BoxScriptable boxInfo = DataAsset.instance.GetBoxData(GetBoxId());

            for(int i = 0; i < GetBoxInfo().gemsUnlockList.Length; i++)
            {
                if (gems == GetBoxInfo().gemsUnlockList[i].gems)
                    return true;
            }

            return false;
        }
    }
}

