using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unboxed
{
    public class HomeFooter : MonoBehaviour
    {
        public void AccountButton()
        {
            Debug.Log("Account");
        }

        public void AnalysisButton()
        {
            Debug.Log("Analysis");
            HideAllTabs();
            ClueManager.instance.EnterAnalysis();
        }

        public void SimulationButton()
        {
            Debug.Log("Simulation");
            HideAllTabs();
            LevelManager.instance.EnterSimulation();
        }

        public void CombinationButton()
        {
            Debug.Log("Combination");
        }

        public void DatabaseButton()
        {
            Debug.Log("Database");
        }

        private void HideAllTabs()
        {
            LevelManager.instance.ExitSimultation();
            ClueManager.instance.ExitAnalysis();
        }
    }
}

