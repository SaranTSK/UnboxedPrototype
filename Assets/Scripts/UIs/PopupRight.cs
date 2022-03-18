using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Unboxed
{
    public class PopupRight : MonoBehaviour
    {
        [SerializeField] private Transform details;

        private TextMeshProUGUI boxName;
        private Transform[] levelDetails;

        public void Initialize()
        {
            if (details != null)
            {
                boxName = details.GetChild(0).GetComponent<TextMeshProUGUI>();

                levelDetails = new Transform[details.childCount - 1];

                for (int i = 0; i < levelDetails.Length; i++)
                {
                    levelDetails[i] = details.GetChild(i + 1);
                }
            }

            transform.gameObject.SetActive(false);
        }

        public void SetPopupDetail(BoxScriptable boxInfo)
        {
            SetBoxName(boxInfo.boxName);
        }

        private void SetBoxName(string value)
        {
            boxName.text = value;
        }

        private void SetLevelDetail()
        {

        }

        public void ClosePopup()
        {
            HidePopup();
            LevelManager.instance.ResetBoxInfo();
            LevelManager.instance.ResetFocus();
            LevelManager.instance.ResetLastedBoxPosition();
        }

        public void EnterBox()
        {
            HidePopup();
            LevelManager.instance.EnterBox();
        }

        public void EnterPuzzle()
        {

        }

        public void ShowPopup()
        {
            transform.gameObject.SetActive(true);
        }

        public void HidePopup()
        {
            transform.gameObject.SetActive(false);
        }
    }
}

