using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public enum DotState
    {
        Empty,
        Fill,
        Clear
    }

    public class Dot : MonoBehaviour
    {
        public int index { get; private set; }
        public DotType type { get; private set; }
        public DotState state { get; private set; }
        public int pointIndex { get; private set; }

        // TODO: Add fill and clear sprite
        [SerializeField] private Sprite[] sprites;

        private SpriteRenderer spriteRenderer;

        public void Initialize(int _index, DotType _type, DotState _state)
        {
            index = _index;
            type = _type;
            state = _state;
            pointIndex = -1;

            spriteRenderer = transform.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprites[(int)_type];
        }

        private void SetSprite()
        {
            // TODO: Get sprite from array index
            switch (state)
            {
                case DotState.Empty: break;

                case DotState.Fill: break;

                case DotState.Clear: break;
            }
        }

        public void SetPointIndex(int index)
        {
            pointIndex = index;
        }

        public void OnEnter()
        {
            // TODO: Play rotate animation
            Debug.Log(name + " | Index: " + index + " | " + type);
        }

        public void CheckDotState()
        {
            if(pointIndex == -1)
            {
                state = DotState.Empty;
            }
            else if(pointIndex == -2)
            {
                state = DotState.Clear;
            }
            else
            {
                state = DotState.Fill;
            }

            SetSprite();
        }

        public int GetListType(DotType dotType)
        {
            switch (dotType)
            {
                case DotType.MainAlphaDot:
                case DotType.AlphaDot:
                    return 0;

                case DotType.MainBetaDot:
                case DotType.BetaDot:
                    return 1;

                case DotType.MainGammaDot:
                case DotType.GammaDot:
                    return 2;

                default: return -1;
            }
        }

        public void DotWarning()
        {
            //TODO: Set warning animation
            StartCoroutine(PlayWarning());
            Debug.Log("Dot " + name + " dont match!!!");
        }

        private IEnumerator PlayWarning()
        {
            spriteRenderer.sprite = sprites[0];
            yield return new WaitForSecondsRealtime(0.1f);
            spriteRenderer.sprite = sprites[(int)type];
            yield return new WaitForSecondsRealtime(0.1f);
            spriteRenderer.sprite = sprites[0];
            yield return new WaitForSecondsRealtime(0.1f);
            spriteRenderer.sprite = sprites[(int)type];
        }
    }

    public class DotList
    {
        public List<List<Dot>> list;

        public DotList()
        {
            list = new List<List<Dot>>();
        }

        public void Reset()
        {
            list.Clear();
        }

        public void Create()
        {
            for (int i = 0; i < Constant.MAX_LIST; i++)
            {
                list.Add(new List<Dot>());
            }
        }
    }

    public enum ListType
    {
        Alpha,
        Beta,
        Gamma
    }
}

