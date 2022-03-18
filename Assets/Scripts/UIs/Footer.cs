using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public enum FooterType
    {
        HomeFooter,
        BoxFooter,
        TextFooter,
        PuzzleFooter
    }

    public class Footer : MonoBehaviour
    {
        [SerializeField] private Transform homeFooter;
        [SerializeField] private Transform boxFooter;
        [SerializeField] private Transform textFooter;
        [SerializeField] private Transform puzzleFooter;

        private Transform currentFooter;

        public void Initialize()
        {
            GetBoxFooter().Initialize();
        }

        public Transform GetFooter(FooterType type)
        {
            switch(type)
            {
                case FooterType.HomeFooter: return homeFooter;
                case FooterType.BoxFooter: return boxFooter;
                case FooterType.TextFooter: return textFooter;
                case FooterType.PuzzleFooter: return puzzleFooter;
            }

            return null;
        }

        public BoxFooter GetBoxFooter()
        {
            return boxFooter.GetComponent<BoxFooter>();
        }

        public void ShowHomeFooter()
        {
            if(currentFooter != null)
            {
                currentFooter.gameObject.SetActive(false);
            }

            homeFooter.gameObject.SetActive(true);
            currentFooter = homeFooter;
        }

        public void HideHomeFooter()
        {
            homeFooter.gameObject.SetActive(false);
            currentFooter = null;
        }

        public void ShowBoxFooter()
        {
            HideCurrentFooter();
            boxFooter.gameObject.SetActive(true);
            currentFooter = boxFooter;
        }

        public void HideBoxFooter()
        {
            boxFooter.gameObject.SetActive(false);
            currentFooter = null;
        }

        public void ShowTextFooter()
        {
            HideCurrentFooter();
            textFooter.gameObject.SetActive(true);
            currentFooter = textFooter;
        }

        public void HideTextFooter()
        {
            textFooter.gameObject.SetActive(false);
            currentFooter = null;
        }

        public void ShowPuzzleFooter()
        {
            HideCurrentFooter();
            textFooter.gameObject.SetActive(true);
            currentFooter = puzzleFooter;
        }

        private void HideCurrentFooter()
        {
            if (currentFooter != null)
            {
                currentFooter.gameObject.SetActive(false);
            }
        }
    }
}

