using UnityEngine;

namespace Project.Application
{
    public class PrefKeys
    {
        public const string HapticsEnabled = "pref_haptics";
        public const string SoundEnabled = "pref_sound";
        public const string MusicEnabled = "pref_music";
        public const string ScreenShakeEnabled = "pref_screen_shake";
        public const string CraftingTutorialCompleted = "crafting_tutorial_completed";
        public const string WeaponSlotTutorialCompleted = "weapon_slot_tutorial_completed";
        public const string WeaponSlotsFullTutorialCompleted = "weapon_slots_full_tutorial_completed";
        public const string WeaponMergeTutorialCompleted = "weapon_merge_tutorial_completed";
        public const string AutoMergeTutorialCompleted = "auto_merge_tutorial_completed";
        public const string ItemsTutorialCompleted = "items_tutorial_completed";
        public const string ItemLockingTutorialCompleted = "item_locking_tutorial_completed";

        public static bool HasCompletedWeaponSlotTutorial()
        {
            return PlayerPrefs.GetInt(WeaponSlotTutorialCompleted, 0) > 0;
        }
        
        public static void SetCompletedWeaponSlotTutorial(bool value)
        {
            SetCompletedTutorial(WeaponSlotTutorialCompleted, value);
        }
        
        public static bool HasCompletedWeaponMergeTutorial()
        {
            return PlayerPrefs.GetInt(WeaponMergeTutorialCompleted, 0) > 0;
        }
        
        public static void SetCompletedWeaponMergeTutorial(bool value)
        {
            SetCompletedTutorial(WeaponMergeTutorialCompleted, value);
        }

        public static bool HasCompletedCraftingTutorial()
        {
            return PlayerPrefs.GetInt(CraftingTutorialCompleted, 0) > 0;
        }
        
        public static void SetCompletedCraftingTutorial(bool value)
        {
            SetCompletedTutorial(CraftingTutorialCompleted, value);
        }

        private static void SetCompletedTutorial(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public static void SetCompletedItemsTutorial(bool value)
        {
            SetCompletedTutorial(ItemsTutorialCompleted, value);
        }

        public static bool HasCompletedItemsTutorial()
        {
            return PlayerPrefs.GetInt(ItemsTutorialCompleted, 0) > 0;
        }

        public static bool HasCompletedAutoMergeTutorial()
        {
            return PlayerPrefs.GetInt(AutoMergeTutorialCompleted, 0) > 0;
        }
        
        public static void SetCompletedAutoMergeTutorial(bool value)
        {
            SetCompletedTutorial(AutoMergeTutorialCompleted, value);
        }
        
        public static bool HasCompletedWeaponSlotsFullTutorial()
        {
            return PlayerPrefs.GetInt(WeaponSlotsFullTutorialCompleted, 0) > 0;
        }

        public static void SetCompletedWeaponSlotsFullTutorial(bool value)
        {
            SetCompletedTutorial(WeaponSlotsFullTutorialCompleted, value);
        }
        
        public static bool HasCompletedItemLockingTutorial()
        {
            return PlayerPrefs.GetInt(ItemLockingTutorialCompleted, 0) > 0;
        }

        public static void SetsCompletedItemLockingTutorial(bool value)
        {
            SetCompletedTutorial(ItemLockingTutorialCompleted, value);
        }
    }
}