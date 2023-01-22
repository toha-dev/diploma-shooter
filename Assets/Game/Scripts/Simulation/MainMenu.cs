using DS.UI.Base.Gui;
using DS.UI.Base.ViewModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace DS.Simulation
{
	public class MainMenu : MonoBehaviour
	{
		[Inject]
		private IGuiService _guiService;

		private async void Start()
		{
			await _guiService.ShowScreenAsync<MainMenuViewModel>(
				ScreenType.MainMenu,
				GuiLayer.Stack,
				viewModel => viewModel.SetData(LoadWorld, ExitGame));
		}

		private static async void LoadWorld()
		{
			await SceneLoader.LoadSceneAsync(
				SceneLoader.WorldSceneName,
				LoadSceneMode.Single,
				showLoadingScreen: true);
		}

		private static void ExitGame()
		{
			Application.Quit();
		}

		private void OnDestroy()
		{
			_guiService.HideScreen(ScreenType.MainMenu, GuiLayer.Stack);
		}
	}
}