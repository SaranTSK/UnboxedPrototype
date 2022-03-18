using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public static class Constant
    {
        // General
        public const int MAX_LIST = 3;
        public const int MAX_GATES = 4;
        public const int MAX_SECTIONS = 5;
        public const int MAX_GEMS = 10;
        public const int MAX_GEMSTIER = 6;
        public const int MIN_UNLOCK_GEMS_PUZZLE_ID = 181;

        // Circle data
        public const int MAX_CIRCLE = 16;
        public const int MAX_LEVEL_0_CIRCLE = 1;
        public const int MAX_LEVEL_1_CIRCLE = 5;
        public const int MAX_LEVEL_2_CIRCLE = 10;
        public const int MAX_LEVEL_3_CIRCLE = 10;
        // Gate 0
        public const int MAX_LEVEL_4_CIRCLE = 5; 
        public const int MAX_LEVEL_5_CIRCLE = 10;
        public const int MAX_LEVEL_6_CIRCLE = 30;
        public const int MAX_LEVEL_7_CIRCLE = 30;
        // Gate 1
        public const int MAX_LEVEL_8_CIRCLE = 10; 
        public const int MAX_LEVEL_9_CIRCLE = 50;
        public const int MAX_LEVEL_10_CIRCLE = 40;
        // Gate 2
        public const int MAX_LEVEL_11_CIRCLE = 10;
        public const int MAX_LEVEL_12_CIRCLE = 30;
        public const int MAX_LEVEL_13_CIRCLE = 20;
        // Gate 3
        public const int MAX_LEVEL_14_CIRCLE = 10;
        public const int MAX_LEVEL_15_CIRCLE = 10;

        // Camera
        public const int MIN_ZOOMSIZE = 3;
        public const int MAX_ZOOMSIZE = 15;
        public const int DEFAULT_ZOOMSIZE = 5;

        public const float X_BORDER = 25;
        public const float Y_BORDER = 30;
    }

    public enum FrameRarity
    {
        Common,
        Magic,
        Rare,
        Epic,
        Legendary,
        Unique
    }

    public enum GemsName
    {
        None,
        Red,
        Orange,
        Yellow,
        Green,
        Turquoise,
        Navy,
        Violet,
        Pink,
        Black,
        White
    }

    public enum DotType
    {
        Empty,
        MainAlphaDot,
        MainBetaDot,
        MainGammaDot,
        AlphaDot,
        BetaDot,
        GammaDot
    }

    public enum SectionName
    {
        Zero,
        First,
        Second,
        Third,
        Fourth,
        Tutorial,
        None,
    }

    public enum GemsTier
    {
        None,
        E,
        D,
        C,
        B,
        A,
        S
    }

    public enum CircleGenerate
    {
        MapCircle,
        SingleCircle,
        DoubleCircle,
        TripleCircle,
        QuatraCircle,
        FirstGate,
        SecondGate,
        ThirdGate
    }
}

