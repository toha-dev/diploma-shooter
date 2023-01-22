using DS.UI.Base.Gui;
using DS.UI.Base.ViewModels;
using UnityEngine;
using Zenject;

namespace DS.Simulation
{
	public class Loading : MonoBehaviour
	{
		[Inject]
		private IGuiService _guiService;

		private void Awake()
		{
			_guiService.ShowScreenAsync<LoadingScreenViewModel>(
				ScreenType.Loading,
				GuiLayer.Stack,
				viewModel => viewModel.SetData(
					SceneLoader.LoadingProgress,
					HandleLoadingComplete));
		}

		private static async void HandleLoadingComplete()
		{
			await SceneLoader.UnloadSceneAsync(SceneLoader.LoadingSceneName);
		}

		private void OnDestroy()
		{
			_guiService.HideScreen(ScreenType.Loading, GuiLayer.Stack);
		}
	}
}