using UnityEngine;
using UnityEngine.UI;

namespace Project.Feedback
{
    [RequireComponent(typeof(Button))]
    public class ButtonFeedback : MonoBehaviour
    {
        [SerializeField] private FeedbackData onPressFeedback;
        [SerializeField] private bool manualSetup;
        
        private Button _button;

        public void Awake()
        {
            if (!manualSetup)
            {
                Setup();
            }
        }

        public void Setup()
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }
            
            _button.onClick.AddListener(OnButtonClick);
        }
        
        private void OnButtonClick()
        {
            onPressFeedback.Play();
        }
    }
}