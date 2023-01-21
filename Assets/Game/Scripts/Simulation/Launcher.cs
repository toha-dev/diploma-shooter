using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviour
{
	private void Awake()
	{
		LoadMenuScene().Forget();
	}

	private static async UniTaskVoid LoadMenuScene()
	{
		await SceneLoader.LoadSceneAsync(
			SceneLoader.MainMenuSceneName,
			LoadSceneMode.Additive,
			showLoadingScreen: false);
	}
}
