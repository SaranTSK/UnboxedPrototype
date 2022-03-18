using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class MapTutorialSectionController : MonoBehaviour
    {
        [SerializeField] Sprite showSection;
        [SerializeField] Sprite hideSection;

        private bool sectionUnlocked;

        public void Initialize(bool _sectionUnlocked)
        {
            sectionUnlocked = _sectionUnlocked;
            CheckSprite();
        }

        public void UpdateSection()
        {
            sectionUnlocked = DataAsset.instance.GetMapData().tutorialUnlock;
            CheckSprite();
        }

        private void CheckSprite()
        {
            SpriteRenderer renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            Sprite sprite = sectionUnlocked ? showSection : hideSection;

            renderer.sprite = sprite;
        }
    }
}

