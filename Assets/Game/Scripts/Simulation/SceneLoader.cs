using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.SceneManagement;

namespace DS.Simulation
{
	public static class SceneLoader
	{
		public const string LoadingSceneName = "Loading";
		public const string MainMenuSceneName = "MainMenu";
		public const string WorldSceneName = "World";

		public static IReadOnlyReactiveProperty<float> LoadingProgress => InternalLoadingProgress;
		private static readonly ReactiveProperty<float> InternalLoadingProgress = new();

		public static async UniTask<AsyncUnit> LoadSceneAsync(
			string sceneName,
			LoadSceneMode mode,
			bool showLoadingScreen)
		{
			if (showLoadingScreen)
			{
				await SceneManager.LoadSceneAsync(LoadingSceneName, LoadSceneMode.Additive);
			}

			var sceneLoad = SceneManager.LoadSceneAsync(sceneName, mode);

			while (!sceneLoad.isDone)
			{
				InternalLoadingProgress.Value = sceneLoad.progress;
				await UniTask.Yield(PlayerLoopTiming.Update);
			}

			if (showLoadingScreen && mode != LoadSceneMode.Single)
			{
				await SceneManager.UnloadSceneAsync(LoadingSceneName);
			}

			return AsyncUnit.Default;
		}

		public static async UniTask<AsyncUnit> UnloadSceneAsync(
			string sceneName,
			UnloadSceneOptions options = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects)
		{
			await SceneManager.UnloadSceneAsync(sceneName, options);
			return AsyncUnit.Default;
		}
	}
}
