using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class DesignManager : MonoBehaviour
    {
        public enum DesignMode
        {
            Box,
            Puzzle
        }

        [SerializeField] private DesignMode mode;
        [SerializeField] private BoxGenerator boxGenerator;
        [SerializeField] private BoxScriptable boxInfo;
        [SerializeField] private PuzzleController puzzleGenerator;
        [SerializeField] private int puzzleId;

        private void Start()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            yield return new WaitUntil(() => SpriteAsset.instance.IsLoaded() == true);
            yield return new WaitUntil(() => DataAsset.instance.IsLoaded() == true);
            yield return new WaitUntil(() => ObjectAsset.instance.IsLoaded() == true);
            DesignInit();
        }

        private void DesignInit()
        {
            switch(mode)
            {
                case DesignMode.Box:
                    boxGenerator.Initailize(boxInfo);
                    break;

                case DesignMode.Puzzle:
                    puzzleGenerator.Initailize(boxInfo.puzzleList[puzzleId]);
                    break;
            }
        }
    }
}

