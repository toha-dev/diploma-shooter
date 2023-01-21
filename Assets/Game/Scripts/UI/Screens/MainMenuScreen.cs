using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DS.UI.Screens
{
	public class MainMenuScreen : MonoBehaviour
	{
		[field: SerializeField]
		private Button PlayButton { get; [UsedImplicitly] set; }

		[field: SerializeField]
		private Button ExitButton { get; [UsedImplicitly] set; }

		private void Awake()
		{
			PlayButton.onClick.AddListener(HandlePlayClicked);
			ExitButton.onClick.AddListener(HandleExitClicked);
		}

		private void OnDestroy()
		{
			PlayButton.onClick.RemoveListener(HandlePlayClicked);
			ExitButton.onClick.RemoveListener(HandleExitClicked);
		}

		private static async void HandlePlayClicked()
		{
			await SceneLoader.UnloadSceneAsync(SceneLoader.MainMenuSceneName);

			await SceneLoader.LoadSceneAsync(
				SceneLoader.WorldSceneName,
				LoadSceneMode.Additive,
				showLoadingScreen: true);
		}

		private static void HandleExitClicked()
		{
			Application.Quit();
		}
	}
}
