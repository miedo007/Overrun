using System;
using System.Collections;
using Mtl.Save;
using UnityEngine;
using CrazyGames;

namespace Project.Application
{
    public static class SaveSystemIntegration
    {
        private static bool _isUserLoggedIn = false;
        private static bool _hasInitialized = false;
        private static bool _hasCheckedUserOnce = false;
        private static bool _authListenerAdded = false;
        private static MonoBehaviour _coroutineRunner;

        public static event Action<bool> OnLoginStatusChanged;

        public static bool IsUserLoggedIn => _isUserLoggedIn;
        public static bool IsLoginStatusDetermined() => _hasInitialized && IsCrazySDKReady() && _hasCheckedUserOnce;

        public static void Initialize()
        {
            if (_hasInitialized) return;
            
            _hasInitialized = true;
            
            // Create a GameObject to run coroutines
            var helperObject = new GameObject("SaveSystemIntegrationHelper");
            _coroutineRunner = helperObject.AddComponent<SaveSystemIntegrationHelper>();
            UnityEngine.Object.DontDestroyOnLoad(helperObject);
            
            // Use the existing CrazySdkManager to ensure proper initialization
            CrazySdkManager.TryInit();
            
            // Start checking for login status
            _coroutineRunner.StartCoroutine(WaitForInitAndCheckLogin());
        }

        private static IEnumerator WaitForInitAndCheckLogin()
        {
            // Wait for CrazySDK to be ready
            while (!IsCrazySDKReady())
            {
                yield return new WaitForSeconds(0.5f);
            }
            
            Debug.Log("CrazySDK is ready, checking user login status");
            CheckUserLoginStatus();
            
            // Listen for auth state changes instead of polling
            SetupAuthListener();
        }

        private static void SetupAuthListener()
        {
            try
            {
                if (IsCrazySDKReady() && !_authListenerAdded)
                {
                    // Add auth listener to detect login/logout events
                    CrazySDK.User.AddAuthListener((user) =>
                    {
                        var isLoggedIn = user != null && !string.IsNullOrEmpty(user.username);
                        SetLoginStatus(isLoggedIn);
                        
                        if (isLoggedIn)
                        {
                            Debug.Log($"User logged in via auth listener: {user.username}");
                        }
                        else
                        {
                            Debug.Log("User logged out via auth listener");
                        }
                    });
                    _authListenerAdded = true;
                    Debug.Log("Auth listener added successfully");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to setup auth listener: {e.Message}");
            }
        }

        private static IEnumerator PollLoginStatus()
        {
            // Removed frequent polling - now only using auth listeners and manual checks
            yield break;
        }

        public static void CheckUserLoginStatus()
        {
            try
            {
                if (!IsCrazySDKReady())
                {
                    SetLoginStatus(false);
                    return;
                }

                // Only call GetUser if we haven't checked before or if explicitly requested
                CrazySDK.User.GetUser(user =>
                {
                    var isLoggedIn = user != null && !string.IsNullOrEmpty(user.username);
                    SetLoginStatus(isLoggedIn);
                    _hasCheckedUserOnce = true;
                    
                    if (isLoggedIn)
                    {
                        Debug.Log($"User login status checked: {user.username}");
                    }
                    else
                    {
                        Debug.Log("User login status checked: Not logged in");
                    }
                });
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to check user login status: {e.Message}");
                SetLoginStatus(false);
                _hasCheckedUserOnce = true;
            }
        }

        // Method to force a fresh check (use sparingly)
        public static void RefreshUserLoginStatus()
        {
            _hasCheckedUserOnce = false;
            CheckUserLoginStatus();
        }

        private static void SetLoginStatus(bool isLoggedIn)
        {
            if (_isUserLoggedIn != isLoggedIn)
            {
                var wasLoggedIn = _isUserLoggedIn;
                _isUserLoggedIn = isLoggedIn;
                
                Debug.Log($"User login status changed: {(_isUserLoggedIn ? "Logged in" : "Logged out")}");
                
                OnLoginStatusChanged?.Invoke(_isUserLoggedIn);
                
                // If user just logged in, trigger migration
                if (_isUserLoggedIn && !wasLoggedIn)
                {
                    Debug.Log("User just logged in - migration will be triggered");
                }
            }
        }

        public static ReadWriter CreateOptimalReadWriter(string saveKey)
        {
            Debug.Log($"Creating dynamic ReadWriter for save key: {saveKey}");
            return new DynamicReadWriter(saveKey);
        }

        public static void MigrateFileDataToCrazyGamesData(string saveKey)
        {
            if (!IsCrazySDKReady())
            {
                Debug.LogWarning($"Cannot migrate {saveKey}: Crazy Games SDK not ready");
                return;
            }
            
            try
            {
                // Check if data already exists in Crazy Games Data module
                var crazyGamesReader = new CrazyGamesDataReadWriter(saveKey);
                if (crazyGamesReader.TryLoad(out _))
                {
                    Debug.Log($"Save data for '{saveKey}' already exists in Crazy Games Data module, skipping migration");
                    return;
                }

                // Try to load from local files
                var fileReader = new FileReadWriter(saveKey);
                if (fileReader.TryLoad(out var rawSave))
                {
                    // Save to Crazy Games Data module
                    crazyGamesReader.Save(rawSave);
                    
                    Debug.Log($"Successfully migrated save data for key '{saveKey}' to Crazy Games Data module");
                }
                else
                {
                    Debug.Log($"No local save data found for key '{saveKey}' to migrate");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to migrate save data for key '{saveKey}': {e.Message}");
            }
        }

        public static bool IsCrazySDKReady()
        {
            try
            {
                return CrazySDK.IsInitialized && CrazySDK.IsAvailable;
            }
            catch
            {
                return false;
            }
        }

        public static void ShowLoginDialog()
        {
            try
            {
                if (IsCrazySDKReady())
                {
                    CrazySDK.User.ShowAuthPrompt((error, user) =>
                    {
                        if (error != null)
                        {
                            Debug.LogError($"Auth prompt error: {error.code}");
                        }
                        else
                        {
                            Debug.Log($"Auth prompt result: {(user != null ? "Success" : "Failed")}");
                        }
                        // Force a refresh after login dialog
                        RefreshUserLoginStatus();
                    });
                }
                else
                {
                    Debug.LogWarning("Cannot show login dialog: Crazy Games SDK not ready");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to show login dialog: {e.Message}");
            }
        }
    }
}