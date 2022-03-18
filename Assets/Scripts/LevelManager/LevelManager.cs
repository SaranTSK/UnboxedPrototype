using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public partial class LevelManager : MonoBehaviour
    {
        public static LevelManager instance;

        [SerializeField] private MapGenerator mapGenerator;
        [SerializeField] private BoxGenerator boxGenerator;
        [SerializeField] PlayerController player;
        [SerializeField] private CameraController cameraController;

        //private int boxId;

        //Temp parent object
        private GameObject puzzle;
        private GameObject box;
        private Vector3 lastedBoxPos;

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
            //ResetBoxId();
            //SetBoxInfo(ScriptableObject.CreateInstance<BoxScriptable>());
            UnlockedManager.instance.Initailize(mapGenerator, boxGenerator, player);
            QueueManager.instance.Initialize();
            dotList = new DotList();
        }

        //public int GetBoxId()
        //{
        //    return boxId;
        //}

        //public void SetBoxId(int id)
        //{
        //    boxId = id;
        //}

        //public void ResetBoxId()
        //{
        //    boxId = -1;
        //}

        public Transform GetPuzzleTransform(int level, int index)
        {
            return box.transform.GetChild(level).GetChild(index);
        }

        public bool CheckBoxUnlocked(int id)
        {
            BoxController box = mapGenerator.GetBoxPosition(id).GetComponent<BoxController>();

            return box.CheckBoxUnlocked();
        }

        public void AddChildPuzzle(Transform transform)
        {
            transform.parent = puzzle.transform;
        }

        public void AddChildBox(Transform transform)
        {
            transform.parent = box.transform;
        }

        // ---------- Handle camera ----------

        public void SetLastedBoxPosition(Vector3 position)
        {
            lastedBoxPos = position;
        }

        public void ResetLastedBoxPosition()
        {
            lastedBoxPos = Vector3.zero;
        }

        public void FocusAtPosition(Vector3 position)
        {
            player.SetPlayerPosition(position);
            player.FocusZoomSize();
        }

        public void FocusAtUnlockPosition(Vector3 position)
        {
            player.SetPlayerPosition(position);
        }

        public void ResetFocus()
        {
            player.ResetZoomSize();
        }

        private void FocusAtSimulation()
        {
            //player.SetPlayerPosition(transform.position);
            if(player.GetPlayerPosition() != lastedBoxPos)
            {
                player.SetPlayerPosition(lastedBoxPos);
                ResetLastedBoxPosition();
            }
            else
            {
                player.ResetPlayerPosition();
            }
            
            cameraController.FocusDefaultZoomSize();
        }

        private void FocusAtBox()
        {
            //BoxScriptable boxInfo = DataAsset.instance.GetBoxData(GetBoxId());
            //player.SetPlayerPosition(box.transform.position);
            player.ResetPlayerPosition();
            cameraController.FocusBoxZoomSize((int)GetBoxInfo().circleGenerate);
        }

        private void FocusAtPuzzle()
        {
            //player.SetPlayerPosition(puzzle.transform.position);
            player.ResetPlayerPosition();
            cameraController.FocusDefaultZoomSize();
        }

        // ---------- Handle game state ----------

        public void EnterSimulation()
        {
            if (GameManager.instance.currentState == GameState.Menu)
            {
                InitSimulationMap();
                FocusAtSimulation();
                EnterSimulationUI();
            }
            else if(GameManager.instance.currentState == GameState.Simulation)
            {
                mapGenerator.ShowMapLine();
                mapGenerator.ShowUnlockedBoxes();
                FocusAtSimulation();
                EnterSimulationUI();
            }
            else if (GameManager.instance.currentState == GameState.BoxLevel)
            {
                RemoveBoxMap();
                FocusAtSimulation();
                EnterSimulationUI();
                UIManager.instance.UpdateHeaderText();
                UnlockedManager.instance.ExecuteSimulationQueue();
            }

            GameManager.instance.SetGameState(GameState.Simulation);
            StartCoroutine(LoadingDelay());
        }

        public void ExitSimultation()
        {
            mapGenerator.HideMapLine();
        }

        private void InitSimulationMap()
        {
            mapGenerator.Initailize();
            mapGenerator.gameObject.SetActive(true);
        }

        private void EnterSimulationUI()
        {
            UIManager.instance.SwitchFooter(FooterType.HomeFooter);
        }

        // ---------- Handle loading -----------
        private IEnumerator LoadingDelay()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            UIManager.instance.IsLoading(false);
        }
    }
}

