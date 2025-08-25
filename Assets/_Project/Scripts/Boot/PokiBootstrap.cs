using UnityEngine;

public class PokiBootstrap : MonoBehaviour
{
    void Awake()
    {
        // Initialize as early as possible
        PokiUnitySDK.Instance.init();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // Optional hygiene on WebGL
        if (hasFocus) PokiUnitySDK.Instance.gameplayStart();
        else PokiUnitySDK.Instance.gameplayStop();
    }
}
