using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Project.Boot;

public class Bootstrap : MonoBehaviour
{
    // Change to your exact scene names in Build Settings
    private const string Gameplay = "Game";
    private const string Home     = "main_menu";

    void Awake()
    {
        // Ensure proper EventSystem management
        gameObject.AddComponent<EventSystemManager>();
        
        var flagPath = Path.Combine(Application.persistentDataPath, "first_launch.flag");
        bool firstLaunch = !File.Exists(flagPath);

        if (firstLaunch)
        {
            // Create the flag so next time we go to Home
            File.WriteAllText(flagPath, "1");
            SceneManager.LoadScene(Gameplay, LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene(Home, LoadSceneMode.Single);
        }
    }
}
