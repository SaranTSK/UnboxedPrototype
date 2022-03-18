using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public enum ArrayValueType
    {
        Normal,
        UpperRightDiangonal,
        UpperVertical,
        UpperLefttDiangonal,
        LowerRightDiangonal,
        LowerVertical,
        LowerLefttDiangonal
    }

    public static class GeneratorCalculator
    {
        public static Vector3 TransformCalculate(float theta, float circleRadius)
        {
            Vector3 childeTrans = Vector3.zero;

            float x = childeTrans.x + circleRadius * Mathf.Cos(Mathf.Deg2Rad * theta);
            float y = childeTrans.y + circleRadius * Mathf.Sin(Mathf.Deg2Rad * theta);

            childeTrans = new Vector3(x, y, 0);

            return childeTrans;
        }

        public static int GetBoxArrayIndex(CircleGeneratorScriptable circleGen, int level, int index)
        {
            int count = 0;

            if(level > 0)
            {
                for (int i = level - 1; i >= 0; i--)
                {
                    count += circleGen.circle[i].splitSection;
                }
            }

            return index + count;
        }

        public static int GetDotArrayIndex(int x, int y, int width, int height)
        {
            int targetX = x, targetY = y;

            //Debug.Log("[Normal] X: " + x + "|Y: " + y + "|Index: " + ((targetX * width) + targetY));

            return CheckIndexReturn(targetX, targetY, width, height);
        }

        public static int GetUpperRightDotArrayIndex(int x, int y, int width, int height)
        {
            int targetX = x, targetY = y;

            if (y > 0 && (y % 2 == 0 || x + 1 < width))
            {
                if (y % 2 != 0)
                {
                    targetX = x + 1;
                    targetY = y - 1;
                }
                else
                {
                    targetX = x;
                    targetY = y - 1;
                }
            }

            //Debug.Log("[UpperR] X: " + x + "|Y: " + y + "|Index: " + ((targetX * width) + targetY + targetX));

            return CheckIndexReturn(targetX, targetY, width, height);
        }

        public static int GetUpperLeftDotArrayIndex(int x, int y, int width, int height)
        {
            int targetX = x, targetY = y;

            if (y > 0 && (y % 2 != 0 || x > 0))
            {
                if (y % 2 != 0)
                {
                    targetX = x;
                    targetY = y - 1;
                }
                else
                {
                    targetX = x - 1;
                    targetY = y - 1;
                }

            }

            //Debug.Log("[UpperL] X: " + x + "|Y: " + y + "|Index: " + ((targetX * width) + targetY + targetX));

            return CheckIndexReturn(targetX, targetY, width, height);
        }

        public static int GetUpperDotArrayIndex(int x, int y, int width, int height)
        {
            int targetX = x, targetY = y;

            if (y - 2 >= 0)
            {
                targetX = x;
                targetY = y - 2;
            }

            //Debug.Log("[Upper] X: " + x + "|Y: " + y + "|Index: " + ((targetX * width) + targetY + targetX));

            return CheckIndexReturn(targetX, targetY, width, height);
        }

        public static int GetLowerRightDotArrayIndex(int x, int y, int width, int height)
        {
            int targetX = x, targetY = y;

            if (y + 1 < height && (y % 2 == 0 || x + 1 < width))
            {
                if (y % 2 != 0)
                {
                    targetX = x + 1;
                    targetY = y + 1;
                }
                else
                {
                    targetX = x;
                    targetY = y + 1;
                }
            }

            //Debug.Log("[LowerR] X: " + x + "|Y: " + y + "|Index: " + ((targetX * width) + targetY + targetX));

            return CheckIndexReturn(targetX, targetY, width, height);
        }

        public static int GetLowerLeftDotArrayIndex(int x, int y, int width, int height)
        {
            int targetX = x, targetY = y;

            if (y + 1 < height && (y % 2 != 0 || x > 0))
            {
                if (y % 2 != 0)
                {
                    targetX = x;
                    targetY = y + 1;
                }
                else
                {
                    targetX = x - 1;
                    targetY = y + 1;
                }
            }

            //Debug.Log("[LowerL] X: " + x + "|Y: " + y + "|Index: " + ((targetX * width) + targetY + targetX));

            return CheckIndexReturn(targetX, targetY, width, height);
        }

        public static int GetLowerDotArrayIndex(int x, int y, int width, int height)
        {
            int targetX = x, targetY = y;

            if (y + 2 < height)
            {
                targetX = x;
                targetY = y + 2;
            }

            //Debug.Log("[Lower] X: " + x + "|Y: " + y + "|Index: " + ((targetX * width) + targetY + targetX));

            return CheckIndexReturn(targetX, targetY, width, height);
        }

        public static bool IsSectionUnlocked(GemsName gems, SectionName section)
        {
            MapSectionScriptable mapData = DataAsset.instance.GetMapData();
            
            if (gems != GemsName.None)
            {
                int index = (int)gems - 1;
                switch (section)
                {
                    case SectionName.Zero: return mapData.gateInfoList[index].zeroSectionUnlock;
                    case SectionName.First: return mapData.gateInfoList[index].firstSectionUnlock;
                    case SectionName.Second: return mapData.gateInfoList[index].secondSectionUnlock;
                    case SectionName.Third: return mapData.gateInfoList[index].thirdSectionUnlock;
                    case SectionName.Fourth: return mapData.gateInfoList[index].fourthSectionUnlock;
                }
            }
            else if(section == SectionName.Tutorial)
            {
                return mapData.tutorialUnlock;
            }
            else if(section == SectionName.None || (gems == GemsName.None && section == SectionName.Zero))
            {
                return true;
            }

            return false;
        }

        private static int CheckIndexReturn(int x, int y, int width, int height)
        {
            if((width % 2 == 0 && height % 2 == 0) || (width % 2 != 0 && height % 2 != 0))
            {
                // width and height are even number
                return (x * width) + y;
            }
            else
            {
                // width and height are odd number
                return (x * width) + y + x; ;
            }
        }
    }
}

