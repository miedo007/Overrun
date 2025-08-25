using UnityEngine;

public static class PokiSignals
{
    private static bool _isPlaying;

    [System.Diagnostics.Conditional("POKI_DEBUG")]
    private static void Log(string m) => Debug.Log($"[Poki] {m}");

    public static void GameplayStart()
    {
        if (_isPlaying) { Log("start skipped"); return; }
        _isPlaying = true; Log("start");
#if UNITY_WEBGL && !UNITY_EDITOR
        PokiUnitySDK.Instance.gameplayStart();
#endif
    }

    public static void GameplayStop()
    {
        if (!_isPlaying) { Log("stop skipped"); return; }
        _isPlaying = false; Log("stop");
#if UNITY_WEBGL && !UNITY_EDITOR
        PokiUnitySDK.Instance.gameplayStop();
#endif
    }

    // Use once when entering a scene to sync our local state.
    public static void ForceReset(bool isPlaying = false) => _isPlaying = isPlaying;
}
