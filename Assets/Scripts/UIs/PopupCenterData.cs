using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class PopupCenterData
    {
        public PopupCenterType popupCenterType;
        public string title;
        public Sprite[] detailImage;
        public string detailText;
        public bool isInitGems;

        public PopupCenterData(PopupCenterType _popupCenterType, string _title, Sprite[] _detailImage, bool _isInitGems)
        {
            popupCenterType = _popupCenterType;
            title = _title;
            detailImage = _detailImage;
            detailText = null;
            isInitGems = _isInitGems;
        }

        public PopupCenterData(PopupCenterType _popupCenterType, string _title, string _detailText, bool _isInitGems)
        {
            popupCenterType = _popupCenterType;
            title = _title;
            detailImage = null;
            detailText = _detailText;
            isInitGems = _isInitGems;
        }
    }
}

