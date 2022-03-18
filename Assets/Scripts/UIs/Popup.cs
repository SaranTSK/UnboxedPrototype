using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Unboxed
{
    public class Popup : MonoBehaviour
    {
        [SerializeField] private Transform popupRightRef;
        [SerializeField] private Transform popupCenterRef;

        private PopupRight popupRight;
        private PopupCenter popupCenter;

        public void Initialize()
        {
            popupRight = popupRightRef.GetComponent<PopupRight>();
            popupCenter = popupCenterRef.GetComponent<PopupCenter>();

            HideRightPopup();
            HideCenterPopup();

            popupRight.Initialize();
            popupCenter.Initialize();
        }

        public PopupCenter GetCenterPopup()
        {
            return popupCenter;
        }

        public PopupRight GetRightPopup()
        {
            return popupRight;
        }

        public void ShowCenterPopup()
        {
            popupCenterRef.gameObject.SetActive(true);
        }

        public void HideCenterPopup()
        {
            popupCenterRef.gameObject.SetActive(false);
        }

        public void ShowRightPopup()
        {
            popupRightRef.gameObject.SetActive(true);
        }

        public void HideRightPopup()
        {
            popupRightRef.gameObject.SetActive(false);
        }

    }
}

