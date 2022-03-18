using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Unboxed
{
    public class SpriteAsset : MonoBehaviour
    {
        public static SpriteAsset instance;

        [SerializeField] private List<AssetReferenceSprite> frameSprites;
        [SerializeField] private List<AssetReferenceSprite> boxSprites;
        [SerializeField] private List<AssetReferenceSprite> puzzleSprites;
        [SerializeField] private List<AssetReferenceSprite> dotSprites;
        [SerializeField] private List<AssetReferenceSprite> lockSprites;
        [SerializeField] private List<AssetReferenceSprite> gemsSprites;

        [SerializeField] private List<Sprite> completeFrameSprites;
        [SerializeField] private List<Sprite> completeBoxSprites;
        [SerializeField] private List<Sprite> completePuzzleSprites;
        [SerializeField] private List<Sprite> completeDotSprites;
        [SerializeField] private List<Sprite> completeLockSprites;
        [SerializeField] private List<Sprite> completeGemsSprites;

        //public void SetInstance(SpriteAsset spriteAsset)
        //{
        //    instance = spriteAsset;
        //}

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(instance);
            }
        }

        private void Start()
        {
            StartCoroutine(LoadAndWaitUntilComplete());
        }

        private IEnumerator LoadAndWaitUntilComplete()
        {
           yield return AssetLoader.LoadSpriteAddToList(frameSprites, completeFrameSprites);
           yield return AssetLoader.LoadSpriteAddToList(boxSprites, completeBoxSprites);
           yield return AssetLoader.LoadSpriteAddToList(puzzleSprites, completePuzzleSprites);
           yield return AssetLoader.LoadSpriteAddToList(dotSprites, completeDotSprites);
           yield return AssetLoader.LoadSpriteAddToList(lockSprites, completeLockSprites);
           yield return AssetLoader.LoadSpriteAddToList(gemsSprites, completeGemsSprites);
        }

        public bool IsLoaded()
        {
            return completeFrameSprites.Count == frameSprites.Count &&
                completeBoxSprites.Count == boxSprites.Count &&
                completePuzzleSprites.Count == puzzleSprites.Count &&
                completeDotSprites.Count == dotSprites.Count &&
                completeLockSprites.Count == lockSprites.Count &&
                completeGemsSprites.Count == gemsSprites.Count;
        }

        public Sprite GetFrameSprite(FrameRarity rarity)
        {
            return completeFrameSprites[(int)rarity];
        }

        public Sprite GetBoxSprite(int id)
        {
            if(id < completeBoxSprites.Count)
            {
                return completeBoxSprites[id];
            }
            else
            {
                Debug.LogWarning("Box sprite id " + id + " has missing sprite reference!");
                return completeBoxSprites[0];
            }
            
        }

        public Sprite GetPuzzleSprite(GemsName gems)
        {
            return completePuzzleSprites[(int)gems];
        }

        public Sprite GetDotSprite(DotType type)
        {
            return completeDotSprites[(int)type];
        }

        public Sprite GetLockSprite(int index)
        {
            return completeLockSprites[index];
        }

        public Sprite GetGemsSprite(int index)
        {
            return completeGemsSprites[index];
        }
    }
}

