using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unboxed
{
    public enum UISection
    {
        Header,
        Content,
        Footer,
        Loading
    }

    public enum ContentSection
    {
        Popup
    }

    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        [SerializeField] private Transform homeHudRef;

        private Popup popup;
        private Header header;
        private Footer footer;
        private Loading loading;

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
            header = homeHudRef.GetChild((int)UISection.Header).GetComponent<Header>();
            header.Initialize("SIMULATION");

            popup = homeHudRef.GetChild((int)UISection.Content).GetChild((int)ContentSection.Popup).GetComponent<Popup>();
            popup.Initialize();

            footer = homeHudRef.GetChild((int)UISection.Footer).GetComponent<Footer>();
            footer.Initialize();

            loading = homeHudRef.GetChild((int)UISection.Loading).GetComponent<Loading>();
            loading.Initialize();
        }

        public void StartButton()
        {
            SceneManager.LoadScene(0);
        }

        public void ExitButton()
        {
            Application.Quit();
        }

        public void OnClickedBox(BoxScriptable boxInfo)
        {
            popup.GetRightPopup().ShowPopup();
            popup.GetRightPopup().SetPopupDetail(boxInfo);
        }

        public void HideHUD()
        {
            homeHudRef.GetChild((int)UISection.Header).gameObject.SetActive(false);
            homeHudRef.GetChild((int)UISection.Content).gameObject.SetActive(false);
            homeHudRef.GetChild((int)UISection.Footer).gameObject.SetActive(false);
        }

        public void ShowHUD()
        {
            homeHudRef.GetChild((int)UISection.Header).gameObject.SetActive(true);
            homeHudRef.GetChild((int)UISection.Content).gameObject.SetActive(true);
            homeHudRef.GetChild((int)UISection.Footer).gameObject.SetActive(true);
        }

        // ---------- Header section ----------

        public void UpdateHeaderText(string text)
        {
            header.AddHeaderText(text);
        }

        public void UpdateHeaderText()
        {
            header.RemoveHeaderText();
        }


        // ---------- Footer section ----------

        public void InitGemsSlot(GemsTier tier, Vector3 position)
        {
            BoxFooter boxFooter = footer.GetBoxFooter();

            if(tier != GemsTier.None)
            {
                boxFooter.InitGemsSlot(tier);
                boxFooter.SetGemsDropPosition(position);
            }
        }

        public void UpdateGemsDropPosition(Vector3 position)
        {
            footer.GetBoxFooter().SetGemsDropPosition(position);
        }

        public void SwitchFooter(FooterType type)
        {
            BoxFooter boxFooter = footer.GetBoxFooter();

            switch (type)
            {
                case FooterType.HomeFooter:
                    footer.ShowHomeFooter();
                    boxFooter.ResetGemsSlot();
                    break;

                case FooterType.BoxFooter:
                    footer.ShowBoxFooter();
                    break;

                case FooterType.TextFooter:
                    footer.ShowTextFooter();
                    break;

                case FooterType.PuzzleFooter:
                    footer.ShowPuzzleFooter();
                    break;
            }
        }

        public void UpdateGemsSlot(int index)
        {
            BoxFooter boxFooter = footer.GetBoxFooter();
            boxFooter.UpdateGemsSlot(index);
        }


        // ---------- Popup section ----------
        public void ShowCenterPopup(PopupCenterData popup)
        {
            SetCenterPopupTitle(popup.title);
            SetInitGems(popup.isInitGems);
            switch (popup.popupCenterType)
            {
                case PopupCenterType.SingleButtonImage:
                    ShowSingleButtonImage(popup.detailImage);
                    break;

                case PopupCenterType.SingleButtonText:
                    ShowSingleButtonText(popup.detailText);
                    break;

                case PopupCenterType.DoubleButtonImage:
                    ShowDoubleButtonImage(popup.detailImage);
                    break;

                case PopupCenterType.DoubleButtonText:
                    ShowDoubleButtonText(popup.detailText);
                    break;
            }
        }

        private void SetInitGems(bool value)
        {
            popup.GetCenterPopup().SetInitGems(value);
        }

        private void SetCenterPopupTitle(string title)
        {
            popup.GetCenterPopup().SetTitleText(title);
        }

        private void ShowSingleButtonImage(Sprite[] sprites)
        {
            popup.GetCenterPopup().ShowSingleButtonPopup();
            popup.GetCenterPopup().SetImageDetail(sprites);
        }

        private void ShowSingleButtonText(string text)
        {
            popup.GetCenterPopup().ShowSingleButtonPopup();
            popup.GetCenterPopup().SetTextDetail(text);
        }

        private void ShowDoubleButtonImage(Sprite[] sprites)
        {
            popup.GetCenterPopup().ShowDoubleButtonPopup();
            popup.GetCenterPopup().SetImageDetail(sprites);
        }

        private void ShowDoubleButtonText(string text)
        {
            popup.GetCenterPopup().ShowDoubleButtonPopup();
            popup.GetCenterPopup().SetTextDetail(text);
        }

        // ---------- Loading section ----------
        public void IsLoading(bool load)
        {
            if(load)
            {
                //Debug.Log("Loading...");
                loading.ShowLoading();
            }
            else
            {
                //Debug.Log("Loaded");
                loading.HideLoading();
            }
        }
    }
}

