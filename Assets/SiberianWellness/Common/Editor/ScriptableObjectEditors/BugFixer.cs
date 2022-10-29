using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace SiberianWellness.ScriptableObjectEditors
{
    /// <summary>
    /// This asset processor resolves an issue with the new v2 AssetDatabase system present on 2019.3 and later. When
    /// renaming a asset, it appears that sometimes the v2 AssetDatabase will swap which asset
    /// is the main asset (present at top level) between the Root and one of its Entity
    /// sub-assets. As a workaround until Unity fixes this, this asset processor checks all renamed assets and if it
    /// finds a case where a Entity has been made the main asset it will swap it back to being a sub-asset
    /// and rename the node to the default name for that node type.
    /// </summary>
    internal sealed class BugFixer : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {  
           FixBug(movedAssets);
           FixBug(importedAssets);
        }

        static void FixBug(string[] assets)
        {
            for (int i = 0; i < assets.Length; i++)
            {
                EntityElement entiry = AssetDatabase.LoadMainAssetAtPath(assets[i]) as EntityElement;

                if (entiry && AssetDatabase.IsMainAsset(entiry))
                {
                    AssetDatabase.SetMainObject(entiry.root, assets[i]);
                    AssetDatabase.ImportAsset(assets[i]);

                   
                    entiry.name = entiry.GetType().Name;
                    
                    EditorUtility.SetDirty(entiry);
                }
            }
        }
    }
}
