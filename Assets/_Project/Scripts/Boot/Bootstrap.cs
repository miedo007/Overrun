using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Project.Boot;
using Project.Application;

public class Bootstrap : MonoBehaviour
{
    private const string Gameplay = "game";
    private const string Home = "main_menu";

    void Awake()
    {
        StartCoroutine(DetermineStartScene());
    }

    private IEnumerator DetermineStartScene()
    {
        Debug.Log("[Bootstrap] Determining start scene...");

        // CRITICAL: Always wait for CrazySDK to initialize first
        Debug.Log("[Bootstrap] Waiting for CrazySDK to initialize...");
        
        float waitTime = 0f;
        while (!SaveSystemIntegration.IsCrazySDKReady())
        {
            yield return new WaitForSeconds(0.1f);
            waitTime += 0.1f;
            
            if (waitTime >= 10f)
            {
                Debug.LogWarning("[Bootstrap] SDK not ready after 10 seconds, checking local data");
                bool localProgress = CheckLocalProgress();
                LoadScene(localProgress ? Home : Gameplay);
                yield break;
            }
        }

        Debug.Log($"[Bootstrap] SDK ready after {waitTime}s, waiting for login status to be determined...");
        
        // Force a check and wait for it to complete
        SaveSystemIntegration.CheckUserLoginStatus();
        
        // Wait for login status to be determined (the callback to complete)
        waitTime = 0f;
        while (!SaveSystemIntegration.IsLoginStatusDetermined())
        {
            yield return new WaitForSeconds(0.1f);
            waitTime += 0.1f;
            
            if (waitTime >= 5f)
            {
                Debug.LogWarning("[Bootstrap] Login status not determined after 5 seconds, assuming not logged in");
                break;
            }
        }
        
        // Now check if user is logged in
        bool isLoggedIn = SaveSystemIntegration.IsUserLoggedIn;
        Debug.Log($"[Bootstrap] Login status determined: {isLoggedIn}");

        if (isLoggedIn)
        {
            // User is logged in - check cloud data
            Debug.Log("[Bootstrap] User logged in, checking cloud data for progress...");
            bool hasProgress = CheckCloudProgress();

            if (hasProgress)
            {
                Debug.Log("[Bootstrap] User has cloud progress - loading main menu");
                LoadScene(Home);
            }
            else
            {
                Debug.Log("[Bootstrap] User has no cloud progress - loading gameplay (first time)");
                LoadScene(Gameplay);
            }
        }
        else
        {
            // User not logged in - check local data
            Debug.Log("[Bootstrap] User not logged in, checking local data...");
            bool hasLocalProgress = CheckLocalProgress();

            if (hasLocalProgress)
            {
                Debug.Log("[Bootstrap] Local progress found - loading main menu");
                LoadScene(Home);
            }
            else
            {
                Debug.Log("[Bootstrap] No local progress - loading gameplay (first time user)");
                LoadScene(Gameplay);
            }
        }
    }

    private bool CheckCloudProgress()
    {
        Debug.Log("[Bootstrap] CheckCloudProgress: Creating optimal read writer...");
        var playerReadWriter = SaveSystemIntegration.CreateOptimalReadWriter("player");
        
        Debug.Log("[Bootstrap] CheckCloudProgress: Attempting to load cloud data...");
        if (playerReadWriter.TryLoad(out var rawPlayerData))
        {
            Debug.Log($"[Bootstrap] CheckCloudProgress: Raw data loaded, length={rawPlayerData.Length}");
            Debug.Log($"[Bootstrap] CheckCloudProgress: Raw data content: {rawPlayerData}");
            
            try
            {
                var playerData = Newtonsoft.Json.JsonConvert.DeserializeObject<Project.Application.PlayerSave>(rawPlayerData);
                bool hasProgress = playerData.TopStageIndex > 0 || playerData.Currency > 0;
                Debug.Log($"[Bootstrap] Cloud data - TopStageIndex: {playerData.TopStageIndex}, Currency: {playerData.Currency}, HasProgress: {hasProgress}");
                return hasProgress;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Bootstrap] Error parsing cloud data: {ex.Message}");
                Debug.LogError($"[Bootstrap] Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        Debug.Log("[Bootstrap] No cloud data found (TryLoad returned false)");
        return false;
    }

    private bool CheckLocalProgress()
    {
        var playerReadWriter = SaveSystemIntegration.CreateOptimalReadWriter("player");
        
        if (playerReadWriter.TryLoad(out var rawPlayerData))
        {
            try
            {
                var playerData = Newtonsoft.Json.JsonConvert.DeserializeObject<Project.Application.PlayerSave>(rawPlayerData);
                bool hasProgress = playerData.TopStageIndex > 0 || playerData.Currency > 0;
                Debug.Log($"[Bootstrap] Local data - TopStageIndex: {playerData.TopStageIndex}, Currency: {playerData.Currency}, HasProgress: {hasProgress}");
                return hasProgress;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Bootstrap] Error parsing local data: {ex.Message}");
                return false;
            }
        }

        Debug.Log("[Bootstrap] No local data found");
        return false;
    }

    private void LoadScene(string sceneName)
    {
        Debug.Log($"[Bootstrap] Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
