using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unboxed
{
    public class GemsDrop : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                Debug.Log("Dropped object was: " + eventData.pointerDrag.name);
                GemsSlot slot = eventData.pointerDrag.transform.GetComponent<GemsSlot>();
                slot.DestroyDragIcon();

                int index = int.Parse(eventData.pointerDrag.name.Split('_').GetValue(1).ToString());
                LevelManager.instance.OnDropGems(index);
            }
        }
    }
}

