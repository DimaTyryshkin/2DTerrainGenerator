using UnityEngine.SceneManagement;

namespace SiberianWellness.Common
{
	public static class SceneManagerExtension
	{
		public static Scene[] GetLoadedScenes()
		{
			int n = SceneManager.sceneCount;
			Scene[] scenes = new Scene[n];
			for (int i = 0; i <n ; i++)
			{
				scenes[i] = SceneManager.GetSceneAt(i);
			}

			return scenes;
		}
	}
}