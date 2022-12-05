using System;
using System.Collections;
using Project.Application;
using UnityEngine;

namespace Project.Game.Shop
{
    public class ShopTutorial : MonoBehaviour
    {
        [SerializeField] private TutorialStage[] stages;

        private int _currentStage = 0;
        private void Awake()
        {
            foreach (var tutorialStage in stages)
            {
                tutorialStage.gameObject.SetActive(false);
            }
        }
        
        public void Initialize()
        {
            _currentStage = 0;
            OpenCurrentStage();
        }

        private void OpenCurrentStage()
        {
            stages[_currentStage].Open();
            stages[_currentStage].Closed += OnStageClosed;
        }

        private void OnStageClosed()
        {
            stages[_currentStage].Closed -= OnStageClosed;
            _currentStage++;
            if (_currentStage >= stages.Length)
            {
                PlayerPrefs.SetInt(PrefKeys.CraftingTutorialCompleted, 1);
                gameObject.SetActive(false);
            }
            else
            {
               OpenCurrentStage();
            }
        }

    }
}