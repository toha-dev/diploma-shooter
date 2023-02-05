using System;
using System.Collections.Generic;
using System.Linq;
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

		public static async UniTask<AsyncUnit> LoadScenesInQueueAsync(
			IEnumerable<(string name, LoadSceneMode mode)> scenes)
		{
			var scenesArray = scenes as (string name, LoadSceneMode mode)[] ?? scenes.ToArray();

			var sceneLoadingIndex = 0;

			foreach (var (name, mode) in scenesArray)
			{
				await LoadSceneAsync(
					name,
					mode,
					progress =>
					{
						InternalLoadingProgress.Value =
							(float)sceneLoadingIndex / scenesArray.Length
							+ progress / scenesArray.Length;
					});

				sceneLoadingIndex++;
			}

			return AsyncUnit.Default;
		}

		public static async UniTask<AsyncUnit> LoadSceneAsync(
			string sceneName,
			LoadSceneMode mode)
		{
			await LoadSceneAsync(
				sceneName,
				mode,
				progress => InternalLoadingProgress.Value = progress);

			return AsyncUnit.Default;
		}

		private static async UniTask<AsyncUnit> LoadSceneAsync(
			string sceneName,
			LoadSceneMode mode,
			Action<float> progressCallback)
		{
			var sceneLoad = SceneManager.LoadSceneAsync(sceneName, mode);

			while (!sceneLoad.isDone)
			{
				progressCallback?.Invoke(sceneLoad.progress);
				await UniTask.Yield(PlayerLoopTiming.Update);
			}

			progressCallback?.Invoke(1);
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
