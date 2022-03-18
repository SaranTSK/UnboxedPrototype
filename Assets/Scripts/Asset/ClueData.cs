using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class ClueData
    {
        public List<int> firstClue;
        public List<int> secondClue;
        public List<int> thirdClue;
        public List<int> gemsClue;

        public ClueData()
        {
            firstClue = new List<int>();
            secondClue = new List<int>();
            thirdClue = new List<int>();
            gemsClue = new List<int>();
        }
    }

    public enum Clue
    {
        FirstClue,
        SecondClue,
        ThirdClue,
        GemsClue
    }
}

