using DS.UI.Base.Gui;
using DS.UI.Base.ViewModels;
using JetBrains.Annotations;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace DS.UI.Screens
{
	public class LoadingScreen : ScreenView<LoadingScreenViewModel>
	{
		[field: SerializeField]
		private Slider LoadingProgressBar { get; [UsedImplicitly] set; }

		[field: SerializeField]
		private TextMeshProUGUI LoadingProgressText { get; [UsedImplicitly] set; }

		[Inject]
		private UiInputActions _uiInputActions;

		protected override void Show()
		{
			_uiInputActions.Enable();
			_uiInputActions.UI.AnyKey.performed += ViewModel.HandleAnyButtonClick;

			ViewModel.LoadingProgress.Subscribe(x =>
			{
				LoadingProgressBar.value = x;
				LoadingProgressText.text = $"{(int)(x * 100)}%";
			}).AddTo(Disposables);
		}

		protected override void Hide()
		{
			base.Hide();

			_uiInputActions.UI.AnyKey.performed -= ViewModel.HandleAnyButtonClick;
			_uiInputActions.Disable();
		}
	}
}