using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
	private const string LoadingSceneName = "Loading";

	public const string MainMenuSceneName = "MainMenu";
	public const string WorldSceneName = "World";

	public static float LoadingProgress { get; private set; }

	public static async UniTask<AsyncUnit> LoadSceneAsync(
		string sceneName,
		LoadSceneMode mode,
		bool showLoadingScreen)
	{
		var completionSource = new UniTaskCompletionSource<AsyncUnit>();

		if (showLoadingScreen)
		{
			await SceneManager.LoadSceneAsync(LoadingSceneName, mode);
		}

		var sceneLoad = SceneManager.LoadSceneAsync(sceneName, mode);

		while (!sceneLoad.isDone)
		{
			LoadingProgress = sceneLoad.progress;
			await UniTask.Yield(PlayerLoopTiming.Update);
		}

		if (showLoadingScreen)
		{
			SceneManager.UnloadSceneAsync(LoadingSceneName);
		}

		completionSource.TrySetResult(AsyncUnit.Default);

		return AsyncUnit.Default;
	}

	public static async UniTask<AsyncUnit> UnloadSceneAsync(
		string sceneName,
		UnloadSceneOptions options = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects)
	{
		var completionSource = new UniTaskCompletionSource<AsyncUnit>();

		await SceneManager.UnloadSceneAsync(sceneName, options);

		completionSource.TrySetResult(AsyncUnit.Default);
		return AsyncUnit.Default;
	}
}
