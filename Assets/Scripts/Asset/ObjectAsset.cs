using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Unboxed
{
    public enum PrefType
    {
        BoxIcon,
        BoxSymbol,
        TutorialSection,
        CircleSection,
        Dot,
        Cursor,
        PlayerLine,
        MapLine,
        PuzzleLine,
        Gems
    }

    public class ObjectAsset : MonoBehaviour
    {
        public static ObjectAsset instance;

        [SerializeField] private List<AssetReference> assetPref;
        [SerializeField] private List<GameObject> completePref;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            StartCoroutine(LoadAndWaitUntilComplete());
        }

        private IEnumerator LoadAndWaitUntilComplete()
        {
            yield return AssetLoader.CreateAssetAddToList(assetPref, completePref);
        }

        public bool IsLoaded()
        {
            return completePref.Count == assetPref.Count; 
        }

        public GameObject GetPref(PrefType type)
        {
            return completePref[(int)type];
        }
    }
}

