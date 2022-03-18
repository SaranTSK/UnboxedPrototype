using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unboxed
{
    public class GemsSlot : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private Canvas canvas;

        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private GameObject dragIcon;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 0.5f;

            dragIcon = new GameObject("Icon");

            dragIcon.transform.SetParent(canvas.transform, false);
            dragIcon.transform.SetAsLastSibling();

            var image = dragIcon.AddComponent<Image>();

            image.sprite = GetComponent<Image>().sprite;
            image.SetNativeSize();

            dragIcon.AddComponent<CanvasGroup>();
            dragIcon.GetComponent<CanvasGroup>().blocksRaycasts = false;

            SetDragPosition(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            SetDragPosition(eventData);
        }

        private void SetDragPosition(PointerEventData eventData)
        {
            var rt = dragIcon.GetComponent<RectTransform>();
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                rt.position = globalMousePos;
                rt.rotation = rectTransform.rotation;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 1f;

            dragIcon.GetComponent<CanvasGroup>().blocksRaycasts = true;

            if (dragIcon != null)
                Destroy(dragIcon);
        }

        public void DestroyDragIcon()
        {
            canvasGroup.alpha = 1f;
            dragIcon.GetComponent<CanvasGroup>().blocksRaycasts = true;

            if (dragIcon != null)
                Destroy(dragIcon);
        }
    }
}

