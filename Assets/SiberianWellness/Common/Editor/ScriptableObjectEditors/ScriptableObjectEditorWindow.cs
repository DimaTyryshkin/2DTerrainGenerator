using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace SiberianWellness.ScriptableObjectEditors.Editor
{
	/// <summary>
	/// Короче идея такая:
	/// Все кастомные редакторы которые рисуются в отдельных окошках содержат кучу одинакового кода связанного с работой самого окна. Этот код вынесен сюда.
	/// А вся уникальная логика (которая кстати теперь вообще не зависит от окна) вынесена в класс редактор <see cref="ScriptableObjectEditor{EditorT}"/>
	/// </summary>
	/// <typeparam name="AssetT">Тип ассета, который дерактируется в редакторе</typeparam>
	/// <typeparam name="EditorT">Тип редактора</typeparam>
	/// <typeparam name="WindowT">Тип самого окна</typeparam>
	public class ScriptableObjectEditorWindow<AssetT,EditorT,WindowT> : EditorWindow
		where AssetT :EntityRoot
		where EditorT :ScriptableObjectEditor<AssetT>,new()
		where WindowT :ScriptableObjectEditorWindow<AssetT,EditorT,WindowT>
	{
		public AssetT  asset;
		public EditorT editor;
          
		protected static bool OnOpenAsset(int instanceId)
		{
			AssetT asset = EditorUtility.InstanceIDToObject(instanceId) as AssetT;
			if (asset && AssetDatabase.Contains(asset))
			{
				Open(asset);
				return true;
			}

			return false;
		}
        
		static void Open(AssetT asset) 
		{ 
			if (!asset) 
				return;
 
			WindowT w = GetWindow(typeof(WindowT), false, asset.name, true) as WindowT;
			w.wantsMouseMove = true;
			w.asset          = asset;
			w.editor = null; //Если окно уже открыто, надо пересоздать редактор
		}
         
		void OnEnable()
		{
			SceneView.duringSceneGui += OnSceneGui;
		}

		void OnDisable()
		{
			SceneView.duringSceneGui -= OnSceneGui;
		}

		void OnSceneGui(SceneView obj)
		{
			Repaint();
			
			if(!asset)
				return;
            
			ValidateEditor();
			editor.OnSceneGui();
		}
        
		void ValidateEditor()
		{  
			if(!asset)
				return;
            
			if (editor == null)
			{
				editor = new EditorT() {asset = asset};
			}
            
			editor.Validate();
		}
        
		void OnGUI()
		{
			if (!asset) 
			{
				Close(); 
				return;
			}
 
			ValidateEditor();      
            
			editor.OnGui();
		}
        
	}
}