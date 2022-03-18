using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class MapSectionController : MonoBehaviour
    {
        [SerializeField] private Sprite[] showSections;
        [SerializeField] private Sprite[] hideSections;

        public void Initailize(SectionProperty sectionProperty, int angle)
        {
            Transform[] gateSections = new Transform[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                gateSections[i] = transform.GetChild(i);
            }

            CheckSprite(gateSections[0], 0, sectionProperty.zeroSectionUnlock);
            CheckSprite(gateSections[1], 1, sectionProperty.firstSectionUnlock);
            CheckSprite(gateSections[2], 2, sectionProperty.secondSectionUnlock);
            CheckSprite(gateSections[3], 3, sectionProperty.thirdSectionUnlock);
            CheckSprite(gateSections[4], 4, sectionProperty.fourthSectionUnlock);
            RotateSection(angle);
        }

        public void UpdateSection(int gems, int section)
        {
            MapSectionScriptable map = DataAsset.instance.GetMapData();
            bool unlock = false;

            switch(section)
            {
                case 0: unlock = map.gateInfoList[gems].zeroSectionUnlock; break;
                case 1: unlock = map.gateInfoList[gems].firstSectionUnlock; break;
                case 2: unlock = map.gateInfoList[gems].secondSectionUnlock; break;
                case 3: unlock = map.gateInfoList[gems].thirdSectionUnlock; break;
                case 4: unlock = map.gateInfoList[gems].fourthSectionUnlock; break;
            }

            CheckSprite(transform.GetChild(section), section, unlock);
        }

        private void RotateSection(int angle)
        {
            transform.Rotate(new Vector3(0, 0, angle), Space.World);
        }

        private void CheckSprite(Transform child, int section, bool isShow)
        {
            if(isShow)
            {
                child.GetComponent<SpriteRenderer>().sprite = showSections[section];
            }
            else
            {
                child.GetComponent<SpriteRenderer>().sprite = hideSections[section];
            }
        }
    }
}

