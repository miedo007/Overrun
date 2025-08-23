#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class UserTrackingPostProcessor
{
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
    {
        // Get plist
        string plistPath = pathToBuiltProject + "/Info.plist";
        var plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));

        // Get root
        PlistElementDict rootDict = plist.root;

        // Set encryption usage boolean
        const string encryptKey = "NSUserTrackingUsageDescription";
        rootDict.SetString(encryptKey, "This data will be used to display custom ads. You can manage your permission in the Settings at any time.");
        
        // Write to file
        File.WriteAllText(plistPath, plist.WriteToString());
    }
}
#endif