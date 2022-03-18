using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Unboxed
{
    public enum HeaderText
    {
        Add,
        Remove
    }

    public class Header : MonoBehaviour
    {
        private TextMeshProUGUI headerText;

        private string currentHeaderText;

        public void Initialize(string text)
        {
            headerText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            headerText.text = text;
            currentHeaderText = text;
        }

        public void BackButton()
        {
            switch (GameManager.instance.currentState)
            {
                case GameState.Simulation:
                    break;

                case GameState.BoxLevel:
                    LevelManager.instance.EnterSimulation();
                    break;

                case GameState.Puzzle:
                    LevelManager.instance.EnterBox();
                    break;
            }
        }

        public void AddHeaderText(string text)
        {
            currentHeaderText += " > " + text;
            UpdateHeaderText();
        }

        public void RemoveHeaderText()
        {
            string[] texts = currentHeaderText.Split('>');
            texts[0] = texts[0].Replace(" ", "");
            currentHeaderText = texts[0];

            for (int i = 1; i < texts.Length - 1; i++)
            {
                texts[i] = texts[i].Replace(" ", "");
                currentHeaderText += " > " + texts[i];
            }
            UpdateHeaderText();
        }

        private void UpdateHeaderText()
        {
            headerText.text = currentHeaderText;
        }
    }
}

