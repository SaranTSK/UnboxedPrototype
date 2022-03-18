using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class Grid
    {
        private int width;
        private int height;
        private int gridGap;
        private SpriteRenderer spriteRenderer;

        private const float X_OFFSET = -1.75f;
        private const float Y_OFFSET = 0.45f;

        private int[,] gridArray;

        public int[,] GridArray { get { return gridArray; } }

        public void Initailize(int _width, int _height, int _gridGap, SpriteRenderer _spriteRenderer)
        {
            width = _width;
            height = _height;
            gridGap = _gridGap < 1 ? 1 : _gridGap * 2;
            spriteRenderer = _spriteRenderer;

            gridArray = new int[_width, _height];
        }

        public Vector3 GetCellWorldPosition(int x, int y)
        {
            //Debug.Log(spriteRenderer.sprite.rect.width + " " + spriteRenderer.sprite.rect.height);

            float spriteWidth = spriteRenderer.sprite.rect.width;
            float spriteHeight = spriteRenderer.sprite.rect.height;

            float widthOffset = spriteWidth / 2;
            float heightOffset = spriteHeight * 1.75f;

            float posX = -width * (spriteWidth / 200 * gridGap) + X_OFFSET;
            float posY = height * 0.75f * (spriteHeight / 200 * gridGap) - Y_OFFSET;

            if (y % 2 == 0 )
            {
                return new Vector3(spriteWidth * x * 2.5f / (100 / gridGap) + posX, (spriteHeight * y - (heightOffset * y)) / (100 / gridGap) + posY, 0);
            }
            else
            {
                return new Vector3((spriteWidth * x + widthOffset) * 2.5f / (100 / gridGap) + posX, (spriteHeight * y - (heightOffset * y)) / (100 / gridGap) + posY, 0);
            }
            
        }
    }
}

