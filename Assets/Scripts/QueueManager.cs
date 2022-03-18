using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class QueueManager : MonoBehaviour
    {
        public static QueueManager instance;

        private Queue<PopupCenterData> popupCenterDatas;

        private void Awake()
        {
            if (instance == null)
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
            popupCenterDatas = new Queue<PopupCenterData>();
        }

        public void AddPopupCenterQueue(PopupCenterData popup)
        {
            popupCenterDatas.Enqueue(popup);
        }

        public void ExecutePopupCenterQueue()
        {
            if(IsPopupCenterQueueNotEmpty())
            {
                PopupCenterData popup = popupCenterDatas.Dequeue();
                UIManager.instance.ShowCenterPopup(popup);
            }
        }

        private bool IsPopupCenterQueueNotEmpty()
        {
            return popupCenterDatas.Count > 0;
        }
    }
}

