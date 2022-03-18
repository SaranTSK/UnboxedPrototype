using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public partial class LevelManager : MonoBehaviour
    {
        //Temp puzzle result
        private DotList dotList;
        private bool[] puzzleResults = new bool[Constant.MAX_LIST];
        private Grid grid;
        private Dot[] dotArray;
        private string puzzleIndex;

        public void CreatePuzzleResult()
        {
            for (int i = 0; i < dotList.list.Count; i++)
            {
                if (dotList.list[i] == null || dotList.list[i].Count == 0)
                {
                    puzzleResults[i] = true;
                }
                else
                {
                    puzzleResults[i] = false;
                }
            }
        }

        public void SetPuzzleIndex(int level, int index)
        {
            puzzleIndex = level + "_" + index;
        }

        public void SetGrid(Grid _grid)
        {
            grid = _grid;
        }

        public void CreateDotArray(int lenght)
        {
            dotArray = new Dot[lenght];
        }

        // ---------- Enter puzzle state ----------
        public void EnterPuzzle()
        {
            UIManager.instance.IsLoading(true);
            InitPuzzleMap();
            FocusAtPuzzle();
            EnterPuzzleUI();
            GameManager.instance.SetGameState(GameState.Puzzle);
            StartCoroutine(LoadingDelay());
        }

        private void InitPuzzleMap()
        {
            puzzle = new GameObject("PuzzleTemp");
            box.gameObject.SetActive(false);
            dotList.Create();
        }

        private void RemovePuzzleMap()
        {
            Destroy(puzzle);
            dotList.Reset();
            box.gameObject.SetActive(true);
        }

        private void EnterPuzzleUI()
        {
            UIManager.instance.SwitchFooter(FooterType.PuzzleFooter);
        }

        // ---------- Create puzzle dot list ----------

        public void AddChildDotList(Dot dot)
        {
            switch (dot.type)
            {
                case DotType.MainAlphaDot:
                case DotType.AlphaDot:
                    dotList.list[(int)ListType.Alpha].Add(dot);
                    break;

                case DotType.MainBetaDot:
                case DotType.BetaDot:
                    dotList.list[(int)ListType.Beta].Add(dot);
                    break;

                case DotType.MainGammaDot:
                case DotType.GammaDot:
                    dotList.list[(int)ListType.Gamma].Add(dot);
                    break;
            }
        }

        public void AddDotArray(int index, Dot dot)
        {
            dotArray[index] = dot;
        }

        // ---------- Handle puzzle state ----------

        public bool CheckPuzzleResult()
        {
            bool result = true;

            for (int i = 0; i < puzzleResults.Length; i++)
            {
                if (puzzleResults[i] == false)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        public void OnClearedPuzzle()
        {
            int level = int.Parse(puzzleIndex.Split('_').GetValue(0).ToString());
            int index = int.Parse(puzzleIndex.Split('_').GetValue(1).ToString());

            PuzzleController puzzleController = box.transform.GetChild(level).GetChild(index).GetComponent<PuzzleController>();
            puzzleController.OnCleared();
        }

        public void CheckListResult(int index, List<Dot> dots)
        {
            List<Dot> tempList = new List<Dot>();

            foreach(Dot dot in dotList.list[index])
            {
                if(!dots.Contains(dot))
                {
                    tempList.Add(dot);
                }
            }

            if(tempList.Count > 0)
            {
                puzzleResults[index] = false;

                for(int i = 0; i < tempList.Count; i++)
                {
                    tempList[i].DotWarning();
                }
            }
            else
            {
                puzzleResults[index] = true;
            }
        }

        public bool GetDotListResult(int index)
        {
            return puzzleResults[index];
        }

        // Use previous dot before insert
        public bool CheckGridValue(Dot prevDot, Dot dot)
        {
            //string[] splite = dot.name.Split('_');
            int x = int.Parse(dot.name.Split('_').GetValue(1).ToString());
            int y = int.Parse(dot.name.Split('_').GetValue(2).ToString());
            int width = grid.GridArray.GetLength(0);
            int height = grid.GridArray.GetLength(1);

            if (prevDot.index == dotArray[GeneratorCalculator.GetUpperRightDotArrayIndex(x, y, width, height)].index ||
                prevDot.index == dotArray[GeneratorCalculator.GetUpperDotArrayIndex(x, y, width, height)].index ||
                prevDot.index == dotArray[GeneratorCalculator.GetUpperLeftDotArrayIndex(x, y, width, height)].index ||
                prevDot.index == dotArray[GeneratorCalculator.GetLowerRightDotArrayIndex(x, y, width, height)].index ||
                prevDot.index == dotArray[GeneratorCalculator.GetLowerDotArrayIndex(x, y, width, height)].index ||
                prevDot.index == dotArray[GeneratorCalculator.GetLowerLeftDotArrayIndex(x, y, width, height)].index)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }


    }
}

