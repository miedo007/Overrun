using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Application
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup transitionGroup;
        
        private void Start()
        {
            transitionGroup.gameObject.SetActive(false);
        }

        public void LoadScene(string sceneName, float transitionOut = 0, float transitionIn = 0)
        {
            if (transitionIn > 0 || transitionOut > 0)
            {
                StartCoroutine(LoadSceneRoutine(sceneName,  transitionOut, transitionIn));
            }
            else
            {
                transitionGroup.gameObject.SetActive(false);                                                                                                              
                SceneManager.LoadScene(sceneName);
            }
        }
        
        private IEnumerator LoadSceneRoutine(string sceneName,  float transitionOut = 0, float transitionIn = 0)
        {
            transitionGroup.gameObject.SetActive(true);
            transitionGroup.alpha = 0;
            
            yield return transitionGroup.DOFade(1, transitionOut).WaitForCompletion();

            yield return new WaitForSeconds(0.1f);       
            DOTween.KillAll();                            
            SceneManager.LoadScene(sceneName);
            yield return new WaitForSeconds(0.1f);
            
            yield return transitionGroup.DOFade(0, transitionIn).WaitForCompletion();
            transitionGroup.gameObject.SetActive(false);
        }

    }
}