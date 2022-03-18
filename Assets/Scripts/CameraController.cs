using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform background;
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float smoothCameraMove;
        [SerializeField] private float smoothCameraZoom;


        private Transform[] childBackground;
        private Camera mainCamera;

        private float targetZoomSize;
        private float zoomSize;
        private float tartgetBackgroundSize;
        private float backgroundSize;
        private bool isFocused;

        private void Awake()
        {
            mainCamera = GetComponent<Camera>();
        }

        private void Start()
        {
            isFocused = false;
            zoomSize = mainCamera.orthographicSize;
            targetZoomSize = zoomSize;

            if (background != null)
            {
                childBackground = new Transform[background.childCount];

                for (int i = 0; i < background.childCount; i++)
                {
                    childBackground[i] = background.GetChild(i);
                    
                }

                backgroundSize = childBackground[0].transform.localScale.x;
                tartgetBackgroundSize = backgroundSize;
            }

            //mainCamera.transform.position = Vector3.zero;
        }

        private void Update()
        {
            //SetCameraPosition();
            //SetMainBackgroundPosition();
            SmoothMove();
            SmoothZoom();
        }

        // Camera position

        private void SetCameraPosition(Vector3 position)
        {
            mainCamera.transform.position = new Vector3(position.x, position.y, offset.z);

            //Vector3 currentPos = mainCamera.transform.position;
            //Vector3 targetPos = target.transform.position;
            //if(currentPos != targetPos)
            //{
            //    currentPos = Vector3.Lerp(currentPos, targetPos, smoothCameraMove);
            //    mainCamera.transform.position = currentPos + offset;
            //}
            
        }

        private void SetMainBackgroundPosition(Vector3 position)
        {
            childBackground[0].transform.position = position;

            //Vector3 currentPos = childBackground[0].transform.position;
            //Vector3 targetPos = target.transform.position;
            //if (currentPos != targetPos)
            //{
            //    currentPos = Vector3.Lerp(currentPos, targetPos, smoothCameraMove);
            //    childBackground[0].transform.position = currentPos;
            //}
        }

        private void SmoothMove()
        {
            Vector3 currentCameraPos = mainCamera.transform.position;
            Vector3 targetCameraPos = target.transform.position;
            //Debug.Log("Camera: " + currentCameraPos + "|Target: " + targetCameraPos);
            if (currentCameraPos != targetCameraPos)
            {
                currentCameraPos = Vector3.Lerp(currentCameraPos, targetCameraPos, smoothCameraMove);
                SetCameraPosition(currentCameraPos);
                //Debug.Log(mainCamera.transform.position);
            }

            Vector3 currentBackgroundPos = childBackground[0].transform.position;
            Vector3 targetBackgroundPos = target.transform.position;
            if (currentBackgroundPos != targetBackgroundPos)
            {
                currentBackgroundPos = Vector3.Lerp(currentBackgroundPos, targetBackgroundPos, smoothCameraMove);
                SetMainBackgroundPosition(currentBackgroundPos);
            }
        }

        private void ForcedMove()
        {
            Vector3 targetPos = target.transform.position;

            SetCameraPosition(targetPos);
            SetMainBackgroundPosition(targetPos);
        }

        // Camera zoom

        private void SmoothZoom()
        {
            if(zoomSize != targetZoomSize)
            {
                zoomSize = Mathf.Lerp(zoomSize, targetZoomSize, smoothCameraZoom);
                SetZoomSize(zoomSize);
            }

            if(backgroundSize != tartgetBackgroundSize)
            {
                backgroundSize = Mathf.Lerp(backgroundSize, tartgetBackgroundSize, smoothCameraZoom);
                SetBackgroundScale(backgroundSize);
            }
        }

        public void ZoomIn()
        {
            if(targetZoomSize > Constant.MIN_ZOOMSIZE && mainCamera != null)
            {
                targetZoomSize -= 1;
                tartgetBackgroundSize -= 0.2f;
            }
        }

        public void ZoomOut()
        {
            if(targetZoomSize < Constant.MAX_ZOOMSIZE && mainCamera != null)
            {
                targetZoomSize += 1;
                tartgetBackgroundSize += 0.2f;
            }
        }

        public float GetZoomSize()
        {
            return zoomSize;
        }

        public void ResetZoomSize(float currentZoomSize)
        {
            isFocused = !isFocused;
            float diff = currentZoomSize - Constant.MIN_ZOOMSIZE;
            targetZoomSize = currentZoomSize;
            tartgetBackgroundSize += 0.2f * diff;
        }

        public void FocusZoomSize()
        {
            isFocused = !isFocused;
            float diff = Constant.MIN_ZOOMSIZE - zoomSize;
            targetZoomSize = Constant.MIN_ZOOMSIZE;
            tartgetBackgroundSize += 0.2f * diff;
        }

        public void FocusDefaultZoomSize()
        {
            float diff = Constant.DEFAULT_ZOOMSIZE - zoomSize;
            targetZoomSize = Constant.DEFAULT_ZOOMSIZE;
            //zoomSize = targetZoomSize;

            tartgetBackgroundSize += 0.2f * diff;
            //backgroundSize = tartgetBackgroundSize;

            SetZoomSize(targetZoomSize);
            SetBackgroundScale(tartgetBackgroundSize);
            ForcedMove();
        }

        public void FocusBoxZoomSize(int circle)
        {
            float focusZoomSize = Constant.MIN_ZOOMSIZE;

            switch(circle)
            {
                case 1: focusZoomSize = 4f; break;
                case 2: focusZoomSize = 5f; break;
                case 3: focusZoomSize = 6f; break;
                case 4: focusZoomSize = 7f; break;
            }

            float diff = focusZoomSize - zoomSize;
            targetZoomSize = focusZoomSize;
            //zoomSize = targetZoomSize;

            tartgetBackgroundSize += 0.2f * diff;
            //backgroundSize = tartgetBackgroundSize;

            SetZoomSize(targetZoomSize);
            SetBackgroundScale(tartgetBackgroundSize);
            ForcedMove();
        }

        public void FocusPuzzleZoomSize(int width, int height)
        {

        }

        private void SetZoomSize(float value)
        {
            mainCamera.orthographicSize = value;
        }

        private void SetBackgroundScale(float value)
        {
            for (int i = 0; i < background.childCount; i++)
            {
                Vector3 scale = childBackground[i].transform.localScale;

                childBackground[i].transform.localScale = new Vector3(value, value, scale.z);
            }
        }

        public bool IsFocused()
        {
            return isFocused;
        }
    }
}

