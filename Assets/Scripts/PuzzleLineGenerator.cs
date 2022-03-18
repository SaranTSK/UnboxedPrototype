using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class PuzzleLineGenerator : MonoBehaviour
    {
        private LineRenderer line;
        private List<Dot> dots;

        private Grid grid;
        private int width;
        private int height;

        public GameObject linePerent { get; private set; }

        public void Initialize(Grid _grid)
        {
            grid = _grid;
            width = grid.GridArray.GetLength(0);
            height = grid.GridArray.GetLength(1);

            line = ObjectAsset.instance.GetPref(PrefType.PuzzleLine).GetComponent<LineRenderer>();
            dots = new List<Dot>();

            linePerent = new GameObject();
            linePerent.name = "Line";
        }

        public void AddChildDot(Dot dot)
        {
            dots.Add(dot);
        }

        public void LineGenerate()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    DrawUpperDiagonal(x, y);
                    DrawLowerDiagonal(x, y);
                    DrawLowerVertical(x, y);
                }
            }
        }

        private void DrawUpperDiagonal(int x, int y)
        {
            Dot startDot = dots[GeneratorCalculator.GetDotArrayIndex(x, y, width, height)];
            Dot endDot = dots[GeneratorCalculator.GetUpperRightDotArrayIndex(x, y, width, height)];

            if(startDot.type != DotType.Empty && endDot.type != DotType.Empty)
            {
                DrawLine(startDot.transform, endDot.transform);
            } 
        }

        private void DrawLowerDiagonal(int x, int y)
        {
            Dot startDot = dots[GeneratorCalculator.GetDotArrayIndex(x, y, width, height)];
            Dot endDot = dots[GeneratorCalculator.GetLowerRightDotArrayIndex(x, y, width, height)];

            if (startDot.type != DotType.Empty && endDot.type != DotType.Empty)
            {
                DrawLine(startDot.transform, endDot.transform);
            }
        }

        private void DrawLowerVertical(int x, int y)
        {
            Dot startDot = dots[GeneratorCalculator.GetDotArrayIndex(x, y, width, height)];
            Dot endDot = dots[GeneratorCalculator.GetLowerDotArrayIndex(x, y, width, height)];

            if (startDot.type != DotType.Empty && endDot.type != DotType.Empty)
            {
                DrawLine(startDot.transform, endDot.transform);
            }
        }

        private void DrawLine(Transform startPoint, Transform endPoint)
        {
            LineRenderer lineRenderer = Instantiate(line);
            lineRenderer.transform.parent = linePerent.transform;

            Vector3 startPos = new Vector3(startPoint.position.x, startPoint.position.y, 1);
            Vector3 endPos = new Vector3(endPoint.position.x, endPoint.position.y, 1);

            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
        }
    }
}

