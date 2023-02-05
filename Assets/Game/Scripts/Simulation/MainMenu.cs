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

		private async void Awake()
		{
			await _guiService.ShowScreenAsync<MainMenuViewModel>(
				ScreenType.MainMenu,
				GuiLayer.Stack,
				viewModel => viewModel.SetData(LoadWorld, ExitGame));

			Application.UpdateState(ApplicationState.MainMenu);
		}

		private async void LoadWorld()
		{
			Application.RunWithParameters(GameModeType.Free, GameMap.FreeModeMap);
			Application.UpdateState(ApplicationState.Loading);

			_guiService.ShowScreenAsync<LoadingScreenViewModel>(
				ScreenType.Loading,
				GuiLayer.Overlay,
				viewModel =>
				{
					viewModel.SetData(SceneLoader.LoadingProgress, () =>
					{
						_guiService.HideScreen(ScreenType.Loading, GuiLayer.Overlay);
						Application.UpdateState(ApplicationState.Running);
					});
				});

			await SceneLoader.LoadScenesInQueueAsync(
				new List<(string name, LoadSceneMode mode)>
				{
					(SceneLoader.WorldSceneName, LoadSceneMode.Single),
					(Application.GameMap.ToString(), LoadSceneMode.Additive),
				});

			Application.UpdateState(ApplicationState.Loaded);
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