using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class UnlockedManager : MonoBehaviour
    {
        public static UnlockedManager instance;

        //private Queue<int> connectedPuzzleQueue; // Unlock all connected line and puzzles on selected 'puzzle'
        private Queue<string> puzzleAndLineQueue; // Unlock specific connected line and puzzle on selected 'puzzle'
        private Queue<int> puzzleQueue; // Unlock only puzzle after mateched gems color
        private Queue<string> puzzleLineQueue; // Unlock only line after cleared puzzle
        private Queue<string> puzzleLineResetQueue; // Unlock only line after cleared puzzle
        private Queue<string> boxQueue; // Unlock all connected line and boxes on selected 'box'
        private Queue<string> sectionQueue; // Unlock new section and all box lines after cleared main puzzle
        private Queue<string> lineQueue; // Unlock only line after cleared main puzzle
        private Queue<int> gemsQueue; // Unlock gems icon after cleared main puzzle

        private MapGenerator mapGenerator;
        private BoxGenerator boxGenerator;
        private PlayerController player;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(instance);
            }
        }

        public void Initailize(MapGenerator _mapGenerator, BoxGenerator _boxGenerator, PlayerController _player)
        {
            mapGenerator = _mapGenerator;
            boxGenerator = _boxGenerator;
            player = _player;

            //connectedPuzzleQueue = new Queue<int>();
            puzzleAndLineQueue = new Queue<string>();
            puzzleQueue = new Queue<int>();
            puzzleLineQueue = new Queue<string>();
            puzzleLineResetQueue = new Queue<string>();
            boxQueue = new Queue<string>();
            sectionQueue = new Queue<string>();
            lineQueue = new Queue<string>();
            gemsQueue = new Queue<int>();
        }

        //public void AddConnectedPuzzleQueue(int index)
        //{
        //    connectedPuzzleQueue.Enqueue(index);
        //}

        public void AddPuzzleAndLineQueue(string value)
        {
            puzzleAndLineQueue.Enqueue(value);
        }

        public void AddPuzzleQueue(int index)
        {
            puzzleQueue.Enqueue(index);
        }

        public void AddPuzzleLineQueue(string value)
        {
            puzzleLineQueue.Enqueue(value);
        }

        public void AddPuzzleLineResetQueue(string value)
        {
            puzzleLineResetQueue.Enqueue(value);
        }

        public void AddBoxQueue(string value)
        {
            boxQueue.Enqueue(value);
        }

        public void AddSectionQueue(string value)
        {
            sectionQueue.Enqueue(value);
        }

        public void AddLineQueue(string value)
        {
            lineQueue.Enqueue(value);
        }

        public void AddGemsQueue(int index)
        {
            gemsQueue.Enqueue(index);
        }

        public void ExecuteSimulationQueue()
        {
            if (IsSimulationQueueNotEmpty())
            {
                StartCoroutine(UpdateSimulationQueue());
            }
        }

        private bool IsSimulationQueueNotEmpty()
        {
            return sectionQueue.Count > 0 || boxQueue.Count > 0 || lineQueue.Count > 0;
        }

        private IEnumerator UpdateSimulationQueue()
        {
            UIManager.instance.HideHUD();
            player.DisableInput();

            StartCoroutine(mapGenerator.SectionUpdate(sectionQueue));
            yield return new WaitUntil(() => mapGenerator.IsSectionUpdated());

            StartCoroutine(mapGenerator.LineAndBoxUpdate(boxQueue));
            yield return new WaitUntil(() => mapGenerator.IsLineUpdated());

            StartCoroutine(mapGenerator.LineOnlyUpdate(lineQueue));
            yield return new WaitUntil(() => mapGenerator.IsLineUpdated());

            yield return new WaitForSecondsRealtime(0.5f);

            sectionQueue.Clear();
            boxQueue.Clear();
            lineQueue.Clear();

            UIManager.instance.ShowHUD();
            player.EnableInput();
        }

        private bool IsBoxMapQueueNotEmpty()
        {
            //return connectedPuzzleQueue.Count > 0 || puzzleAndLineQueue.Count > 0;
            Debug.Log("All: " + puzzleAndLineQueue.Count + "|Puzzle: " + puzzleQueue.Count + "|Line: " + puzzleLineQueue.Count);
            return puzzleAndLineQueue.Count > 0 || puzzleQueue.Count > 0 || puzzleLineQueue.Count > 0 || puzzleLineResetQueue.Count > 0;
        }

        public void ExecuteBoxPuzzleQueue()
        {
            if (IsBoxMapQueueNotEmpty())
            {
                StartCoroutine(UpdatePuzzleQueue());
            }
        }

        // TODO: Fix some updated are wrong
        private IEnumerator UpdatePuzzleQueue()
        {
            UIManager.instance.HideHUD();
            player.DisableInput();

            //StartCoroutine(boxGenerator.UpdateQueue(connectedPuzzleQueue));
            //yield return new WaitUntil(() => boxGenerator.IsPuzzleUpdated());

            StartCoroutine(boxGenerator.UpdatePuzzleAndLineQueue(puzzleAndLineQueue));
            yield return new WaitUntil(() => boxGenerator.IsPuzzleUpdated());

            StartCoroutine(boxGenerator.UpdatePuzzleLineQueue(puzzleLineQueue));
            yield return new WaitUntil(() => boxGenerator.IsPuzzleUpdated());

            StartCoroutine(boxGenerator.UpdatePuzzleQueue(puzzleQueue));
            yield return new WaitUntil(() => boxGenerator.IsPuzzleUpdated());

            StartCoroutine(boxGenerator.ResetLinePuzzleQueue(puzzleLineResetQueue));
            yield return new WaitUntil(() => boxGenerator.IsPuzzleUpdated());

            yield return new WaitForSecondsRealtime(0.5f);

            //connectedPuzzleQueue.Clear();
            puzzleAndLineQueue.Clear();
            puzzleLineQueue.Clear();
            puzzleQueue.Clear();
            puzzleLineResetQueue.Clear();

            UIManager.instance.ShowHUD();
            player.EnableInput();
        }

        public void ExecuteGemsQueue()
        {
            if(IsGemsQueueNotEmpty())
            {
                // TODO: Use max sprite per popup or change to slider
                Sprite[] sprites = new Sprite[5];

                for(int i = 0; i < sprites.Length; i++)
                {
                    if(IsGemsQueueNotEmpty())
                    {
                        int id = gemsQueue.Dequeue();
                        Sprite sprite = SpriteAsset.instance.GetGemsSprite(id);
                        sprites[i] = sprite;
                    }
                    else
                    {
                        sprites[i] = null;
                    }
                }

                PopupCenterData popup = new PopupCenterData(PopupCenterType.SingleButtonImage, "UNLOCK", sprites, false);
                QueueManager.instance.AddPopupCenterQueue(popup);
            }
        }

        private bool IsGemsQueueNotEmpty()
        {
            return gemsQueue.Count > 0;
        }
    }
}

