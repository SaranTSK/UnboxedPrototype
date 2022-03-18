using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unboxed
{
    public class BoxFooter : MonoBehaviour
    {
        [SerializeField] private Transform gemsSlot;
        [SerializeField] private Transform gemsDrop;

        private Transform[] slots;
        private GemsTier gemsTier;

        public void Initialize()
        {
            slots = new Transform[gemsSlot.childCount];

            for(int i = 0; i < slots.Length; i++)
            {
                slots[i] = gemsSlot.GetChild(i);
                //Debug.Log(slots[i].name + "_" + i);
            }
        }

        public void InitGemsSlot(GemsTier _gemsTier)
        {
            gemsTier = _gemsTier;

            bool[] tiers = DataAsset.instance.GetGemsTierData(gemsTier);

            if(tiers != null)
            {
                for(int i = 0; i < tiers.Length; i++)
                {
                    Image image = slots[i].GetChild(0).GetComponent<Image>();

                    //if (i % 2 == 0)
                    //    tiers[i] = true;

                    if (tiers[i])
                    {
                        image.sprite = SpriteAsset.instance.GetGemsSprite((((int)gemsTier - 1) * Constant.MAX_GEMS) + i);
                    }
                    else
                    {
                        image.sprite = SpriteAsset.instance.GetLockSprite(11); // TODO: Change to 'length - 1'
                    }
                }
            }
            else
            {
                Debug.LogWarning("Missing gems tier data");
            }
        }

        public void UpdateGemsSlot(int index)
        {
            bool[] tiers = DataAsset.instance.GetGemsTierData(gemsTier);

            if(tiers[index])
            {
                Image image = slots[index].GetChild(0).GetComponent<Image>();
                image.sprite = SpriteAsset.instance.GetGemsSprite((int)gemsTier * index);
            }
            else
            {
                Debug.LogWarning("Update gems is wrong!");
            }
            
        }

        public void ResetGemsSlot()
        {
            gemsTier = GemsTier.None;
        }

        public void SetGemsDropPosition(Vector3 position)
        {
            gemsDrop.position = RectTransformUtility.WorldToScreenPoint(Camera.main, position);
        }
    }
}

