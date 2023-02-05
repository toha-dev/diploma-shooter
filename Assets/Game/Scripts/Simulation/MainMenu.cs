using System.Collections.Generic;
using DS.Simulation.GameModes;
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

		private async void LoadWorld()
		{
			Application.SetGameParameters(GameModeType.Free, GameMap.FreeModeMap);

			_guiService.ShowScreenAsync<LoadingScreenViewModel>(
				ScreenType.Loading,
				GuiLayer.Overlay,
				viewModel =>
				{
					viewModel.SetData(SceneLoader.LoadingProgress, () =>
					{
						_guiService.HideScreen(ScreenType.Loading, GuiLayer.Overlay);
					});
				});

			await SceneLoader.LoadScenesInQueueAsync(
				new List<(string name, LoadSceneMode mode)>
				{
					(SceneLoader.WorldSceneName, LoadSceneMode.Single),
					(Application.GameMap.ToString(), LoadSceneMode.Additive),
				});
		}

		private static void ExitGame()
		{
			UnityEngine.Application.Quit();
		}

		private void OnDestroy()
		{
			_guiService.HideScreen(ScreenType.MainMenu, GuiLayer.Stack);
		}
	}
}