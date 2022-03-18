using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class Loading : MonoBehaviour
    {
        public void Initialize()
        {
            HideLoading();
        }

        public Loading GetLoading()
        {
            return this;
        }

        public void ShowLoading()
        {
            transform.gameObject.SetActive(true);
        }

        public void HideLoading()
        {
            transform.gameObject.SetActive(false);
        }
    }
}

