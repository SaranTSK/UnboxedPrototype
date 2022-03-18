using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    [CreateAssetMenu(fileName = "MapCircleData", menuName = "ScriptableObjects/CircleGenrator", order = 2)]
    public class CircleGeneratorScriptable : ScriptableObject
    {
        public CircleProperty[] circle;
    }

    [System.Serializable]
    public class CircleProperty
    {
        public int modifyAngle;
        public int splitSection;
        public float circleRadius;
    }
}

