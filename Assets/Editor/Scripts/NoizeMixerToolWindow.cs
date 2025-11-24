using GamePackages.Core.ScriptableObjectEditors.Editor;
using UnityEditor.Callbacks;

namespace Game.NoizeMixerTool
{
    class NoizeMixerToolWindow : ScriptableObjectEditorWindow<NoizeMixerToolModel, NoizeMixerToolEditor, NoizeMixerToolWindow>
    {
        [OnOpenAssetAttribute]
        public static bool OnOpenAsset(int instanceId)
        {
            return ScriptableObjectEditorWindow<NoizeMixerToolModel, NoizeMixerToolEditor, NoizeMixerToolWindow>.OnOpenAssetInternal(instanceId);
        }
    }
}
