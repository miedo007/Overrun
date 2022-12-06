using Mtl.UiFramework;
using Project.Game.Shop;
using UnityEngine;

namespace Project.Game.Tutorials
{
    public class TutorialScreen : UIScreen
    {
        [SerializeField] private TutorialStage[] stages;

        private int _currentStage;
        
        private void Awake()
        {
            foreach (var tutorialStage in stages)
            {
                tutorialStage.gameObject.SetActive(false);
            }
        }

        protected override void OnOpened()
        {
            base.OnOpened();
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
                Close();
            }
            else
            {
                OpenCurrentStage();
            }
        }
    }
}