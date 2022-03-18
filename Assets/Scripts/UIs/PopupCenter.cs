using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unboxed
{
    public enum PopupCenterType
    {
        SingleButtonImage,
        SingleButtonText,
        DoubleButtonImage,
        DoubleButtonText,
    }

    public class PopupCenter : MonoBehaviour
    {
        [SerializeField] private Transform singleButtonPopup;
        [SerializeField] private Transform doubleButtonPopup;

        private TextMeshProUGUI title;
        private Transform[] details;
        private Image[] detailImage;
        private TextMeshProUGUI detailText;

        private bool isInitGems;

        public enum ContentType
        {
            Image,
            Text,
        }

        public void Initialize()
        {
            transform.gameObject.SetActive(false);
            Transform title = singleButtonPopup.GetChild((int)UISection.Header);
            Transform detail = singleButtonPopup.GetChild((int)UISection.Content);
            InitPopupChild(title, detail);
            HideSingleButtonPopup();
            HideDoubleButtonPopup();
        }

        private void ShowPopup()
        {
            transform.gameObject.SetActive(true);
        }

        private void ClosePopup()
        {
            transform.gameObject.SetActive(false);
            //ResetPopupChild();
        }

        public void ShowSingleButtonPopup()
        {
            ShowPopup();
            HideDoubleButtonPopup();
            singleButtonPopup.gameObject.SetActive(true);
            Transform title = singleButtonPopup.GetChild((int)UISection.Header);
            Transform detail = singleButtonPopup.GetChild((int)UISection.Content);
            InitPopupChild(title, detail);
        }

        private void HideSingleButtonPopup()
        {
            singleButtonPopup.gameObject.SetActive(false);
        }

        public void ShowDoubleButtonPopup()
        {
            ShowPopup();
            HideSingleButtonPopup();
            doubleButtonPopup.gameObject.SetActive(true);
            Transform title = doubleButtonPopup.GetChild((int)UISection.Header);
            Transform detail = doubleButtonPopup.GetChild((int)UISection.Content);
            InitPopupChild(title, detail);
        }

        private void HideDoubleButtonPopup()
        {
            doubleButtonPopup.gameObject.SetActive(false);
        }

        public void SetTitleText(string text)
        {
            if (text != null)
            {
                title.text = text;
            }
            else
            {
                title.text = "Missing title text";
            }
        }

        public void SetTextDetail(string text)
        {
            if (text != null)
            {
                detailText.text = text;
            }
            else
            {
                detailText.text = "Missing detail text";
            }

            SwitchContent(ContentType.Text);
        }

        public void SetImageDetail(Sprite[] sprites)
        {
            for(int i = 0; i < detailImage.Length; i++)
            {
                if(sprites[i] != null)
                {
                    detailImage[i].gameObject.SetActive(true);
                    detailImage[i].sprite = sprites[i];
                }
                else
                {
                    detailImage[i].gameObject.SetActive(false);
                }
            }

            SwitchContent(ContentType.Image);
        }

        public void SetInitGems(bool value)
        {
            isInitGems = value;
        }

        public void ConfirmSingleButton()
        {
            ClosePopup();


            if (isInitGems && GameManager.instance.currentState != GameState.Puzzle)
            {
                LevelManager.instance.InitBoxFooterGemsUI();
            }
            else if(GameManager.instance.currentState == GameState.BoxLevel)
            {
                QueueManager.instance.ExecutePopupCenterQueue();
            }
        }

        public void ConfirmDoubleButton()
        {
            // TODO: confirm something
        }

        public void CancelDoubleButton()
        {
            // TODO: cancel something
        }

        private void InitPopupChild(Transform _title, Transform _details)
        {
            title = _title.GetComponent<TextMeshProUGUI>();

            details = new Transform[_details.childCount];
            for (int i = 0; i < details.Length; i++)
            {
                details[i] = _details.GetChild(i);
            }

            detailImage = new Image[details[0].childCount];
            for(int i = 0; i < detailImage.Length; i++)
            {
                detailImage[i] = details[0].GetChild(i).GetComponent<Image>();
            }

            detailText = details[1].GetComponent<TextMeshProUGUI>();

            //Debug.Log(title.text + "|" + details.Length + "|" + detailImage.Length + "|" + detailText.text);
        }

        private void ResetPopupChild()
        {
            title = null;
            details = null;
            detailImage = null;
            detailText = null;
        }

        private void SwitchContent(ContentType type)
        {
            switch(type)
            {
                case ContentType.Image:
                    HideTextDetail();
                    ShowImageDetail();
                    break;

                case ContentType.Text:
                    HideImageDetail();
                    ShowTextDetail();
                    break;
            }
        }

        private void ShowImageDetail()
        {
            details[(int)ContentType.Image].gameObject.SetActive(true);
        }

        private void HideImageDetail()
        {
            details[(int)ContentType.Image].gameObject.SetActive(false);
        }

        private void ShowTextDetail()
        {
            details[(int)ContentType.Text].gameObject.SetActive(true);
        }

        private void HideTextDetail()
        {
            details[(int)ContentType.Text].gameObject.SetActive(false);
        }
    }
}

