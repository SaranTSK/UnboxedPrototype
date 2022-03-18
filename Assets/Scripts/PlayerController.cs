using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unboxed
{
    public enum PlayerState
    {
        Idle,
        Click,
        Hold,
        Release
    }

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private float moveSpeed;
        [SerializeField] private CameraController cameraController;

        private float xBorderOffset;
        private float yBorderOffset;

        LineController lineController;
        private PlayerState currentState;
        private Dot currentDot;
        private float currentZoomSize;
        private Vector3 currentMousePos;
        private bool enableInput;

        // Handle camera move
        private Vector3 origin;
        private Vector3 diff;
        private bool isDrag = false;

        private void Start()
        {
            lineController = GetComponent<LineController>();
            currentZoomSize = cameraController.GetZoomSize();
            EnableInput();
        }

        private void Update()
        {
            if(enableInput)
            {
                CheckPlayerState();
                //CheckPlayerMovement();
                CheckPlayerZoom();
            }
        }

        private void LateUpdate()
        {
            if (enableInput)
            {
                CheckPlayerCemeraMove();
            }
        }

        public void SetPlayerPosition(Vector3 position)
        {
            player.transform.position = position;
        }

        public void ResetPlayerPosition()
        {
            player.transform.position = Vector3.zero;
        }

        public Vector3 GetPlayerPosition()
        {
            return player.transform.position;
        }

        public void FocusZoomSize()
        {
            currentZoomSize = cameraController.GetZoomSize();
            Debug.Log("Focus: " + currentZoomSize);
            cameraController.FocusZoomSize();
        }

        public void ResetZoomSize()
        {
            Debug.Log("Reset: " + currentZoomSize);
            cameraController.ResetZoomSize(currentZoomSize);
        }

        public void EnableInput()
        {
            enableInput = true;
        }

        public void DisableInput()
        {
            enableInput = false;
        }

        //private void CheckPlayerMovement()
        //{
        //    float x = CheckXBorder(player.transform.position.x);
        //    float y = CheckYBorder(player.transform.position.y);
        //    float z = 0;

        //    if(currentZoomSize != cameraController.GetZoomSize())
        //    {
        //        CalculateBorder();
        //    }

        //    player.transform.position += new Vector3(x, y, z) * Time.fixedDeltaTime;

        //    //Debug.Log(player.transform.position);
        //}

        private void CalculateBorder()
        {
            if(!cameraController.IsFocused())
            {
                currentZoomSize = cameraController.GetZoomSize();
                xBorderOffset = currentZoomSize;
                yBorderOffset = currentZoomSize;
            }
            
        }

        //private float CheckXBorder(float x)
        //{
        //    float xInput = Input.GetAxisRaw("Horizontal");

        //    if (x - xBorderOffset < -Constant.X_BORDER)
        //    {
        //        if(xInput > 0)
        //        {
        //            return xInput * moveSpeed; ;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
                
        //    }
        //   else if(x + xBorderOffset > Constant.X_BORDER)
        //    {
        //        if (xInput < 0)
        //        {
        //            return xInput * moveSpeed; ;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //   else
        //    {
        //        return xInput * moveSpeed;
        //    }
        //}

        //private float CheckYBorder(float y)
        //{
        //    float yInput = Input.GetAxisRaw("Vertical");

        //    if (y - yBorderOffset < -Constant.Y_BORDER)
        //    {
        //        if (yInput > 0)
        //        {
        //            return yInput * moveSpeed; ;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //    else if(y + yBorderOffset > Constant.Y_BORDER)
        //    {
        //        if (yInput < 0)
        //        {
        //            return yInput * moveSpeed; ;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //    else
        //    {
        //        return yInput * moveSpeed; ;
        //    }
        //}

        private float CheckDragXBorder(float x)
        {
            if (x < -Constant.X_BORDER - xBorderOffset)
            {
                return -Constant.X_BORDER - xBorderOffset;
            }
            else if (x  > Constant.X_BORDER + xBorderOffset)
            {
                return Constant.X_BORDER + xBorderOffset;
            }
            else
            {
                return x;
            }
        }

        private float CheckDragYBorder(float y)
        {
            if (y < -Constant.Y_BORDER - yBorderOffset)
            {
                return -Constant.Y_BORDER - yBorderOffset;
            }
            else if (y > Constant.Y_BORDER + yBorderOffset)
            {
                return Constant.Y_BORDER + yBorderOffset;
            }
            else
            {
                return y;
            }
        }

        private Vector3 GetMousePos()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void CheckPlayerState()
        {
            if (Input.GetMouseButtonDown(0) && currentState == PlayerState.Idle)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                currentState = PlayerState.Click;
                //Debug.Log(currentState);
                CheckRaycast();
            }

            if (Input.GetMouseButton(0) && currentState == PlayerState.Hold)
            {
                //Debug.Log(currentState);
                CheckRaycast();

                if (GameManager.instance.currentState == GameState.Puzzle)
                {
                    if (currentMousePos != GetMousePos())
                    {
                        currentMousePos = GetMousePos();
                        lineController.SetMousePos(currentMousePos);
                        lineController.DrawLine();
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && currentState == PlayerState.Hold)
            {
                currentState = PlayerState.Release;
                
                //Debug.Log(currentState);
            }

            if (currentState == PlayerState.Release)
            {
                if (GameManager.instance.currentState == GameState.Puzzle)
                {
                    lineController.OnRelease();
                }

                currentState = PlayerState.Idle;
                //Debug.Log(currentState);
            }
        }

        private void CheckPlayerCemeraMove()
        {
            if (currentZoomSize != cameraController.GetZoomSize())
            {
                CalculateBorder();
            }

            if (Input.GetMouseButton(0) && GameManager.instance.currentState == GameState.Simulation)
            {
                diff = GetMousePos() - GetPlayerPosition();

                //Debug.Log("Diff: " + diff + "|Mouse: " + GetMousePos() + "|Player: " + GetPlayerPosition());
                if (!isDrag)
                {
                    isDrag = true;
                    origin = GetMousePos();
                }
            }
            else
            {
                isDrag = false;
            }

            if(isDrag)
            {
                //Debug.Log("Mouse: " + GetMousePos() + "|Player: " + GetPlayerPosition());
                Debug.Log("Origin: " + origin + "|Diff: " + diff);
                Vector3 result = origin - diff;

                float x = CheckDragXBorder(result.x);
                float y = CheckDragYBorder(result.y);
                float z = 0;

                result = new Vector3(x, y, z);

                Debug.Log("Result: " + result);

                SetPlayerPosition(result);
            }
        }

        private void CheckRaycast()
        {
            //Debug.Log("Raycast");

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit))
            {
                //Debug.Log("Raycast Hit " + hit.collider.tag);

                if (hit.collider != null)
                {
                    CheckHitTag(hit);
                }
                else if(hit.collider == null && currentState == PlayerState.Hold)
                {
                    CheckHitTag(hit);
                }
                else
                {
                    currentState = PlayerState.Idle;
                }
            }
            else if(currentState == PlayerState.Click)
            {
                currentState = PlayerState.Idle;
            }
            
        }

        private void CheckPlayerZoom()
        {
            // TODO: Disable zooming when selected box
            if(Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                cameraController.ZoomIn();
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                cameraController.ZoomOut();
            }
        }

        private void CheckHitTag(RaycastHit hit)
        {
            string tag = hit.collider.tag;
            //Debug.Log(tag);

            switch (tag)
            {
                case "Box":
                    BoxController box = hit.transform.GetComponent<BoxController>();
                    box.OnClick();
                    currentState = PlayerState.Idle;
                    break;

                case "Puzzle":
                    PuzzleController puzzle = hit.transform.GetComponent<PuzzleController>();
                    puzzle.OnClick();
                    currentState = PlayerState.Idle;

                    lineController.Initialize();
                    break;

                case "Dot":
                    HitDotTag(hit);
                    break;
            }
        }

        private void HitDotTag(RaycastHit hit)
        {
            Dot dot = hit.transform.GetComponent<Dot>();

            if (currentState == PlayerState.Click)
            {
                currentDot = dot;
                currentState = PlayerState.Hold;

                lineController.SetCurrentList(dot.GetListType(currentDot.type));
                lineController.OnClick(dot);
            }
            else if (currentState == PlayerState.Hold)
            {
                if (dot.name != currentDot.name)
                {
                    if (lineController.CheckMatchType(dot.GetListType(currentDot.type), dot))
                    {
                        currentDot = dot;
                        dot.OnEnter();

                        lineController.SetCurrentList(dot.GetListType(currentDot.type));
                        lineController.OnEnter(dot);
                    }
                    else
                    {
                        // TODO: Prevent repeated!
                        dot.DotWarning();
                    }
                }
            }
            else
            {
                currentState = PlayerState.Idle;
            }
        }

    }
}

