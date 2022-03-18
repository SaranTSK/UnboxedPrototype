using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class BoxController : MonoBehaviour
    {
        private BoxScriptable boxInfo;
        private SpriteRenderer frame;
        private SpriteRenderer icon;

        public void Initailize(BoxScriptable _boxInfo)
        {
            boxInfo = _boxInfo;
            frame = transform.GetChild(0).GetComponent<SpriteRenderer>();
            icon = transform.GetChild(1).GetComponent<SpriteRenderer>();

            SetFrameSprite();
            SetIconSprite();
        }

        public void UpdateSprite()
        {
            //Debug.Log("Update box: " + boxInfo.boxId + " = " + boxInfo.unlock);
            boxInfo = DataAsset.instance.GetBoxData(boxInfo.boxId);
            SetIconSprite();
        }

        public void OnClick()
        {
            if(boxInfo.unlock)
            {
                UIManager.instance.OnClickedBox(boxInfo);
                LevelManager.instance.FocusAtPosition(transform.position);
                LevelManager.instance.SetLastedBoxPosition(transform.position);
                LevelManager.instance.SetBoxInfo(boxInfo);
            }
            else
            {
                PopupCenterData popup = new PopupCenterData(PopupCenterType.SingleButtonText, "ALERT", "Box [" + boxInfo.boxName + "] is locked!", false);
                QueueManager.instance.AddPopupCenterQueue(popup);
                QueueManager.instance.ExecutePopupCenterQueue();
                Debug.LogWarning("Box locked!!!");
            }
            
        }

        public bool CheckBoxUnlocked()
        {
            boxInfo = DataAsset.instance.GetBoxData(boxInfo.boxId);
            bool unlocked = false;
            if (boxInfo.unlockConditionPuzzleId.Length > 0)
            {
                // Check every puzzles connection are cleared
                string[] id = boxInfo.unlockConditionPuzzleId.Split('_');
                for (int i = 0; i < id.Length; i++)
                {
                    PuzzleScriptable puzzle = DataAsset.instance.GetPuzzleDataValue(int.Parse(id[i]));

                    Debug.Log("Box " + boxInfo.boxId + " | " + puzzle.clear);
                    if (puzzle.clear)
                    {
                        unlocked = true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Box " + boxInfo.name + " is missing unlock condition");
            }


            return unlocked;
        }

        private void SetFrameSprite()
        {
            frame.sprite = SpriteAsset.instance.GetFrameSprite(boxInfo.rarity);
        }

        private void SetIconSprite()
        {
            //Debug.Log("Set sprite box: " + boxInfo.boxId + " = " + boxInfo.unlock);
            if (!boxInfo.unlock)
            {
                icon.sprite = SpriteAsset.instance.GetLockSprite(0);
            }
            else
            {
                icon.sprite = SpriteAsset.instance.GetBoxSprite(boxInfo.boxId);
            }
        }
    }
}

