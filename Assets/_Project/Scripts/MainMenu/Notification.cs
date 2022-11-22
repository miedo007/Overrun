using UnityEngine;

namespace Project.MainMenu
{
    public class Notification : MonoBehaviour
    {
        [SerializeField] private GameObject root;

        public void Show(bool shouldShow)
        {
            root.SetActive(shouldShow);
        }
    }
}