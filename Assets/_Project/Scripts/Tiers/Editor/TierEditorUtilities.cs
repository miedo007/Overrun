using System.IO;
using System.Linq;
using Project.Application;
using UnityEditor;
using UnityEngine;

namespace Project.Tiers.Editor
{
    public class TierEditorUtilities : MonoBehaviour
    {
        [MenuItem("Assets/Utilities/Create Tiered Group")]
        public static void CreateTieredGroup()
        {
            var selectedData = Selection.activeObject as BaseData;
            if (selectedData == null)
            {
                Debug.LogError("Selected item is not derived from BaseData");
                return;
            }
            
            // Load the tiers list
            var guids = AssetDatabase.FindAssets("tiers_list");
            var tiersList = AssetDatabase.LoadAssetAtPath<TiersList>(AssetDatabase.GUIDToAssetPath(guids[0]));
            
            // Create a new tiered group
            var tieredGroup = ScriptableObject.CreateInstance<TieredDataGroup>();
            for (var i = 0; i < tiersList.Tiers.Count; i++)
            {
                tieredGroup.Tiers.Add(new TierInfo()
                {
                    Tier = tiersList.Tiers[i]
                });
            }

            tieredGroup.Tiers[0].Data = selectedData;

            var path = AssetDatabase.GetAssetPath(selectedData);
            path = path.Replace("_0", "_{0}");

            // create duplicates of selected data
            for (var i = 1; i < tiersList.Tiers.Count; i++)
            {
                var uniquePath = string.Format(path, i.ToString());
                var dataCopy = Instantiate(selectedData);
                AssetDatabase.CreateAsset(dataCopy, uniquePath);

                tieredGroup.Tiers[i].Data = dataCopy;
            }

            var groupName = $"tiered_group_{selectedData.name.Replace("_0", "")}.asset";
            Debug.Log(groupName);
            var parentDirectory = Directory.GetParent(path).ToString();
            var relativePath = parentDirectory.Substring(parentDirectory.IndexOf("Assets"));
            var tieredGroupPath = $"{relativePath}\\{groupName}";
            AssetDatabase.CreateAsset(tieredGroup, tieredGroupPath);
        }
    }
}