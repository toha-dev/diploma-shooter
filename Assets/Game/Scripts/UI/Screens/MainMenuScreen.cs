using DS.UI.Base.Gui;
using DS.UI.Base.ViewModels;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace DS.UI.Screens
{
	public class MainMenuScreen : ScreenView<MainMenuViewModel>
	{
		[field: SerializeField]
		private Button PlayButton { get; [UsedImplicitly] set; }

		[field: SerializeField]
		private Button ExitButton { get; [UsedImplicitly] set; }

		protected override void Show()
		{
			PlayButton.onClick.AddListener(ViewModel.HandlePlayClicked);
			ExitButton.onClick.AddListener(ViewModel.HandleExitClicked);
		}

		protected override void Hide()
		{
			PlayButton.onClick.RemoveListener(ViewModel.HandlePlayClicked);
			ExitButton.onClick.RemoveListener(ViewModel.HandleExitClicked);
		}
	}
}