// File: CrazySdkManager.cs  (attach to Boot scene; keep only ONE)
using UnityEngine;
using CrazyGames;

public class CrazySdkManager : MonoBehaviour
{
    static CrazySdkManager _instance;

    static bool _initing, _ready;
    static bool _startedSent;          // true only after we successfully send start
    static bool _wantStarted = true;   // desired state; we replay it after init/focus

    void Awake()
    {
        if (_instance != null) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        TryInit();
    }

    public static void TryInit()
    {
        if (_ready || _initing) return;

#if UNITY_WEBGL && !UNITY_EDITOR
        if (!CrazySDK.IsAvailable)
        {
            Debug.Log("[CrazySDK] Not available (editor/local/whitelist/other host)");
            return;
        }

        _initing = true;
        CrazySDK.Init(() =>
        {
            _initing = false;
            _ready = CrazySDK.IsInitialized;
            if (!_ready) return;

            Debug.Log("[CrazySDK] Init OK");
            // Reapply desired state after init
            if (_wantStarted) DoStart();
            else              DoStop();
        });
#endif
    }

    // ---- Call these from gameplay code ----
    public static void GameplayStart()
    {
        _wantStarted = true;
        if (_ready) DoStart();
    }

    public static void GameplayStop()
    {
        _wantStarted = false;
        if (_ready) DoStop();
    }

    // Stop on blur/pause; resume to desired state on refocus/unpause
    // Note: Disabled for web builds to prevent gameplayStop when clicking outside game frame
    void OnApplicationFocus(bool hasFocus)
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (!hasFocus) DoStop();
        else if (_wantStarted) DoStart();
#endif
    }

    void OnApplicationPause(bool paused)
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (paused) DoStop();
        else if (_wantStarted) DoStart();
#endif
    }

    // ---- Internals (send once; only flip _startedSent when SDK call is made) ----
    static void DoStart()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (!_ready || _startedSent) return;
        try { CrazySDK.Game.GameplayStart(); Debug.Log("[CrazySDK] GameplayStart sent"); }
        catch {}
        _startedSent = true;
#endif
    }

    static void DoStop()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (!_ready || !_startedSent) return;
        try { CrazySDK.Game.GameplayStop(); Debug.Log("[CrazySDK] GameplayStop sent"); }
        catch {}
        _startedSent = false;
#endif
    }
}
